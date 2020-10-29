using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTest : MonoBehaviour
{
    [SerializeField]
    float speed = 1;

    Quaternion startRot;

    void Start()
    {
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float fac = speed*Time.realtimeSinceStartup;
        transform.rotation = startRot * Quaternion.Euler(0,0,fac);
    }
}
