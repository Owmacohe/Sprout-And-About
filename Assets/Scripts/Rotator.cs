using System;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    float speed;


    void FixedUpdate()
    {
        transform.localEulerAngles += Vector3.up * speed;
    }
}