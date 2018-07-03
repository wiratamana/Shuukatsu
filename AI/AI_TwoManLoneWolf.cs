using UnityEngine;
using System.Collections;

using Tamana.AnimatorManager;

namespace Tamana.AI
{
    public class AI_TwoManLoneWolf : AI_Brain
    {

        public bool isUsingWhirlwind;
        public Tamana.AnimatorManager.SetBool whirlwindEnter { get; private set; }
        public Tamana.AnimatorManager.SetBool whirlwindExit { get; private set; }
        public float cooldownWhirlwind;
        private float cooldownWhirlwind_r;
        private bool isWhirlwindReady { get { return cooldownWhirlwind_r == 0; } }

        public bool isUsingForwardCounter;
        public SetBool forwardCounterEnter { get; private set; }
        public SetBool forwardCounterExit { get; private set; }
        public float cooldownForwardCounter;
        private float cooldownForwardCounter_r;
        private bool isForwardCounterReady { get { return cooldownForwardCounter_r == 0; } }

        public bool isUsingThreeCombo;
        public SetBool threeComboEnter { get; private set; }
        public SetBool threeComboExit { get; private set; }
        public float cooldownThreeCombo;
        private float cooldownThreeCombo_r;
        private bool isThreeComboReady { get { return cooldownThreeCombo_r == 0; } }


        public bool isParryAndCounter;
        public SetBool parryAndCounterEnter { get; private set; }
        public SetBool parryAndCounterExit { get; private set; }

        private ComponentAI_PlayerAttackReader attackReader = new ComponentAI_PlayerAttackReader(0.0f);
        private readonly short[] defensiveMethods = { 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1 };
        private ushort currenIndex = 0;

        private float action;

        public override void aiStart()
        {
            base.aiStart();

            whirlwindEnter = GM.GetSetBool(whirlwindEnter_stateName, ai.animator);
            whirlwindExit = GM.GetSetBool(whirlwindExit_stateName, ai.animator);

            forwardCounterEnter = GM.GetSetBool(forwardCounterEnter_stateName, ai.animator);
            forwardCounterExit = GM.GetSetBool(forwardCounterExit_stateName, ai.animator);

            threeComboEnter = GM.GetSetBool(threeComboEnter_stateName, ai.animator);
            threeComboExit = GM.GetSetBool(threeComboExit_stateName, ai.animator);

            parryAndCounterEnter = GM.GetSetBool(parryAndCounterEnter_stateName, ai.animator);
            parryAndCounterExit = GM.GetSetBool(parryAndCounterExit_stateName, ai.animator);
        }

        public override void aiUpdate()
        {
            attackReader.ReadForIncomingInput(ai.playerAttackInstance);

            UpdateCooldown();
            LogicUpdate();

            attackReader.WaitingBeforeRelease();

            Cleaning();
        }

        public void LogicUpdate()
        {
            if (ai.playerTransform == null)
            {
                if (ai.aiStrafing.strafeHorizontal != 0.0f && ai.aiStrafing.strafeVertical != 0.0f)
                    ai.aiStrafing.Stop();
                return;
            }

            if (ai.playerAttackInstance.isDeath)
            {
                ai.aiStrafing.Stop();
                ai.playerTransform = null;
            }

            //===================================================================================================//

            if (ai.playerAttackInstance.isTakingHit)
            {
                if (ai.distanceFromPlayer < 9.5f)
                    ai.aiStrafing.MoveBackward();
                else ai.aiStrafing.Stop();
            }
            else if (ai.distanceFromPlayer > 5.0f)
            {
                ai.aiStrafing.MoveForward();

                //if (isForwardCounterReady && !ai.isUsingSpecialAttack)
                //{
                //    if (ai.distanceFromPlayer < 16.14f)
                //    {
                //        ai.aiStrafing.Stop();
                //        GM.SlerpRotation(ai.transform, GM.playerPosition, 10000);
                //        UseForwardCounter();
                //        action = 0;
                //    }
                //
                //    return;
                //}

                var isPlayerFacingMe = GM.GetAngleBetweenMyForwardAndPlayerForward(ai.transform) > 150;
                if (ai.distanceFromPlayer < 8.5f && attackReader.isPlayerBeginToAttack && !(ai.isTakingDamage || ai.isTakingDamageBIG) && isPlayerFacingMe)
                {
                    if (defensiveMethods[currenIndex] == 0)
                    {
                        PlayDodgeAndCounter();
                        action = 0;
                    }
                    else { ai.aiBlock.StartBlocking(); isParryAndCounter = true; action = 0; }

                    currenIndex++;
                    if (currenIndex > defensiveMethods.Length - 1)
                        currenIndex++;
                    return;
                }
            }
            else
            {
                if (ai.isTakingDamage || ai.isTakingDamageBIG) return;

                var isPlayerFacingMe = GM.GetAngleBetweenMyForwardAndPlayerForward(ai.transform) > 150;
                if (ai.distanceFromPlayer < 8.5f && attackReader.isPlayerBeginToAttack && !(ai.isTakingDamage || ai.isTakingDamageBIG) && isPlayerFacingMe)
                {
                    if (defensiveMethods[currenIndex] == 0)
                    {
                        PlayDodgeAndCounter();
                        action = 0;
                    }
                    else { ai.aiBlock.StartBlocking(); isParryAndCounter = true; action = 0; }

                    currenIndex++;
                    if (currenIndex > defensiveMethods.Length - 1)
                        currenIndex++;
                    return;
                }

                if (isWhirlwindReady && !ai.isUsingSpecialAttack)
                {
                    if (ai.distanceFromPlayer < 3.14f)
                    {
                        UseWhirlwind();
                        action = 0;
                    }

                    return;
                }

                if (isThreeComboReady && !ai.isUsingSpecialAttack)
                {
                    if (ai.distanceFromPlayer < 4.14f)
                    {
                        UseThreeCombo();
                        action = 0;
                    }

                    return;
                }

                var blockImpact = ai.isBlockImpact || ai.isBlockImpactAIR || ai.isBlockImpactBIG;
                if (ai.distanceFromPlayer < 3.14f && !ai.isAttacking && !ai.playerAttackInstance.isAttacking && !ai.isDodging || blockImpact)
                {
                    ai.aiAttack.PlayAttackAnimation(ai.aiAttack.mediumAttacks, ref ai.aiAttack.isCanAttack);
                    action = 0;
                }
            }
        }

        private void UseWhirlwind()
        {
            UseWhirlwind(ref isUsingWhirlwind, whirlwindEnter, whirlwindExit);
            cooldownWhirlwind_r = cooldownWhirlwind;
        }

        private void UseForwardCounter()
        {
            UseForwardCounter(ref isUsingForwardCounter, forwardCounterEnter, forwardCounterExit);
            cooldownForwardCounter_r = cooldownForwardCounter;
        }

        private void UseThreeCombo()
        {
            UseThreeCombo(ref isUsingThreeCombo, threeComboEnter, threeComboExit);
            cooldownThreeCombo_r = cooldownThreeCombo;
        }

        private void UpdateCooldown()
        {
            cooldownWhirlwind_r = Mathf.MoveTowards(cooldownWhirlwind_r, 0.0f, Time.deltaTime);
            cooldownForwardCounter_r = Mathf.MoveTowards(cooldownForwardCounter_r, 0.0f, Time.deltaTime);
            cooldownThreeCombo_r = Mathf.MoveTowards(cooldownThreeCombo_r, 0.0f, Time.deltaTime);
        }

        private void Cleaning()
        {
            action = Mathf.MoveTowards(action, 5, Time.deltaTime);
            if (action == 5)
            {
                if (ai.isAttacking) ai.isAttacking = false;
                if (ai.isBlockImpact) ai.isBlockImpact = false;
                if (ai.isBlockImpactBIG) ai.isBlockImpactBIG = false;
                if (ai.isUsingSpecialAttack) ai.isUsingSpecialAttack = false;
                if (ai.isBlocking) ai.aiBlock.StopBlocking();
                action = 0;
            }
        }
    }

}
