using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class StatusInit : MonoBehaviour
    {
        public Status Player;

        private void Start()
        {
            StartCoroutine(WaitForTwoSecond());
        }

        private IEnumerator WaitForTwoSecond()
        {
            yield return new WaitForSeconds(2.0f);

            Message.InstantiateControllerMap();
        }
    }
}

