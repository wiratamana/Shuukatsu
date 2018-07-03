using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class GPS : MonoBehaviour
    {
        public Transform targetDestination;   
        public UnityEngine.AI.NavMeshAgent agent;
        public int Length;

        private readonly WaitForSeconds delay = new WaitForSeconds(0.10f);
        private BoxCollider_and_EnterExit[] boxColliders;

        private int currentIndex = -1;

        private Collider[] a;
        private Collider[] b;

        public static GameObject gpsNavigationPrefab { private set; get; }
        public static GameObject gpsIconParent { private set; get; }

        private void Awake()
        {
            gpsNavigationPrefab = GM.LoadResources("GPS Navigation");
            gpsIconParent = new GameObject("GPS Icon Parent");
        }

        void Start()
        {
            StartCoroutine(Repeater());
        }

        private void CheckForPlayerCollision()
        {
            if (currentIndex == -1)
            {
                a = Physics.OverlapBox(boxColliders[0].boxCollider.transform.position, 
                    boxColliders[0].boxCollider.size * 0.5f, boxColliders[0].boxCollider.transform.rotation,
                        LayerMask.GetMask("Player"));
                if (a.Length == 0)
                    return;

                boxColliders[0].FlipEnterExit();
                currentIndex = 0;
            }
            if(currentIndex == boxColliders.Length - 1)
            {
                b = Physics.OverlapBox(boxColliders[currentIndex + 1].boxCollider.transform.position, boxColliders[currentIndex + 1].boxCollider.size * 0.5f,
                    boxColliders[currentIndex + 1].boxCollider.transform.rotation, LayerMask.GetMask("Player"));

                if (b.Length > 0)
                {
                    boxColliders[currentIndex + 1].FlipEnterExit();
                    currentIndex--;
                    return;
                }
            }
            else
            {
                a = Physics.OverlapBox(boxColliders[currentIndex].boxCollider.transform.position, boxColliders[currentIndex].boxCollider.size * 0.5f,
                    boxColliders[currentIndex].boxCollider.transform.rotation, LayerMask.GetMask("Player"));
                b = Physics.OverlapBox(boxColliders[currentIndex + 1].boxCollider.transform.position, boxColliders[currentIndex + 1].boxCollider.size * 0.5f, 
                    boxColliders[currentIndex + 1].boxCollider.transform.rotation, LayerMask.GetMask("Player"));

                if(a.Length > 0)
                {
                    boxColliders[currentIndex].FlipEnterExit();
                    currentIndex--;
                    return;
                }

                if(b.Length > 0)
                {
                    boxColliders[currentIndex + 1].FlipEnterExit();
                    currentIndex++;
                    return;
                }
            }
        }

        private IEnumerator Repeater()
        {
            yield return new WaitForEndOfFrame();

            boxColliders = new BoxCollider_and_EnterExit[transform.childCount];
            for (int i = 0; i < boxColliders.Length; i++)
            {
                boxColliders[i] = new BoxCollider_and_EnterExit(transform.GetChild(i).GetComponent<BoxCollider>(), EnterExit.Enter);
            }

            while (true)
            {
                CheckForPlayerCollision();
                Length = currentIndex;

                yield return delay;
            }
        }
    }

    public enum EnterExit { Enter, Exit }
    public class BoxCollider_and_EnterExit
    {
        public BoxCollider boxCollider { private set; get; }
        public EnterExit enterExit { private set; get; }
        public UnityEngine.UI.Image icon { private set; get; }

        public BoxCollider_and_EnterExit(BoxCollider bc, EnterExit ee)
        {
            boxCollider = bc;
            enterExit = ee;

            InstantiateNew();
        }

        public void FlipEnterExit()
        {
            if(enterExit == EnterExit.Enter)
            {
                boxCollider.transform.position = boxCollider.transform.position - boxCollider.transform.forward;
                icon.transform.SetParent(GPS.gpsIconParent.transform);
                icon = null;
                enterExit = EnterExit.Exit;
            }
            else
            {
                boxCollider.transform.position = boxCollider.transform.position + boxCollider.transform.forward;
                enterExit = EnterExit.Enter;

                if(GPS.gpsIconParent.transform.childCount == 0) InstantiateNew();
                else LoadFromExisting();
            }
        }

        private void InstantiateNew()
        {
            icon = Object.Instantiate(GPS.gpsNavigationPrefab).GetComponent<UnityEngine.UI.Image>();
            icon.transform.SetParent(Minimap.miniMap);
            icon.rectTransform.sizeDelta = Minimap.mInstance.mask.sizeDelta * 0.035f;
            icon.rectTransform.localPosition = Minimap.GetPositionInMap(boxCollider.transform.position);
        }

        private void LoadFromExisting()
        {
            icon = GPS.gpsIconParent.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
            icon.transform.SetParent(Minimap.miniMap);
            icon.rectTransform.sizeDelta = Minimap.mInstance.mask.sizeDelta * 0.035f;
            icon.rectTransform.localPosition = Minimap.GetPositionInMap(boxCollider.transform.position);
        }
    }
}

