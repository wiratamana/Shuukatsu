using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class Destination : MonoBehaviour
    {
        public static Sprite n0 { private set; get; }
        public static Sprite n1 { private set; get; }
        public static Sprite n2 { private set; get; }
        public static Sprite n3 { private set; get; }
        public static Sprite n4 { private set; get; }
        public static Sprite n5 { private set; get; }
        public static Sprite n6 { private set; get; }
        public static Sprite n7 { private set; get; }
        public static Sprite n8 { private set; get; }
        public static Sprite n9 { private set; get; }
        public static Sprite meter { private set; get; }

        [Header("RectTransforms")]
        public RectTransform parent;
        public RectTransform image;
        public RectTransform arrowParent;
        public RectTransform arrowTop;
        public RectTransform arrowRight;
        public RectTransform arrowBot;
        public RectTransform arrowLeft;
        public RectTransform mokuteki;
        public RectTransform distanceParent;
        public RectTransform distance0;
        public RectTransform distance1;
        public RectTransform distance2;
        public RectTransform distance3;
        public RectTransform distance4;

        public UnityEngine.UI.Image dist0;
        public UnityEngine.UI.Image dist1;
        public UnityEngine.UI.Image dist2;
        public UnityEngine.UI.Image dist3;
        public UnityEngine.UI.Image dist4;

        [Header("RectTransforms")]
        public BoxCollider destination1;
        public BoxCollider destination2;

        private bool isDestination1Completed;
        private Vector3 currentDestination
        {
            get
            {
                if (isDestination1Completed) return destination2.transform.position;
                return destination1.transform.position;
            }
        }
        private bool isCameraFacingCurrentObjectivePosition;
        private int number;

        private readonly WaitForSeconds delay = new WaitForSeconds(0.33f);

        private const float MULTIPLE_BY = 15.0f;
        public static Destination mInstance { private set; get; }

        private void Awake()
        {
            mInstance = this;
            LoadResources();

            Resize();
            Reposition();

            GetDistanceImageComponent();
        }

        private void Start()
        {
            StartCoroutine(Repeater());
        }

        private void Update()
        {
            RotateArrowParent();
            UpdatePositionUI();
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(Repeater());
            enabled = true;
        }

        //=========================================================================================================================
        //   I N I T I A L I Z A T I O N
        //=========================================================================================================================
        private void LoadResources()
        {
            n0 = Resources.Load<Sprite>("DestinationNumbers/Destination0");
            n1 = Resources.Load<Sprite>("DestinationNumbers/Destination1");
            n2 = Resources.Load<Sprite>("DestinationNumbers/Destination2");
            n3 = Resources.Load<Sprite>("DestinationNumbers/Destination3");
            n4 = Resources.Load<Sprite>("DestinationNumbers/Destination4");
            n5 = Resources.Load<Sprite>("DestinationNumbers/Destination5");
            n6 = Resources.Load<Sprite>("DestinationNumbers/Destination6");
            n7 = Resources.Load<Sprite>("DestinationNumbers/Destination7");
            n8 = Resources.Load<Sprite>("DestinationNumbers/Destination8");
            n9 = Resources.Load<Sprite>("DestinationNumbers/Destination9");
            meter = Resources.Load<Sprite>("DestinationNumbers/DestinationM");
        }
        private void Resize()
        {
            var X = Screen.height / MULTIPLE_BY;
            var size = new Vector2(X, X);

            parent.sizeDelta = size;
            image.sizeDelta = size / 2.0f;
            arrowParent.sizeDelta = size;

            arrowTop.sizeDelta = size * 0.25f;
            arrowLeft.sizeDelta = size * 0.25f;
            arrowBot.sizeDelta = size * 0.25f;
            arrowRight.sizeDelta = size * 0.25f;

            mokuteki.sizeDelta = new Vector2(X, X / 2.0f);
            distanceParent.sizeDelta = new Vector2(X, X / 2.0f);

            var y = X / 2.0f;
            var x = (y / 5.0f) * 3.0f;
            size = new Vector2(x, y);
            distance0.sizeDelta = size;
            distance1.sizeDelta = size;
            distance2.sizeDelta = size;
            distance3.sizeDelta = size;
            distance4.sizeDelta = size;
        }
        private void Reposition()
        {
            image.localPosition = Vector3.zero;
            arrowParent.localPosition = Vector3.zero;
            arrowTop.localPosition = new Vector3(0.0f, (arrowParent.sizeDelta.x * 0.5f) - (arrowTop.sizeDelta.x * 0.5f), 0.0f);
            arrowRight.localPosition = new Vector3((arrowParent.sizeDelta.x * 0.5f) - (arrowTop.sizeDelta.x * 0.5f), 0.0f, 0.0f);
            arrowBot.localPosition = new Vector3(0.0f, (arrowParent.sizeDelta.x * -0.5f) + (arrowTop.sizeDelta.x * 0.5f), 0.0f);
            arrowLeft.localPosition = new Vector3((arrowParent.sizeDelta.x * -0.5f) + (arrowTop.sizeDelta.x * 0.5f), 0.0f, 0.0f);
            mokuteki.localPosition = new Vector3((arrowParent.sizeDelta.x + mokuteki.sizeDelta.x) / 2.0f, arrowParent.sizeDelta.y / 4, 0);
            distanceParent.localPosition = new Vector3((arrowParent.sizeDelta.x + mokuteki.sizeDelta.x) / 2.0f, arrowParent.sizeDelta.y / -4, 0);
            var x = (distanceParent.sizeDelta.x * -0.5f) + (distance0.sizeDelta.x * 0.5f);
            distance0.localPosition = new Vector3(x, 0, 0);
            distance1.localPosition = new Vector3(x + (distance0.sizeDelta.x * 1), 0, 0);
            distance2.localPosition = new Vector3(x + (distance0.sizeDelta.x * 2), 0, 0);
            distance3.localPosition = new Vector3(x + (distance0.sizeDelta.x * 3), 0, 0);
            distance4.localPosition = new Vector3(x + (distance0.sizeDelta.x * 4), 0, 0);
        }
        private void GetDistanceImageComponent()
        {
            dist0 = distance0.GetComponent<UnityEngine.UI.Image>();
            dist1 = distance1.GetComponent<UnityEngine.UI.Image>();
            dist2 = distance2.GetComponent<UnityEngine.UI.Image>();
            dist3 = distance3.GetComponent<UnityEngine.UI.Image>();
            dist4 = distance4.GetComponent<UnityEngine.UI.Image>();
        }

        //=========================================================================================================================
        //   U P D A T E
        //=========================================================================================================================
        private void RotateArrowParent()
        {
            arrowParent.eulerAngles = new Vector3(0, 0, arrowParent.eulerAngles.z + (90 * Time.deltaTime));
        }
        private void UpdatePositionUI()
        {
            parent.position = GM.mainCamera.WorldToScreenPoint(currentDestination);
        }
        private void UpdateDistance()
        {
            var distance = ((int)Vector3.Distance(GM.playerPosition, currentDestination)).ToString();

            switch(distance.Length)
            {
                case 1:
                    distance0.gameObject.SetActive(true);
                    distance1.gameObject.SetActive(true);
                    distance2.gameObject.SetActive(false);
                    distance3.gameObject.SetActive(false);
                    distance4.gameObject.SetActive(false);

                    dist0.sprite = GetCurrentNumberSprite(distance[0].ToString());
                    dist1.sprite = meter;
                    break;
                case 2:
                    distance0.gameObject.SetActive(true);
                    distance1.gameObject.SetActive(true);
                    distance2.gameObject.SetActive(true);
                    distance3.gameObject.SetActive(false);
                    distance4.gameObject.SetActive(false);

                    dist0.sprite = GetCurrentNumberSprite(distance[0].ToString());
                    dist1.sprite = GetCurrentNumberSprite(distance[1].ToString());
                    dist2.sprite = meter;
                    break;
                case 3:
                    distance0.gameObject.SetActive(true);
                    distance1.gameObject.SetActive(true);
                    distance2.gameObject.SetActive(true);
                    distance3.gameObject.SetActive(true);
                    distance4.gameObject.SetActive(false);

                    dist0.sprite = GetCurrentNumberSprite(distance[0].ToString());
                    dist1.sprite = GetCurrentNumberSprite(distance[1].ToString());
                    dist2.sprite = GetCurrentNumberSprite(distance[2].ToString());
                    dist3.sprite = meter;
                    break;
                case 4:
                    distance0.gameObject.SetActive(true);
                    distance1.gameObject.SetActive(true);
                    distance2.gameObject.SetActive(true);
                    distance3.gameObject.SetActive(true);
                    distance4.gameObject.SetActive(true);

                    dist0.sprite = GetCurrentNumberSprite(distance[0].ToString());
                    dist1.sprite = GetCurrentNumberSprite(distance[1].ToString());
                    dist2.sprite = GetCurrentNumberSprite(distance[2].ToString());
                    dist3.sprite = GetCurrentNumberSprite(distance[3].ToString());
                    dist4.sprite = meter;
                    break;
            }
        }

        private void CheckIfCurrentDestinationIsInsideCameraSight()
        {
            var directionTowardCurrentDirection = (currentDestination - GM.mainCamera.transform.position).normalized;
            var cameraForward = GM.mainCamera.transform.forward;

            if (Vector3.Angle(cameraForward, directionTowardCurrentDirection) > 90)
                isCameraFacingCurrentObjectivePosition = false;
            else isCameraFacingCurrentObjectivePosition = true;
        }
        private bool CheckIfPlayerIsInsideCurrentDestinationBoxCollider(BoxCollider currentDestinationCollider)
        {
            var OverlapBox = Physics.OverlapBox(currentDestination, currentDestinationCollider.size * 0.5f,
                currentDestinationCollider.transform.rotation, LayerMask.GetMask("Player"));
            if (OverlapBox.Length > 0)
            {
                QuestLog.ChangeCurrentObjective(Objective.ReachLastBoss);
                return true; 
            }

            return false;
        }

        public Sprite GetCurrentNumberSprite(string number)
        {
            switch(number)
            {
                case "0": return n0;
                case "1": return n1;
                case "2": return n2;
                case "3": return n3;
                case "4": return n4;
                case "5": return n5;
                case "6": return n6;
                case "7": return n7;
                case "8": return n8;
                case "9": return n9;
            }

            return null;
        }

        //=========================================================================================================================
        //   E N U M E R A T O R
        //=========================================================================================================================
        private IEnumerator Repeater()
        {
            yield return new WaitForEndOfFrame();
            while(true)
            {
                CheckIfCurrentDestinationIsInsideCameraSight();

                if (!isDestination1Completed)
                    isDestination1Completed = CheckIfPlayerIsInsideCurrentDestinationBoxCollider(destination1);
                else CheckIfPlayerIsInsideCurrentDestinationBoxCollider(destination2);

                if (isCameraFacingCurrentObjectivePosition && enabled == false)
                {
                    enabled = true;
                }

                if((PlayerControl.Attack.temporaryTarget != null ||
                    PlayerControl.CameraBattle.mInstance.targetTransforms.Count > 0 || 
                    !isCameraFacingCurrentObjectivePosition) && 
                    enabled == true)
                {
                    enabled = false;
                    parent.position = Vector3.one * 1000000.0f;
                }

                number++;
                if(number == 5)
                {
                    UpdateDistance();
                    number = 0;
                }

                yield return delay;
            }
        }
    }
}

