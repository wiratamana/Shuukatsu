using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class SetBoolUpdate : StateMachineBehaviour
    {
        public Bool[] bools;
        public float animationLength;
        [Range(0.0f, 100.0f)]
        public float stopAt;

        private float stopTime;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            stopTime = Time.time + ((animationLength/60.0f) * (stopAt / 100.0f));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (bools.Length == 0)
                return;

            if (Time.time > stopTime)
            {
                for (int i = 0; i < bools.Length; i++)
                {
                    animator.SetBool(bools[i].name, bools[i].value);
                }
            }

        }
    }
}

