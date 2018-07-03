using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class Cinematic : MonoBehaviour
    {
        public static bool isCinematicON { private set; get; }
        private static bool lastBossScenario;
        private WaitForSeconds cinematicWaiter = new WaitForSeconds(0.33f);

        public Tamana.AI.BaseAI baseAI;
        public BoxCollider boxCollider;
        public Camera cinematicCamera;
        public ParticleSystem flame;
        private UnityEngine.UI.Image blackScreen;
        public Transform targetCameraPosition;
        public float cinematicCameraSpeed;
        public Transform lastBoss;

        public GameObject HPST;
        public GameObject POTION;
        public GameObject MINIMAP;
        public GameObject DESTINATION;
        public GameObject QUESTLOG;

        public AudioSource boss1;
        public AudioSource boss2;
        public AudioSource boss3;
        public Subtitle[] subtitles;

        public float timeFlamePlay;
        public float timeSpawnLastBoss;
        public enum state { begin, play, close, stop }
        public state State;

        private bool flamePlayed;
        private bool lastBossSpawned;
        public bool isPlayedBoss1;
        public bool isPlayedBoss2;
        public bool isPlayedBoss3;
        public Vector3 lastBossSpawnPosition;
        public GameObject obstacle;

        public float timeElapsedSinceCinematicStarted;
        private float startTime;
        public float timeBeforeClosing;
        public float playBoss1;
        public float playBoss2;
        public float playBoss3;

        private bool isSkipRequested;
        
        public static Cinematic mInstance;

        private void Awake()
        {
            mInstance = this;

            enabled = false;
            StartCoroutine(WaitForPlayerToCome());
        }

        // Use this for initialization
        void Start()
        {
            var navMesh = lastBoss.GetComponent<UnityEngine.AI.NavMeshAgent>();
            navMesh.enabled = false;
            lastBoss.transform.position += Vector3.up * 1000.0f;
        }

        // Update is called once per frame
        void Update()
        {
            if(!isSkipRequested)
                timeElapsedSinceCinematicStarted = Time.time - startTime;

            switch(State)
            {
                case state.begin    :       Begin();    break;
                case state.play     :       Play();     break;
                case state.close    :       Close();    break;
                case state.stop     :       Stop();     break;
            }


            // カットインをスキップ。
            if(Input.GetButtonDown("Circle") && !isSkipRequested)
            {
                for (int i = 0; i < subtitles.Length; i++)
                {
                    subtitles[i].isCreated = true;
                    subtitles[i].isDestroyed = true;
                }

                timeElapsedSinceCinematicStarted = 999.0f;
                isSkipRequested = true;
                State = state.play;
            }
        }

        private void Begin()
        {
            blackScreen.color = Vector4.MoveTowards(blackScreen.color, Color.black, 0.5f * Time.deltaTime);

            if (blackScreen.color == Color.black)
            {
                GM.mainCamera.enabled = false;
                cinematicCamera.enabled = true;

                HPST.SetActive(false);
                POTION.SetActive(false);
                MINIMAP.SetActive(false);
                DESTINATION.SetActive(false);
                QUESTLOG.SetActive(false);

                State = state.play; 
            }
        }

        private void Play()
        {
            if (blackScreen.color != Color.clear && timeElapsedSinceCinematicStarted < timeBeforeClosing)
                blackScreen.color = Vector4.MoveTowards(blackScreen.color, Color.clear, 0.5f * Time.deltaTime);

            cinematicCamera.transform.position = Vector3.MoveTowards(cinematicCamera.transform.position, targetCameraPosition.position, cinematicCameraSpeed * Time.deltaTime);

            if (timeElapsedSinceCinematicStarted > timeFlamePlay && !flamePlayed && !isSkipRequested)
            {
                flamePlayed = true;
                flame.Play();
            }

            if (timeElapsedSinceCinematicStarted > timeSpawnLastBoss && !lastBossSpawned && !isSkipRequested)
            {
                lastBossSpawned = true;
                lastBoss.transform.position = lastBossSpawnPosition;
                lastBoss.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            }

            if (timeElapsedSinceCinematicStarted > playBoss1 && !isPlayedBoss1 && !isSkipRequested)
            {
                boss1.Play();
                isPlayedBoss1 = true;
            }
            if (timeElapsedSinceCinematicStarted > playBoss2 && !isPlayedBoss2 && !isSkipRequested)
            {
                boss2.Play();
                isPlayedBoss2 = true;
            }
            if (timeElapsedSinceCinematicStarted > playBoss3 && !isPlayedBoss3 && !isSkipRequested)
            {
                boss3.Play();
                isPlayedBoss3 = true;
            }

            for (int i = 0; i < subtitles.Length; i++)
            {
                subtitles[i].Update(timeElapsedSinceCinematicStarted);
            }

            if (timeElapsedSinceCinematicStarted > timeBeforeClosing)
            {
                blackScreen.color = Vector4.MoveTowards(blackScreen.color, Color.black, 0.5f * Time.deltaTime);

                if (blackScreen.color == Color.black)
                {
                    cinematicCamera.enabled = false;
                    GM.mainCamera.enabled = true;

                     if(isSkipRequested)
                    {
                        lastBossSpawned = true;
                        lastBoss.transform.position = lastBossSpawnPosition;
                        lastBoss.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                    }

                    State = state.close;
                }
            }
        }

        private void Close()
        {
            blackScreen.color = Vector4.MoveTowards(blackScreen.color, Color.clear, 0.5f * Time.deltaTime);

            if (blackScreen.color == Color.clear)
            {
                Destroy(blackScreen.gameObject);
                Destroy(boss1.gameObject);
                Destroy(boss2.gameObject);
                Destroy(boss3.gameObject);
                for (int i = 0; i < subtitles.Length; i++)
                    Destroy(subtitles[i].text.gameObject);
                State = state.stop;
            }
        }

        private void Stop()
        {
            TurnCinematic(false);
            HPST.SetActive(true);
            POTION.SetActive(true);
            MINIMAP.SetActive(true);
            QUESTLOG.SetActive(true);
            enabled = false;

            obstacle.SetActive(true);
            QuestLog.ChangeCurrentObjective(Objective.DefeatLastBoss);

            baseAI.InstantiateBarHP();

            LastBossDetector.mInstance.BEGIN();

            baseAI.playerTransform = GM.playerTransform;
            baseAI.playerInfo = GM.playerTransform.GetComponent<PlayerInfo>();
            baseAI.playerAttackInstance = PlayerControl.Attack.mInstance;
        }

        public static void TurnCinematic(bool value)
        {
            isCinematicON = value;
        }

        private IEnumerator WaitForPlayerToCome()
        {
            while(true)
            {
                yield return cinematicWaiter;

                var player = Physics.OverlapBox(boxCollider.transform.position, boxCollider.size * 0.5f, boxCollider.transform.rotation, LayerMask.GetMask("Player"));

                if (player.Length != 0)
                {
                    enabled = true;
                    startTime = Time.time;
                    TurnCinematic(true);
                    blackScreen = Instantiate(DeathMessage.blackScreenPrefab).GetComponent<UnityEngine.UI.Image>();
                    blackScreen.rectTransform.SetParent(GM.canvas.transform);
                    blackScreen.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                    blackScreen.rectTransform.localPosition = Vector3.zero;
                    blackScreen.color = Color.clear;
                    Theme.ChangeBGM(BGM.Silence);
                    StopCoroutine(WaitForPlayerToCome());
                    break;
                }
            }
        }
    }

    [System.Serializable]
    public class Subtitle
    {
        public UnityEngine.UI.Text text;
        public float spawnAt;
        public float destroyAt;
        public bool isCreated;
        public bool isDestroyed;

        public void Update(float elapsedTime)
        {
            if(elapsedTime > spawnAt && !isCreated)
            {
                isCreated = true;
                text.rectTransform.SetParent(GM.canvas.transform);
                text.rectTransform.sizeDelta = new Vector2(Screen.width, 64.0f);
                text.rectTransform.localPosition = new Vector3(0.0f, (Screen.height * -0.5f) + 32.0f, 0.0f);
            }

            if (elapsedTime > destroyAt && !isDestroyed)
            {
                isDestroyed = true;
                text.rectTransform.SetParent(null);
            }
        }
    }
}

