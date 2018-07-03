using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class Damage : MonoBehaviour
    {
        public static GameObject damagePrefab { get; private set; }
        private static Canvas canvas { get { return LevelUP.canvas; } }

        private RectTransform myRT;
        private bool isLeft;
        private float velocityHorizontal = Screen.width * 0.06f;
        private float velocityVertical = Screen.height * 0.2f;
        private float depleteRate = Screen.height * 0.5f;

        private readonly float destination = -(Screen.height * 0.2f);

        // Use this for initialization
        void Start()
        {
            var random = Random.Range(0, 2);
            if (random == 0)
                isLeft = true;

            myRT = GetComponent<RectTransform>();
            var size = Mathf.Abs(Mathf.Abs(velocityVertical) - Screen.height * 0.2f);
            size = Mathf.Clamp(size, 5.0f, 128.0f);
            myRT.sizeDelta = new Vector2(size, size);
        }

        // Update is called once per frame
        void Update()
        {
            myRT.localPosition += (4 * Vector3.up) * (velocityVertical * Time.deltaTime);
            if (isLeft)
                myRT.localPosition += -Vector3.right * (velocityHorizontal * Time.deltaTime);
            else myRT.localPosition += Vector3.right * (velocityHorizontal * Time.deltaTime);

            var size = Mathf.Abs(Mathf.Abs(velocityVertical) - Screen.height * 0.2f);
            size = Mathf.Clamp(size, 5.0f, 96.0f);
            myRT.sizeDelta = new Vector2(size, size);

            velocityVertical = Mathf.MoveTowards(velocityVertical, destination, depleteRate * Time.deltaTime);
            if (velocityVertical == destination && size < 20.0f) Destroy(gameObject);
        }

        public static RectTransform InstantiateDamage(float damagePoint, Vector3 target)
        {
            if(damagePrefab == null)
                damagePrefab = GM.LoadResources("Damage");

            var rt = Instantiate(damagePrefab).GetComponent<RectTransform>();
            rt.SetParent(canvas.transform);
            rt.position = GM.mainCamera.WorldToScreenPoint(target);

            rt.GetComponent<TMPro.TextMeshProUGUI>().text = ((int)damagePoint).ToString();
            rt.gameObject.AddComponent < Damage > ();

            return rt;
        }
    } 
}
