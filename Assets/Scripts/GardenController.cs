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
    
    public Garden garden;

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
            garden.GrowAllPlants(true);
        }
    }

    void CreatePlant()
    {
        if (!garden.IsFull)
        {
            GameObject tempPlant = Instantiate(plants[Random.Range(0, plants.Length)], transform);
            
            Vector2 temp = garden.CreatePlant(tempPlant);

            tempPlant.transform.localPosition = new Vector3(start.x + temp.x, 0.1f, start.y - temp.y);   
            tempPlant.transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
        }
    }

    public void DestroyPlant(Vector2 pos, bool checkForFullGrowth)
    {
        if (!garden.IsEmpty)
        {
            GameObject temp = garden.DestroyPlant((int)pos.x, (int)pos.y, checkForFullGrowth);

            if (temp != null)
            {
                Destroy(temp);
            
                mulch.text = (int.Parse(mulch.text) + 1).ToString();   
            }
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