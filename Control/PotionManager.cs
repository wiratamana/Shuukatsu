using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tamana.PlayerControl
{
    public class PotionManager : MonoBehaviour
    {
        [Header("Setting")]
        public float cooldown;

        private static float currentCooldown;
        private static GameObject healingEffectPrefab;
        private static Image cooldownImg;
        private static TMPro.TextMeshProUGUI potionQuantityTEXT;
        private static int potionQuantity = 3;
        private static Tamana.AnimatorManager.SetBool enter;
        private static Tamana.AnimatorManager.SetBool exit;
        public static bool isTakingPotion { private set; get; }
        private static bool isCooldowing
        { get { return currentCooldown > 0.0f; } }
        private static bool isAbleToUsePotion
        {
            get { return potionQuantity > 0 && !isCooldowing; }
        }

        public static bool isUsingPotion
        {
            get { return Attack.mInstance.isUsingPotion; }
            set { Attack.mInstance.isUsingPotion = value; }
        }

        public static PotionManager mInstance { private set; get; }

        private void Awake()
        {
            mInstance = this;

            healingEffectPrefab = GM.LoadResources("Healing");
            var PotionManager = GM.FindWithTag("PotionManager");
            cooldownImg = PotionManager.transform.GetChild(2).GetComponent<Image>();
            potionQuantityTEXT = PotionManager.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>();
            potionQuantityTEXT.text = potionQuantity.ToString();

            FitToScreen();
        }

        // Update is called once per frame
        void Update()
        {
            ReadForInput();

            Cooldowning();
        }

        private void ReadForInput()
        {
            if (Input.GetButtonDown("R1"))
            {
                if (Attack.mInstance.isInterupted || !isAbleToUsePotion || Cinematic.isCinematicON)
                    return;

                UsePotion();
            }
        }

        private void Cooldowning()
        {
            if (!isCooldowing) return;

            currentCooldown = Mathf.MoveTowards(currentCooldown, 0.0f, Time.deltaTime);
            cooldownImg.fillAmount = currentCooldown / cooldown;
        }

        private void UsePotion()
        {
            currentCooldown = cooldown;
            potionQuantity--;
            potionQuantityTEXT.text = potionQuantity.ToString();

            var healPoint = 75.0f;
            if (PlayerInfo.mInstance.baseStatus.HP * 0.33f > healPoint)
                healPoint = PlayerInfo.mInstance.baseStatus.HP * 0.33f;
            PlayerInfo.mInstance.DoHeal(healPoint);

            var obj = Instantiate(healingEffectPrefab);
            obj.transform.SetParent(GM.playerTransform);
            obj.transform.localPosition = Vector3.zero + new Vector3(0.0f, 0.1f, 0.0f);

            if(enter == null)
                enter = GM.GetSetBool("Heal Enter", Attack.mInstance.animator);
            enter.enabled = true;
            if (exit == null)
                exit = GM.GetSetBool("Heal Exit", Attack.mInstance.animator);
            exit.enabled = true;

            Attack.mInstance.animator.CrossFade("Heal", 0.1f);
        }

        private void FitToScreen()
        {
            var rt = GetComponent<RectTransform>();
            var posx = (Screen.width * -0.5f) + (rt.sizeDelta.x * 0.5f) + (Screen.width * 0.1f);
            var posy = (Screen.height * -0.5f) + (rt.sizeDelta.y * 0.5f) + (Screen.height * 0.1f);

            rt.localPosition = new Vector3(posx, posy, 0.0f);
        }

        public static void PickUpPotion(GameObject obj)
        {
            if (LevelUP.statusReference != null) return;
            isTakingPotion = true;

            mInstance.StartCoroutine(mInstance.SetTakePotion());
            AddPotion();
            Destroy(obj);
            ItemPickUp.UI_PickItem.SetActive(false);
            TamanaMessenger.InstantiateMessage("Get Potion");
        }

        public static void AddPotion()
        {
            potionQuantity++;
            potionQuantityTEXT.text = potionQuantity.ToString();
        }

        public IEnumerator SetTakePotion()
        {
            while (isTakingPotion)
            {
                yield return new WaitForEndOfFrame();

                isTakingPotion = false;
            }
        }
    }
}

