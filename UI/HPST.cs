using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Tamana
{
    public class HPST
    {
        private static RectTransform HP;
        private static RectTransform ST;
        private static RectTransform Pic;

        private const float baseLength = 150.0f;

        public static void Adjust(Status stats)
        {
            if (HP == null)
                HP = GM.FindWithTag("BarHP").GetComponent<RectTransform>();
            if (ST == null)
                ST = GM.FindWithTag("BarST").GetComponent<RectTransform>();
            if (Pic == null)
                Pic = GM.FindWithTag("PicUI").GetComponent<RectTransform>();

            var sizeHP = baseLength + (stats.HP * 0.85f);
            var sizeST = baseLength + (stats.ST * 0.85f);

            HP.sizeDelta = new Vector2(sizeHP, HP.sizeDelta.y);
            ST.sizeDelta = new Vector2(sizeST, ST.sizeDelta.y);

            var posHP = Pic.localPosition.x;
            posHP += (Pic.sizeDelta.x + sizeHP) * 0.5f;
            var posST = Pic.localPosition.x;
            posST += (Pic.sizeDelta.x + sizeST) * 0.5f;

            HP.localPosition = new Vector3(posHP, HP.localPosition.y, 0);
            ST.localPosition = new Vector3(posST, ST.localPosition.y, 0);

            var child0 = HP.transform.GetChild(0).GetComponent<RectTransform>();
            child0.sizeDelta = HP.sizeDelta;
            child0.localPosition = Vector3.zero;
            var child1 = HP.transform.GetChild(1).GetComponent<RectTransform>();
            child1.sizeDelta = HP.sizeDelta + new Vector2(5.0f, 5.0f);
            child0.localPosition = Vector3.zero;

            child0 = ST.transform.GetChild(0).GetComponent<RectTransform>();
            child0.sizeDelta = ST.sizeDelta;
            child0.localPosition = Vector3.zero;
            child1 = ST.transform.GetChild(1).GetComponent<RectTransform>();
            child1.sizeDelta = ST.sizeDelta + new Vector2(5.0f, 5.0f);
            child0.localPosition = Vector3.zero;
        }

    }
}

