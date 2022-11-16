using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//As prescribed by Owen and some Youtube vid: https://www.youtube.com/watch?v=HtiWt0SWxk8
public class SoundPlayer : MonoBehaviour
{
   [SerializeField] AudioSource source;

    public void playTheSound()
    {
        source.Play();
    }

}
