using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tamana
{
    public class LevelUP : MonoBehaviour
    {
        public static Canvas canvas { get; private set; }
        public static GameObject HP { get; private set; }
        public static GameObject ST { get; private set; }
        public static GameObject AT { get; private set; }
        public static GameObject DF { get; private set; }

        private static GameObject statusPrefab { get; set; }
        private static GameObject tokenPrefab { get; set; }
        private static GameObject buttonGuidePrefab { get; set; }
        private static GameObject pointPrefab { get; set; }

        public static bool isON { get; private set; }
        public static LevelUP mInstance { get; private set; }
        public static int currentIndex { get; private set; }
        public static bool isAbleToOpenLevelUPMenu { get; set; }

        private static List<RectTransform> statsUP = new List<RectTransform>();
        private static List<RectTransform> text = new List<RectTransform>();
        private static Image[] tokenHP = { null, null, null, null, null, null, null, null, null, null, null };
        private static Image[] tokenST = { null, null, null, null, null, null, null, null, null, null, null };
        private static Image[] tokenAT = { null, null, null, null, null, null, null, null, null, null, null };
        private static Image[] tokenDF = { null, null, null, null, null, null, null, null, null, null, null };

        private static Color[] colors = { Color.blue, Color.green, Color.red, Color.yellow };

        public static GameObject statusReference { private set; get; }
        private static Image frame;
        private static RectTransform hpst;
        private static Image pointImage;
        private static TMPro.TextMeshProUGUI pointText;
        private static RectTransform buttonReference;

        private static Vector2 SIZE_SELECTED { get { return new Vector2(Screen.width * 0.3f, Screen.width * 0.3f); } }
        private static Vector2 SIZE_UNSELECTED { get { return new Vector2(Screen.width * 0.15f, Screen.width * 0.15f); } }
        private const float MAGIC_NUMBER = 3.125f;

        private void Awake()
        {
            canvas = GM.FindWithTag("Canvas").GetComponent<Canvas>();
            HP = GM.LoadResources("LevelUP HP");
            ST = GM.LoadResources("LevelUP Stamina");
            AT = GM.LoadResources("LevelUP Attack");
            DF = GM.LoadResources("LevelUP Defense");
            statusPrefab = GM.LoadResources("Status");
            tokenPrefab = GM.LoadResources("Token");
            buttonGuidePrefab = GM.LoadResources("LevelUP Button");
            pointPrefab = GM.LoadResources("Point");

            isAbleToOpenLevelUPMenu = true;

            mInstance = this;
            this.enabled = false;
        }

        private void Start()
        {
            StartCoroutine(WaitUntilPlayerDeath());
        }

        private void Update()
        {
            InputReader();

            Repositioning();
            ColorLerp();
        }

        private void InputReader()
        {
            if (DPad.GetKeyDown(DPadDirection.Left))
                currentIndex = (int)Mathf.MoveTowards(currentIndex, 0, 1);
            if (DPad.GetKeyDown(DPadDirection.Right))
                currentIndex = (int)Mathf.MoveTowards(currentIndex, 3, 1);

            if (Input.GetButtonDown("Cross"))
            {
                Hide();
                enabled = false;
                isON = false;
                return;
            }

            if (Input.GetButtonDown("Circle"))
            {
                switch(currentIndex)
                {
                    case 0: PlayerInfo.mInstance.UseLearningPoint(Stats.HP); break;
                    case 1: PlayerInfo.mInstance.UseLearningPoint(Stats.ST); break;
                    case 2: PlayerInfo.mInstance.UseLearningPoint(Stats.AT); break;
                    case 3: PlayerInfo.mInstance.UseLearningPoint(Stats.DF); break;
                }

                PlayerInfo.mInstance.ApplyLevelUP();
                InstantiateToken(text[0]);
                pointText.text = PlayerInfo.mInstance.learningPoint.ToString();
            }
        }

        private void Repositioning()
        {
            if (!isON) return;

            if(currentIndex == 0)
                statsUP[0].sizeDelta = Vector2.MoveTowards(statsUP[0].sizeDelta, SIZE_SELECTED, Screen.width * Time.deltaTime);
            else statsUP[0].sizeDelta = Vector2.MoveTowards(statsUP[0].sizeDelta, SIZE_UNSELECTED, Screen.width * Time.deltaTime);
            var posX = -(Screen.width * 0.5f) + (Screen.width * 0.125f) + (statsUP[0].sizeDelta.x * 0.5f);
            var posY = (SIZE_UNSELECTED.y - statsUP[0].sizeDelta.y) * 0.5f;
            statsUP[0].localPosition = new Vector3(posX, posY, 0.0f);

            for (int i = 1; i < 4; i++)
            {
                if (currentIndex == i)
                    statsUP[i].sizeDelta = Vector2.MoveTowards(statsUP[i].sizeDelta, SIZE_SELECTED, Screen.width * Time.deltaTime);
                else statsUP[i].sizeDelta = Vector2.MoveTowards(statsUP[i].sizeDelta, SIZE_UNSELECTED, Screen.width * Time.deltaTime);
                posX = (statsUP[i-1].localPosition.x) + (statsUP[i-1].sizeDelta.x * 0.5f) + (Screen.width * 0.015f) + (statsUP[i].sizeDelta.x * 0.5f);
                posY = (SIZE_UNSELECTED.y - statsUP[i].sizeDelta.y) * 0.5f;
                statsUP[i].localPosition = new Vector3(posX, posY, 0.0f);
            }
        }

        private void ColorLerp()
        {
            if (frame == null) return;

            Color wira = Color.Lerp(frame.color, colors[currentIndex], 3 * Time.deltaTime);
            frame.color = wira;
            pointImage.color = wira;
            pointText.color = wira;
        }

        public static void Show()
        {
            var rt = Instantiate(HP).GetComponent<RectTransform>();
            rt.sizeDelta = SIZE_SELECTED;
            rt.SetParent(canvas.transform);
            var posX = -(Screen.width * 0.5f) + (Screen.width * 0.125f) + (rt.sizeDelta.x * 0.5f);
            var posY = (SIZE_UNSELECTED.y - rt.sizeDelta.y) * 0.5f;
            rt.localPosition = new Vector3(posX, posY, 0.0f);
            statsUP.Add(rt);

            GameObject[] temp = { ST, AT, DF };
            for(int i = 0; i < 3; i++)
            {
                rt = Instantiate(temp[i]).GetComponent<RectTransform>();
                rt.sizeDelta = SIZE_UNSELECTED;
                rt.SetParent(canvas.transform);
                posX = (statsUP[i].localPosition.x) + (statsUP[i].sizeDelta.x * 0.5f) + (Screen.width * 0.015f) + (rt.sizeDelta.x * 0.5f);
                posY = SIZE_UNSELECTED.y - rt.sizeDelta.y;
                rt.localPosition = new Vector3(posX, posY, 0.0f);
                statsUP.Add(rt);
            }

            // INSTANTIATE BUTTON GUIDE
            rt = Instantiate(buttonGuidePrefab).GetComponent<RectTransform>();
            buttonReference = rt;
            rt.SetParent(canvas.transform);
            posX = (Screen.width * 0.5f) - (rt.sizeDelta.x * 0.5f);
            posY = -(Screen.height * 0.5f) + (rt.sizeDelta.y * 0.5f);
            rt.localPosition = new Vector3(posX, posY, 0.0f);

            isON = true;
            LevelUP.mInstance.enabled = true;
            currentIndex = 0;

            ShowStatus();

            // INSTANTIATE POINT
            rt = Instantiate(pointPrefab).GetComponent<RectTransform>();
            rt.SetParent(canvas.transform);
            var refRT = statusReference.GetComponent<RectTransform>();
            posX = (refRT.localPosition.x + (refRT.sizeDelta.x * 0.5f)) + (rt.sizeDelta.x * 0.5f);
            posY = (refRT.localPosition.y + (refRT.sizeDelta.y * 0.5f)) - (rt.sizeDelta.y * 0.5f);
            rt.localPosition = new Vector3(posX, posY, 0.0f);
            pointText = rt.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            pointText.text = PlayerInfo.mInstance.learningPoint.ToString();
            pointImage = rt.GetChild(1).GetComponent<Image>();

            pointText.color = Color.blue;
            pointImage.color = Color.blue;

            ItemPickUp.UI_PickItem.SetActive(false);
        }

        public static void Hide()
        {
            isON = false;
            for (int i = 0; i < 11; i++)
            {
                if (i < statsUP.Count)
                    Destroy(statsUP[i].gameObject);

                tokenHP[i] = null;
                tokenST[i] = null;
                tokenAT[i] = null;
                tokenDF[i] = null;
            }

            Destroy(buttonReference.gameObject);
            Destroy(statusReference.gameObject);
            Destroy(pointText.transform.parent.gameObject);
            statsUP.Clear();
            text.Clear();
        }

        public static void ShowStatus()
        {
            statusReference = Instantiate(statusPrefab);
            var rt = statusReference.GetComponent<RectTransform>();
            rt.SetParent(canvas.transform);
            if (hpst == null)
                hpst = GM.FindWithTag("HPST").GetComponent<RectTransform>();

            var up = hpst.localPosition.y - (hpst.sizeDelta.y * 0.5f);
            var low = SIZE_UNSELECTED.y * 0.5f;
            var sizeY = Mathf.Abs(up - low);
            var posY = low + (sizeY * 0.5f);

            var sela = rt.sizeDelta.y * 0.05f;
            var left = -(Screen.width * 0.5f) + (Screen.width * 0.125f);
            var fontY = (((sizeY - sela) - sela) * 0.9f) / 4.0f;
            var fontX = fontY * MAGIC_NUMBER;
            var tokenSizeX = fontY * 0.8f;
            var tokenSela = tokenSizeX * 0.25f;
            var sizeX = (sela * 2.5f) + fontX + (tokenSizeX * 11) + (tokenSela * 10);
            var posX = left + sizeX * 0.5f;

            rt.sizeDelta = new Vector2(sizeX, sizeY);
            rt.localPosition = new Vector3(posX, posY);

            frame = rt.GetChild(0).GetComponent<Image>();
            var body = rt.GetChild(1).GetComponent<Image>();

            frame.rectTransform.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y - sela);
            frame.rectTransform.localPosition = Vector3.zero + new Vector3(0.0f, sela * 0.5f, 0.0f);

            body.rectTransform.sizeDelta = frame.rectTransform.sizeDelta - new Vector2(sela, sela);
            body.rectTransform.localPosition = frame.rectTransform.localPosition;

            text.Add(rt.GetChild(2).GetComponent<RectTransform>());
            text.Add(rt.GetChild(3).GetComponent<RectTransform>());
            text.Add(rt.GetChild(4).GetComponent<RectTransform>());
            text.Add(rt.GetChild(5).GetComponent<RectTransform>());

            var bodyY = (body.rectTransform.sizeDelta.y * 0.1f) * 0.5f;
            sizeY = (body.rectTransform.sizeDelta.y * 0.9f) / 4.0f;
            sizeX = sizeY * MAGIC_NUMBER;
            var textSize = new Vector2(sizeX, sizeY);
            posY = (body.rectTransform.localPosition.y + (body.rectTransform.sizeDelta.y * 0.5f)) - (sizeY * 0.5f) - bodyY;
            posX = (body.rectTransform.localPosition.x - (body.rectTransform.sizeDelta.x * 0.5f)) + (sizeX * 0.5f) + bodyY;
            text[0].sizeDelta = textSize;
            text[0].localPosition = new Vector3(posX, posY, 0.0f);
            
            for(int i = 1; i < 4; i++)
            {
                posY = text[i - 1].localPosition.y - textSize.y;
                text[i].sizeDelta = textSize;
                text[i].localPosition = new Vector3(posX, posY, 0.0f);
            }

            InstantiateToken(text[0]);
        }

        private static void InstantiateToken(RectTransform text0)
        {
            var size = new Vector2(text0.sizeDelta.y * 0.8f, text0.sizeDelta.y * 0.8f);
            var posX = (text0.localPosition.x + (text0.sizeDelta.x * 0.5f)) + (size.x * 0.5f);
            var posY = text0.localPosition.y;
            var sela = size.x * 0.25f;

            for (int i = 0; i < 11; i++)
            {
                if(i < PlayerInfo.mInstance.statusPoint.HP && tokenHP[i] == null)
                {
                    var obj = Instantiate(tokenPrefab).GetComponent<Image>();
                    obj.rectTransform.SetParent(statusReference.transform);
                    obj.rectTransform.localPosition = new Vector3(posX + ((size.x * i) + (sela * i)), posY);
                    obj.rectTransform.sizeDelta = size;
                    obj.color = Color.blue;
                    tokenHP[i] = obj;
                }

                if (i < PlayerInfo.mInstance.statusPoint.ST && tokenST[i] == null)
                {
                    var obj = Instantiate(tokenPrefab).GetComponent<Image>();
                    obj.rectTransform.SetParent(statusReference.transform);
                    obj.rectTransform.localPosition = new Vector3(posX + ((size.x * i) + (sela * i)), posY - (text0.sizeDelta.y * 1));
                    obj.rectTransform.sizeDelta = size;
                    obj.color = Color.green;
                    tokenST[i] = obj;
                }

                if (i < PlayerInfo.mInstance.statusPoint.AT && tokenAT[i] == null)
                {
                    var obj = Instantiate(tokenPrefab).GetComponent<Image>();
                    obj.rectTransform.SetParent(statusReference.transform);
                    obj.rectTransform.localPosition = new Vector3(posX + ((size.x * i) + (sela * i)), posY - (text0.sizeDelta.y * 2));
                    obj.rectTransform.sizeDelta = size;
                    obj.color = Color.red;
                    tokenAT[i] = obj;
                }

                if (i < PlayerInfo.mInstance.statusPoint.DF && tokenDF[i] == null)
                {
                    var obj = Instantiate(tokenPrefab).GetComponent<Image>();
                    obj.rectTransform.SetParent(statusReference.transform);
                    obj.rectTransform.localPosition = new Vector3(posX + ((size.x * i) + (sela * i)), posY - (text0.sizeDelta.y * 3));
                    obj.rectTransform.sizeDelta = size;
                    obj.color = Color.yellow;
                    tokenDF[i] = obj;
                }
            }
        }

        public IEnumerator WaitUntilPlayerDeath()
        {
            yield return new WaitUntil(() => PlayerControl.Attack.mInstance.isDeath);

            if (statusReference != null)
                Hide();

            isAbleToOpenLevelUPMenu = false;
        }
    }
}

