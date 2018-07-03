using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Tamana.AnimatorManager
{
    public class PlayTimerOnStart : StateMachineBehaviour
    {
        public string animationName;
        private PlayerControl.AttackAnimationSetting animationInfo;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AnimAttack_Custom.mInstance.LookFor(animationName, ref animationInfo);
            var ai = animator.GetComponent<Tamana.AI.BaseAI>();
            ai.aiAttack.PlayTimer(animationInfo);
        }
    }
}

