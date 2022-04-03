using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum EBallStatus
{
    BREAK,
    UNBREAK
}

public class Controller_Ball : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float jumpForce;
    [SerializeField] private EBallStatus currentBallStatus;
    public int blocksPassedWithoutTouch = 0;
    public bool canMove;
    public bool isTouched;

    [Header("Components")]
    [SerializeField] private Rigidbody rigidBody;

    [Header("VFX")]
    [SerializeField] private GameObject prefabVfxSplash;

    #region Events
        public static Action OnBallBreakableMode;
    #endregion

    private static Controller_Ball Instance;

    [Header("Managers & Controllers")]
    public Manager_Platform managerPlatform;
    public Manager_Game managerGame;
    
    public static Controller_Ball GetInstance()  { return Instance; }


    private void OnEnable() 
    {
        OnBallBreakableMode = EventFunction_OnBallBreakableMode_BrekableMode;
    }
    private void OnDestroy() 
    {
        OnBallBreakableMode = null;
    }

    private void Awake() 
    {
        Instance = this;

        this.rigidBody = GetComponent<Rigidbody>();    
        this.isTouched = false;
        this.canMove = true;
        this.currentBallStatus = EBallStatus.UNBREAK;
        
    }

    private void Start() 
    {
        managerPlatform = Manager_Platform.GetInstance();
        managerGame = Manager_Game.GetInstance();
    }
    
    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space)) managerPlatform.EnableBooster(managerGame.movingBoosterPlatformBreakCount);    
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (!this.isTouched)
        {
            if (other.collider.transform.parent.GetComponent<Controller_Block>() != null) //If we hit block...
            {
                Controller_Block colliderBlock = other.collider.transform.parent.GetComponent<Controller_Block>();
                blocksPassedWithoutTouch = 0;
                if (this.currentBallStatus == EBallStatus.UNBREAK)
                {
                    if (colliderBlock.GetBlockType() == EBlockType.SAFE)
                    {
                        if (colliderBlock.controllerPlatform.GetPlatformType() != EPlatformType.ENDING)
                        {
                            rigidBody.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
                            StartCoroutine(CO_ResetTouchStatus());
                        }
                        else if (colliderBlock.controllerPlatform.GetPlatformType() == EPlatformType.ENDING)
                        {
                            Debug.Log("Level finished!");
                            
                            this.canMove = false;
                            Manager_UI.GetInstance().CheckHighScore();
                            Manager_UI.GetInstance().PassToNextLevel();
                            return;
                        }
                    }
                    else if (colliderBlock.GetBlockType() == EBlockType.UNSAFE)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    else if (colliderBlock.GetBlockType() == EBlockType.BOOSTER)
                    {
                        managerPlatform.EnableBooster(managerGame.movingBoosterPlatformBreakCount);    
                        OnBallBreakableMode?.Invoke();
                        return;
                    }
                }
                else if (this.currentBallStatus == EBallStatus.BREAK)
                {
                    this.currentBallStatus = EBallStatus.UNBREAK;
                    if (colliderBlock.controllerPlatform.GetPlatformType() != EPlatformType.ENDING)
                    {
                        rigidBody.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
                        StartCoroutine(CO_ResetTouchStatus());
                        managerPlatform.listPlatform.Remove(colliderBlock.controllerPlatform);
                        colliderBlock.controllerPlatform.DestroyPlatform();
                    }
                    else if (colliderBlock.controllerPlatform.GetPlatformType() == EPlatformType.ENDING)
                    {
                        Debug.Log("Level finished!");
                        
                        this.canMove = false;
                        Manager_UI.GetInstance().CheckHighScore();
                        Manager_UI.GetInstance().PassToNextLevel();
                        return;
                    }
                }

                //VFX
                CreateVFXSplash(other.collider.transform);
            }
            else if (other.collider.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle")) //If we hit one of the obstacles on the block
            {
                
                if (this.currentBallStatus == EBallStatus.UNBREAK)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else
                {
                    this.currentBallStatus = EBallStatus.UNBREAK;
                    other.collider.transform.parent.parent.GetComponent<Controller_Block>().controllerPlatform.DestroyPlatform();
                    Manager_UI.GetInstance().IncreaseCurrentScore();
                }
            }
            else if (other.transform.gameObject.layer == LayerMask.NameToLayer("Booster")) //If we hit one of the obstacles on the block
            {
                managerPlatform.EnableBooster(managerGame.normalBoosterPlatformBreakCount);
            }
        }
    }

    private IEnumerator CO_ResetTouchStatus()
    {
        isTouched = true;
        yield return new WaitForSeconds(0.1f);
        isTouched = false;
    }
    
    public void EventFunction_OnBallBreakableMode_BrekableMode()
    {
        this.currentBallStatus = EBallStatus.BREAK;
    }

    public void CreateVFXSplash(Transform pSplashParent)
    {
        GameObject vfxSplash = GameObject.Instantiate(prefabVfxSplash, pSplashParent);
        vfxSplash.transform.position = new Vector3(this.transform.position.x,this.transform.position.y - 0.05f,this.transform.position.z + 0.2f);
    }

}
