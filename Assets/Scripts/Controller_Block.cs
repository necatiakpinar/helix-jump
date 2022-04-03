using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum EBlockType
{
    SAFE,
    UNSAFE,
    BOOSTER
}

[System.Serializable]
public enum EBlockState
{
    IDLE,
    ROTATOR
}

public class Controller_Block : MonoBehaviour
{

    [Header("Attributes")]
    [SerializeField] private EBlockType blockType;
    [SerializeField] private EBlockState blockState;

    [Header("Objects")]
    public GameObject meshBlock;
    public GameObject meshObstacle;
    public GameObject meshBooster;
    
    [Header("Materials")]
    [SerializeField] private Material materialSafe;
    [SerializeField] private Material materialUnsafe;
    [SerializeField] private Material materialBooster;

    public bool isObstacleEnabled = false;
    public Controller_Platform controllerPlatform;

    private void OnValidate() 
    {
        if (materialSafe != null & materialUnsafe != null && materialBooster != null)    
        {
            ChangeBlockType(blockType);
        }
    }

    private void Awake() 
    {
        controllerPlatform = this.transform.parent.GetComponent<Controller_Platform>();
        blockState = EBlockState.IDLE;
        meshObstacle.SetActive(false);
        meshBooster.SetActive(false);
    }

    public void ChangeBlockType(EBlockType pBlockType)
    {
        blockType = pBlockType;
        if (blockType == EBlockType.SAFE) meshBlock.GetComponent<MeshRenderer>().material = materialSafe;
        else if (blockType == EBlockType.UNSAFE) meshBlock.GetComponent<MeshRenderer>().material = materialUnsafe;
        else if (blockType == EBlockType.BOOSTER) meshBlock.GetComponent<MeshRenderer>().material = materialBooster;
    }

    public EBlockType GetBlockType()
    {
        return blockType;
    }
    
    public void EnableObstacle()
    {
        meshObstacle.SetActive(true);
        isObstacleEnabled = true;
    }
    public void EnableNormalBooster()
    {
        meshBooster.SetActive(true);
    }
    
}