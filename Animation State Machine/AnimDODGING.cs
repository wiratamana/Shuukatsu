using UnityEngine;
using System.Collections;

namespace Tamana.AnimatorManager
{
    public class AnimDODGING
    {
        private readonly static WaitForSeconds DODGING_WAIT_TIME = new WaitForSeconds(0.6f);
        public static AB DODGING;

        public static IEnumerator DO_DODGING()
        {
            yield return DODGING_WAIT_TIME;

            PlayerControl.Attack.mInstance.isDodging = false;
            GM.SetAnimatorLayerWeight("Attack", 1.0f);
            PlayerControl.Attack.mInstance.leftAnalogHorizontal = 0.0f;
            PlayerControl.Attack.mInstance.leftAnalogVertical = 0.0f;
        }
    }
}

