using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class FitToScreen : MonoBehaviour
    {
        public RectTransform HPST;

        private void Awake()
        {
            AdjustHPST();
        }

        private void AdjustHPST()
        {
            var scWidth = Screen.width * -0.5f;
            var scHeight = Screen.height * 0.5f;
            var myWidth = HPST.sizeDelta.x * 0.5f;
            var myHeight = HPST.sizeDelta.y * 0.5f;

            var posX = myWidth + scWidth;
            var posY = scHeight - myHeight;

            HPST.localPosition = new Vector3(posX, posY, 0);
        }
    }
}

