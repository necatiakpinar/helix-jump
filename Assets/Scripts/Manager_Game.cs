using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Game : MonoBehaviour
{
    [Header("Ball")]
    public int ballBreakModeCount = 3;

    [Header("Normal Booster Variables")]
    [Tooltip("Probability must be in 0 - 100")] public float normalBoosterProbability = 10;
    public int normalBoosterPlatformBreakCount = 5;

    [Header("Moving Booster Variables")]
    [Tooltip("Probability must be in 0 - 100")] public float movingBoosterProbability = 30;
    public float movingBoosterBlockTurnSpeed;
    public int movingBoosterPlatformBreakCount = 5;

    [Header("Probabilities")]
    [Tooltip("Probability must be in 0 - 100")] public float rotatorBlockProbability = 30;
    
    [Header("Platform Variables")]
    [Tooltip("Closed block count can be maximum 4.")] public int maxClosedBlockCount = 4;
    [Tooltip("Probability must be in 0 - 100")] public int obstacleProbability = 25;

    private static Manager_Game Instance;

    private void Awake() 
    {
        Instance = this;    
    }
    
    public static Manager_Game GetInstance()
    {
        return Instance;
    }
}
