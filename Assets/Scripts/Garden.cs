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

        public Plant(Vector2 pos, GameObject obj, GameObject plot)
        {
            Growth = 0;
            Position = pos;
            Object = obj;
            Plot = plot;
        }

        public void Reset()
        {
            Growth = 0;
            Object = null;
        }
    }

    public int SizeX { get; }
    public int SizeY { get; }
    public bool IsEmpty { get; private set; }
    public bool IsFull { get; private set; }
    public int Mulch { get; private set; }

    Plant[,] plants;
    List<Plant> planted;

    bool verbose;

    public Garden(int x, int y, bool verbose)
    {
        SizeX = x;
        SizeY = y;
        
        plants = new Plant[x, y];
        planted = new List<Plant>();
        
        IsEmpty = true;
        this.verbose = verbose;
    }

    public void SetPlot(int x, int y, GameObject plot)
    {
        plants[x, y] = new Plant(new Vector2(x, y), null, plot);
    }

    Vector2 GetRandomPos(bool empty)
    {
        if ((empty && IsFull) || (!empty && IsEmpty))
        {
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> unable to get position");    
            }
            
            return Vector2.one * -1;
        }
        
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

    public void ResetPlant(int x, int y)
    {
        plants[x, y].Reset();
    }

    public Vector2 CreatePlant(GameObject obj, int x, int y, bool isInit = true)
    {
        if (!IsFull)
        {
            if (plants[x, y].Object == null)
            {
                if (planted.Count == 0)
                {
                    IsEmpty = false;
                }

                plants[x, y].Object = obj;

                planted.Add(plants[x, y]);

                if (isInit)
                {
                    plants[x, y].Growth = 1;
                }
                else
                {
                    Mulch--;
                }

                if (planted.Count >= (SizeX * SizeY))
                {
                    IsFull = true;
                }
                
                if (verbose)
                {
                    Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] planted");
                }

                return new Vector2(x, y);
            }
            else
            {
                if (verbose)
                {
                    Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] full");
                }
            }
        }
        else
        {
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> full");   
            }
        }

        return Vector2.one * -1;
    }

    public Vector2 CreatePlant(GameObject obj, bool isInit = true)
    {
        Vector2 temp = GetRandomPos(true);

        return CreatePlant(obj, (int)temp.x, (int)temp.y, isInit);
    }

    public Vector2 GrowPlant(int x, int y)
    {
        if (!IsEmpty)
        {
            Plant temp = plants[x, y];

            if (temp.Object != null)
            {
                if (temp.Growth < 1)
                {
                    temp.Growth += Random.Range(0.05f, 0.2f); // TODO: may need to be tweaked
                    
                    if (temp.Growth >= 1 && verbose)
                    {
                        Debug.Log("<b>GARDEN:</b> [" + (int)temp.Position.x + ", " + (int)temp.Position.y + "] fully grown");
                    }
                    
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
        }
        else
        {
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> empty");   
            }
        }
        
        return Vector2.one * -1;
    }

    public Vector2 GrowPlant(bool verbose = false)
    {
        Vector2 temp = GetRandomPos(false);
        
        return GrowPlant((int)temp.x, (int)temp.y);
    }

    public List<Vector2> GrowAllPlants(bool verbose = false)
    {
        List<Vector2> grown = new List<Vector2>();
        
        foreach (Plant i in planted)
        {
            GrowPlant((int)i.Position.x, (int)i.Position.y);

            if (i.Growth >= 1)
            {
                grown.Add(i.Position);
            }
        }

        return grown;
    }

    public GameObject DestroyPlant(int x, int y, bool checkForFullGrowth)
    {
        if (!IsEmpty)
        {
            Plant temp = plants[x, y];

            if (temp.Object != null)
            {
                if ((checkForFullGrowth && temp.Growth >= 1) || !checkForFullGrowth)
                {
                    if (planted.Count >= (SizeX * SizeY))
                    {
                        IsFull = false;
                    }
                    
                    planted.Remove(temp);
                    Mulch++;

                    if (planted.Count == 0)
                    {
                        IsEmpty = true;
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
        }
        else
        {
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> empty");   
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