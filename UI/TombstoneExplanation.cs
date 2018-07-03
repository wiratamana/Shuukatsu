using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class TombstoneExplanation : MonoBehaviour
    {
        public Transform tombstone1;
        public Transform tombstone2;
        public Transform tombstone3;

        private GameObject prefab;

        private bool loaded;
        private readonly WaitForSeconds delay = new WaitForSeconds(0.33f);

        // Use this for initialization
        void Start()
        {
            StartCoroutine(Repeater());
        }

        private bool GetAngleAndDistance()
        {
            Transform[] dists = new Transform[3];
            dists[0] = tombstone1;
            dists[1] = tombstone2;
            dists[2] = tombstone3;

            Transform selected = null;

            for (int i = 0; i < dists.Length; i++)
            {
                if ((GM.playerPosition - dists[i].position).sqrMagnitude > 225.0f)
                    continue;
                else
                {
                    selected = dists[i];
                    break;
                }
            }

            if (selected == null) return false;

            if (GM.isVisibleByCamera_Float(selected, false) > 25.0f)
                return false;

            return true;
        }

        private IEnumerator Repeater()
        {
            yield return new WaitForEndOfFrame();
            while(!loaded)
            {
                if(GetAngleAndDistance())
                {
                    Message.InstantiateBosekiExplanation();
                    loaded = true;

                    StopAllCoroutines();
                }

                yield return delay;
            }
        }
    }
}

