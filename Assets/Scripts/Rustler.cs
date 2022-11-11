using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Rustler : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float amplitude;

    float randomOffset;

    void Start()
    {
        randomOffset = Random.Range(0.8f, 1.2f);
    }

    void FixedUpdate()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(
            amplitude * Mathf.Sin(Time.time * speed * randomOffset),
            transform.localEulerAngles.y,
            transform.localEulerAngles.z
        ));
    }
}