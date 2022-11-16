using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//Designated to play random sounds for the plants when tearing them out
//Found through Youtube: https://www.youtube.com/watch?v=OCRzBX3ON_c
public class RandomSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] sfxSelector;
    [SerializeField] bool playOnStart;
    [SerializeField] bool loop;
    [SerializeField] float volume;
    
    AudioSource theSources;

    void Start()
    {
        theSources = gameObject.AddComponent<AudioSource>();
        theSources.loop = loop;
        theSources.volume = volume;
        
        if (playOnStart)
        {
            RandomSoundPlayed();
        }
    }

    /*
    // we probably don't need to play a new sound every frame
    public void Update()
    {
        RandomSoundPlayed();
    }
    */

    public void RandomSoundPlayed()
    {
        AudioClip randomSound = sfxSelector[UnityEngine.Random.Range(0, sfxSelector.Length)];
        theSources.clip = randomSound;
        theSources.Play();
    }

}