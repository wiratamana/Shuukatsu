using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class SetFloat : StateMachineBehaviour
    {
        public WhatState state;
        public Float[] floats;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(state == WhatState.OnStateEnter)
                for (int i = 0; i < floats.Length; i++)
                    animator.SetFloat(floats[i].floatName, floats[i].value);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state == WhatState.OnStateExit)
                for (int i = 0; i < floats.Length; i++)
                    animator.SetFloat(floats[i].floatName, floats[i].value);
        }
    }

    [System.Serializable]
    public struct Float
    {
        public string floatName;
        public float value;
    }
}

