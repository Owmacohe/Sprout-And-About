using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GardenController : MonoBehaviour
{
    public bool paused, debug;
    [HideInInspector]
    public bool canCreate, canDestroy;

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

    [SerializeField]
    GameObject loseFadeout, winFadeout;

    bool loseFadingOut, winFadingOut;
    
    public Garden garden;
    int lastMulch;
    TutorialController tc;
    SoundManager sm;

    List<GameObject> dyingChecker;
    List<GameObject> deadChecker;
    
    Garden.PlantTypes plantType = Garden.PlantTypes.None;

    void Start()
    {
        garden = new Garden(6, 6, 10, verbose, debug);
        tc = FindObjectOfType<TutorialController>();
        sm = FindObjectOfType<SoundManager>();

        dyingChecker = new List<GameObject>();
        deadChecker = new List<GameObject>();

        Invoke(nameof(Reset), 0.1f);
    }

    void FixedUpdate()
    {
        if (!paused && !debug)
        {
            List<GameObject>[] temp = garden.GrowAllPlants(growthSpeed);

            foreach (GameObject dying in temp[0])
            {
                if (!dyingChecker.Contains(dying))
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
                
                    dyingChecker.Add(dying);   
                }
            }

            if (temp[1].Count > 0 && tc.Check(false, 2))
            {
                tc.Pause(
                    Camera.main.WorldToScreenPoint(temp[1][Random.Range(0, temp[1].Count)].transform.position),
                    Vector2.up * 100,
                    180,
                    true,
                    false,
                    true
                );
            }
        
            foreach (GameObject dead in temp[2])
            {
                if (!deadChecker.Contains(dead))
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
                    dead.GetComponentInChildren<Canvas>().enabled = true;
                
                    dead.GetComponent<RandomSoundPlayer>().RandomSoundPlayed();
                    
                    deadChecker.Add(dead);
                }
            }
            
            if (garden.Mulch <= 0 && garden.Planted == temp[2].Count)
            {
                loseFadingOut = true;
            }
        }

        if (loseFadingOut || winFadingOut)
        {
            GameObject fadeout = loseFadingOut ? loseFadeout : winFadeout;

            if (!fadeout.activeSelf)
            {
                paused = true;
                fadeout.SetActive(true);
                
                if (loseFadingOut)
                {
                    sm.Lose();   
                }
                else if (winFadingOut)
                {
                    sm.Win();
                }
            }
            
            foreach (Image i in fadeout.GetComponentsInChildren<Image>())
            {
                Color temp = i.color;
                float opacity = temp.a;

                if (opacity < 0.9f)
                {
                    i.color = new Color(temp.r, temp.g, temp.b, opacity + 0.01f);
                }
            }
        }
    }

    bool CreatePlant(int x, int y, bool isInit)
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

            temp = garden.CreatePlant(plantType, tempPlant, x, y, isInit);

            tempPlant.transform.localPosition = new Vector3(start.x + temp.x, 0.3f, start.y - temp.y);   
            tempPlant.transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
            tempPlant.transform.localScale = Vector3.one * 1.2f;

            return true;
        }
        
        Debug.Log("<b>GARDEN CONTROLLER:</b> [" + x + ", " + y + "] not enough mulch");
        return false;
    }

    public void ActivatePlot(Vector2 pos)
    {
        // Tutorial step 1: place plant in plot
        if (tc.Check(true, 1))
        {
            tc.Continue();
        }

        bool createValid = !paused || (paused && canCreate);
        bool destroyValid = !paused || (paused && canDestroy);

        if (createValid || destroyValid)
        {
            GameObject targetPlant = garden.GetPlantFromPos((int) pos.x, (int) pos.y).Object;
            
            if (createValid && targetPlant == null)
            {
                bool temp = CreatePlant((int)pos.x, (int)pos.y, false);

                if (temp)
                {
                    StartCoroutine(UpdateUI());
                
                    sm.Create();   
                }
            }
            else if (destroyValid && targetPlant != null)
            {
                // Tutorial step 2: destroy plant
                if (tc.Check(true, 2))
                {
                    tc.Continue();
                }
                
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
                    
                    sm.Destroy();
                }
            }
        
            if (garden.CanProgress())
            {
                IncreaseStage();
            }   
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

        tulipsBar.sizeDelta = new Vector2(temp[0] * 100f, 30);
        tulipsBar.anchoredPosition = new Vector2((0.5f * temp[0] * 100f) - 50, 120);

        if (garden.Stage > 0)
        {
            aloeBar.sizeDelta = new Vector2(temp[1] * 100f, 30);
            aloeBar.anchoredPosition = new Vector2((0.5f * temp[1] * 100f) - 50, 80);
            
            if (garden.Stage > 1)
            {
                snakePlantBar.sizeDelta = new Vector2(temp[2] * 100f, 30);
                snakePlantBar.anchoredPosition = new Vector2((0.5f * temp[2] * 100f) - 50, 40);
                
                if (garden.Stage > 2)
                {
                    birdOfParadiseBar.sizeDelta = new Vector2(temp[3] * 100f, 30);
                    birdOfParadiseBar.anchoredPosition = new Vector2((0.5f * temp[3] * 100f) - 50, 0);
                }
            }
        }
    }

    public void SetPlantType(string type)
    {
        tulipsUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        aloeUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        snakePlantUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        birdOfParadiseUI.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
    
        switch (type)
        {
            case "Tulips":
                // Tutorial step 0: click tulip in menu
                if (tc.Check(true, 0))
                {
                    tc.Continue();
                    tc.Pause(
                        Camera.main.WorldToScreenPoint(garden.GetRandomPlant().Plot.transform.position), 
                        Vector2.up * 100,
                        180,
                        true,
                        true
                    );
                }
                
                plantType = Garden.PlantTypes.Tulips;
                tulipsUI.GetComponent<Image>().color = Color.white;
                break;
            case "Aloe":
                // Tutorial step 3: click aloe in menu
                if (tc.Check(true, 3))
                {
                    tc.Continue();
                }

                plantType = Garden.PlantTypes.Aloe;
                aloeUI.GetComponent<Image>().color = Color.white;
                break;
            case "SnakePlant":
                // Tutorial step 4: click snake plant in menu
                if (tc.Check(true, 4))
                {
                    tc.Continue();
                }
                
                plantType = Garden.PlantTypes.SnakePlant;
                snakePlantUI.GetComponent<Image>().color = Color.white;
                break;
            case "BirdOfParadise":
                // Tutorial step 5: click bird of paradise in menu
                if (tc.Check(true, 5))
                {
                    tc.Continue();
                }
                
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
                tc.Pause(aloeUI.transform.position, Vector2.up * 100, 180, true);
                
                aloeUI.SetActive(true);
                aloeMarker.SetActive(true);
                break;
            case 2:
                tc.Pause(snakePlantUI.transform.position, Vector2.up * 100, 180, true);
                
                snakePlantUI.SetActive(true);
                snakePlantMarker.SetActive(true);
                break;
            case 3:
                tc.Pause(birdOfParadiseUI.transform.position, Vector2.up * 100, 180, true);
                
                birdOfParadiseUI.SetActive(true);
                birdOfParadiseMarker.SetActive(true);
                break;
            case 4:
                winFadingOut = true;
                break;
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
                
                garden.SetPlot(plantType, a, b, temp);

                b++;
            }
            
            a++;
        }
        
        StartCoroutine(UpdateUI());

        if (!debug)
        {
            aloeUI.SetActive(false);
            snakePlantUI.SetActive(false);
            birdOfParadiseUI.SetActive(false);
        
            aloeMarker.SetActive(false);
            snakePlantMarker.SetActive(false);
            birdOfParadiseMarker.SetActive(false);   
        }
        
        SetPlantType("Tulips");
        
        Invoke(nameof(ResetTutorial), 0.5f);
    }

    void ResetTutorial()
    {
        tc.Pause(tulipsUI.transform.position, Vector2.up * 100, 180, true);
    }
}