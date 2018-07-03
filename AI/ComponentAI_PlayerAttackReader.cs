using UnityEngine;
using System.Collections;

namespace Tamana.AI
{
    public class ComponentAI_PlayerAttackReader
    {
        public bool isPlayerBeginToAttack { get; private set; }

        private float waitTimeBeforeReset;
        private float waitingTime;

        private bool playerCurrentAttackCondition;
        private bool playerLastAttackCondition;

        public ComponentAI_PlayerAttackReader(float waitTimeBeforeReset)
        {
            this.waitTimeBeforeReset = waitTimeBeforeReset;
        }

        public void ReadForIncomingInput(Tamana.PlayerControl.Attack playerAttackInstance)
        {
            if (isPlayerBeginToAttack) return;
            if (playerAttackInstance == null) return;

            var isPlayerAttacking = playerAttackInstance.isAttacking || playerAttackInstance.isUsingSuperStrongAttack;
            playerCurrentAttackCondition = isPlayerAttacking;
            var isConditionChanged = (playerCurrentAttackCondition && !playerLastAttackCondition);

            //Debug.Log("playerAttackInstance.isAttacking = " + playerAttackInstance.isAttacking);
            //Debug.Log("isPlayerAttacking = " + isPlayerAttacking);
            //Debug.Log("isConditionChanged = " + (playerCurrentAttackCondition && !playerLastAttackCondition));
            //Debug.Log("====================================================================================");

            if (isConditionChanged)
            {
                isPlayerBeginToAttack = true;
                waitingTime = waitTimeBeforeReset;
            }

            playerLastAttackCondition = playerCurrentAttackCondition;
        }

        public void WaitingBeforeRelease()
        {
            if (!isPlayerBeginToAttack)
                return;

            if (waitingTime != 0)
                waitingTime = Mathf.MoveTowards(waitingTime, 0.0f, Time.deltaTime);
            else isPlayerBeginToAttack = false;
        }
    }
}
