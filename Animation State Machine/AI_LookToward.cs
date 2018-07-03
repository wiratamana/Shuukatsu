using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class AI_LookToward : StateMachineBehaviour
    {
        public string stateName;

        private AI.BaseAI ai;
        private bool enabled;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(ai == null)
                ai = animator.GetComponent<AI.BaseAI>();

            ai.isForceToLookPlayer = true;         
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ai.isForceToLookPlayer = false;
        }
    }
}

