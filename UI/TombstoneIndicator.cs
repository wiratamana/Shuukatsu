using UnityEngine;
using System.Collections;

namespace Tamana
{
    public class TombstoneIndicator : MonoBehaviour
    {
        private ResurrectionTombstone resurrectionTombstone;
        private RectTransform indicatorReference;
        private UnityEngine.UI.Image img1;
        private UnityEngine.UI.Image img2;
        private UnityEngine.UI.Image img3;
        private bool isVisible
        {
            get
            {
                var directionFromCamera = (transform.position - GM.mainCamera.transform.position).normalized;
                var cameraDirection = GM.mainCamera.transform.forward;
                directionFromCamera.y = 0; cameraDirection.y = 0;
                var angle = Vector3.Angle(cameraDirection, directionFromCamera);
                if (angle > 90) return false;

                return true;
            }
        }
        private float timer;

        private Vector3 screenPosition;

        private static readonly Vector3 offset = new Vector3(0, Screen.height * 0.1f, 0);
        private static readonly WaitForSeconds delay = new WaitForSeconds(0.33f);
        private static GameObject indicatorPrefab;

        private void Awake()
        {
            if (indicatorPrefab == null)
                indicatorPrefab = GM.LoadResources("Tombstone Indicator");
        }

        // Use this for initialization
        void Start()
        {
            resurrectionTombstone = GetComponent<ResurrectionTombstone>();
            StartCoroutine(Repeater());

            enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if(indicatorReference != null)
            {
                if(isVisible)
                {
                    screenPosition = GM.mainCamera.WorldToScreenPoint(transform.position);
                    screenPosition += offset;
                    indicatorReference.position = screenPosition;  
                }
                else
                {
                    screenPosition = Vector3.one * 100000.0f;
                    screenPosition += offset;
                    indicatorReference.position = screenPosition;
                }
            }

            if(Time.time > timer)
            {
                timer += 1.0f;

                UpdateMeter();
            }
        }

        private void UpdateMeter()
        {
            var distance = ((int)Vector3.Distance(GM.playerPosition, transform.position)).ToString();

            if (distance.Length == 1)
            {
                img1.sprite = Destination.mInstance.GetCurrentNumberSprite(distance[0].ToString());
                img2.sprite = Destination.meter;
                img3.enabled = false;
            }
            else
            {
                img1.sprite = Destination.mInstance.GetCurrentNumberSprite(distance[0].ToString());
                img2.sprite = Destination.mInstance.GetCurrentNumberSprite(distance[1].ToString());

                if (!img3.enabled)
                    img3.enabled = true;
                img3.sprite = Destination.meter;
            }
        }

        private IEnumerator Repeater()
        {
            yield return new WaitForEndOfFrame();
            while(true)
            {
                var a = GM.playerPosition;
                var b = transform.position;
                a.y = 0; b.y = 0;

                if ((a - b).sqrMagnitude < 6000.0f && !resurrectionTombstone.isActive)
                {
                    enabled = true;
                    if (indicatorReference == null)
                    {
                        indicatorReference = Instantiate(indicatorPrefab).GetComponent<RectTransform>();
                        indicatorReference.SetParent(GM.canvas.transform);
                        indicatorReference.SetAsFirstSibling();

                        img1 = indicatorReference.GetChild(1).GetComponent<UnityEngine.UI.Image>();
                        img2 = indicatorReference.GetChild(2).GetComponent<UnityEngine.UI.Image>();
                        img3 = indicatorReference.GetChild(3).GetComponent<UnityEngine.UI.Image>();

                        UpdateMeter();

                        timer = Time.time + 1.0f;
                    }
                }
                else
                {
                    enabled = false;
                    if (indicatorReference != null)
                        Destroy(indicatorReference.gameObject);
                }

                yield return delay;
            }
        }
    }
}

