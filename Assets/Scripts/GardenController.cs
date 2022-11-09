using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GardenController : MonoBehaviour
{
    public bool paused;
    
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
    GameObject tulipsPrefab, aloePrefab, snakePlantPrefab, birdOfParadisePrefab;
    [SerializeField]
    Mesh[] tulipsStates, aloeStates, snakePlantStates, birdOfParadiseStates;
    [SerializeField]
    GameObject tulipsUI, aloeUI, snakePlantUI, birdOfParadiseUI;

    [SerializeField]
    RectTransform tulipsBar, aloeBar, snakePlantBar, birdOfParadiseBar;
    [SerializeField]
    GameObject tulipsMarker, aloeMarker, snakePlantMarker, birdOfParadiseMarker;
    
    public Garden garden;
    int lastMulch;
    
    Garden.PlantTypes plantType = Garden.PlantTypes.None;

    void Start()
    {
        garden = new Garden(6, 6, 10, verbose);

        Reset(false);
    }

    void FixedUpdate()
    {
        if (!paused)
        {
            List<GameObject>[] temp = garden.GrowAllPlants(growthSpeed);

            foreach (GameObject dying in temp[0])
            {
                Mesh newState = null;
            
                switch (garden.GetPlantFromObject(dying).Type)
                {
                    case Garden.PlantTypes.Tulips:
                        newState = tulipsStates[1];
                        break;
                    case Garden.PlantTypes.Aloe:
                        newState = aloeStates[1];
                        break;
                    case Garden.PlantTypes.SnakePlant:
                        newState = snakePlantStates[1];
                        break;
                    case Garden.PlantTypes.BirdOfParadise:
                        newState = birdOfParadiseStates[1];
                        break;
                }

                dying.GetComponentInChildren<MeshFilter>().mesh = newState;
            }
        
            foreach (GameObject dead in temp[1])
            {
                Mesh newState = null;
            
                switch (garden.GetPlantFromObject(dead).Type)
                {
                    case Garden.PlantTypes.Tulips:
                        newState = tulipsStates[2];
                        break;
                    case Garden.PlantTypes.Aloe:
                        newState = aloeStates[2];
                        break;
                    case Garden.PlantTypes.SnakePlant:
                        newState = snakePlantStates[2];
                        break;
                    case Garden.PlantTypes.BirdOfParadise:
                        newState = birdOfParadiseStates[2];
                        break;
                }

                dead.GetComponentInChildren<MeshFilter>().mesh = newState;
            }   
        }
    }

    void CreatePlant(int x, int y, bool isInit, bool isRandom)
    {
        if (garden.Mulch >= (int)plantType)
        {
            GameObject tempObject = null;

            switch (plantType)
            {
                case Garden.PlantTypes.Tulips:
                    tempObject = tulipsPrefab;
                    break;
                case Garden.PlantTypes.Aloe:
                    tempObject = aloePrefab;
                    break;
                case Garden.PlantTypes.SnakePlant:
                    tempObject = snakePlantPrefab;
                    break;
                case Garden.PlantTypes.BirdOfParadise:
                    tempObject = birdOfParadisePrefab;
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
            
            StartCoroutine(UpdateUI());
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
            
                StartCoroutine(UpdateUI());
            }
        }
        
        if (garden.CanProgress())
        {
            IncreaseStage();
        }
    }

    IEnumerator UpdateUI()
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

        tulipsBar.sizeDelta = new Vector2(0, 0);
        aloeBar.sizeDelta = new Vector2(0, 0);
        snakePlantBar.sizeDelta = new Vector2(0, 0);
        birdOfParadiseBar.sizeDelta = new Vector2(0, 0);

        float[] temp = garden.GetScores();

        tulipsBar.sizeDelta = new Vector2(temp[0] * 100f, 20f);
        tulipsBar.anchoredPosition = new Vector2((0.5f * temp[0] * 100f) - 200, 0);

        if (garden.Stage > 0)
        {
            aloeBar.sizeDelta = new Vector2(temp[1] * 100f, 20f);
            aloeBar.anchoredPosition = new Vector2((0.5f * temp[1] * 100f) - 100, 0);
            
            if (garden.Stage > 1)
            {
                snakePlantBar.sizeDelta = new Vector2(temp[2] * 100f, 20f);
                snakePlantBar.anchoredPosition = new Vector2((0.5f * temp[2] * 100f), 0);
                
                if (garden.Stage > 2)
                {
                    birdOfParadiseBar.sizeDelta = new Vector2(temp[3] * 100f, 20f);
                    birdOfParadiseBar.anchoredPosition = new Vector2((0.5f * temp[3] * 100f) + 100, 0);
                }
            }
        }
    }

    public void SetPlantType(string type)
    {
        tulipsUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        aloeUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        snakePlantUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        birdOfParadiseUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
    
        switch (type)
        {
            case "Tulips":
                plantType = Garden.PlantTypes.Tulips;
                tulipsUI.GetComponent<Image>().color = Color.white;
                break;
            case "Aloe":
                plantType = Garden.PlantTypes.Aloe;
                aloeUI.GetComponent<Image>().color = Color.white;
                break;
            case "SnakePlant":
                plantType = Garden.PlantTypes.SnakePlant;
                snakePlantUI.GetComponent<Image>().color = Color.white;
                break;
            case "BirdOfParadise":
                plantType = Garden.PlantTypes.BirdOfParadise;
                birdOfParadiseUI.GetComponent<Image>().color = Color.white;
                break;
        }
    }

    void IncreaseStage()
    {
        garden.Stage++;
        
        switch (garden.Stage)
        {
            case 1:
                aloeUI.SetActive(true);
                aloeMarker.SetActive(true);
                break;
            case 2:
                snakePlantUI.SetActive(true);
                snakePlantMarker.SetActive(true);
                break;
            case 3:
                birdOfParadiseUI.SetActive(true);
                birdOfParadiseMarker.SetActive(true);
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
                
                garden.SetPlot(plantType, a, b, temp);

                b++;
            }
            
            a++;
        }
        
        StartCoroutine(UpdateUI());
        
        aloeUI.SetActive(false);
        snakePlantUI.SetActive(false);
        birdOfParadiseUI.SetActive(false);
        
        aloeMarker.SetActive(false);
        snakePlantMarker.SetActive(false);
        birdOfParadiseMarker.SetActive(false);

        if (startFull)
        {
            for (int k = 0; k < garden.SizeX * garden.SizeY; k++)
            {
                CreatePlant(0, 0, true, true);
            }
        }
        
        SetPlantType("Tulips");
    }
}