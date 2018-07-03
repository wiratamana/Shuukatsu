using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class SetBool : StateMachineBehaviour
    {
        public string stateName;
        public WhatState state;
        public Bool[] bools;
        public bool enabled = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(state == WhatState.OnStateEnter && enabled)
            {
                if (bools.Length > 0)
                    for (int i = 0; i < bools.Length; i++)
                        animator.SetBool(bools[i].name, bools[i].value);

                enabled = false;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state == WhatState.OnStateExit && enabled)
            {
                if (bools.Length > 0)
                    for (int i = 0; i < bools.Length; i++)
                        animator.SetBool(bools[i].name, bools[i].value);

                enabled = false;
            }
        }
    }
}

