using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Garden
{
    class Plant
    {
        public float Growth { get; set; }
        public Vector2 Position { get; private set; }
        public GameObject Object { get; set; }
        public GameObject Plot { get; }
        public SpriteRenderer UI { get; }

        public Plant(Vector2 pos, GameObject obj, GameObject plot, SpriteRenderer ui)
        {
            Growth = 0;
            Position = pos;
            Object = obj;
            Plot = plot;
            UI = ui;
        }

        public void Reset()
        {
            Growth = 0;
            Object = null;
        }
    }

    public int SizeX { get; }
    public int SizeY { get; }
    public int Mulch { get; private set; }

    Plant[,] plants;
    bool verbose;

    public Garden(int x, int y, bool verbose)
    {
        SizeX = x;
        SizeY = y;
        
        plants = new Plant[x, y];
        
        this.verbose = verbose;
    }

    public void SetPlot(int x, int y, GameObject plot, SpriteRenderer ui)
    {
        plants[x, y] = new Plant(new Vector2(x, y), null, plot, ui);
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

    public Vector2 GetPosFromPlot(GameObject plot)
    {
        foreach (Plant i in plants)
        {
            if (i.Plot.Equals(plot))
            {
                return i.Position;
            }
        }

        if (verbose)
        {
            Debug.Log("<b>GARDEN:</b> unable to get position");    
        }
        
        return Vector2.one * -1;
    }

    public GameObject GetObjectFromPlot(GameObject plot)
    {
        foreach (Plant i in plants)
        {
            if (i.Plot.Equals(plot))
            {
                return i.Object;
            }
        }

        if (verbose)
        {
            Debug.Log("<b>GARDEN:</b> unable to get object");    
        }
        
        return  null;
    }

    public void ResetPlant(int x, int y)
    {
        plants[x, y].Reset();
    }

    public bool IsPlotEmpty(int x, int y)
    {
        return plants[x, y].Object == null;
    }

    public Vector2 CreatePlant(GameObject obj, int x, int y, bool isInit = true)
    {
        if (plants[x, y].Object == null)
        {
            plants[x, y].Object = obj;
            
            plants[x, y].UI.enabled = true;

            if (isInit)
            {
                plants[x, y].Growth = 1;
                
                plants[x, y].UI.color = Color.red;
            }
            else
            {
                Mulch--;
                
                plants[x, y].UI.color = Color.green;
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

    public Vector2 CreatePlant(GameObject obj, bool isInit = true)
    {
        Vector2 temp = GetRandomPos(true);

        return CreatePlant(obj, (int)temp.x, (int)temp.y, isInit);
    }

    public Vector2 GrowPlant(int x, int y, float growthSpeed)
    {
        Plant temp = plants[x, y];

        if (temp.Object != null)
        {
            if (temp.Growth < 1)
            {
                temp.Growth += Random.Range(0.05f, 0.2f) * growthSpeed; // TODO: may need to be tweaked
                
                plants[x, y].UI.color = Color.Lerp(Color.green, Color.red, temp.Growth);

                /*
                if (temp.Growth >= 1)
                {
                    if (verbose)
                    {
                        Debug.Log("<b>GARDEN:</b> [" + (int)temp.Position.x + ", " + (int)temp.Position.y + "] fully grown");   
                    }
                }
                */
                    
                return new Vector2(x, y);
            }
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

    public List<Vector2> GrowAllPlants(float growthSpeed)
    {
        List<Vector2> grown = new List<Vector2>();
        
        foreach (Plant i in plants)
        {
            if (i.Object != null)
            {
                GrowPlant((int)i.Position.x, (int)i.Position.y, growthSpeed);

                if (i.Growth >= 1)
                {
                    grown.Add(i.Position);
                }   
            }
        }

        return grown;
    }

    public GameObject DestroyPlant(int x, int y, bool checkForFullGrowth)
    {
        Plant temp = plants[x, y];

        if (temp.Object != null)
        {
            if ((checkForFullGrowth && temp.Growth >= 1) || !checkForFullGrowth)
            {
                plants[x, y].UI.enabled = false;
                
                Mulch++;

                if (verbose)
                {
                    Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] destroyed"); 
                }
                
                return plants[x, y].Object;
            }
            else
            {
                if (verbose)
                {
                    Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] not fully grown");   
                }   
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
    
    public GameObject DestroyPlant(bool checkForFullGrowth)
    {
        Vector2 temp = GetRandomPos(false);
        
        return DestroyPlant((int)temp.x, (int)temp.y, checkForFullGrowth);
    }
}