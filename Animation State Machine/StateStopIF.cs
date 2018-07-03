using UnityEngine;
using System.Collections;

namespace Tamana.AnimatorManager
{
    public class StateStopIF : StateMachineBehaviour
    {
        public DontStopIF[] dontStopIFs;
        public BoolComparation[] conditions;
        public string boolName;
        public bool value;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool(boolName) == value) return;

            if (dontStopIFs.Length > 0)
                for (int i = 0; i < dontStopIFs.Length; i++)
                    if (animator.GetBool(dontStopIFs[i].boolName) == dontStopIFs[i].value)
                        return;

            if (conditions.Length > 0)
                for (int i = 0; i < conditions.Length; i++)
                    if (animator.GetBool(conditions[i].boolName) == conditions[i].value)
                    {
                        animator.SetBool(boolName, value);
                        return;
                    }
        }
    }

    [System.Serializable]
    public struct BoolComparation
    {
        public string boolName;
        public bool value;
    }
}

