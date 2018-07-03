using UnityEngine;
using UnityEngine.UI;

namespace Tamana
{
    public static class LastBossBarHP
    {
        private static Image HP;
        private static Image HP_Delay;
        private static Image HP_Frame;
        private static RectTransform BossNameText;

        public static GameObject Prefab()
        {
            GameObject obj = Resources.Load("LastBoss HP Bar") as GameObject;
            if(obj == null)
            { Debug.LogError("Couldn't find 'LastBoss HP Bar' object from 'Resources.Load' function."); return null; }

            return obj;
        }

        public static void AdjustToFitScreen(GameObject instantiatedObject, ref Image hp, ref Image hp_delay)
        {
            if(instantiatedObject == null)
            { Debug.LogError("object 'instantiatedObject' is null. This could be the 'Instantiate' method from was returned null value." +
                " Therefore, please check if 'LastBoss HP Bar' object is in appropriate 'Resources' folder."); return; }

            HP = instantiatedObject.transform.GetChild(0).GetComponent<Image>();
            HP_Delay = instantiatedObject.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            HP_Frame = instantiatedObject.transform.GetChild(0).GetChild(1).GetComponent<Image>();

            BossNameText = instantiatedObject.transform.GetChild(1).GetComponent<RectTransform>();

            hp = HP;
            hp_delay = HP_Delay;

            var rectTransform = instantiatedObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width, 180.0f);
            rectTransform.localPosition = new Vector3(0.0f, -(Screen.height * 0.5f) + 90.0f, 0.0f);

            HP.rectTransform.sizeDelta = new Vector2(Screen.width * 0.65f, 24.0f);
            HP_Delay.rectTransform.sizeDelta = new Vector2(Screen.width * 0.65f, 24.0f);
            HP_Frame.rectTransform.sizeDelta = new Vector2((Screen.width * 0.65f) + 5, 28.0f);

            var PosX = ((Screen.width * 0.5f) - (HP.rectTransform.sizeDelta.x * 0.5f)) - 50.0f;
            var PosY = -(HP.rectTransform.sizeDelta.y * 0.5f);
            HP.rectTransform.localPosition = new Vector3(PosX, PosY, 0.0f);

            PosX = (HP.rectTransform.localPosition.x) - (HP.rectTransform.sizeDelta.x * 0.5f) + (BossNameText.sizeDelta.x * 0.5f);
            PosY = (BossNameText.sizeDelta.y * 0.5f);
            BossNameText.localPosition = new Vector3(PosX, PosY, 0.0f);
        }
    }
}

