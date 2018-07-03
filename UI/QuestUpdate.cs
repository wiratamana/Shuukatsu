using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class QuestUpdate : MonoBehaviour
    {
        private bool down;
        private float waitingTime;
        private UnityEngine.UI.Image img;

        private void Start()
        {
            img = GetComponent<UnityEngine.UI.Image>();
        }

        void Update()
        {
            if(!down)
            {
                img.color = Vector4.MoveTowards(img.color, Color.white, 0.8f * Time.deltaTime);

                if (img.color == Color.white)
                {
                    if (waitingTime == 0)
                        waitingTime = Time.time + 1.0f;

                    if (Time.time > waitingTime)
                        down = true;
                }
            }
            else
            {
                img.color = Vector4.MoveTowards(img.color, Color.clear, 0.8f * Time.deltaTime);

                if (img.color == Color.clear)
                    Destroy(gameObject);
            }
        }
    }
}

