using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Garden
{
    public enum PlantTypes { None = 0, Aloe = 1, BirdOfParadise = 2, Tulips = 3, SnakePlant = 4 }
    
    public class Plant
    {
        public PlantTypes Type { get; set; }
        public float Growth { get; set; }
        public Vector2 Position { get; }
        public GameObject Object { get; set; }
        public GameObject Plot { get; }
        public SpriteRenderer UI { get; }

        public Plant(PlantTypes type, Vector2 pos, GameObject obj, GameObject plot, SpriteRenderer ui)
        {
            Type = type;
            Growth = 0;
            Position = pos;
            Object = obj;
            Plot = plot;
            UI = ui;
        }

        public void Reset()
        {
            Type = PlantTypes.None;
            Growth = 0;
            Object = null;
        }
    }
    
    public int SizeX { get; }
    public int SizeY { get; }
    public int Mulch { get; private set; }

    Plant[,] plants;
    int planted;
    bool verbose;

    public Garden(int x, int y, int startingMulch, bool verbose)
    {
        SizeX = x;
        SizeY = y;

        Mulch = startingMulch;
        
        plants = new Plant[x, y];
        
        this.verbose = verbose;
    }

    public void SetPlot(PlantTypes type, int x, int y, GameObject plot, SpriteRenderer ui)
    {
        plants[x, y] = new Plant(type, new Vector2(x, y), null, plot, ui);
        plants[x, y].UI.enabled = false;
    }

    Vector2 GetRandomPos(bool empty)
    {
        Vector2 temp = new Vector2(Random.Range(0, SizeX), Random.Range(0, SizeY));
        
        while ((empty && plants[(int)temp.x, (int)temp.y].Object != null) ||
               (!empty && plants[(int)temp.x, (int)temp.y].Object == null))
        {
            temp = new Vector2(Random.Range(0, SizeX), Random.Range(0, SizeY));
        }

        return temp;
    }

    public Plant GetPlantFromPlot(GameObject plot)
    {
        foreach (Plant i in plants)
        {
            if (i.Plot.Equals(plot))
            {
                return i;
            }
        }

        if (verbose)
        {
            Debug.Log("<b>GARDEN:</b> unable to get plant");    
        }
        
        return null;
    }
    
    public Plant GetPlantFromPos(int x, int y)
    {
        foreach (Plant i in plants)
        {
            if (i.Position.Equals(new Vector2(x, y)))
            {
                return i;
            }
        }

        if (verbose)
        {
            Debug.Log("<b>GARDEN:</b> unable to get plant");    
        }
        
        return null;
    }
    
    public Plant GetPlantFromObject(GameObject obj)
    {
        foreach (Plant i in plants)
        {
            if (i.Object != null && i.Object.Equals(obj))
            {
                return i;
            }
        }

        if (verbose)
        {
            Debug.Log("<b>GARDEN:</b> unable to get plant");    
        }
        
        return null;
    }

    float GetIndividualDiversityScore(int count)
    {
        //float temp = (1f / planted) * -Mathf.Pow(count - (planted / 4f), 2) + 0.25f;
        float temp = -Mathf.Pow(50f, -count) + 0.25f;

        return temp > 0 ? temp : 0;
    }

    public float GetDiversity()
    {
        int aloeCount = 0;
        int birdOfParadiseCount = 0;
        int tulipsCount = 0;
        int snakePlantCount = 0;

        foreach (Plant i in plants)
        {
            switch (i.Type)
            {
                case PlantTypes.Aloe:
                    aloeCount++;
                    break;
                case PlantTypes.BirdOfParadise:
                    birdOfParadiseCount++;
                    break;
                case PlantTypes.Tulips:
                    tulipsCount++;
                    break;
                case PlantTypes.SnakePlant:
                    snakePlantCount++;
                    break;
            }
        }

        return GetIndividualDiversityScore(aloeCount) +
               GetIndividualDiversityScore(birdOfParadiseCount) +
               GetIndividualDiversityScore(tulipsCount) +
               GetIndividualDiversityScore(snakePlantCount);
    }

    public Vector2 CreatePlant(PlantTypes type, GameObject obj, int x, int y, bool isInit = true)
    {
        if (plants[x, y].Object == null)
        {
            plants[x, y].Object = obj;
            plants[x, y].Type = type;
            
            plants[x, y].UI.enabled = true;

            if (isInit)
            {
                plants[x, y].Growth = 1;
                
                plants[x, y].UI.color = Color.red;
            }
            else
            {
                Mulch -= (int)type;
                
                plants[x, y].UI.color = Color.green;
            }
                
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] planted");
            }

            planted++;

            return new Vector2(x, y);
        }
        
        if (verbose)
        {
            Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] full");
        }

        return Vector2.one * -1;
    }

    public Vector2 CreatePlant(PlantTypes type, GameObject obj, bool isInit = true)
    {
        Vector2 temp = GetRandomPos(true);

        return CreatePlant(type, obj, (int)temp.x, (int)temp.y, isInit);
    }

    public Vector2 GrowPlant(int x, int y, float growthSpeed)
    {
        if (plants[x, y].Object != null)
        {
            plants[x, y].Growth += (Random.Range(0.05f, 0.2f) * growthSpeed) / (int)plants[x, y].Type;

            if (plants[x, y].Growth <= 1)
            {
                plants[x, y].UI.color = Color.Lerp(Color.green, Color.yellow, plants[x, y].Growth);
            }
            else
            {
                plants[x, y].UI.color = Color.Lerp(Color.yellow, Color.red, -2 * (1 - plants[x, y].Growth));
            }

            /*
            if (plants[x, y].Growth >= 1)
            {
                if (verbose)
                {
                    Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] fully grown");
                }
            }
            */
                    
            return new Vector2(x, y);
        }
        else
        {
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] empty");   
            }
        }
        
        return Vector2.one * -1;
    }

    public Vector2 GrowPlant(float growthSpeed)
    {
        Vector2 temp = GetRandomPos(false);
        
        return GrowPlant((int)temp.x, (int)temp.y, growthSpeed);
    }

    public List<GameObject>[] GrowAllPlants(float growthSpeed)
    {
        List<GameObject> dying = new List<GameObject>();
        List<GameObject> dead = new List<GameObject>();
        
        List<GameObject>[] temp = { dying, dead };
        
        foreach (Plant i in plants)
        {
            if (i.Object != null)
            {
                GrowPlant((int)i.Position.x, (int)i.Position.y, growthSpeed);

                if (i.Growth >= 0.5 && i.Growth < 1)
                {
                    dying.Add(i.Object);
                }
                else if (i.Growth >= 1.5f)
                {
                    dead.Add(i.Object);
                }
            }
        }

        return temp;
    }

    public GameObject DestroyPlant(PlantTypes type, int x, int y, bool checkForFullGrowth)
    {
        Plant temp = plants[x, y];

        if (temp.Object != null)
        {
            if ((checkForFullGrowth && temp.Growth >= 1) || !checkForFullGrowth)
            {
                plants[x, y].UI.enabled = false;

                if (plants[x, y].Growth < 1.5f)
                {
                    Mulch += (int)type * 2;   
                }

                if (verbose)
                {
                    Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] destroyed"); 
                }
                
                planted--;
                
                return plants[x, y].Object;
            }
            
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] not fully grown");   
            }
        }
        else
        {
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] empty");   
            }
        }
        
        return null;
    }
    
    public GameObject DestroyPlant(PlantTypes type, bool checkForFullGrowth)
    {
        Vector2 temp = GetRandomPos(false);
        
        return DestroyPlant(type, (int)temp.x, (int)temp.y, checkForFullGrowth);
    }
}