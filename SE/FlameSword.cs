using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class FlameSword : MonoBehaviour
    {
        private static GameObject flameSwordPrefab;
        private static GameObject explosionPrefab;
        private static GameObject swingPrefab;
        private static GameObject parryPrefab;

        private AudioSource audioSource;
        private float length;
        private float up;
        private float down;
        private float timeSincePlayed;
        private float elapsedTime;

        // Use this for initialization
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            length = audioSource.clip.length;
            audioSource.volume = 1.0f;

            up = length * 0.1f;
            up /= 1.0f;
            down = length * 0.1f;
            down /= 1.0f;
            timeSincePlayed = Time.time;
            audioSource.Play();
        }

        // Update is called once per frame
        void Update()
        {
            elapsedTime = Time.time - timeSincePlayed;
            if (elapsedTime < (length * 0.1f))
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, 1.0f, up * 2 * Time.deltaTime);
            if(elapsedTime > (length * 0.9f))
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0.0f, down * 2 * Time.deltaTime);

            if (!audioSource.isPlaying)
                Destroy(gameObject);

            if (elapsedTime > 5.0f)
                Destroy(gameObject);
        }

        public static void InstantiateFlameSE()
        {
            if (flameSwordPrefab == null)
                flameSwordPrefab = GM.LoadResources("FlameSwordSE");

            Instantiate(flameSwordPrefab);
        }

        public static void InstantiateExplosionSE()
        {
            if (explosionPrefab == null)
                explosionPrefab = GM.LoadResources("ExplosionSE");

            Instantiate(explosionPrefab);
        }

        public static void InstantiateSwingSE()
        {
            if (swingPrefab == null)
                swingPrefab = GM.LoadResources("SwingSE");

            Instantiate(swingPrefab);
        }

        public static void InstantiateParrySE()
        {
            if (parryPrefab == null)
                parryPrefab = GM.LoadResources("ParrySE");

            Instantiate(parryPrefab);
        }
    }
}

