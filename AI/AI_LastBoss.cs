using UnityEngine;
using System.Collections;
using Tamana.AnimatorManager;

namespace Tamana.AI
{
    public class AI_LastBoss : AI_Brain
    {
        private Skill flameCombo;
        private Skill flameWhirlwind;
        private Skill flameSword;
        private Skill flameDragonBreath;
        private Skill flameDiagonal;
        private Skill flameVertical;
        private Skill flameHorizontal;
        private Skill bigDamage;
        private Skill bigDamageCounter;

        private FlameProjectile flameProjectile;

        public static ParticleSystem flameEffect { get; private set; }
        public bool isTakingBigDamage
        {
            get { return ai.animator.GetBool("isTakingBigDamage"); }
            set { ai.animator.SetBool("isTakingBigDamage", value); }
        }
        private int totalHitTaken;

        private ComponentAI_PlayerAttackReader attackReader = new ComponentAI_PlayerAttackReader(0.0f);

        private float action;
        private float offensiveRepeater;

        public override void aiStart()
        {
            base.aiStart();
            flameWhirlwind = new Skill(flameWhirlwindEnter_stateName, flameWhirlwindExit_stateName, ai.animator, 10);
            flameCombo = new Skill(flameComboEnter_stateName, flameComboExit_stateName, ai.animator, 25);
            flameSword = new Skill(flameSwordEnter_stateName, flameSwordExit_stateName, ai.animator, 10);
            flameEffect = GM.FindWithTag("LastBoss FlameSword").GetComponent<ParticleSystem>();

            flameDragonBreath = new Skill(flameDragonBreathEnter_stateName, flameDragonBreathExit_stateName, ai.animator, 0.0f);
            flameHorizontal = new Skill(flameHorizontalEnter_stateName, flameHorizontalExit_stateName, ai.animator, 0.0f);
            flameDiagonal = new Skill(flameDiagonalEnter_stateName, flameDiagonalExit_stateName, ai.animator, 0.0f);
            flameVertical = new Skill(flameVerticalEnter_stateName, flameVerticalExit_stateName, ai.animator, 0.0f);

            bigDamage = new Skill("BigDamage Enter", "BigDamage Exit", ai.animator, 0.0f);
            bigDamageCounter = new Skill("DamageCounter Enter", "DamageCounter Exit", ai.animator, 0.0f);

            flameProjectile = new FlameProjectile(4.50f);

            Cinematic.mInstance.lastBossSpawnPosition = ai.transform.position;
            ai.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            ai.transform.position = Vector3.up * 1000.0f;

            flameEffect.Stop();
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
                ai.RecoverEnemyHP();
                return;
            }

            //===================================================================================================//
            MoveTowardPlayer();

            OffensiveMovement(2.0f);
        }

        //=========================================================================================================================
        //   M O V E M E N T    S K I L L S
        //=========================================================================================================================
        private void MoveTowardPlayer()
        {
            var isAttacking = ai.isAttacking || ai.isUsingSpecialAttack;
            var isTakingDamage = ai.isTakingDamage || ai.isTakingDamageBIG;
            if (isAttacking || isTakingDamage)
            {
                ai.aiStrafing.Stop();
                return;
            }

            if (ai.playerAttackInstance.isBlockImpactBIG)
            {
                if (ai.distanceFromPlayer < 16.0f)
                    ai.aiStrafing.StrafeBackward();
                else ai.aiStrafing.Stop();
            }
            else
            {
                if (ai.distanceFromPlayer > 2.0f)
                    ai.aiStrafing.StrafeForward();
                else ai.aiStrafing.Stop();
            }
        }

        private void OffensiveMovement(float everySecond)
        {
            if (Time.time < offensiveRepeater) return;
            if (ai.isInteruptedByPlayer) return;
            if (ai.isInOffensiveState) return;

            if (ai.distanceFromPlayer < 150.0f && ai.distanceFromPlayer > 6.0f)
            {
                UseFlameProjectile();
                return;
            }

            if (ai.distanceFromPlayer < 10.0f)
            {
                UseFlameCombo();
                UseFlameSword();
                UseFlameWhirlwind();

                UseNormalAttack();
            }

            offensiveRepeater = Time.time + everySecond;
        }

        //=========================================================================================================================
        //   O F F E N S I V E    S K I L L S
        //=========================================================================================================================
        private void UseFlameCombo()
        {
            if (!flameCombo.isReady || ai.isUsingSpecialAttack || ai.isAttacking) return;
            UseFlameCombo(ref flameCombo.isUsing, flameCombo.enter, flameCombo.exit);
            flameCombo.cooldown_r = flameCombo.cooldown;
            AddRandomCooldown();
        }

        private void UseFlameSword()
        {
            if (!flameSword.isReady || ai.isUsingSpecialAttack || ai.isAttacking) return;
 
            UseFlameSword(ref flameSword.isUsing, flameSword.enter, flameSword.exit);
            flameSword.cooldown_r = flameSword.cooldown;
            AddRandomCooldown();
        }

        private void UseFlameWhirlwind()
        {
            if (!flameWhirlwind.isReady || ai.isUsingSpecialAttack || ai.isAttacking) return;
            UseFlameWhirlwind(ref flameWhirlwind.isUsing, flameWhirlwind.enter, flameWhirlwind.exit);
            flameWhirlwind.cooldown_r = flameWhirlwind.cooldown;
            AddRandomCooldown();
        }

        private void UseFlameProjectile()
        {
            if (!flameProjectile.isReady || ai.isUsingSpecialAttack || ai.isAttacking) return;
            int randomNumber = Random.Range(0, 4);
            switch (randomNumber)
            {
                case 0:
                    UseDragonBreath(ref flameDragonBreath.isUsing, flameDragonBreath.enter, flameDragonBreath.exit);
                    break;
                case 1:
                    UseFlameDiagonal(ref flameDiagonal.isUsing, flameDiagonal.enter, flameDiagonal.exit);
                    break;
                case 2:
                    UseFlameVertical(ref flameVertical.isUsing, flameVertical.enter, flameVertical.exit);
                    break;
                case 3:
                    UseFlameHorizontal(ref flameHorizontal.isUsing, flameHorizontal.enter, flameHorizontal.exit);
                    break;
            }
            flameProjectile.cooldown_r = flameProjectile.cooldown;
        }

        private void UseNormalAttack()
        {
            if (ai.isUsingSpecialAttack || ai.isAttacking) return;
            var rndNumber = Random.Range(0, 3);
            switch (rndNumber)
            {
                case 0: ai.aiAttack.PlayAttackAnimation(ai.aiAttack.lightAttacks, ref ai.aiAttack.isCanAttack); break;
                case 1: ai.aiAttack.PlayAttackAnimation(ai.aiAttack.mediumAttacks, ref ai.aiAttack.isCanAttack); break;
                case 2: ai.aiAttack.PlayAttackAnimation(ai.aiAttack.heavyAttacks, ref ai.aiAttack.isCanAttack); break;
            }
        }

        //=========================================================================================================================
        //   D E F E N S I V E    S K I L L S
        //=========================================================================================================================
        private void UseBigDamageCounter()
        {
            bigDamage.enter.enabled = true;
            bigDamage.exit.enabled = true;
            bigDamageCounter.enter.enabled = true;
            bigDamageCounter.exit.enabled = true;

            flameProjectile.cooldown_r += 3.0f;

            ai.animator.Play("BigDamage");
        }

        //=========================================================================================================================
        //   U T I L I T Y     M E T H O D S
        //=========================================================================================================================
        private void AddRandomCooldown()
        {
            var rndNumber = Random.Range(5.0f, 20.0f);
            flameCombo.cooldown_r += rndNumber;
            rndNumber = Random.Range(5.0f, 20.0f);
            flameSword.cooldown_r += rndNumber;
            rndNumber = Random.Range(5.0f, 20.0f);
            flameWhirlwind.cooldown_r += rndNumber;
        }

        private void UpdateCooldown()
        {
            flameCombo.cooldown_r = Mathf.MoveTowards(flameCombo.cooldown_r, 0.0f, Time.deltaTime);
            flameSword.cooldown_r = Mathf.MoveTowards(flameSword.cooldown_r, 0.0f, Time.deltaTime);
            flameWhirlwind.cooldown_r = Mathf.MoveTowards(flameWhirlwind.cooldown_r, 0.0f, Time.deltaTime);
            flameProjectile.cooldown_r = Mathf.MoveTowards(flameProjectile.cooldown_r, 0.0f, Time.deltaTime);
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

        public void TakeDamageFromPlayer()
        {
            if (isTakingBigDamage) return;

            totalHitTaken++;

            if (totalHitTaken > 2)
            {
                totalHitTaken = 0;
                UseBigDamageCounter();
                isTakingBigDamage = true;
                flameProjectile.cooldown_r += 5.0f;
            }
        }
    }

    public struct Skill
    {
        public bool isUsing;
        public SetBool enter { get; private set; }
        public SetBool exit { get; private set; }
        public float cooldown;
        public float cooldown_r;
        public bool isReady { get { return cooldown_r == 0; } }

        public Skill(string enter, string exit, Animator ai, float cooldown)
        {
            isUsing = false;
            this.enter = GM.GetSetBool(enter, ai);
            this.exit = GM.GetSetBool(exit, ai); 
            this.cooldown = cooldown;
            cooldown_r = cooldown;
        }
    }
    public struct FlameProjectile
    {
        public bool isReady { get { return cooldown_r == 0; } }
        public float cooldown;
        public float cooldown_r;

        public FlameProjectile(float cooldown)
        {
            this.cooldown = cooldown;
            cooldown_r = cooldown;
        }
    }
}

