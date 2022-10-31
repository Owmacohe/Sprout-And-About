using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GardenController : MonoBehaviour
{
    [SerializeField]
    Vector2 start;
    [SerializeField]
    GameObject plotObject;
    [SerializeField]
    GameObject[] plants;
    [SerializeField]
    TMP_Text mulch;
    
    Garden garden;

    void Start()
    {
        garden = new Garden(6, 6);
        
        Reset();
    }

    void FixedUpdate()
    {
        if (Time.time % 0.75f == 0)
        {
            CreatePlant();
        }
        
        if (Time.time % 1 == 0)
        {
            foreach (Vector2 i in garden.GrowAllPlants())
            {
                DestroyPlant(i);
                mulch.text = (int.Parse(mulch.text) + 1).ToString();
            }
        }
    }

    void CreatePlant()
    {
        if (!garden.IsFull)
        {
            GameObject tempPlant = Instantiate(plants[Random.Range(0, plants.Length)], transform);
            
            Vector2 temp = garden.CreatePlant(tempPlant);

            tempPlant.transform.localPosition = new Vector3(start.x + temp.x, 0, start.y - temp.y);   
        }
    }

    void DestroyPlant(Vector2 pos)
    {
        if (!garden.IsEmpty)
        {
            GameObject temp = garden.DestroyPlant((int)pos.x, (int)pos.y);
        
            Destroy(temp);   
        }
    }

    void Reset()
    {
        for (float i = start.x; i < start.x + garden.Size.x; i++)
        {
            for (float j = start.y; j > start.y - garden.Size.y; j--)
            {
                GameObject temp = Instantiate(plotObject, transform);

                temp.transform.localPosition = new Vector3(i, 0.1f, j);
            }
        }
    }
}