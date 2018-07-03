using UnityEngine;
using System.Collections;

namespace Tamana
{
    public enum BGM { Forest, Castle, Fight, Bridge, LastBoss, Silence, Field }
    public class Theme : MonoBehaviour
    {
        public AudioClip Forest;
        public AudioClip Castle;
        public AudioClip Fight;
        public AudioClip Bridge;
        public AudioClip LastBoss;

        public AudioClip field { private set; get; }

        private bool isLooping;

        private AudioSource A;
        private AudioSource B;

        private GameObject parent;
        private GameObject playing;

        private static Theme mInstance;
        public static bool silence;

        private WaitForSeconds delay = new WaitForSeconds(0.33f);
        private BoxCollider boxCollider;

        private void Awake()
        {
            mInstance = this;
            boxCollider = transform.GetChild(0).GetComponent<BoxCollider>();
        }

        private void Start()
        {
            parent = new GameObject("Theme Parent");
            playing = new GameObject("Playing");
            A = new GameObject("Current").AddComponent<AudioSource>();
            B = new GameObject("Next").AddComponent<AudioSource>();

            A.loop = true;
            B.loop = true;

            B.transform.SetParent(parent.transform);
            A.transform.SetParent(playing.transform);

            A.clip = Forest;
            A.volume = .5f;
            A.Play();

            StartCoroutine(Repeater());
            enabled = false;
        }

        public void Update()
        {
            if(silence)
            {
                A.volume = Mathf.MoveTowards(A.volume, 0.0f, 0.33f * Time.deltaTime);
                B.volume = Mathf.MoveTowards(B.volume, 0.0f, 0.33f * Time.deltaTime);
                if (A.volume == 0.0f && B.volume == 0.0f)
                {
                    silence = false;
                    enabled = false;
                }

                return;
            }

            if (A.volume < 1.0f && B.volume > 0.0f)
            {
                A.volume = Mathf.MoveTowards(A.volume, 1.0f, 0.15f * Time.deltaTime);
                B.volume = Mathf.MoveTowards(B.volume, 0.0f, 0.33f * Time.deltaTime);
            }
            else
            {
                B.Stop();

                enabled = false; 
            }
        }

        public static void ChangeBGM(BGM bgm)
        {
            switch(bgm)
            {
                case BGM.Bridge:
                    Setting(mInstance.Bridge);
                    break;
                case BGM.Castle:
                    Setting(mInstance.Castle);
                    break;
                case BGM.Fight:
                    Setting(mInstance.Fight);
                    break;
                case BGM.Forest:
                    Setting(mInstance.Forest);
                    break;
                case BGM.LastBoss:
                    Setting(mInstance.LastBoss);
                    break;
                case BGM.Silence:
                    silence = true;
                    mInstance.A = mInstance.playing.transform.GetChild(0).GetComponent<AudioSource>();
                    mInstance.B = mInstance.parent.transform.GetChild(0).GetComponent<AudioSource>();
                    mInstance.enabled = true;
                    break;
                case BGM.Field:
                    if (mInstance.field == mInstance.Forest)
                        Setting(mInstance.Forest);
                    else Setting(mInstance.Castle);
                    break;
            }
        }

        private static void Setting(AudioClip clip)
        {
            if (mInstance.A.clip == clip) return;

            if (mInstance.parent.transform.childCount == 0)
            {
                var obj = new GameObject("Theme");
                obj.AddComponent<AudioSource>();
                obj.transform.SetParent(mInstance.playing.transform);
            }
            else
            {
                mInstance.B = mInstance.parent.transform.GetChild(0).GetComponent<AudioSource>();
                mInstance.B.transform.SetParent(mInstance.playing.transform);
            }

            mInstance.A.transform.SetParent(mInstance.parent.transform);

            mInstance.B.clip = clip;

            mInstance.B.volume = 0.5f;
            mInstance.B.Play();

            var wira = mInstance.A;
            mInstance.A = mInstance.playing.transform.GetChild(0).GetComponent<AudioSource>();
            mInstance.B = wira;

            mInstance.enabled = true;
        }

        private IEnumerator Repeater()
        {
            yield return new WaitForEndOfFrame();

            while(true)
            {
                var wira = Physics.OverlapBox(boxCollider.transform.position, boxCollider.size * 0.5f,
                    boxCollider.transform.rotation, LayerMask.GetMask("Player"));

                if (wira.Length == 1)
                    field = Castle;
                else field = Forest;

                if (A.clip == Forest && field == Castle)
                    ChangeBGM(BGM.Castle);

                if (A.clip == Castle && field == Forest)
                    ChangeBGM(BGM.Forest);

                yield return delay;
            }
        }
    } 
}
