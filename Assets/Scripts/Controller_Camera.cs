using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Camera : MonoBehaviour
{
    [SerializeField] private GameObject cylinder;
    [SerializeField] private float turnSpeed;

    [SerializeField] private GameObject followTarget;
    [SerializeField] private Vector3 followOffset;
    void Start()
    {
        followTarget = Controller_Ball.GetInstance().gameObject;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Controller_Ball.GetInstance().canMove)
        {
            float touchInputX = turnSpeed * Input.GetAxis("Mouse X");
            cylinder.transform.Rotate(0, -touchInputX, 0);
        }

        if ( followTarget != null) this.transform.position = new Vector3(followTarget.transform.position.x,followTarget.transform.position.y,followTarget.transform.position.z) + followOffset;
    }
}
