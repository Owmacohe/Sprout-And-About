using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//Designated to play random sounds for the plants when tearing them out
//Found through Youtube: https://www.youtube.com/watch?v=OCRzBX3ON_c
public class RandomSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioSource theSources;
    [SerializeField] AudioClip[] sfxSelector;


    public void Start()
    {
        theSources = GetComponent<AudioSource>();
    }

    public void Update()
    {
        RandomSoundPlayed();
    }
    public void RandomSoundPlayed()
    {
        AudioClip randomSound = sfxSelector[UnityEngine.Random.Range(0, sfxSelector.Length)];
        theSources.PlayOneShot(randomSound);
    }

}
