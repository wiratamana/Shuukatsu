using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AI
{
    public class AI_Stupid : AI_Brain
    {
        public override void aiUpdate()
        {
            if (ai.playerTransform == null)
            {
                ai.aiStrafing.Stop();
                return;
            }

            if (ai.playerAttackInstance.isDeath)
            {
                ai.playerTransform = null;
                ai.DestroyBarHP();
                return;
            }

            if((ai.isTakingDamageBIG || ai.isBlockImpactAIR))
            {
                ai.isBlocking = false;
                return;
            }

            if(ai.playerAttackInstance.isTakingHit)
            {
                if (!ai.isBlocking && !ai.isAttacking && (ai.isTakingDamageBIG || ai.isBlockImpactAIR)) ai.aiBlock.StartBlocking();

                if (ai.distanceFromPlayer < 4.0f)
                    ai.aiStrafing.MoveBackward();
                else ai.aiStrafing.Stop();
            }
            else if (ai.distanceFromPlayer > 4.0f)
            {
                ai.aiStrafing.MoveForward();
                if (!ai.isBlocking && !ai.isAttacking) ai.aiBlock.StartBlocking();
            }
            else
            {
                if (ai.distanceFromPlayer < 3.14f && !ai.isAttacking && !ai.playerAttackInstance.isAttacking)
                {
                    ai.Attack();
                }
            }
        }
    }
}

