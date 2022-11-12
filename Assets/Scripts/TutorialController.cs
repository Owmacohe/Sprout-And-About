using System;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    Transform triangle;
    [SerializeField]
    float speed;
    [SerializeField]
    float amplitude;
    
    GardenController gc;

    bool isPaused;
    int tutorialStage;

    bool bobVertical;
    Vector3 newPosition;

    void Start()
    {
        gc = FindObjectOfType<GardenController>();

        tutorialStage = -1;
        Continue();
    }

    void FixedUpdate()
    {
        if (isPaused)
        {
            Vector3 direction = bobVertical ? Vector3.up : Vector3.right;
            triangle.position = newPosition + (direction * (amplitude * Mathf.Sin(Time.time * speed)));
        }
    }

    public void Pause(Vector3 target, Vector2 offset, float rotation, bool vertical, bool canCreate = false, bool canDestroy = false)
    {
        print("<b>TUTORIAL:</b> pause (" + tutorialStage + ")");
        
        gc.paused = true;
        gc.canCreate = canCreate;
        gc.canDestroy = canDestroy;
        isPaused = true;

        triangle.gameObject.SetActive(true);
        triangle.position = target + (Vector3)offset;
        newPosition = triangle.position;
        triangle.eulerAngles = Vector3.forward * rotation;

        bobVertical = vertical;
    }

    public void Continue()
    {
        triangle.gameObject.SetActive(false);

        tutorialStage++;
        
        isPaused = false;
        gc.canCreate = false;
        gc.canDestroy = false;
        gc.paused = false;
        
        print("<b>TUTORIAL:</b> continue (" + tutorialStage + ")");
    }

    public bool Check(bool paused, int stage)
    {
        return isPaused == paused && tutorialStage == stage;
    }
}