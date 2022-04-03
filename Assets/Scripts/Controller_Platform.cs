using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public enum EPlatformType
{
    STARTING,
    GAMEPLAY,
    ENDING
}
public class Controller_Platform : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private EPlatformType platformType;
    [SerializeField] private List<Controller_Block> listBlock;
    [SerializeField] private List<Controller_Block> listOpenedBlock;
    [SerializeField] private List<Controller_Block> listClosedBlock;
    [SerializeField] private float destroyDistance = 0.2f;
    [SerializeField] private int closedBlocksCount = 0;
    [SerializeField] private int closedBlockIndex = 0;
    [SerializeField] private List<int> listClosedBlockIndex;
    [SerializeField] private bool isDestroyed = false;

    [Header("Managers & Controllers")]
    private Manager_Game managerGame;
    private Manager_Platform managerPlatform;
    private Controller_Ball controllerBall;


    

    private void Awake() 
    {
        managerPlatform = this.transform.parent.GetComponent<Manager_Platform>();
    }
    private void Start() 
    {
        controllerBall = Controller_Ball.GetInstance();    
        managerGame = Manager_Game.GetInstance();

        if (platformType == EPlatformType.GAMEPLAY)
        {
            InitializePlatform();
            InitializeFeatures();
        }
    }

    private void Update() 
    {
        CheckDestroyPlatform();
    }

    #region Platform Functions
        private void InitializePlatform()
        {
            foreach (Controller_Block controllerBlock in listBlock) listOpenedBlock.Add(controllerBlock);

            closedBlocksCount = UnityEngine.Random.Range(1,managerGame.maxClosedBlockCount);
            for (int i=0; i < closedBlocksCount;i++) CloseBlock();

            CreateUnsafeBlocks();
            EnableObstacleOnBlocks();
            EnableNormalBooster();
        }
        private void CloseBlock()
        {
            if (listClosedBlockIndex.Count == 0)
            {
                closedBlockIndex = UnityEngine.Random.Range(0,listBlock.Count);
                listClosedBlockIndex.Add(closedBlockIndex);
            }
            else
            {
                closedBlockIndex += 2;
                if (closedBlockIndex == listBlock.Count) closedBlockIndex = 0;
                else if (closedBlockIndex == listBlock.Count + 1) closedBlockIndex = 1;
                else if (closedBlockIndex == listBlock.Count + 2) closedBlockIndex = 2;

                listClosedBlockIndex.Add(closedBlockIndex);
            }

            listBlock[listClosedBlockIndex[listClosedBlockIndex.Count - 1]].gameObject.SetActive(false);
            listOpenedBlock.Remove(listBlock[listClosedBlockIndex[listClosedBlockIndex.Count - 1]]);
            listClosedBlock.Add(listBlock[listClosedBlockIndex[listClosedBlockIndex.Count - 1]]);
        }
        
        private void CreateUnsafeBlocks()
        {
            int unsafeBlockCount = UnityEngine.Random.Range(1,listOpenedBlock.Count / 2 + 1);
            for (int i=0; i < unsafeBlockCount; i++)
            {
                if (listOpenedBlock[i] != null) listOpenedBlock[i].ChangeBlockType(EBlockType.UNSAFE);
            }
        }

        private void EnableObstacleOnBlocks()
        {
            int obstacleProbability = UnityEngine.Random.Range(0,100);
            if (obstacleProbability <= managerGame.obstacleProbability)
            {
                int obstacleCount = UnityEngine.Random.Range(0,3);
                
                for (int i=0;i < obstacleCount; i++)
                {
                    int obstacleEnabledBlockIndex = UnityEngine.Random.Range(0,listOpenedBlock.Count);
                    listOpenedBlock[obstacleEnabledBlockIndex].EnableObstacle();
                }
            }
        }

        private void EnableNormalBooster()
        {
            int normalBoosterProbability = UnityEngine.Random.Range(0,100);
            if (normalBoosterProbability <= managerGame.normalBoosterProbability)
            {
                int normalBoosterIndex = UnityEngine.Random.Range(0,listOpenedBlock.Count);
                listOpenedBlock[normalBoosterIndex].EnableNormalBooster();
            }
        }

        private void CheckDestroyPlatform()
        {
            if (Controller_Ball.GetInstance() != null)
            {
                if (Controller_Ball.GetInstance().transform.position.y <= this.transform.position.y - destroyDistance && !isDestroyed)
                {
                    controllerBall.blocksPassedWithoutTouch++;
                    if (controllerBall.blocksPassedWithoutTouch == managerGame.ballBreakModeCount) Controller_Ball.OnBallBreakableMode?.Invoke();
                    managerPlatform.listPlatform.Remove(this);
                    //Manager_UI.GetInstance().IncreaseCurrentScore();
                    //Destroy(this.gameObject);
                    isDestroyed = true;
                    DestroyPlatform();
                }
            }
        }
        
        public void AddForceToBlocks()
        {
            foreach (Controller_Block controllerBlock in listOpenedBlock)
            {
                controllerBlock.transform.gameObject.AddComponent<Rigidbody>();
                Rigidbody rigidBody = controllerBlock.transform.gameObject.GetComponent<Rigidbody>();
                MeshCollider collider = controllerBlock.transform.gameObject.GetComponent<MeshCollider>();
                rigidBody.AddForce(new Vector3(100f,100f,10f));
                collider.isTrigger = true;

            }
        }
        public void DestroyPlatform()
        {
            Manager_UI.GetInstance().IncreaseCurrentScore();
            foreach(Controller_Block controllerBlock in listOpenedBlock) //Make platform objects trigger to not affect player ball!
            {
                controllerBlock.meshBlock.GetComponent<MeshCollider>().isTrigger = true;
                controllerBlock.meshObstacle.GetComponent<Collider>().isTrigger = true; 
                controllerBlock.meshBooster.GetComponent<Collider>().isTrigger = true; 


                if (controllerBlock.meshBlock.GetComponent<Rigidbody>() == null) controllerBlock.meshBlock.AddComponent<Rigidbody>();
                //controllerBlock.meshBlock.GetComponent<Rigidbody>().useGravity = false;
                controllerBlock.meshBlock.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-200,200),0,UnityEngine.Random.Range(100,200)));

            }

            Destroy(this.gameObject,1f);
        }

        public void SetPlatformType(EPlatformType pPlatformType)
        {
            platformType = pPlatformType;

            if (pPlatformType == EPlatformType.STARTING) listBlock[4].gameObject.SetActive(false);
        }
        public EPlatformType GetPlatformType()
        {
            return platformType;
        }
    #endregion


    #region Platform Features
        private void InitializeFeatures()
        {
            #region Rotator
                if (closedBlocksCount == 1)
                {
                    float rotatorBlockProbability = UnityEngine.Random.Range(0,100);
                    if (rotatorBlockProbability <= managerGame.rotatorBlockProbability) CreateRotatorBlock(); 
                } 
            #endregion

            #region Moving Booster
                float movingBoosterProbability = UnityEngine.Random.Range(0,100);
                if (movingBoosterProbability <= managerGame.movingBoosterProbability) CreateMovingBoosterPlatform();
            #endregion
        }
        
        private void CreateRotatorBlock()
        {
            int rotatorIndex = 0;

            if (listClosedBlockIndex[0] == 0) rotatorIndex = 7;
            else rotatorIndex = listClosedBlockIndex[0] - 1;
            
            Vector3 startRotation = listBlock[rotatorIndex].gameObject.transform.eulerAngles;
            Vector3 endRotation = new Vector3(0,listBlock[rotatorIndex].gameObject.transform.eulerAngles.y - 45,0);
            Sequence sequence = DOTween.Sequence();
            
            sequence.SetAutoKill(false);
            sequence.Append(listBlock[rotatorIndex].gameObject.transform.DOLocalRotate(endRotation,2f).OnComplete(delegate {
                listBlock[rotatorIndex].gameObject.transform.DOLocalRotate(startRotation,2f).OnComplete(delegate {
                sequence.Restart();
            });
            }
            ));
        }

        private void CreateMovingBoosterPlatform()
        {
            StartCoroutine(CO_CreateMovingBoosterPlatform());
        }

        private IEnumerator CO_CreateMovingBoosterPlatform()
        {
            for (int i=0; i < listOpenedBlock.Count; i++)
            {
                EBlockType previousBlockType = listOpenedBlock[i].GetBlockType();
                listOpenedBlock[i].ChangeBlockType(EBlockType.BOOSTER);
                yield return new WaitForSeconds(managerGame.movingBoosterBlockTurnSpeed);
                listOpenedBlock[i].ChangeBlockType(previousBlockType);
            }

            StartCoroutine(CO_CreateMovingBoosterPlatform());
        }
    #endregion


}


