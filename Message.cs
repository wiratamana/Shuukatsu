using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tamana
{
    public class Message : MonoBehaviour
    {
        public static Image activeMessage { private set; get; }
        public static GameObject message_UI { private set; get; }

        public Sprite[] deathMessage;

        public static Message mInstance { private set; get; }
        public static GameObject messagePrefab { private set; get; }

        void Start()
        {
            mInstance = this;
        }
       
        void Update()
        {
            InputReader();
        }

        public void InputReader()
        {
            if(Input.GetButtonDown("Circle"))
            {
                StartCoroutine(WaitBeforeDestroyDeathMessage());
                message_UI.SetActive(false);

                var dest = Cinematic.mInstance.DESTINATION.GetComponent<Destination>();
                if (dest.enabled == false)
                    dest.enabled = true;

                SetUI(true);
            }
        }

        private static void InstantiateDeathMessage()
        {
            if (DeathMessage.deathNumber < 0 || DeathMessage.deathNumber >= 5)
                return;

            if(messagePrefab == null)
                messagePrefab = GM.LoadResources("DeathMessage");

            var obj = Instantiate(messagePrefab);
            var img = obj.GetComponent<Image>();
            mInstance = obj.GetComponent<Message>();
            img.sprite = mInstance.deathMessage[DeathMessage.deathNumber];
            img.rectTransform.SetParent(GM.canvas.transform);
            img.rectTransform.localPosition = Vector3.zero;
            message_UI.SetActive(true);

            PlayerInfo.mInstance.AddLearningPoint();
            PlayerInfo.mInstance.AddLearningPoint();
            PlayerInfo.mInstance.AddLearningPoint();
            PlayerControl.PotionManager.AddPotion();
            PlayerControl.PotionManager.AddPotion();
            PlayerControl.PotionManager.AddPotion();

            DeathMessage.deathNumber++;
            activeMessage = img;

            GM.playerAnimator.SetFloat("MovementSpeed", 0.0f);
            PlayerControl.Attack.mInstance.leftAnalogHorizontal = 0.0f;
            PlayerControl.Attack.mInstance.leftAnalogVertical = 0.0f;
        }

        public static void InstantiateControllerMap()
        {
            var prefab = GM.LoadResources("ControllerMap");
            var obj = Instantiate(prefab);
            var img = obj.GetComponent<Image>();

            obj.AddComponent<Message>();

            var sizeY = Screen.height / 4;

            img.rectTransform.sizeDelta = new Vector2(sizeY * 4, sizeY * 3);
            img.rectTransform.SetParent(GM.canvas.transform);
            img.rectTransform.localPosition = Vector3.zero;
            message_UI.SetActive(true);

            activeMessage = img;
            GM.playerAnimator.SetFloat("MovementSpeed", 0.0f);
            PlayerControl.Attack.mInstance.leftAnalogHorizontal = 0.0f;
            PlayerControl.Attack.mInstance.leftAnalogVertical = 0.0f;

            SetUI(false);
        }

        public static void InstantiateBosekiExplanation()
        {
            var prefab = GM.LoadResources("Boseki Explanation");
            var obj = Instantiate(prefab);
            var img = obj.GetComponent<Image>();

            obj.AddComponent<Message>();

            var sizeY = Screen.height / 4;

            img.rectTransform.sizeDelta = new Vector2(sizeY * 4, sizeY * 3);
            img.rectTransform.SetParent(GM.canvas.transform);
            img.rectTransform.localPosition = Vector3.zero;
            message_UI.SetActive(true);

            activeMessage = img;
            GM.playerAnimator.SetFloat("MovementSpeed", 0.0f);
            PlayerControl.Attack.mInstance.leftAnalogHorizontal = 0.0f;
            PlayerControl.Attack.mInstance.leftAnalogVertical = 0.0f;

            SetUI(false);
        }

        public static void RepositionMessageUI()
        {
            if (message_UI == null)
                message_UI = GM.FindWithTag("CloseMessage");

            var rt = message_UI.GetComponent<RectTransform>();
            var posX = Screen.width * 0.5f - (rt.sizeDelta.x * 0.5f);
            var posY = Screen.height * -0.5f + (rt.sizeDelta.y * 0.5f);
            rt.localPosition = new Vector3(posX, posY, 0.0f);

            message_UI.SetActive(false);
        }

        public static IEnumerator WaitAndShowDeathMessage()
        {
            yield return new WaitForSeconds(1.0f);

            InstantiateDeathMessage();
        }

        public static IEnumerator WaitBeforeDestroyDeathMessage()
        {
            yield return new WaitForEndOfFrame();

            Destroy(activeMessage.gameObject);
        }

        private static void SetUI(bool value)
        {
            Cinematic.mInstance.MINIMAP.SetActive(value);
            Cinematic.mInstance.QUESTLOG.SetActive(value);
            Cinematic.mInstance.POTION.SetActive(value);
            Cinematic.mInstance.HPST.SetActive(value);
            Cinematic.mInstance.DESTINATION.SetActive(value);
        }
    }
}

