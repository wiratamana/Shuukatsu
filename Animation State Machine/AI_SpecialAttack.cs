using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public class AI_SpecialAttack : StateMachineBehaviour
    {
        public string stateName;
        public string originalAnimationName;
        public bool playKnockedAIR;
        public Tamana.AI.FlameType flameType;
        public float damageTiming;
        public float speedMultiplier;

        private Tamana.AI.BaseAI ai;
        private PlayerControl.AttackAnimationSetting animationData;
        private float attackTimer;
        private bool enabled;
        private Transform attackCollider;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (string.IsNullOrEmpty(animationData.animationName))
                animationData = GM.GetAnimationData(originalAnimationName);
            if (ai == null)
                ai = animator.GetComponent<AI.BaseAI>();
            if (attackCollider == null)
                attackCollider = GM.FindChildWithTag("AttackCollider", animator.transform).transform;
            if (speedMultiplier > 0) animator.SetFloat("specialAttackMultiplier", speedMultiplier);
            else speedMultiplier = 1.0f;

            var dmgTmng = damageTiming * (1.0f / speedMultiplier);
            attackTimer = Time.time + dmgTmng;
            enabled = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enabled && Time.time > attackTimer)
            {
                var playerInfo = GM.GetPlayerInfo(attackCollider.position, attackCollider.localScale.z * 0.5f);
                if (playerInfo != null && flameType == AI.FlameType.None)
                {
                    playerInfo.DoDamage(animationData.attackMultiplier * ai.baseStatus.AT);
                    if(playKnockedAIR)
                        PlayerControl.Attack.mInstance.PlayAnimation_KnockedAIR();                   
                }

                if(flameType != AI.FlameType.None)
                {
                    var dmg = animationData.attackMultiplier * ai.baseStatus.AT;
                    var direction = (GM.playerPosition - attackCollider.transform.position).normalized;
                    Tamana.AI.FlameSword.InstantiateFlame(attackCollider.transform.position, direction, dmg, flameType);
                }

                enabled = false;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat("specialAttackMultiplier", 1.0f);
        }
    }
}

