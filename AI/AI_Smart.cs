using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AI
{
    public class AI_Smart : AI_Brain
    {
        ComponentAI_PlayerAttackReader attackReader = new ComponentAI_PlayerAttackReader(0.0f);

        public override void aiUpdate()
        {
            attackReader.ReadForIncomingInput(ai.playerAttackInstance);

            BattleLogic();

            attackReader.WaitingBeforeRelease();
        }

        private void BattleLogic()
        {
            if (ai.playerTransform == null)
            {
                if (ai.aiStrafing.strafeHorizontal != 0.0f && ai.aiStrafing.strafeVertical != 0.0f)
                    ai.aiStrafing.Stop();
                return;
            }

            if (ai.playerAttackInstance.isDeath)
            {
                ai.playerTransform = null;
                ai.DestroyBarHP();
                return;
            }

            if (ai.isTakingDamageBIG)
            {
                ai.isBlocking = false;
                return;
            }

            if (ai.playerAttackInstance.isTakingHit)
            {
                if (!ai.isBlocking && !ai.isAttacking && (ai.isTakingDamageBIG || ai.isBlockImpactAIR)) ai.aiBlock.StartBlocking();

                if (ai.distanceFromPlayer < 9.5f)
                    ai.aiStrafing.MoveBackward();
                else ai.aiStrafing.Stop();
            }
            else if (ai.distanceFromPlayer > 4.0f)
            {
                if (ai.isTakingDamage || ai.isTakingDamageBIG) return;
                    DodgeAndCounter();

                ai.aiStrafing.MoveForward();
                if (!ai.isBlocking && !ai.isAttacking && !ai.isTakingDamageBIG) ai.aiBlock.StartBlocking();
            }
            else
            {
                if (ai.isTakingDamage || ai.isTakingDamageBIG) return;

                DodgeAndCounter();

                if (ai.distanceFromPlayer < 3.14f && !ai.isAttacking && !ai.playerAttackInstance.isAttacking && !ai.isUsingSpecialAttack)
                {
                    ai.Attack();
                }
            }
        }

        private void DodgeAndCounter()
        {
            var condition = ai.distanceFromPlayer < 8.0f && attackReader.isPlayerBeginToAttack && !ai.isUsingSpecialAttack && !ai.isAttacking;
            if (!condition) return;

            PlayDodgeAndCounter();
            ai.isBlockImpact = false;
            ai.isBlockImpactAIR = false;
            ai.isBlockImpactBIG = false;
        }
    }
}

