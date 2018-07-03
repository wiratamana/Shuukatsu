using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class AI_Kick : StateMachineBehaviour
    {
        public string stateName;
        public float hitTime;

        private bool enabled;
        private float hit;
        private Transform attackCollider;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            hit = hitTime + Time.time;
            enabled = true;
            attackCollider = GM.FindChildWithTag("AttackCollider", animator.transform).transform;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(enabled && Time.time > hit)
            {
                if(GM.GetPlayerInfo(attackCollider.position, 0.1f) != null)
                    PlayerControl.Attack.mInstance.PlayAnimation_KickedByEnemy();
            }
        }
    }
}

