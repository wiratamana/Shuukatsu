using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class StateStop : StateMachineBehaviour
    {
        /// <summary>
        /// こいつは、アニメーションに付いているアニメーション名で値を入れてあげなければいけないよ！
        /// </summary>
        public string thisAnimationName;
        public string boolName;
        public bool value;

        public DontStopIF[] dontStopIFs;
        public AnotherBoolYouWantToStop[] anotherBoolYouWantToStops;
        public BoolYouWantToActive[] boolYouWantToActives;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var isTakingHitLookUp = animator.GetComponent<BooleanComparator>();
            if(isTakingHitLookUp != null && !string.IsNullOrEmpty(thisAnimationName))
            {
                // ループでistakingHitLookUpのbools配列から見て、thisAnimationNameに合っていたら、それの値をfalseに変える。
                for (int x = 0; x < isTakingHitLookUp.bools.Length; x++)
                    if (isTakingHitLookUp.bools[x].name == thisAnimationName)
                    {
                        isTakingHitLookUp.bools[x].value = false;
                        break;
                    }
                
                // さらに、 ループでistakingHitLookUpのbools配列から見て、いずれかの値がtrueの場合returnする。
                for (int x = 0; x < isTakingHitLookUp.bools.Length; x++)
                    if (isTakingHitLookUp.bools[x].value)
                        return;
            }

            if (dontStopIFs.Length > 0)
                for (int i = 0; i < dontStopIFs.Length; i++)
                    if (animator.GetBool(dontStopIFs[i].boolName) == dontStopIFs[i].value)
                        return;
            
            if(!string.IsNullOrEmpty(boolName))
                animator.SetBool(boolName, value);

            if (anotherBoolYouWantToStops.Length > 0)
                for (int i = 0; i < anotherBoolYouWantToStops.Length; i++)
                    animator.SetBool(anotherBoolYouWantToStops[i].boolName, anotherBoolYouWantToStops[i].value);

            if (anotherBoolYouWantToStops.Length > 0)
                for (int i = 0; i < anotherBoolYouWantToStops.Length; i++)
                    animator.SetBool(anotherBoolYouWantToStops[i].boolName, anotherBoolYouWantToStops[i].value);

            if (boolYouWantToActives.Length > 0)
                for (int i = 0; i < boolYouWantToActives.Length; i++)
                    animator.SetBool(boolYouWantToActives[i].boolName, boolYouWantToActives[i].value);
        }
    }

    [System.Serializable]
    public struct DontStopIF
    {
        public string boolName;
        public bool value;
    }

    [System.Serializable]
    public struct AnotherBoolYouWantToStop
    {
        public string boolName;
        public bool value;
    }

    [System.Serializable]
    public struct BoolYouWantToActive
    {
        public string boolName;
        public bool value;
    }
}

