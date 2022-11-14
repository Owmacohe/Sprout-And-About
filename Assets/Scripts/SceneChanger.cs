using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static void StaticChange(string name)
    {
        SceneManager.LoadScene(name);
    }
    
    public void Change(string name)
    {
        StaticChange(name);
    }
}