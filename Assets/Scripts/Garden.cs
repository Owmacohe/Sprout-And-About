using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Garden
{
    public enum PlantTypes { None = 0, Tulips = 1, Aloe = 2, SnakePlant = 3, BirdOfParadise = 4 }

    public class Plant
    {
        public PlantTypes Type { get; set; }
        public float Growth { get; set; }
        public Vector2 Position { get; }
        public GameObject Object { get; set; }
        public GameObject Plot { get; }
        public MeshRenderer Renderer { get; set; }

        public Plant(PlantTypes type, Vector2 pos, GameObject obj, GameObject plot)
        {
            Type = type;
            Growth = 0;
            Position = pos;
            Object = obj;
            Plot = plot;
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
    public int Stage { get; set; }

    Plant[,] plants;
    bool verbose, debug;
    
    readonly int[,] stageCounts =
    {
        {18, 0, 0, 0}, // 18
        {10, 10, 0, 0}, // 20
        {7, 7, 7, 0}, // ~22
        {6, 6, 6, 6} // 24
    };

    public Garden(int x, int y, int startingMulch, bool verbose, bool debug)
    {
        SizeX = x;
        SizeY = y;

        Mulch = startingMulch;
        
        plants = new Plant[x, y];
        
        this.verbose = verbose;
        this.debug = debug;
    }

    public void SetPlot(PlantTypes type, int x, int y, GameObject plot)
    {
        plants[x, y] = new Plant(type, new Vector2(x, y), null, plot);
    }

    public Plant GetRandomPlant()
    {
        Vector2 temp = new Vector2(Random.Range(0, SizeX), Random.Range(0, SizeY));
        
        return GetPlantFromPos((int)temp.x, (int)temp.y);
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

    float GetIndividualScore(int count, int index)
    {
        float temp = count / ((Stage > (index - 1)) ? (float)stageCounts[Stage, index] : count);

        return temp > 1 ? 1 : temp;
    }

    public float[] GetScores()
    {
        int tulipsCount = 0;
        int aloeCount = 0;
        int snakePlantCount = 0;
        int birdOfParadiseCount = 0;

        foreach (Plant i in plants)
        {
            switch (i.Type)
            {
                case PlantTypes.Tulips:
                    tulipsCount++;
                    break;
                case PlantTypes.Aloe:
                    aloeCount++;
                    break;
                case PlantTypes.SnakePlant:
                    snakePlantCount++;
                    break;
                case PlantTypes.BirdOfParadise:
                    birdOfParadiseCount++;
                    break;
            }
        }

        float[] temp = {
            GetIndividualScore(tulipsCount, 0),
            GetIndividualScore(aloeCount, 1),
            GetIndividualScore(snakePlantCount, 2),
            GetIndividualScore(birdOfParadiseCount, 3)
        };

        return temp;
    }

    public bool CanProgress()
    {
        float[] temp = GetScores();

        return (Stage == 0 && temp[0] >= 1) ||
               (Stage == 1 && temp[0] >= 1 && temp[1] >= 1) ||
               (Stage == 2 && temp[0] >= 1 && temp[1] >= 1 && temp[2] >= 1) ||
               (Stage == 3 && temp[0] >= 1 && temp[1] >= 1 && temp[2] >= 1 && temp[3] >= 1);
    }

    public Vector2 CreatePlant(PlantTypes type, GameObject obj, int x, int y, bool isInit = true)
    {
        if (plants[x, y].Object == null)
        {
            plants[x, y].Object = obj;
            plants[x, y].Renderer = obj.GetComponentInChildren<MeshRenderer>();
            plants[x, y].Type = type;

            if (isInit)
            {
                plants[x, y].Growth = 1;

                plants[x, y].Renderer.material.color = Color.red;
            }
            else
            {
                if (!debug)
                {
                    Mulch -= (int)type;
                }

                plants[x, y].Renderer.material.color = Color.green;
            }
                
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] planted");
            }

            return new Vector2(x, y);
        }
        
        if (verbose)
        {
            Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] full");
        }

        return Vector2.one * -1;
    }

    public Vector2 GrowPlant(int x, int y, float growthSpeed)
    {
        if (plants[x, y].Object != null)
        {
            plants[x, y].Growth += (Random.Range(0.05f, 0.2f) * growthSpeed) / (int)plants[x, y].Type;

            if (plants[x, y].Growth <= 1)
            {
                plants[x, y].Renderer.material.color = Color.Lerp(
                    Color.green, 
                    Color.yellow,
                    plants[x, y].Growth
                );
            }
            else
            {
                plants[x, y].Renderer.material.color = Color.Lerp(
                    Color.yellow, 
                    Color.red, 
                    -2 * (1 - plants[x, y].Growth)
                );
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

    public List<GameObject>[] GrowAllPlants(float growthSpeed)
    {
        List<GameObject> dying = new List<GameObject>();
        List<GameObject> grown = new List<GameObject>();
        List<GameObject> dead = new List<GameObject>();
        
        List<GameObject>[] temp = { dying, grown, dead };
        
        foreach (Plant i in plants)
        {
            if (i != null && i.Object != null)
            {
                GrowPlant((int)i.Position.x, (int)i.Position.y, growthSpeed);

                if (i.Growth >= 0.5 && i.Growth < 1)
                {
                    dying.Add(i.Object);
                }
                else if (i.Growth >= 1 && i.Growth < 1.5f)
                {
                    grown.Add(i.Object);
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
                if (plants[x, y].Growth < 1.5f && !debug)
                {
                    Mulch += (int)type + 1;   
                }

                if (verbose)
                {
                    Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] destroyed"); 
                }

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
}