using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class TamanaMessenger : MonoBehaviour
    {
        public string[] prefabNames;
        public TheMessage[] messages;

        public static readonly List<TheMessage> objectReferences = new List<TheMessage>();
        public static readonly WaitForSeconds waitTime = new WaitForSeconds(0.33f);

        public static TamanaMessenger mInstance { private set; get; }

        private void Awake()
        {
            mInstance = this;
            messages = new TheMessage[prefabNames.Length];

            for(int i = 0; i < messages.Length; i++)
                messages[i] = new TheMessage(prefabNames[i]);
        }

        // Update is called once per frame
        void Update()
        {
            if (objectReferences.Count == 0) return; 

            for (int i = 0; i < objectReferences.Count; i++)
            {
                if(Time.time < objectReferences[i].timeBeforeStartDisappear)
                {
                    objectReferences[i].objectReference.color = Color.Lerp(objectReferences[i].objectReference.color, Color.white, Time.deltaTime);
                }
                else
                {
                    objectReferences[i].objectReference.color = Color.Lerp(objectReferences[i].objectReference.color, Color.clear, Time.deltaTime);
                    if(objectReferences[i].objectReference.color.a < 0.009f)
                    {
                        var reference = objectReferences[i];
                        Destroy(objectReferences[i].objectReference.gameObject);
                        objectReferences.Remove(reference);
                    }

                }
            }
        }

        public static void InstantiateMessage(string fileNameFromResourcesFolder)
        {
            for(int i = 0; i < mInstance.messages.Length; i++)
            {
                if(mInstance.messages[i].prefabName == fileNameFromResourcesFolder)
                {
                    var obj = Instantiate(mInstance.messages[i].prefab);
                    var img = obj.GetComponent<UnityEngine.UI.Image>();
                    img.rectTransform.SetParent(GM.canvas.transform);
                    img.rectTransform.localPosition = new Vector3(0.0f, Screen.height * -0.5f + (Screen.height * 0.17f), 0.0f);

                    mInstance.messages[i].objectReference = img;
                    mInstance.messages[i].timeBeforeStartDisappear = Time.time + 2.0f;

                    objectReferences.Add(mInstance.messages[i]);

                    mInstance.StartCoroutine(mInstance.CheckForObjectReferences());

                    return;
                }
            }
        }

        public IEnumerator CheckForObjectReferences()
        {
            while(true)
            {
                if (objectReferences.Count == 0)
                {
                    enabled = false;
                    StopCoroutine(CheckForObjectReferences());
                }
                else enabled = true;

                yield return waitTime;
            }
        }
    } 

    public class TheMessage
    {
        public readonly GameObject prefab;
        public readonly string prefabName;
        public UnityEngine.UI.Image objectReference;
        public float timeBeforeStartDisappear;

        public TheMessage(string prefabName)
        {
            prefab = GM.LoadResources(prefabName);
            this.prefabName = prefabName;
        }
    }
}
