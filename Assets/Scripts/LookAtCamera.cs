using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    bool useMainCam = true;

    void Start()
    {
        if (useMainCam)
        {
            cam = Camera.main;
        }
    }

    void FixedUpdate()
    {
        transform.LookAt(cam.transform);
    }
}