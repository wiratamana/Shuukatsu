using UnityEngine;
using System.Collections;

namespace Tamana.AI
{
    public class AutoHideBarHP : MonoBehaviour
    {
        public BaseAI baseAI;

        void Start()
        {
            baseAI = GetComponentInParent<BaseAI>();
        }

        private void OnBecameVisible()
        {
            baseAI.isVisibleToPlayer = true;
        }

        private void OnBecameInvisible()
        {
            baseAI.isVisibleToPlayer = false;
        }
    }
}

