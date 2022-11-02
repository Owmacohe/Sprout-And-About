using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GardenController : MonoBehaviour
{
    [SerializeField]
    bool verbose;
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
        garden = new Garden(6, 6, verbose);
        
        Reset();
    }

    void FixedUpdate()
    {
        if (Time.time % 1 == 0)
        {
            garden.GrowAllPlants(true);
        }
    }

    void CreatePlant(int x, int y, bool isInit, bool isRandom)
    {
        if (!garden.IsFull)
        {
            GameObject tempPlant = Instantiate(plants[Random.Range(0, plants.Length)], transform);

            Vector2 temp;

            if (isRandom)
            {
                temp = garden.CreatePlant(tempPlant, isInit);
            }
            else
            {
                temp = garden.CreatePlant(tempPlant, x, y, isInit);
            }

            tempPlant.transform.localPosition = new Vector3(start.x + temp.x, 0.1f, start.y - temp.y);   
            tempPlant.transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
        }
    }

    public void ActivatePlot(Vector2 pos)
    {
        if (!garden.IsEmpty)
        {
            GameObject temp = garden.DestroyPlant((int)pos.x, (int)pos.y, true);

            if (temp != null)
            {
                Destroy(temp);
                garden.ResetPlant((int)pos.x, (int)pos.y);
            
                mulch.text = (int.Parse(mulch.text) + 1).ToString();   
            }
            else
            {
                CreatePlant((int)pos.x, (int)pos.y, false, false);
            }
        }
    }

    void Reset()
    {
        int a = 0;
        int b = 0;
        
        for (float i = start.x; i < start.x + garden.SizeX; i++)
        {
            b = 0;
            
            for (float j = start.y; j > start.y - garden.SizeY; j--)
            {
                GameObject temp = Instantiate(plotObject, transform);

                temp.transform.localPosition = new Vector3(i, 0.1f, j);
                
                garden.SetPlot(a, b, temp);

                b++;
            }
            
            a++;
        }

        for (int k = 0; k < garden.SizeX * garden.SizeY; k++)
        {
            CreatePlant(0, 0, true, true);
        }
    }
}