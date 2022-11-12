using System;
using UnityEngine;

[RequireComponent(typeof(Rotator))]

public class DayNightCycle : MonoBehaviour
{
    [SerializeField]
    Color dayColour, nightColour;
    
    Light l;

    void Start()
    {
        l = GetComponent<Light>();
    }

    void FixedUpdate()
    {
        float temp = transform.eulerAngles.y;

        if (temp >= 0 && temp < 180)
        {
            l.color = Color.Lerp(dayColour, nightColour, temp / 180f);
        }
        else if (temp >= 180)
        {
            l.color = Color.Lerp(nightColour, dayColour, (temp - 180) / 180f);
        }
    }
}