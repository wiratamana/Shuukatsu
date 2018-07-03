using UnityEngine;
using System.Collections;

namespace Tamana.AnimatorManager
{
    public class StateStopMultipleBool : StateMachineBehaviour
    {
        public string[] boolNames;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            for(int x = 0; x < boolNames.Length; x++)
                if(animator.GetBool(boolNames[x]))
                {
                    animator.SetBool(boolNames[x], false);
                    return;
                }
        }
    }
}

