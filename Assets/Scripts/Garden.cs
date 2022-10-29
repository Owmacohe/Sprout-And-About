public class Garden
{
    class Plant
    {
        public float Growth { get; set; }
        public Vector2 Position { get; }

        public Plant(Vector2 pos)
        {
            Growth = 0;
            Position = pos;
        }
    }

    Plant[,] plants;
    Vector2 size;

    int numPlanted;
    bool isEmpty, isFull;
    
    public Garden(Vector2 size)
    {
        this.size = size;
        plants = new Plant[size.x, size.y];
        isEmpty = true;
    }

    Vector2 GetRandomPos(bool empty)
    {
        if ((empty && isFull) || (!empty && isEmpty))
        {
            return null; // TODO
        }
        
        Vector2 temp = new Vector2(Random.Range(0, size.x), Random.Range(0, size.y));

        while ((empty && plants[temp.x, temp.y] == null) ||
               (!empty && plants[temp.x, temp.y] != null))
        {
            temp = new Vector2(Random.Range(0, size.x), Random.Range(0, size.y));
        }

        return temp;
    }

    public void CreatePlant(Vector2 pos)
    {
        if (!isFull)
        {
            if (plants[pos.x, pos, y] == null)
            {
                if (numPlanted == 0)
                {
                    isEmpty = false;
                }
                
                plants[pos.x, pos.y] = new Plant(pos);
                numPlanted++;
                
                // TODO: create in-game

                if (numPlanted >= (size.x * size.y))
                {
                    isFull = true;
                }
            }
            else
            {
                Debug.Log("<b>GARDEN:</b> " + pos + "full");
            }
        }
        else
        {
            Debug.Log("<b>GARDEN:</b> full");
        }
    }

    public void CreatePlant()
    {
        CreatePlant(GetRandomPos(false));
    }

    public void GrowPlant(Vector2 pos)
    {
        if (!isEmpty)
        {
            Plant temp = plants[pos.x, pos.y];

            if (temp != null)
            {
                if (temp.Growth < 1)
                {
                    temp.Growth += 0.1f;
                    
                    // TODO: grow in-game
                }
                else
                {
                    Debug.Log("<b>GARDEN:</b> " + temp.Position + " fully grown");
                }
            }
            else
            {
                Debug.Log("<b>GARDEN:</b> " + pos + " empty");
            }  
        }
        else
        {
            Debug.Log("<b>GARDEN:</b> empty");
        }
    }

    public void GrowPlant()
    {
        GrowPlant(GetRandomPos(true));
    }

    public void DestroyPlant(Vector2 pos)
    {
        if (!isEmpty)
        {
            Plant temp = plants[pos.x, pos.y];

            if (temp != null)
            {
                if (numPlanted >= (size.x * size.y))
                {
                    isFull = false;
                }
                
                temp = null;
                numPlanted--;

                if (numPlanted == 0)
                {
                    isEmpty = true;
                }

                // TODO: destroy in-game
            }
            else
            {
                Debug.Log("<b>GARDEN:</b> " + pos + " empty");
            }
        }
        else
        {
            Debug.Log("<b>GARDEN:</b> empty");
        }
    }
    
    public void DestroyPlant()
    {
        DestroyPlant(GetRandomPos(true));
    }
}