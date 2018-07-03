using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class Minimap : MonoBehaviour
    {
        public RectTransform rotationCameraFace;
        public RectTransform rotationPlayerFace;
        public RectTransform map;
        public RectTransform mask;
        public RectTransform N;

        [Header("4 Empty GameObjects")]
        public Transform leftTop;
        public Transform leftBot;
        public Transform rightBot;
        public Transform rightTop;
        public Transform middle;

        public float mapSizeAgains360point8 { get { return map.sizeDelta.x / 360.8f; } }

        public static List<RectTransform_And_Transform> enemiesIcon = new List<RectTransform_And_Transform>(); 
        public static Transform miniMap { private set; get; }
        public static GameObject enemyIconPrefab { private set; get; }

        public static Minimap mInstance;

        private void Awake()
        {
            mInstance = this;

            enemyIconPrefab = GM.LoadResources("enemyIconPrefab");
            miniMap = GM.FindWithTag("Minimap").transform;
        }

        // Use this for initialization
        void Start()
        {
            SetSize();
            SetPosition();
        }

        // Update is called once per frame
        void Update()
        {
            RotateCameraFace();
            RotatePlayerFace();
            MoveCamera();

            UpdateEnemiesIconPosition();
        }

        //=========================================================================================================================
        //   I N I T I A L I Z E R S
        //=========================================================================================================================
        private void SetSize()
        {
            var multiplier = (Screen.height / 4.5f) / 100.0f;

            rotationCameraFace.sizeDelta *= multiplier;
            rotationPlayerFace.sizeDelta *= multiplier;
            map.sizeDelta *= multiplier;
            mask.sizeDelta *= multiplier;
        }
        private void SetPosition()
        {
            var parent = rotationCameraFace.transform.parent.GetComponent<RectTransform>();

            parent.localPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
            parent.localPosition -= new Vector3(50.0f + (mask.sizeDelta.x * 0.5f), 50.0f + (mask.sizeDelta.y * 0.5f), 0.0f);

            N.localPosition = new Vector3(0, N.sizeDelta.y * 0.5f + mask.sizeDelta.y * 0.5f, 0.0f);
        }

        //=========================================================================================================================
        //   M I N I M A P   M O V E M E N T    M E T H O D S
        //=========================================================================================================================
        private void RotateCameraFace()
        {
            rotationCameraFace.rotation = Quaternion.Euler(new Vector3(0, 0, GM.mainCamera.transform.eulerAngles.y));
        }
        private void RotatePlayerFace()
        {
            var dir = GetPositionInMap(GM.playerPosition + GM.playerTransform.forward) - GetPositionInMap(GM.playerTransform);
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var lookRotation = Quaternion.AngleAxis(angle - 90 + GM.mainCamera.transform.eulerAngles.y, Vector3.forward);
            rotationPlayerFace.rotation = lookRotation;
        }
        private void MoveCamera()
        {
            var coordinat = middle.position - GM.playerPosition;
            coordinat *= mapSizeAgains360point8;
            var rtPosition = new Vector3(coordinat.x, coordinat.z, 0);
            map.localPosition = rtPosition;
        }
        private void UpdateEnemiesIconPosition()
        {
            if (enemiesIcon.Count == 0) return;

            for (int i = 0; i < enemiesIcon.Count; i++)
            {
                enemiesIcon[i].rectTransform.localPosition = GetPositionInMap(enemiesIcon[i].transform); 
            }
        }

        //=========================================================================================================================
        //   U T I L I T Y    M E T H O D S
        //=========================================================================================================================
        public static Vector3 GetPositionInMap(Transform transform)
        {
            var coord = transform.position - mInstance.middle.position;
            coord *= mInstance.mapSizeAgains360point8;
            var rtPos = new Vector3(coord.x, coord.z, 0);
            return rtPos;
        }
        public static Vector3 GetPositionInMap(Vector3 position)
        {
            var coord = position - mInstance.middle.position;
            coord *= mInstance.mapSizeAgains360point8;
            var rtPos = new Vector3(coord.x, coord.z, 0);
            return rtPos;
        }
        public static void AddEnemy(Transform transform)
        {
            var enemyIcon = Instantiate(enemyIconPrefab).GetComponent<RectTransform>();
            enemyIcon.SetParent(miniMap.transform);
            enemyIcon.localPosition = GetPositionInMap(transform);
            enemiesIcon.Add(new RectTransform_And_Transform(transform, enemyIcon));
        }
        public static void DestroyEnemy(Transform transform)
        {
            for(int i = 0; i < enemiesIcon.Count; i++)
            {
                if (enemiesIcon[i].transform != transform) continue;

                var temp = enemiesIcon[i].rectTransform.gameObject;
                enemiesIcon.RemoveAt(i);
                Destroy(temp);
                return;
            }
        }
    }

    public class RectTransform_And_Transform
    {
        public readonly Transform transform;
        public readonly RectTransform rectTransform;

        public RectTransform_And_Transform(Transform transform, RectTransform rectTransform)
        {
            this.transform = transform;
            this.rectTransform = rectTransform;
        }
    }
}

