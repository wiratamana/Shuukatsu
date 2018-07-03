using UnityEngine;
using System.Collections;

namespace Tamana.AI
{
    public class BaseAI_Attack : MonoBehaviour
    {
        public Tamana.PlayerControl.AttackAnimationSetting[] lightAttacks;
        public Tamana.PlayerControl.AttackAnimationSetting[] mediumAttacks;
        public Tamana.PlayerControl.AttackAnimationSetting[] heavyAttacks;
        public bool isCanAttack;

        private BaseAI baseAI;
        public BoxCollider attackCollider;

        private bool damaging;
        private float animationTimer;
        private Tamana.PlayerControl.AttackAnimationSetting currentAnimation;
        private bool isFirstHit;
        private bool isSecondHit;
        private int randomNumber;
        private int currentIndex;
        private Collider[] players;

        private System.Diagnostics.Stopwatch sw;

        private bool isThisTheBestTimeToAttack
        {
            get
            {
                players = Physics.OverlapSphere(attackCollider.transform.position, .1f, LayerMask.GetMask("Player"));
                if (players.Length == 0)
                    return false;

                return true;
            }
        }

        public void SetUp(BaseAI baseAI, string[] light, string[] medium, string[] heavy)
        {
            this.baseAI = baseAI;
            sw = new System.Diagnostics.Stopwatch();

            if(this == null)
                Debug.Log("this == null");

            if (AnimAttack_Light.mInstance == null)
                Debug.Log("AnimAttack_Light.mInstance == null");

            GetAnimation(ref lightAttacks, light, AnimAttack_Light.mInstance.LightAttackAnimation);
            GetAnimation(ref mediumAttacks, medium, AnimAttack_Medium.mInstance.MediumAttackAnimation);
            GetAnimation(ref heavyAttacks, heavy, AnimAttack_Heavy.mInstance.HeavyAttackAnimations);

            if (lightAttacks.Length == 0)
                Debug.Log("lightAttacks.Length == 0");

            attackCollider = GM.FindChildWithTag("AttackCollider", baseAI.transform).GetComponent<BoxCollider>();

            isCanAttack = true;
        }

        public void Update()
        {
            Damaging();
        }

        private void GetAnimation(ref PlayerControl.AttackAnimationSetting[] me, string[] stringCompare, PlayerControl.AttackAnimationSetting[] lookUp)
        {
            if (stringCompare.Length == 0) return;

            me = new PlayerControl.AttackAnimationSetting[stringCompare.Length];

            for (int x = 0; x < stringCompare.Length; x++)
            {
                for (int y = 0; y < lookUp.Length; y++)
                {
                    if (lookUp[y].animationName == stringCompare[x])
                    {
                        me[x] = lookUp[y];
                        break;
                    }
                }
            }
        }

        private void Damaging()
        {
            if (!damaging) return;
            if (baseAI.isTakingDamage || baseAI.isTakingDamageBIG || baseAI.isDeath || baseAI.isBlockImpact || baseAI.isBlockImpactBIG || baseAI.isBlockImpactAIR)
            {
                damaging = false;
                return;
            }

            animationTimer += Time.deltaTime;

            if (sw.ElapsedMilliseconds * .001f > currentAnimation.damageTiming && isFirstHit)
            {
                if (isSecondHit)
                    isFirstHit = false;
                else damaging = false;
                DoDamage(currentAnimation);
            }

            if (isSecondHit && sw.ElapsedMilliseconds * .001f > currentAnimation.damageTiming2)
            {
                damaging = false;
                isSecondHit = false;
                DoDamage(currentAnimation);
            }
        }

        private void DoDamage(PlayerControl.AttackAnimationSetting attackAnimationSetting)
        {
            var cols = Physics.OverlapSphere(attackCollider.transform.position, .5f, LayerMask.GetMask("Player"));
            if (cols.Length == 0) return;

            for (int x = 0; x < cols.Length; x++)
            {
                var playerInfo = cols[x].GetComponent<PlayerInfo>();
                PlayerControl.Attack.lastEnemyAttackedYou = this.transform;
                playerInfo.DoDamage(baseAI.baseStatus.AT * attackAnimationSetting.attackMultiplier);
            }
        }

        public void PlayAttackAnimation(PlayerControl.AttackAnimationSetting animationName, ref bool isCanAttack)
        {
            if (baseAI.isAttacking || baseAI.isUsingSpecialAttack) return;

            if (baseAI.isTakingDamage || baseAI.isTakingDamageBIG || baseAI.isDeath) return;
            if (baseAI.isBlockImpact || baseAI.isBlockImpactBIG || baseAI.isBlockImpactAIR || baseAI.isRolling || baseAI.isDodging) return;
            if (isCanAttack == false) return;
            if (!isThisTheBestTimeToAttack) return;

            if (baseAI.isBlocking) baseAI.isBlocking = false;

            if (baseAI.animator.GetLayerWeight(baseAI.animator.GetLayerIndex("Attack")) != 1.0f)
                baseAI.animator.SetLayerWeight(baseAI.animator.GetLayerIndex("Attack"), 1.0f);

            baseAI.animator.Play(animationName.animationName);
            currentAnimation = animationName;

            if (currentAnimation.damageTiming2 != 0)
            {
                isFirstHit = true;
                isSecondHit = true;
            }
            else
            {
                isFirstHit = true;
                isSecondHit = false;
            }

            baseAI.isAttacking = true;
            sw.Restart();
            damaging = true;
        }

        public void PlayAttackAnimation(PlayerControl.AttackAnimationSetting[] attackArray, ref bool isCanAttack)
        {
            if (baseAI.isAttacking || baseAI.isUsingSpecialAttack) return;

            randomNumber = Random.Range(0, attackArray.Length);
            while (randomNumber == currentIndex)
                randomNumber = Random.Range(0, attackArray.Length);
            currentIndex = randomNumber;

            if (baseAI.isTakingDamage || baseAI.isTakingDamageBIG || baseAI.isDeath) return;
            if (baseAI.isBlockImpact || baseAI.isBlockImpactBIG || baseAI.isBlockImpactAIR || baseAI.isRolling || baseAI.isDodging) return;
            if (isCanAttack == false) return;
            if (!isThisTheBestTimeToAttack) return;

            if (baseAI.isBlocking) baseAI.isBlocking = false;

            if (baseAI.animator.GetLayerWeight(baseAI.animator.GetLayerIndex("Attack")) != 1.0f)
                baseAI.animator.SetLayerWeight(baseAI.animator.GetLayerIndex("Attack"), 1.0f);

            baseAI.animator.Play(attackArray[currentIndex].animationName);
            currentAnimation = attackArray[currentIndex];

            if (currentAnimation.damageTiming2 != 0)
            {
                isFirstHit = true;
                isSecondHit = true;
            }
            else
            {
                isFirstHit = true;
                isSecondHit = false;
            }

            baseAI.isAttacking = true;
            sw.Restart();
            damaging = true;
        }

        public void PlayTimer(PlayerControl.AttackAnimationSetting animationInfo)
        {
            currentAnimation = animationInfo;

            if (currentAnimation.damageTiming2 != 0)
            {
                isFirstHit = true;
                isSecondHit = true;
            }
            else
            {
                isFirstHit = true;
                isSecondHit = false;
            }

            damaging = true;
            sw.Restart();
            baseAI.isAttacking = true;
        }
    }
}

