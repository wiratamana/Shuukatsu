using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public enum Objective { ReachCastle, ReachLastBoss, DefeatLastBoss, Completed }

    public class QuestLog : MonoBehaviour
    {
        public RectTransform minimap;
        public static UnityEngine.UI.Image image;

        private static Sprite objective0;
        private static Sprite objective1;
        private static Sprite objective2;
        private static Sprite objective3;

        private static Sprite questUpdate1;
        private static Sprite questUpdate2;
        private static Sprite questUpdate3;
        private static Sprite questClear;

        private static GameObject questUpdatePrefab;

        public static Objective currentObjective { private set; get; }

        // Use this for initialization
        void Start()
        {
            image = GetComponent<UnityEngine.UI.Image>();

            StartCoroutine(Repeater());
        }

        //=========================================================================================================================
        //   I N I T I A L I Z A T I O N
        //=========================================================================================================================
        private void LoadResources()
        {
            objective0 = Resources.Load<Sprite>("Objectives/Objective1");
            objective1 = Resources.Load<Sprite>("Objectives/Objective2");
            objective2 = Resources.Load<Sprite>("Objectives/Objective3");
            objective3 = Resources.Load<Sprite>("Objectives/Objective4");

            questUpdate1 = Resources.Load<Sprite>("Objectives/QuestUpdate1");
            questUpdate2 = Resources.Load<Sprite>("Objectives/QuestUpdate2");
            questUpdate3 = Resources.Load<Sprite>("Objectives/QuestUpdate3");
            questClear = Resources.Load<Sprite>("Objectives/QuestClear");

            questUpdatePrefab = GM.LoadResources("QuestUpdate");
        }
        private void Resize()
        {
            image.rectTransform.sizeDelta = new Vector2(Screen.height / 3, Screen.height / 6);
        }
        private void Reposition()
        {
            var mapsize = minimap.sizeDelta.y * 0.5f;
            var mapPos = minimap.parent.GetComponent<RectTransform>().localPosition;
            var mySize = image.rectTransform.sizeDelta.y * 0.5f;
            image.rectTransform.localPosition = new Vector3(mapPos.x, mapPos.y - mapsize - mySize - 36.0f);
        }

        //=========================================================================================================================
        //   U P D A T E
        //=========================================================================================================================
        public static void ChangeCurrentObjective(Objective objective)
        {
            currentObjective = objective;

            switch(objective)
            {
                case Objective.ReachCastle      : image.sprite = objective0; InstantiateQuestUpdate(questUpdate1); break;
                case Objective.ReachLastBoss    : image.sprite = objective1; InstantiateQuestUpdate(questUpdate2); break;
                case Objective.DefeatLastBoss   : image.sprite = objective2; InstantiateQuestUpdate(questUpdate3); break;
                case Objective.Completed        : image.sprite = objective3; InstantiateQuestUpdate(questClear); break;
            }

            // ReachLastBoss   = DestinationクラスのCheckIfPlayerIsInsideCurrentDestinationBoxColliderメソットから値を変えられる。
            // DefeatLastBoss  = CinematicのStopメソットから値を変えられる。
            // Completed       = CongratulationMessageのPhasesメソットから値を変えられる。
        }

        public static void InstantiateQuestUpdate(Sprite sprite)
        {
            var obj = Instantiate(questUpdatePrefab);
            var img = obj.GetComponent<UnityEngine.UI.Image>();

            var ySize = Screen.height / 5.0f;

            img.sprite = sprite;
            img.color = Color.clear;
            img.rectTransform.sizeDelta = new Vector2(ySize * 3.0f, ySize);
            img.rectTransform.SetParent(GM.canvas.transform);
            img.rectTransform.localPosition = new Vector3((Screen.width * -0.5f) + (ySize * 1.5f), 0.0f, 0.0f);
        }

        //=========================================================================================================================
        //   E N U M E R A T O R
        //=========================================================================================================================
        private IEnumerator Repeater()
        {
            yield return new WaitForEndOfFrame();

            LoadResources();
            Resize();
            Reposition();
        }
    }
}

