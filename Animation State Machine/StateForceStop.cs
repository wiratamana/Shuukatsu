using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Tamana.AnimatorManager
{
    public class StateForceStop : StateMachineBehaviour
    {
        public string StopAnimationName;
        public Bool[] conditions;
        public Bool[] changeBoolAfterStop;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(conditions.Length > 0)
                for(int i = 0; i < conditions.Length; i++)
                    if(animator.GetBool(conditions[i].name) == conditions[i].value)
                    {
                        animator.Play(StopAnimationName);

                        if (changeBoolAfterStop.Length > 0)
                            for (int j = 0; j < changeBoolAfterStop.Length; j++)
                                animator.SetBool(changeBoolAfterStop[j].name, changeBoolAfterStop[j].value);
                    }
        }

    }
}

