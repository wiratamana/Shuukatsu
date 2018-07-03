using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    [RequireComponent(typeof(AudioSource))]
    public class Footstep : MonoBehaviour
    {
        public AudioClip[] clips;
        private AudioSource source;

        // Use this for initialization
        void Start()
        {
            source = GetComponent<AudioSource>();
        }

        private void Step()
        {
            source.clip = GetRandomSoundFromClips;
            source.Play();
        }

        private AudioClip GetRandomSoundFromClips { get { return clips[Random.Range(0, clips.Length)]; } }

    }
}

