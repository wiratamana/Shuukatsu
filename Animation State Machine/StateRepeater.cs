using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class StateRepeater : StateMachineBehaviour
    {
        public string condition;
        public string animationName;
        public DontRepeatIF[] dontRepeatIF;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(dontRepeatIF.Length > 0)
                for (int x = 0; x < dontRepeatIF.Length; x++)
                    if (animator.GetBool(dontRepeatIF[x].boolName) == dontRepeatIF[x].value) return;

            if (animator.GetBool(condition))
            {
                animator.CrossFade(animationName, .1f);
            }
        }
    }

    [System.Serializable]
    public struct DontRepeatIF
    {
        public string boolName;
        public bool value;
    }
}

