using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] RandomSoundPlayer lose, win, create, destroy;

    public void Lose()
    {
        lose.RandomSoundPlayed();
    }
    
    public void Win()
    {
        win.RandomSoundPlayed();
    }
    
    public void Create()
    {
        create.RandomSoundPlayed();
    }
    
    public void Destroy()
    {
        destroy.RandomSoundPlayed();
    }
}