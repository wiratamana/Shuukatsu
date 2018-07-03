using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class TombstoneIcon : MonoBehaviour
    {
        public Sprite ON;
        public Sprite OFF;
        public ResurrectionTombstone resurrectionTombstone;
        public enum ONOFF { ON, OFF }

        private UnityEngine.UI.Image image;
        private static readonly WaitForSeconds delay = new WaitForSeconds(0.33f);
        private static List<TombstoneIcon> icons = new List<TombstoneIcon>();

        private void Awake()
        {
            image = GetComponent<UnityEngine.UI.Image>();
            icons.Add(this);
        }

        // Use this for initialization
        void Start()
        {
            image.rectTransform.sizeDelta = new Vector2(Screen.height / 20, Screen.height / 20);
            enabled = false;
            resurrectionTombstone.tombstoneIcon = this;

            StopAllCoroutines();
            StartCoroutine(Repeater());
        }

        private void Update()
        {
            var dir = Minimap.GetPositionInMap(image.rectTransform.position + Vector3.forward) - Minimap.GetPositionInMap(image.rectTransform.position);
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var lookRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            image.rectTransform.rotation = lookRotation;

            image.rectTransform.localPosition = Minimap.GetPositionInMap(resurrectionTombstone.transform);
        }

        public void Switch(ONOFF onoff)
        {
            if (onoff == ONOFF.OFF)
                image.sprite = OFF;
            else image.sprite = ON;
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(Repeater());
        }

        private static IEnumerator Repeater()
        {
            yield return new WaitForEndOfFrame();
            while(true)
            {
                yield return delay;

                for(int i = 0; i < icons.Count; i++)
                {
                    var a = icons[i].resurrectionTombstone.transform.position;
                    var b = GM.playerPosition;
                    a.y = 0; b.y = 0;

                    if ((a - b).sqrMagnitude < 2000.0f)
                        icons[i].enabled = true;
                    else icons[i].enabled = false;
                }

            }
        }
    }
}

