using UnityEngine;
using System.Collections;

using Tamana.AnimatorManager;

namespace Tamana.AI
{
    public class AI_Brain : MonoBehaviour
    {
        public BaseAI ai;
        public Status initStatus;
        protected Tamana.AnimatorManager.SetBool dodgeAndCounterEnter;
        protected Tamana.AnimatorManager.SetBool dodgeAndCounterExit;
        private GameObject BarHP;

        #region Const Skills Name
        //=========================================================================================================================
        //   P H Y S I C A L     S K I L L S
        //=========================================================================================================================
        public const string whirlwindEnter_stateName = "Whirlwind Enter";
        public const string whirlwindExit_stateName = "Whirlwind Exit";

        public const string forwardCounterEnter_stateName = "ForwardCounter Enter";
        public const string forwardCounterExit_stateName = "ForwardCounter Exit";

        public const string threeComboEnter_stateName = "3 Combo Enter";
        public const string threeComboExit_stateName = "3 Combo Exit";

        //=========================================================================================================================
        //   D E F E N S I V E     S K I L L S
        //=========================================================================================================================
        public const string parryKickHit3Enter_stateName = "ParryKickHit3 Enter";
        public const string parryKickHit3Exit_stateName = "ParryKickHit3 Exit";

        public const string dodgeAndCounterEnter_stateName = "Dodge and Counter Enter";
        public const string dodgeAndCounterExit_stateName = "Dodge and Counter Exit";

        public const string parryAndCounterEnter_stateName = "ParryAndCounter Enter";
        public const string parryAndCounterExit_stateName = "ParryAndCounter Exit";

        //=========================================================================================================================
        //   F L A M E    S K I L L S
        //=========================================================================================================================
        public const string flameSwordEnter_stateName = "FlameSword Enter";
        public const string flameSwordExit_stateName = "FlameSword Exit";

        public const string flameComboEnter_stateName = "FlameCombo Enter";
        public const string flameComboExit_stateName = "FlameCombo Exit";

        public const string flameWhirlwindEnter_stateName = "FlameWhirlwind Enter";
        public const string flameWhirlwindExit_stateName = "FlameWhirlwind Exit";

        public const string flameDragonBreathEnter_stateName = "DragonBreath Enter";
        public const string flameDragonBreathExit_stateName = "DragonBreath Exit";

        public const string flameDiagonalEnter_stateName = "FlameDiagonal Enter";
        public const string flameDiagonalExit_stateName = "FlameDiagonal Exit";

        public const string flameVerticalEnter_stateName = "FlameVertical Enter";
        public const string flameVerticalExit_stateName = "FlameVertical Exit";

        public const string flameHorizontalEnter_stateName = "FlameHorizontal Enter";
        public const string flameHorizontalExit_stateName = "FlameHorizontal Exit";
        #endregion

        public virtual void aiStart()
        {
            ai.baseStatus = initStatus;
            ai.currentStatus = initStatus;

            dodgeAndCounterEnter = GM.GetSetBool(dodgeAndCounterEnter_stateName, ai.animator);
            dodgeAndCounterExit = GM.GetSetBool(dodgeAndCounterExit_stateName, ai.animator);
        }
        public virtual void aiUpdate() { }

        //=========================================================================================================================
        //   P H Y S I C A L     S K I L L S
        //=========================================================================================================================
        public void UseWhirlwind(ref bool isUsingWhirlwind, SetBool enter, SetBool exit)
        {
            if(ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingWhirlwind = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("Whirlwind");
        }

        public void UseForwardCounter(ref bool isUsingForwardCounter, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingForwardCounter = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("ForwardCounter");
        }

        public void UseThreeCombo(ref bool isUsingThreeCombo, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingThreeCombo = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("3 Combo1");
        }


        //=========================================================================================================================
        //   F L A M E    S K I L L S
        //=========================================================================================================================
        public void UseFlameCombo(ref bool isUsingFlameCombo, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingFlameCombo = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("FlameCombo");
        }

        public void UseFlameWhirlwind(ref bool isUsingFlameWhirlwind, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingFlameWhirlwind = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("FlameWhirlwind");
        }

        public void UseFlameSword(ref bool isUsingFlameSword, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingFlameSword = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("FlameSword");
        }

        public void UseDragonBreath(ref bool isUsingDragonBreath, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingDragonBreath = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("DragonBreath");
        }

        public void UseFlameVertical(ref bool isUsingFlameVertical, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingFlameVertical = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("FlameVertical");
        }

        public void UseFlameHorizontal(ref bool isUsingFlameHorizontal, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingFlameHorizontal = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("FlameHorizontal");
        }

        public void UseFlameDiagonal(ref bool isUsingFlameDiagonal, SetBool enter, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            enter.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            ai.isAttacking = true;
            isUsingFlameDiagonal = true;
            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("FlameDiagonal");
        }


        //=========================================================================================================================
        //   D E F E N S I V E     S K I L L S
        //=========================================================================================================================
        public void UseRollBackward(ref bool isRollBackward, SetBool start, SetBool exit)
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking)
                return;

            start.enabled = true;
            exit.enabled = true;

            ai.isUsingSpecialAttack = true;
            isRollBackward = true;
            ai.animator.Play("RollBackward");
        }

        public void PlayDodgeAndCounter()
        {
            if (ai.isAttacking) ai.isAttacking = false;
            if (ai.isUsingSpecialAttack) ai.isUsingSpecialAttack = false;

            dodgeAndCounterEnter.enabled = true;
            dodgeAndCounterExit.enabled = true;

            ai.animator.SetLayerWeight(ai.animator.GetLayerIndex("Attack"), 1.0f);
            ai.animator.Play("DodgeAttack");
        }

        public void PlayParryAndCounter(SetBool enter, SetBool exit)
        {
            (ai.brain as AI_Strong).isParryAndCounter = false;

            if (ai.isAttacking) ai.isAttacking = false;
            if (ai.isUsingSpecialAttack) ai.isUsingSpecialAttack = false;

            ai.isBlockImpact = true;
            ai.isBlockImpactBIG = true;

            enter.enabled = true;
            exit.enabled = true;

            GM.SetAnimatorLayerWeight("Attack", 1.0f, ai.animator);
            ai.animator.Play("ParryAndCounter");
        }

        //=========================================================================================================================
        //   U T I L I T Y     M E T H O D S
        //=========================================================================================================================
        public void SetNewBarHP(GameObject barHP)
        {
            BarHP = barHP;
            StartCoroutine(WaitUntilDeath());
        }

        private IEnumerator WaitUntilDeath()
        {
            yield return new WaitUntil(() => ai.isDeath);

            if(BarHP != null) Destroy(BarHP);
        }
    }
}