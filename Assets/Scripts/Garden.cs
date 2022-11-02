using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Garden
{
    class Plant
    {
        public float Growth { get; set; }
        public Vector2 Position { get; }
        public GameObject Object { get; }

        public Plant(Vector2 pos, GameObject obj)
        {
            Growth = 0;
            Position = pos;
            Object = obj;
        }
    }

    public Vector2 Size { get; }
    public bool IsEmpty { get; private set; }
    public bool IsFull { get; private set; }

    Plant[,] plants;
    List<Plant> planted;

    public Garden(int x, int y)
    {
        Size = new Vector2(x, y);
        
        plants = new Plant[x, y];
        planted = new List<Plant>();
        
        IsEmpty = true;
    }

    Vector2 GetRandomPos(bool empty, bool verbose = false)
    {
        if ((empty && IsFull) || (!empty && IsEmpty))
        {
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> unable to get position");    
            }
            
            return Vector2.one * -1;
        }
        
        Vector2 temp = new Vector2(Random.Range(0, Size.x), Random.Range(0, Size.y));

        while ((empty && plants[(int)temp.x, (int)temp.y] != null) ||
               (!empty && plants[(int)temp.x, (int)temp.y] == null))
        {
            temp = new Vector2(Random.Range(0, Size.x), Random.Range(0, Size.y));
        }

        return temp;
    }

    public Vector2 GetPosFromObject(GameObject obj, bool verbose = false)
    {
        foreach (Plant i in planted)
        {
            if (i.Object.Equals(obj))
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

    public Vector2 CreatePlant(GameObject obj, int x, int y, bool verbose = false)
    {
        if (!IsFull)
        {
            if (plants[x, y] == null)
            {
                if (planted.Count == 0)
                {
                    IsEmpty = false;
                }

                Vector2 temp = new Vector2(x, y);
                Plant tempPlant = new Plant(temp, obj);
                
                plants[x, y] = tempPlant;
                planted.Add(tempPlant);
                
                if (planted.Count >= (Size.x * Size.y))
                {
                    IsFull = true;
                }

                return temp;
            }
            
            if (verbose)
            {
                Debug.Log("<b>GARDEN:</b> [" + x + ", " + y + "] full");   
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

    public Vector2 CreatePlant(GameObject obj, bool verbose = false)
    {
        Vector2 temp = GetRandomPos(true);

        return CreatePlant(obj, (int)temp.x, (int)temp.y, verbose);
    }

    public Vector2 GrowPlant(int x, int y, bool verbose = false)
    {
        if (!IsEmpty)
        {
            Plant temp = plants[x, y];

            if (temp != null)
            {
                if (temp.Growth < 1)
                {
                    temp.Growth += Random.Range(0.05f, 0.2f); // TODO: may need to be tweaked
                    
                    if (temp.Growth >= 1 && verbose)
                    {
                        Debug.Log("<b>GARDEN:</b> " + temp.Position + " fully grown");
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
        
        return GrowPlant((int)temp.x, (int)temp.y, verbose);
    }

    public List<Vector2> GrowAllPlants(bool verbose = false)
    {
        List<Vector2> grown = new List<Vector2>();
        
        foreach (Plant i in planted)
        {
            GrowPlant((int)i.Position.x, (int)i.Position.y, verbose);

            if (i.Growth >= 1)
            {
                grown.Add(i.Position);
            }
        }

        return grown;
    }

    public GameObject DestroyPlant(int x, int y, bool checkForFullGrowth, bool verbose = false)
    {
        if (!IsEmpty)
        {
            Plant temp = plants[x, y];

            if (temp != null)
            {
                if ((checkForFullGrowth && temp.Growth >= 1) || !checkForFullGrowth)
                {
                    if (planted.Count >= (Size.x * Size.y))
                    {
                        IsFull = false;
                    }

                    plants[x, y] = null;
                    planted.Remove(temp);

                    if (planted.Count == 0)
                    {
                        IsEmpty = true;
                    }

                    return temp.Object;
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
    
    public GameObject DestroyPlant(bool checkForFullGrowth, bool verbose = false)
    {
        Vector2 temp = GetRandomPos(false);
        
        return DestroyPlant((int)temp.x, (int)temp.y, checkForFullGrowth, verbose);
    }
}