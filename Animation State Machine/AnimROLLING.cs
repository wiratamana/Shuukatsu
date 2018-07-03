using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Tamana.AnimatorManager
{
    public class AnimROLLING
    {
        private readonly static WaitForSeconds ROLLING_WAIT_TIME = new WaitForSeconds(0.8f);


        public static IEnumerator DO_ROLLING()
        {
            yield return ROLLING_WAIT_TIME;

            PlayerControl.Attack.mInstance.isRolling = false;
        }
    } 
}
