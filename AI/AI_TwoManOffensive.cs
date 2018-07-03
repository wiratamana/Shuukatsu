using UnityEngine;
using System.Collections;

namespace Tamana.AI
{
    public class AI_TwoManOffensive
    {
        private readonly AI_TwoMan tm;
        private readonly BaseAI ai;

        private readonly BaseAI partner;
        private readonly ComponentAI_PlayerAttackReader attackReader = new ComponentAI_PlayerAttackReader(0.0f);

        public Direction directionHorizontal;
        private System.Diagnostics.Stopwatch sw;

        public AI_TwoManOffensive(AI_TwoMan TM)
        {
            tm = TM;
            ai = tm.ai;

            partner = tm.partner;

            directionHorizontal = Direction.Right;

            sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
        }

        public void UpdateLogic()
        {
            if (tm.myRole != Role.Offensive) return;

            attackReader.ReadForIncomingInput(ai.playerAttackInstance);

            MoveFurtherFromMyPartner();

            Dodge();
            Attack();

            attackReader.WaitingBeforeRelease();
        }

        public void MoveFurtherFromMyPartner()
        {
            if (partner.isDeath) return;

            if (ai.distanceFromPlayer > 3.5f)
                ai.aiStrafing.StrafeForward();
            else ai.aiStrafing.StopStrafingVertically();

            if (tm.getAngleBetweenMyPartner < 105.0f && !tm.partner.isDeath)
            {
                if (directionHorizontal == Direction.Right)
                    ai.aiStrafing.StrafeRight();
                else ai.aiStrafing.StrafeLeft();
            
                if(sw.ElapsedMilliseconds > 5000)
                {
                    if (tm.getAngleBetweenMyPartner_Delta < 0.0f)
                        directionHorizontal = Direction.Right;
                    else directionHorizontal = Direction.Left;

                    sw.Restart();
                }

            }
            else ai.aiStrafing.StopStrafingHorizontally();
        }

        public void Attack()
        {
            if (ai.distanceFromPlayer < 4.0f && !ai.isAttacking && !ai.isDodging)
                ai.Attack();
        }

        public void Dodge()
        {
            if (ai.distanceFromPlayer < 5.0f && attackReader.isPlayerBeginToAttack)
                ai.Dodge();
        }
    }
}

