using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class ItemPickUp : MonoBehaviour
    {
        public static bool isPlayerNearby { private set; get; }
        public static GameObject UI_PickItem { private set; get; }
        
        public float distanceFromPlayer { get { return (GM.playerPosition - transform.position).sqrMagnitude; } }
        private WaitForSeconds waitingTime = new WaitForSeconds(0.33f);

        private void Awake()
        {
            UI_PickItem = GM.FindWithTag("PickItem");
            PickItemFitToScreen();

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 300.0f, LayerMask.GetMask("Scene")))
                transform.position = hit.point + (Vector3.up * 0.25f);
        }

        private void Start()
        {
            StartCoroutine(CheckPlayerDistance());

            if (UI_PickItem.activeInHierarchy)
                UI_PickItem.SetActive(false);
        }

        private void Update()
        {
            if (distanceFromPlayer < 0.6f && LevelUP.statusReference == null)
            {
                UI_PickItem.SetActive(true);
                isPlayerNearby = true;
                if (Input.GetButtonDown("Circle"))
                {                 
                    PlayerControl.PotionManager.PickUpPotion(gameObject);
                    isPlayerNearby = false;
                }
            }
            else
            {
                isPlayerNearby = false;
                UI_PickItem.SetActive(false); 
            }
        }

        private IEnumerator CheckPlayerDistance()
        {
            while(true)
            {
                if (distanceFromPlayer < 16 && LevelUP.statusReference == null)
                    enabled = true;
                else enabled = false;

                yield return waitingTime;
            }
        }

        private void PickItemFitToScreen()
        {
            var rt = UI_PickItem.GetComponent<RectTransform>();
            var posx = Screen.width * 0.0f - (rt.sizeDelta.x * 0.0f);
            var posy = Screen.height * -0.5f + (rt.sizeDelta.y * 0.5f) + (Screen.height * 0.1f);

            rt.localPosition = new Vector3(posx, posy, 0);
        }
    }
}

