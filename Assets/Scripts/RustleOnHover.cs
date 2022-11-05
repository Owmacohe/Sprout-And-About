using System;
using UnityEngine;

public class RustleOnHover : MonoBehaviour
{
    [SerializeField]
    float rustleTime;
    [SerializeField]
    float speed;
    [SerializeField]
    float amplitude;

    Garden garden;
    
    bool isRustling;
    float rustleStartTime;
    Transform rustle;

    void Start()
    {
        garden = FindObjectOfType<GardenController>().garden;
    }

    void FixedUpdate()
    {
        if (isRustling && rustle != null)
        {
            float elapsedTime = Time.time - rustleStartTime;

            if (elapsedTime < 1)
            {
                elapsedTime = 1;
            }
            
            if (Time.time - rustleStartTime < rustleTime)
            {
                rustle.localRotation = Quaternion.Euler(new Vector3(
                    Mathf.Sin(Time.time * speed) * (amplitude / elapsedTime),
                    rustle.localEulerAngles.y,
                    rustle.localEulerAngles.z
                ));
            }
            else
            {
                isRustling = false;
            }
        }
    }

    void OnMouseEnter()
    {
        isRustling = true;
        rustleStartTime = Time.time;

        GameObject temp = garden.GetObjectFromPlot(gameObject);
        
        rustle = (temp != null) ? temp.transform : null;
    }
}