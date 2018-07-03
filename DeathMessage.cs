using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tamana
{
    public enum DeathPhase { Waiting, AppearingMessage, AppearingBlackScreen, VanishingBlackScreen }

    public class DeathMessage : MonoBehaviour
    {
        private static GameObject messagePrefab;
        public static GameObject blackScreenPrefab { private set; get; }
        public static GameObject resurrectionLightPrefab { private set; get; }
        public static int deathNumber { get; set; }

        private bool messageStartToAppear;
        private DeathPhase phase;

        [HideInInspector]
        public Image img;
        [HideInInspector]
        public Image txt;
        [HideInInspector]
        public Image blackScreen;

        private Vector2 txtSize;
        private float resizeSpeed;
        private float waitingTime;

        public static DeathMessage mInstance { private set; get; }

        private void Awake()
        {
            messagePrefab = GM.LoadResources("Death Message");
            blackScreenPrefab = GM.LoadResources("BlackScreen");
            resurrectionLightPrefab = GM.LoadResources("ResurrectionLight");
            mInstance = this;
            enabled = false;

            Message.RepositionMessageUI();
        }

        void Start()
        {
            messageStartToAppear = false;

            txtSize = txt.rectTransform.sizeDelta;
            txt.rectTransform.sizeDelta = txt.rectTransform.sizeDelta * 0.7f;
            resizeSpeed = Mathf.Sqrt((txtSize - txt.rectTransform.sizeDelta).sqrMagnitude / 2.0f);

            StartCoroutine(WaitBeforeTextAppear());

            phase = DeathPhase.AppearingMessage;
        }
        
        void Update()
        {
            switch(phase)
            {
                case DeathPhase.AppearingMessage        :       AppearingMessage();         break;
                case DeathPhase.AppearingBlackScreen    :       AppearingBlackScreen();     break;
                case DeathPhase.VanishingBlackScreen    :       VanishingBlackScreen();     break;
            }
        }

        public static void InstantiateMessage()
        {
            var obj = Instantiate(messagePrefab);
            mInstance.img = obj.GetComponent<UnityEngine.UI.Image>();
            mInstance.txt = obj.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();

            if(Screen.width > 1920)
            {
                mInstance.img.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.width / 8);
                mInstance.txt.rectTransform.sizeDelta = new Vector2(mInstance.img.rectTransform.sizeDelta.y * 2, mInstance.img.rectTransform.sizeDelta.y);
            }

            mInstance.img.color = Color.clear;
            mInstance.txt.color = Color.clear;

            obj.transform.SetParent(GM.canvas.transform);
            mInstance.img.rectTransform.localPosition = Vector3.zero;
            mInstance.txt.rectTransform.localPosition = Vector3.zero;
        }

        //=========================================================================================================================
        //   P H A S E S
        //=========================================================================================================================
        private void AppearingMessage()
        {
            img.color = Color.Lerp(img.color, Color.white, Time.deltaTime);
            if (messageStartToAppear)
            {
                txt.color = Color.Lerp(txt.color, Color.white, Time.deltaTime);
                txt.rectTransform.sizeDelta = Vector2.MoveTowards(txt.rectTransform.sizeDelta, txtSize, resizeSpeed * Time.deltaTime);
            }

            if (((Vector4)(txt.color - Color.white)).sqrMagnitude < 1.0f && txt.rectTransform.sizeDelta == txtSize)
            {
                StartCoroutine(WaitBeforeBlackScreenAppear());
                phase = DeathPhase.AppearingBlackScreen;
            }
        }

        private void AppearingBlackScreen()
        {
            if (blackScreen == null)
                return;

            blackScreen.color = Color.Lerp(blackScreen.color, Color.black, 2 * Time.deltaTime);

            if (Mathf.Abs(blackScreen.color.a - 1.0f) <= 0.0001f)
            {
                Destroy(img.gameObject);
                Destroy(txt.gameObject);
                ResurrectionTombstone.ResurrectPlayer();
                waitingTime = Time.time + 1.5f;

                phase = DeathPhase.VanishingBlackScreen;
            }
        }

        private void VanishingBlackScreen()
        {
            if (Time.time < waitingTime)
                return;

            blackScreen.color = Color.Lerp(blackScreen.color, Color.clear, 2 * Time.deltaTime);

            if (Mathf.Abs(blackScreen.color.a - 1.0f) >= 0.99f)
            {
                Destroy(blackScreen.gameObject);
                StartCoroutine(WaitAndPlayReviveAnimation());
                Instantiate(resurrectionLightPrefab).transform.position = GM.playerPosition;
                enabled = false;
            }
        }

        //=========================================================================================================================
        //   E N U M E R A T O R S
        //=========================================================================================================================

        public IEnumerator WaitUntilDeath()
        {
            yield return new WaitUntil(() => PlayerControl.Attack.mInstance.isDeath == true);

            StartCoroutine(WaitBeforeInstantiate());
        }

        public IEnumerator WaitBeforeInstantiate()
        {
            yield return new WaitForSeconds(2.0f);

            this.enabled = true;
            InstantiateMessage();
            Start();
        }

        private IEnumerator WaitBeforeTextAppear()
        {
            yield return new WaitForSeconds(1.0f);

            messageStartToAppear = true;
        }

        private IEnumerator WaitBeforeBlackScreenAppear()
        {
            yield return new WaitForSeconds(1.5f);

            var obj = Instantiate(blackScreenPrefab);
            blackScreen = obj.GetComponent<Image>();

            blackScreen.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            blackScreen.color = Color.clear;
            blackScreen.transform.SetParent(GM.canvas.transform);
            blackScreen.rectTransform.localPosition = Vector3.zero;
        }

        private IEnumerator WaitAndPlayReviveAnimation()
        {
            yield return new WaitForSeconds(1.0f);

            GM.playerAnimator.Play("Revive");

            StartCoroutine(WaitUntilDeath());
        }
    }
}

