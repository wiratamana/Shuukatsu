using UnityEngine;
using System.Collections;

namespace Tamana.AnimatorManager
{
    public enum WhatState { OnStateEnter, OnStateUpdate, OnStateExit }
    public class SetLayerWeight : StateMachineBehaviour
    {
        public WhatState state;
        public string layerName;
        [Range(0.0f, 1.0f)]
        public float value;

        public DontChangeIF[] dontChangeIFs;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state == WhatState.OnStateEnter)
            {
                if (dontChangeIFs.Length > 0)
                    for (int x = 0; x < dontChangeIFs.Length; x++)
                        if (animator.GetBool(dontChangeIFs[x].boolName) == dontChangeIFs[x].value && string.IsNullOrEmpty(dontChangeIFs[x].boolName))
                            return;

                animator.SetLayerWeight(animator.GetLayerIndex(layerName), value);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state == WhatState.OnStateUpdate)
            {
                if (dontChangeIFs.Length > 0)
                    for (int x = 0; x < dontChangeIFs.Length; x++)
                        if (animator.GetBool(dontChangeIFs[x].boolName) == dontChangeIFs[x].value && string.IsNullOrEmpty(dontChangeIFs[x].boolName))
                            return;

                animator.SetLayerWeight(animator.GetLayerIndex(layerName), value);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state == WhatState.OnStateExit)
            {
                if (dontChangeIFs.Length > 0)
                    for (int x = 0; x < dontChangeIFs.Length; x++)
                        if (animator.GetBool(dontChangeIFs[x].boolName) == dontChangeIFs[x].value　&& string.IsNullOrEmpty(dontChangeIFs[x].boolName))
                            return;

                animator.SetLayerWeight(animator.GetLayerIndex(layerName), value);
            }
        }
    }

    [System.Serializable]
    public struct DontChangeIF
    {
        public string boolName;
        public bool value;
    }
}

