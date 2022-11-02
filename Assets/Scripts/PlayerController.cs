using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Camera cam;
    GardenController gc;

    void Start()
    {
        cam = Camera.main;
        gc = FindObjectOfType<GardenController>();
    }

    void OnMove()
    {
        // character movement
    }

    void OnClick()
    {
        // selection
    }

    void OnHold()
    {
        // dragging plants
    }

    void OnFire()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out hit))
        {
            GameObject temp = hit.transform.gameObject;

            if (temp.CompareTag("Plot"))
            {
                gc.ActivatePlot(gc.garden.GetPosFromPlot(temp));
            }
        }
    }
}