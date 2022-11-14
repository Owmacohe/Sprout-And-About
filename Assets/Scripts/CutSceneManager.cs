using System;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField]
    string next;
    
    VideoPlayer cutscene;

    void Start()
    {
        cutscene = GetComponent<VideoPlayer>();
        cutscene.loopPointReached += OnEndReached;
    }

    void OnEndReached(VideoPlayer source)
    {
        SceneChanger.StaticChange(next);
    }
}