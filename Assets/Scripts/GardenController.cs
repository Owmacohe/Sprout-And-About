using System;
using System.Collections;
using System.Collections.Generic;
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
    GameObject mulch, mulchBag;

    [SerializeField]
    GameObject aloePrefab, birdOfParadisePrefab, tulipsPrefab, snakePlantPrefab;
    [SerializeField]
    Mesh[] aloeStates, birdOfParadiseStates, tulipsStates, snakePlantStates;
    [SerializeField]
    GameObject aloeUI, birdOfParadiseUI, tulipsUI, snakePlantUI;
    
    public Garden garden;
    int gardenStage;
    int lastMulch;
    
    Garden.PlantTypes plantType = Garden.PlantTypes.Aloe;

    void Start()
    {
        garden = new Garden(6, 6, 10, verbose);

        Reset(false);
    }

    void FixedUpdate()
    {
        List<GameObject>[] temp = garden.GrowAllPlants(growthSpeed);

        foreach (GameObject dying in temp[0])
        {
            Mesh newState = null;
            
            switch (garden.GetPlantFromObject(dying).Type)
            {
                case Garden.PlantTypes.Aloe:
                    newState = aloeStates[1];
                    break;
                case Garden.PlantTypes.BirdOfParadise:
                    newState = birdOfParadiseStates[1];
                    break;
                case Garden.PlantTypes.Tulips:
                    newState = tulipsStates[1];
                    break;
                case Garden.PlantTypes.SnakePlant:
                    newState = snakePlantStates[1];
                    break;
            }

            dying.GetComponentInChildren<MeshFilter>().mesh = newState;
        }
        
        foreach (GameObject dead in temp[1])
        {
            Mesh newState = null;
            
            switch (garden.GetPlantFromObject(dead).Type)
            {
                case Garden.PlantTypes.Aloe:
                    newState = aloeStates[2];
                    break;
                case Garden.PlantTypes.BirdOfParadise:
                    newState = birdOfParadiseStates[2];
                    break;
                case Garden.PlantTypes.Tulips:
                    newState = tulipsStates[2];
                    break;
                case Garden.PlantTypes.SnakePlant:
                    newState = snakePlantStates[2];
                    break;
            }

            dead.GetComponentInChildren<MeshFilter>().mesh = newState;
        }
    }

    void CreatePlant(int x, int y, bool isInit, bool isRandom)
    {
        if (garden.Mulch >= (int)plantType)
        {
            GameObject tempObject = null;

            switch (plantType)
            {
                case Garden.PlantTypes.Aloe:
                    tempObject = aloePrefab;
                    break;
                case Garden.PlantTypes.BirdOfParadise:
                    tempObject = birdOfParadisePrefab;
                    break;
                case Garden.PlantTypes.Tulips:
                    tempObject = tulipsPrefab;
                    break;
                case Garden.PlantTypes.SnakePlant:
                    tempObject = snakePlantPrefab;
                    break;
            }
        
            GameObject tempPlant = Instantiate(tempObject, transform);

            Vector2 temp;

            if (isRandom)
            {
                temp = garden.CreatePlant(plantType, tempPlant, isInit);
            }
            else
            {
                temp = garden.CreatePlant(plantType, tempPlant, x, y, isInit);
            }

            tempPlant.transform.localPosition = new Vector3(start.x + temp.x, 0.2f, start.y - temp.y);   
            tempPlant.transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            tempPlant.transform.localScale = Vector3.one * 1.2f;   
        }
        else
        {
            Debug.Log("<b>GARDEN CONTROLLER:</b> [" + x + ", " + y + "] not enough mulch");
        }
    }

    public void ActivatePlot(Vector2 pos)
    {
        if (garden.GetPlantFromPos((int)pos.x, (int)pos.y).Object == null)
        {
            CreatePlant((int)pos.x, (int)pos.y, false, false);
            
            StartCoroutine(UpdateMulch());
        }
        else
        {
            GameObject temp = garden.DestroyPlant(
                garden.GetPlantFromPos((int)pos.x, (int)pos.y).Type,
                (int)pos.x,
                (int)pos.y,
                true
            );

            if (temp != null)
            {
                Destroy(temp);
                garden.GetPlantFromPos((int) pos.x, (int) pos.y).Reset();
            
                StartCoroutine(UpdateMulch());

                if ((gardenStage == 0 && garden.Mulch >= 15) ||
                    (gardenStage == 1 && garden.Mulch >= 30) ||
                    (gardenStage == 2 && garden.Mulch >= 60))
                {
                    IncreaseStage();
                }
            }
        }
    }

    IEnumerator UpdateMulch()
    {
        if (garden.Mulch >= lastMulch)
        {
            for (int i = 0; i < garden.Mulch - lastMulch; i++)
            {
                Instantiate(mulchBag, mulch.transform);
            }
        }
        else
        {
            for (int i = 0; i < lastMulch - garden.Mulch; i++)
            {
                Destroy(mulch.transform.GetChild(0).gameObject);
                yield return new WaitForSeconds(0.01f);
            }
        }

        lastMulch = garden.Mulch;
        
        print(garden.GetDiversity());
    }

    public void SetPlantType(string type)
    {
        switch (type)
        {
            case "Aloe":
                plantType = Garden.PlantTypes.Aloe;
                break;
            case "BirdOfParadise":
                plantType = Garden.PlantTypes.BirdOfParadise;
                break;
            case "Tulips":
                plantType = Garden.PlantTypes.Tulips;
                break;
            case "SnakePlant":
                plantType = Garden.PlantTypes.SnakePlant;
                break;
        }
    }

    void IncreaseStage()
    {
        gardenStage++;
        
        switch (gardenStage)
        {
            case 1:
                birdOfParadiseUI.SetActive(true);
                break;
            case 2:
                tulipsUI.SetActive(true);
                break;
            case 3:
                snakePlantUI.SetActive(true);
                break;
        }
    }

    void Reset(bool startFull)
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
                
                garden.SetPlot(plantType, a, b, temp, temp.GetComponentInChildren<SpriteRenderer>());

                b++;
            }
            
            a++;
        }
        
        StartCoroutine(UpdateMulch());
        
        /*
        birdOfParadiseUI.SetActive(false);
        tulipsUI.SetActive(false);
        snakePlantUI.SetActive(false);
        */

        if (startFull)
        {
            for (int k = 0; k < garden.SizeX * garden.SizeY; k++)
            {
                CreatePlant(0, 0, true, true);
            }   
        }
    }
}