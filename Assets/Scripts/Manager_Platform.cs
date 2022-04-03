using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Platform : MonoBehaviour
{
    public List<Controller_Platform> listPlatform;
    public GameObject prefabPlatform;
    public int platformCount = 30;
    public float distanceBetweenPlatforms = 2f;
    private static Manager_Platform Instance;

    public static Manager_Platform GetInstance()
    {
        return Instance;
    }
 
    private void Awake() 
    {
        Instance = this;
        InitializePlatforms();
    }

    private void InitializePlatforms()
    {
        for (int i=0; i < platformCount; i++)
        {
            GameObject platform = GameObject.Instantiate(prefabPlatform,this.transform);
            Controller_Platform controllerPlatform = platform.GetComponent<Controller_Platform>();
            listPlatform.Add(controllerPlatform);

            if (i == 0) listPlatform[i].SetPlatformType(EPlatformType.STARTING);
            else if (i == platformCount - 1) 
            {
                listPlatform[i].SetPlatformType(EPlatformType.ENDING);
                listPlatform[i].transform.position = new Vector3(listPlatform[i-1].transform.position.x,listPlatform[i-1].transform.position.y - distanceBetweenPlatforms ,listPlatform[i-1].transform.position.z);
            }
            else listPlatform[i].transform.position = new Vector3(listPlatform[i-1].transform.position.x,listPlatform[i-1].transform.position.y - distanceBetweenPlatforms ,listPlatform[i-1].transform.position.z);
        }
    }
    public void DestroyPlatform()
    {
        if (listPlatform.Count > 3)
        {
            listPlatform[0].DestroyPlatform();
            listPlatform.RemoveAt(0);
        }
        else return;
    }

    public void EnableBooster(int pmovingBoosterPlatformBreakCount)
    {
        for (int i=0;i<pmovingBoosterPlatformBreakCount;i++) DestroyPlatform();
        Controller_Ball.OnBallBreakableMode?.Invoke();
    }
    
}
