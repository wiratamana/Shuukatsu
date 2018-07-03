using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class AI_FlameEffect : StateMachineBehaviour
    {
        public WhatState state;
        public FlameSword flameSword;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state != WhatState.OnStateEnter) return;

            if (flameSword == FlameSword.Play)
                Tamana.AI.AI_LastBoss.flameEffect.Play();
            else Tamana.AI.AI_LastBoss.flameEffect.Stop();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (state != WhatState.OnStateExit) return;

            if (flameSword == FlameSword.Play)
                Tamana.AI.AI_LastBoss.flameEffect.Play();
            else Tamana.AI.AI_LastBoss.flameEffect.Stop();
        }
    }

    public enum FlameSword { Play, Stop }
}

