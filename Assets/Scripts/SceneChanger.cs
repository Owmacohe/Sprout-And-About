﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void Change(string name)
    {
        SceneManager.LoadScene(name);
    }
}