using System;
using UnityEngine;

public class Hoverer : MonoBehaviour
{
    [SerializeField]
    float amplitude;
    [SerializeField]
    float speed;

    RectTransform rt;
    Vector2 startingPosition;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        startingPosition = rt.anchoredPosition;
    }

    void FixedUpdate()
    {
        rt.anchoredPosition = startingPosition + (Vector2.up * (amplitude * Mathf.Sin(Time.time * speed)));
    }
}