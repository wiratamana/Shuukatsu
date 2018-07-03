using UnityEngine;
using System.Collections;

namespace Tamana.AI
{
    public class AI_TwoManDefensive
    {
        private readonly AI_TwoMan tm;
        private readonly BaseAI ai;

        private readonly BaseAI partner;

        public Direction directionHorizontal;

        private System.Diagnostics.Stopwatch sw;

        public AI_TwoManDefensive(AI_TwoMan TM)
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
            if (tm.myRole != Role.Defensive) return;

            MoveFurtherFromMyPartner();

            Dodge();
        }

        public void MoveFurtherFromMyPartner()
        {
            if (partner.isDeath) return;

            if (ai.distanceFromPlayer < 16.0f)
                ai.aiStrafing.StrafeBackward();
            else if (ai.distanceFromPlayer > 20.0f)
                ai.aiStrafing.StrafeForward();
            else ai.aiStrafing.StopStrafingVertically();

            if (tm.getAngleBetweenMyPartner < 60.0f)
            {
                if (directionHorizontal == Direction.Right)
                    ai.aiStrafing.StrafeRight();
                else ai.aiStrafing.StrafeLeft();

                if(sw.ElapsedMilliseconds > 2500)
                {
                    if (tm.getAngleBetweenMyPartner_Delta < 0.0f)
                        directionHorizontal = Direction.Right;
                    else directionHorizontal = Direction.Left;

                    sw.Restart();
                }

            }
            else ai.aiStrafing.StopStrafingHorizontally();
        }

        public void Dodge()
        {
            var isPlayerAttacking = (ai.playerAttackInstance.isAttacking || ai.playerAttackInstance.isUsingSuperStrongAttack);
            if (ai.distanceFromPlayer < 5.0f && isPlayerAttacking && !ai.isDodging)
                ai.Dodge();
        }
    }
}

