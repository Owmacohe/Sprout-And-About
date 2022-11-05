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
    float growthSpeed = 0.1f;
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
        garden.GrowAllPlants(growthSpeed);
    }

    void CreatePlant(int x, int y, bool isInit, bool isRandom)
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

        tempPlant.transform.localPosition = new Vector3(start.x + temp.x, 0.2f, start.y - temp.y);   
        tempPlant.transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
        tempPlant.transform.localScale = Vector3.one * 1.2f;
    }

    public void ActivatePlot(Vector2 pos)
    {
        if (garden.IsPlotEmpty((int) pos.x, (int) pos.y))
        {
            CreatePlant((int)pos.x, (int)pos.y, false, false);
            
            UpdateMulch();
        }
        else
        {
            GameObject temp = garden.DestroyPlant((int)pos.x, (int)pos.y, true);

            if (temp != null)
            {
                Destroy(temp);
                garden.ResetPlant((int)pos.x, (int)pos.y);
            
                UpdateMulch();
            }
        }
    }

    void UpdateMulch()
    {
        mulch.text = garden.Mulch.ToString();
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
                
                garden.SetPlot(a, b, temp, temp.GetComponentInChildren<SpriteRenderer>());

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