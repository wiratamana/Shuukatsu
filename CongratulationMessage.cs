using UnityEngine;
using System.Collections;

namespace Tamana
{
    public enum CongratulationMessagePhase
    {
        NULL,
        AppearingLine,
        AppearingMessage,
        DisapearringLine,
        DisappearingMessage,
        Waiting,
        SpawningMonitor,
        ResizeMonitorVertically,
        ResizeMonitorHorizontally,
        PlayVideo,
        End
    }
    public class CongratulationMessage : MonoBehaviour
    {
        public Tamana.AI.BaseAI lastBoss;
        public GameObject HPST;
        public GameObject MINIMAP;
        public GameObject POTION;
        public GameObject QUESTLOG;

        private GameObject prefab;
        private UnityEngine.UI.Image line;
        private UnityEngine.UI.Image congratulation;

        private GameObject monitorPrefab;
        private UnityEngine.Video.VideoPlayer videoPlayer;
        private GameObject monitor;

        private CongratulationMessagePhase Phase;

        private Vector2 congratulationBaseSize;
        private float resizeSpeed;
        private float timer;
        private CongratulationMessagePhase nextPhase;
        private readonly Vector3 sizeVertical = new Vector3(0.1f, 10.0f, 10.0f);
        private readonly Vector3 sizeHorizontal = new Vector3(10.0f, 10.0f, 10.0f);

        private bool condition;

        // Use this for initialization
        void Start()
        {
            prefab = GM.LoadResources("CongratulationMessage");
            monitorPrefab = GM.LoadResources("Monitor");

            StartCoroutine(Phases());
        }

        // Update is called once per frame
        void Update()
        {
            switch (Phase)
            {
                case CongratulationMessagePhase.AppearingLine               : ApearingLine();                   break;

                case CongratulationMessagePhase.AppearingMessage            : ApearingMessage();                break;

                case CongratulationMessagePhase.DisapearringLine            : DisappearingLine();               break;

                case CongratulationMessagePhase.DisappearingMessage         : DisappearingMessage();            break;

                case CongratulationMessagePhase.SpawningMonitor             : SpawningMonitor();                break;

                case CongratulationMessagePhase.ResizeMonitorVertically     : ResizeMonitorVertically();        break;

                case CongratulationMessagePhase.ResizeMonitorHorizontally   : ResizeMonitorHorizontally();      break;

                case CongratulationMessagePhase.PlayVideo                   : PlayVideo();                      break;

                case CongratulationMessagePhase.Waiting                     : Waiting();                        break;
            }
        }

        private void InstantiatePrefab()
        {
            var obj = Instantiate(prefab);
            line = obj.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            congratulation = obj.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();

            line.color = Color.clear;
            congratulation.color = Color.clear;

            obj.transform.SetParent(GM.canvas.transform);
            obj.GetComponent<RectTransform>().localPosition = Vector3.zero;

            if(Screen.width > 1920)
            {
                line.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.width / 8);
            }
        }
        private void Setting()
        {
            congratulationBaseSize = congratulation.rectTransform.sizeDelta;
            congratulation.rectTransform.sizeDelta = congratulation.rectTransform.sizeDelta * 0.7f;

            resizeSpeed = Mathf.Sqrt((congratulationBaseSize - congratulation.rectTransform.sizeDelta).sqrMagnitude / 2.0f);

            HPST.SetActive(false);
            POTION.SetActive(false);
            MINIMAP.SetActive(false);
            QUESTLOG.SetActive(false);
        }
        private void Wait(float second, CongratulationMessagePhase nextPhase)
        {
            timer = Time.time + second;
            this.nextPhase = nextPhase;
        }

        //=========================================================================================================================
        //   P H A S E S
        //=========================================================================================================================
        private void ApearingLine()
        {
            line.color = Vector4.MoveTowards(line.color, Color.white, Time.deltaTime);
        }
        private void ApearingMessage()
        {
            condition = (congratulation.color == Color.white) && (congratulation.rectTransform.sizeDelta == congratulationBaseSize);

            line.color = Vector4.MoveTowards(line.color, Color.white, Time.deltaTime);
            congratulation.color = Vector4.MoveTowards(congratulation.color, Color.white, Time.deltaTime);
            congratulation.rectTransform.sizeDelta = Vector2.MoveTowards(congratulation.rectTransform.sizeDelta, congratulationBaseSize, resizeSpeed * Time.deltaTime);

            if (condition)
            {
                Wait(2.0f, CongratulationMessagePhase.DisappearingMessage);
                congratulationBaseSize = congratulation.rectTransform.sizeDelta * 0.7f;
                Phase = CongratulationMessagePhase.Waiting; 
            }
        }

        private void DisappearingLine()
        {
            condition = line.color == Color.clear;

            congratulation.color = Vector4.MoveTowards(congratulation.color, Color.clear, Time.deltaTime);
            congratulation.rectTransform.sizeDelta = Vector2.MoveTowards(congratulation.rectTransform.sizeDelta, congratulationBaseSize, resizeSpeed * Time.deltaTime);

            line.color = Vector4.MoveTowards(line.color, Color.clear, Time.deltaTime);

            if(condition)
            {
                Wait(2.0f, CongratulationMessagePhase.SpawningMonitor);
                Phase = CongratulationMessagePhase.Waiting;
            }
        }
        private void DisappearingMessage()
        {
            congratulation.color = Vector4.MoveTowards(congratulation.color, Color.clear, Time.deltaTime);
            congratulation.rectTransform.sizeDelta = Vector2.MoveTowards(congratulation.rectTransform.sizeDelta, congratulationBaseSize, resizeSpeed * Time.deltaTime);
        }

        private void SpawningMonitor()
        {
            monitor = Instantiate(monitorPrefab);

            videoPlayer = monitor.GetComponentInChildren<UnityEngine.Video.VideoPlayer>();

            monitor.transform.localScale = Vector3.one * 0.1f;

            monitor.transform.position = new Vector3(170.18f, 52.34f, 60.27f);

            Phase = CongratulationMessagePhase.ResizeMonitorVertically;
        }
        private void ResizeMonitorVertically()
        {
            monitor.transform.localScale = Vector3.MoveTowards(monitor.transform.localScale, sizeVertical, 10 * Time.deltaTime);

            condition = monitor.transform.localScale.y == 10.0f;
            if (condition)
                Phase = CongratulationMessagePhase.ResizeMonitorHorizontally;
        }
        private void ResizeMonitorHorizontally()
        {
            monitor.transform.localScale = Vector3.MoveTowards(monitor.transform.localScale, sizeHorizontal, 10 * Time.deltaTime);

            condition = monitor.transform.localScale.x == 10.0f;
            if (condition)
            {
                Wait(2.0f, CongratulationMessagePhase.PlayVideo);
                Phase = CongratulationMessagePhase.Waiting;
            }
        }
        private void PlayVideo()
        {
            // ビデオはPhase()から再生する。
        }

        private void Waiting()
        {
            if(Time.time > timer)
            {
                Phase = nextPhase;
                return;
            }
        }

        //=========================================================================================================================
        //   E N U M E R A T O R S
        //=========================================================================================================================
        private IEnumerator Phases()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => lastBoss.isDeath);
            QuestLog.ChangeCurrentObjective(Objective.Completed);
            yield return new WaitForSeconds(5.0f);

            InstantiatePrefab();
            Setting();
            Phase = CongratulationMessagePhase.AppearingLine;

            yield return new WaitForSeconds(1.0f);
            Phase = CongratulationMessagePhase.AppearingMessage;

            yield return new WaitUntil(() => Phase == CongratulationMessagePhase.DisappearingMessage);
            yield return new WaitForSeconds(1.0f);
            Phase = CongratulationMessagePhase.DisapearringLine;

            yield return new WaitUntil(() => Phase == CongratulationMessagePhase.ResizeMonitorVertically);
            videoPlayer.Play();
            yield return new WaitUntil(() => videoPlayer.isPlaying);
            videoPlayer.Pause();
            yield return new WaitUntil(() => Phase == CongratulationMessagePhase.PlayVideo);
            videoPlayer.Play();
            yield return new WaitUntil(() => videoPlayer.isPlaying);
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
            Phase = CongratulationMessagePhase.End;
            Debug.Log("END!");
        }
    }
}

