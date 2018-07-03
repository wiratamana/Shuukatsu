using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tamana.AnimatorManager;

namespace Tamana.PlayerControl
{
    public class Attack : MonoBehaviour
    {
        public Animator animator;
        public AttackAnimationSetting[] strongAttack;
        public AttackAnimationSetting[] lightAttack;
        public AttackAnimationSetting superStrongAttack;
        public BlockAnimationSetting[] blockLight;
        public BlockAnimationSetting[] blockHeavy;

        public static AttackAnimationSetting currentAttackAnimation;
        public static BlockAnimationSetting currentBlockAnimation;
        public static SetBool BigDamageEnter { private set; get; }
        public static SetBool BigDamageExit { private set; get; }
        public static Transform temporaryTarget { get; set; }
        public static Transform lastEnemyAttackedYou { get; set; }
        public static bool isAbleToAttackAgain;
        public static float animationTimer;
        public const string blockAnimationName = "Longs_BlockStart";
        public const string blockAnimationNameStop = "Longs_BlockEnd";
        public const string blockAnimationNameCounter = "Longs_Kick";
        public const string superStrongAttackName = "Super Strong Attack";
        private static float superAttackTimerThreshold;
        public static TrailRenderer swordTrail { private set; get; }

        public const string deathAnimationName = "Death";
        public const string deathAirAnimationName = "DeathAir";
        public const string hitAnimationNameAir = "BigDamage";
        public static readonly string[] hitAnimationNames = { "Longs_Hit_D", "Longs_Hit_R", "Longs_Hit_L" };
        public static readonly string[] hitAnimationNamesBIG = { "Longs_Hit_R2", "Longs_Hit_L2" };

        public bool isAttacking
        {
            get { return animator.GetBool("isAttacking"); }
            set { animator.SetBool("isAttacking", value); }
        }
        public bool isUsingSuperStrongAttack
        {
            get { return animator.GetBool("isUsingSuperStrongAttack"); }
            set { animator.SetBool("isUsingSuperStrongAttack", value); }
        }
        public bool isUsingSuperStrongAttackSlowDown
        {
            get { return animator.GetBool("isUsingSuperStrongAttackSlowDown"); }
            set { animator.SetBool("isUsingSuperStrongAttackSlowDown", value); }
        }
        public float superStrongAttackSpeedMultiplier
        {
            get { return animator.GetFloat("superStrongAttackSpeedMultiplier"); }
            set { animator.SetFloat("superStrongAttackSpeedMultiplier", value); }
        }
        public bool isBlocking
        {
            get { return animator.GetBool("isBlocking"); }
            set { animator.SetBool("isBlocking", value); }
        }
        public bool isBlockImpact
        {
            get { return animator.GetBool("isBlockImpact"); }
            set { animator.SetBool("isBlockImpact", value); }
        }
        public bool isBlockImpactBIG
        {
            get { return animator.GetBool("isBlockImpactBIG"); }
            set { animator.SetBool("isBlockImpactBIG", value); }
        }
        public bool isBlockCounter
        {
            get { return animator.GetBool("isBlockCounter"); }
            set { animator.SetBool("isBlockCounter", value); }
        }
        public bool isRolling
        {
            get { return animator.GetBool("isRolling"); }
            set { animator.SetBool("isRolling", value); }
        }
        public bool isDodging
        {
            get { return animator.GetBool("isDodging"); }
            set { animator.SetBool("isDodging", value); }
        }
        public bool isTakingHit
        {
            get { return animator.GetBool("isTakingHit"); }
            set { animator.SetBool("isTakingHit", value); }
        }
        public bool isTakingHitBIG
        {
            get { return animator.GetBool("isTakingHitBIG"); }
            set { animator.SetBool("isTakingHitBIG", value); }
        }
        public bool isDeath
        {
            get { return animator.GetBool("isDeath"); }
            set { animator.SetBool("isDeath", value); }
        }

        public bool isUsingPotion
        {
            get { return animator.GetBool("isUsingPotion"); }
            set { animator.SetBool("isUsingPotion", value); }
        }

        public float leftAnalogVertical
        {
            get { return animator.GetFloat("leftAnalogVertical"); }
            set { animator.SetFloat("leftAnalogVertical", value); }
        }
        public float leftAnalogHorizontal
        {
            get { return animator.GetFloat("leftAnalogHorizontal"); }
            set { animator.SetFloat("leftAnalogHorizontal", value); }
        }
        public float attackSpeedMultiplier
        {
            get { return animator.GetFloat("attackSpeedMultiplier"); }
            set { animator.SetFloat("attackSpeedMultiplier", value); }
        }
        public bool isInterupted
        {
            get
            {
                return isAttacking || isUsingSuperStrongAttack || isTakingHit || isTakingHitBIG || isRolling || isDodging ||
                    isDeath || isBlockImpact || isBlockImpactBIG || isBlockCounter;
            }
        }

        private int randomNumber;
        private int currentIndex_Attack;
        private int currentIndex_Block;
        private int currentIndex_Hit;

        private bool damaging;
        private bool isCanAttack;
        private bool isCanRolling;
        private bool isFacingToNearbyEnemy;
        private bool isReadForCounterBlock;
        private bool isReadForSuperStrongAttack;
        private bool isDamagingSuperStrongAttack;
        private float superStrongAttackDamageMultiplier;

        private const float counterMakeEnemyStunTimer = 0.3667f;
        private const float superStrongAttack_SlowDown = 0.68333f;

        private Vector3 directionToNearbyEnemy;
        private System.Diagnostics.Stopwatch blockCounterTimer = new System.Diagnostics.Stopwatch();
        private float superStrongAttackTimer;

        private bool isFirstHit;
        private bool isSecondHit;

        public static Attack mInstance;

        // Use this for initialization
        void Start()
        {
            isAbleToAttackAgain = true;
            mInstance = this;
            superStrongAttackSpeedMultiplier = 1.0f;
            swordTrail = GM.playerTransform.GetComponentInChildren<TrailRenderer>();

            BigDamageEnter = GM.GetSetBool("BigDamage Enter", animator);
            BigDamageExit = GM.GetSetBool("BigDamage Exit", animator);

            for (int i = 0; i < 10; i++)
            {
                if (i < blockLight.Length)
                {
                    blockLight[i].kickTimerEnd = (blockLight[i].kickTimerStart + 35) / 60.0f;
                    blockLight[i].kickTimerStart /= 60.0f;
                }

                if (i < blockHeavy.Length)
                {
                    blockHeavy[i].kickTimerEnd = (blockHeavy[i].kickTimerStart + 35) / 60.0f;
                    blockHeavy[i].kickTimerStart /= 60.0f;
                }
            }

            StartCoroutine(DeathMessage.mInstance.WaitUntilDeath());
        }

        // Update is called once per frame
        void Update()
        {
            if (LevelUP.isON || Message.activeMessage != null || Cinematic.isCinematicON) return;

            Attacking();
            SuperStrongAttack();
            Blocking();
            ReadInputForCounter();
            Rolling();
            Damaging();
            Dodging();          
        }

        //========================================================================================================================//
        //   O F F E N S I V E     C O M M A N D S                                                                                //
        //========================================================================================================================//
        private void Attacking()
        {           
            if (isRolling || isDodging || isReadForSuperStrongAttack || isDamagingSuperStrongAttack) return;
            if (isDeath || isTakingHit || isTakingHitBIG || isBlockImpact || isBlockImpactBIG || isUsingPotion) return;
            if (PlayerInfo.mInstance.currentStatus.ST < PlayerInfo.mInstance.baseStatus.ST * 0.1f) return;

            if (isAbleToAttackAgain)
            {
                if (Input.GetButtonDown("Triangle"))
                {
                    GM.SetAnimatorLayerWeight("Attack", 1.0f);
                    superStrongAttackTimer = Time.time + 0.25f;
                    isReadForSuperStrongAttack = true;

                    currentAttackAnimation.animationLength = currentAttackAnimation.animationLength - (currentAttackAnimation.animationLength * (attackSpeedMultiplier - 1.0f));
                    currentAttackAnimation.damageTiming = currentAttackAnimation.damageTiming - (currentAttackAnimation.damageTiming * (attackSpeedMultiplier - 1.0f));
                    currentAttackAnimation.damageTiming2 = currentAttackAnimation.damageTiming2 - (currentAttackAnimation.damageTiming2 * (attackSpeedMultiplier - 1.0f));                   
                }

                if (Input.GetButtonDown("Square"))
                {
                    PlayAttackAnimation(lightAttack, ref isCanAttack);
                    FaceTowardNearestEnemyWhenAttacking();

                    currentAttackAnimation.animationLength = currentAttackAnimation.animationLength - (currentAttackAnimation.animationLength * (attackSpeedMultiplier - 1.0f));
                    currentAttackAnimation.damageTiming = currentAttackAnimation.damageTiming - (currentAttackAnimation.damageTiming * (attackSpeedMultiplier - 1.0f));
                    currentAttackAnimation.damageTiming2 = currentAttackAnimation.damageTiming2 - (currentAttackAnimation.damageTiming2 * (attackSpeedMultiplier - 1.0f));
                }
            }
        }

        private void SuperStrongAttack()
        {
            if(isDamagingSuperStrongAttack)
            {
                if(Time.time > superStrongAttackTimer)
                {
                    if (PlayerInfo.mInstance.currentStatus.ST < 50.0f)
                        currentAttackAnimation.attackMultiplier *= (PlayerInfo.mInstance.currentStatus.ST / 50.0f);
                    PlayerInfo.mInstance.StaminaManipulator(50, ref isCanAttack);
                    DoDamage(currentAttackAnimation);
                    isDamagingSuperStrongAttack = false;
                }

                return;
            }

            if(isUsingSuperStrongAttack)
            {
                // 敵から攻撃を受けた場合
                if(isTakingHit)
                {
                    isDamagingSuperStrongAttack = false;
                    superStrongAttackSpeedMultiplier = 1.0f;
                    isUsingSuperStrongAttackSlowDown = false;
                    isUsingSuperStrongAttack = false;
                    return;
                }

                //　プレイヤーを敵方向に向かせる。
                if(CameraBattle.mInstance.targetTransforms.Count == 0 && temporaryTarget != null)
                {
                    var directionTowardEnemy = (temporaryTarget.position - ControlManager.controlTransforms.player.position).normalized;
                    directionToNearbyEnemy.y = 0;
                    var lookRotation = Quaternion.LookRotation(directionTowardEnemy);
                    var forward = ControlManager.controlTransforms.player.forward;
                    forward.y = 0;
                    if (Vector3.Angle(forward, directionTowardEnemy) > 5.0f)
                        ControlManager.controlTransforms.player.rotation = Quaternion.Slerp(ControlManager.controlTransforms.player.rotation, lookRotation, 4.0f * Time.deltaTime);
                }

                if (Time.time > superStrongAttackTimer && !isUsingSuperStrongAttackSlowDown)
                {
                    isUsingSuperStrongAttackSlowDown = true;
                    superStrongAttackSpeedMultiplier = 0.02f;
                }

                superStrongAttackDamageMultiplier = 1.8f + (Time.time - superStrongAttackTimer);

                if (Input.GetButtonUp("Triangle") || Time.time > superAttackTimerThreshold)
                {
                    swordTrail.enabled = true;
                    StartCoroutine(SwordTrailAutoTurnOFF());
                    isUsingSuperStrongAttackSlowDown = false;
                    isUsingSuperStrongAttack = false;
                    superStrongAttackSpeedMultiplier = 1.0f;
                    superStrongAttackTimer = Time.time + 0.15f;
                    isDamagingSuperStrongAttack = true;
                    currentAttackAnimation.attackMultiplier = superStrongAttackDamageMultiplier;
                    FlameSword.InstantiateSwingSE();
                }

                return;
            }

            if (!isReadForSuperStrongAttack) return;
            isReadForSuperStrongAttack = Input.GetButton("Triangle");

            if(Time.time > superStrongAttackTimer)
            {
                animator.Play(superStrongAttackName);
                superStrongAttackTimer = Time.time + superStrongAttack_SlowDown;
                isUsingSuperStrongAttack = true;
                isReadForSuperStrongAttack = false;
                currentAttackAnimation = superStrongAttack;
                var cols = Physics.OverlapSphere(ControlManager.controlTransforms.player.position, 9.0f, LayerMask.GetMask("Enemy"));
                temporaryTarget = GM.GetNearestEnemy(cols, ControlManager.controlTransforms.player.position);
                superAttackTimerThreshold = Time.time + 2.1f;
            }
            else
            {
                if(Input.GetButtonUp("Triangle"))
                {
                    PlayAttackAnimation(strongAttack, ref isCanAttack);
                    FaceTowardNearestEnemyWhenAttacking();
                    isReadForSuperStrongAttack = false;
                }
            }
        }

        //========================================================================================================================//
        //   D E F E N S I V E     C O M M A N D S                                                                                //
        //========================================================================================================================//
        private void Blocking()
        {
            if (isAttacking || isDeath || isTakingHit || isTakingHitBIG || isUsingPotion)
            {
                isBlocking = false;
                return;
            }


            if (Input.GetButton("L2") && !isBlocking)
            {
                isBlocking = true;
                ControlManager.playerAnimator.CrossFade(blockAnimationName, .1f);
                if (temporaryTarget == null)
                    temporaryTarget = GM.GetNearestEnemy(9.0f, GM.playerPosition);
            }

            if (isBlocking && !Input.GetButton("L2"))
            {
                isBlocking = false;
                ControlManager.playerAnimator.CrossFade(blockAnimationNameStop, .1f);
            }
        }

        private void Rolling()
        {
            if (isRolling || isDodging || isUsingPotion) return;
            if (isBlockImpact || isBlockImpactBIG || isTakingHit || isTakingHitBIG || isAttacking || isUsingSuperStrongAttack || isDodging) return;

            if (Input.GetButtonDown("R2"))
            {
                ControlManager.playerInfo.StaminaManipulator(25, ref isCanRolling);
                if (!isCanRolling) return;

                GM.SetAnimatorLayerWeight("Attack", 0.0f);
                isRolling = true;
                var dir = ControlManager.controlTransforms.leftAnalog.position - ControlManager.controlTransforms.player.position;
                dir.y = 0;
                var rot = Quaternion.LookRotation(dir);
                ControlManager.controlTransforms.player.rotation = Quaternion.Lerp(ControlManager.controlTransforms.player.rotation, rot, 35 * Time.deltaTime);

                if(AnimatorManager.isRolling.ROLLING == AB.A)
                {
                    animator.Play("Longs_RollFwd");
                    AnimatorManager.isRolling.ROLLING = AB.B;
                    StartCoroutine(AnimatorManager.AnimROLLING.DO_ROLLING());
                }
                else
                {
                    animator.Play("Longs_RollFwd 0");
                    AnimatorManager.isRolling.ROLLING = AB.A;
                    StartCoroutine(AnimatorManager.AnimROLLING.DO_ROLLING());
                }
            }
        }

        private void Dodging()
        {
            if (isDodging || isRolling || isUsingPotion) return;
            if (isBlockImpact || isBlockImpactBIG || isTakingHit || isTakingHitBIG || isAttacking) return;
            if (ItemPickUp.isPlayerNearby || PotionManager.isTakingPotion) return;
            if (ResurrectionTombstone.isPlayerNearby) return;

            if (Input.GetButtonDown("Circle"))
            {
                if (PlayerInfo.mInstance.currentStatus.ST < 10) return;
                ControlManager.playerInfo.StaminaManipulator(10, ref isCanRolling);
                var analogHorizontal = Mathf.Abs(ControlManager.joyStick.LeftAnalogHorizontal);
                var analogVertical = Mathf.Abs(ControlManager.joyStick.LeftAnalogVertical);
                GM.SetAnimatorLayerWeight("Attack", 0.0f);

                if (DodgingFromFlameSword()) goto ApplyDodge;

                if (analogHorizontal < 0.2f && analogVertical < 0.2f)
                {                   
                    var cols = Physics.OverlapSphere(ControlManager.controlTransforms.player.position, 9.0f, LayerMask.GetMask("Enemy"));
                    if (cols.Length == 0)
                    {
                        leftAnalogHorizontal = 0.0f;
                        leftAnalogVertical = -1.0f;
                        goto ApplyDodge;
                    }
                    else
                    {
                        var nearestEnemy = GM.GetNearestEnemy(cols, ControlManager.controlTransforms.player.position);
                        Vector3 direction = Vector3.zero;
                        if (nearestEnemy != null)
                            direction = (nearestEnemy.position - ControlManager.controlTransforms.player.position).normalized;
                        else direction = GM.playerTransform.forward;
                        direction.y = 0;
                        ControlManager.controlTransforms.player.rotation = Quaternion.LookRotation(direction);
                        leftAnalogHorizontal = 0.0f;
                        leftAnalogVertical = -1.0f;
                        goto ApplyDodge;
                    }
                }
                else
                {
                    var direction = (ControlManager.controlTransforms.leftAnalog.position - GM.playerPosition).normalized;
                    direction.y = 0;
                    GM.playerTransform.rotation = Quaternion.LookRotation(direction);
                    leftAnalogHorizontal = 0.0f;
                    leftAnalogVertical = 1.0f;
                    animator.SetFloat("MovementSpeed", 0.0f);
                    goto ApplyDodge;
                }


                ApplyDodge:
                isDodging = true;
                if(AnimatorManager.AnimDODGING.DODGING == AB.A)
                {
                    ControlManager.playerAnimator.Play("Dodge");
                    AnimatorManager.AnimDODGING.DODGING = AB.B;
                    StartCoroutine(AnimatorManager.AnimDODGING.DO_DODGING());
                }
                else
                {
                    ControlManager.playerAnimator.Play("Dodge 0");
                    AnimatorManager.AnimDODGING.DODGING = AB.A;
                    StartCoroutine(AnimatorManager.AnimDODGING.DO_DODGING());
                }
            }
        }

        private bool DodgingFromFlameSword()
        {
            var checkForNearbyFlameSword = Physics.OverlapSphere(GM.playerPosition, 7.0f, LayerMask.GetMask("FlameSword"));
            if (checkForNearbyFlameSword.Length == 0) return false;

            var flameForward = checkForNearbyFlameSword[0].transform.forward;
            float leftDistance = -1.0f;
            float rightDistance = -1.0f;
            RaycastHit hit;

            if (Physics.Raycast(GM.playerPosition, checkForNearbyFlameSword[0].transform.right, out hit, 1000.0f, LayerMask.GetMask("LastArena")))
                rightDistance = (hit.point - GM.playerPosition).sqrMagnitude;
            if (Physics.Raycast(GM.playerPosition, -checkForNearbyFlameSword[0].transform.right, out hit, 1000.0f, LayerMask.GetMask("LastArena")))
                leftDistance = (hit.point - GM.playerPosition).sqrMagnitude;

            if (leftDistance > rightDistance)
                GM.playerTransform.rotation = Quaternion.LookRotation(-checkForNearbyFlameSword[0].transform.right);
            else GM.playerTransform.rotation = Quaternion.LookRotation(checkForNearbyFlameSword[0].transform.right);

            leftAnalogHorizontal = 0.0f;
            leftAnalogVertical = -1.0f;

            return true;
        }

        //========================================================================================================================//
        //   A N I M A T I O N     T R I G G E R S                                                                                //
        //========================================================================================================================//
        private void PlayAttackAnimation(AttackAnimationSetting[] attackArray, ref bool isCanAttack)
        {
            if (isUsingSuperStrongAttack) return;

            GM.GetRandomNumberWithDifferentValueFromBefore(ref randomNumber, ref currentIndex_Attack, attackArray.Length);

            ControlManager.playerInfo.StaminaManipulator(attackArray[currentIndex_Attack].staminaCost, ref isCanAttack);
            if (isCanAttack == false) return;

            isBlocking = false;
            isAttacking = true;

            GM.SetAnimatorLayerWeight("Attack", 1.0f);

            animator.Play(attackArray[currentIndex_Attack].animationName);
            currentAttackAnimation = attackArray[currentIndex_Attack];

            if (currentAttackAnimation.damageTiming2 != 0)
            {
                isFirstHit = true;
                isSecondHit = true;
            }
            else
            {
                isFirstHit = true;
                isSecondHit = false;
            }

            animationTimer = 0;
            damaging = true;
            isAbleToAttackAgain = false;
        }

        public void PlayAnimation_BlockImpact(float damagePoint, float damageThreshold, Status baseStatus, ref Status currentStatus)
        {
            if (!isBlocking)
                return;

            var staminaAfterReduction = currentStatus.ST - damagePoint;
            currentStatus.ST -= damagePoint;
            if (currentStatus.ST <= 0) currentStatus.ST = 0;

            if (staminaAfterReduction < 0)
                currentStatus.HP -= Mathf.Abs(staminaAfterReduction);

            FlameSword.InstantiateParrySE();
            SwordParryEffect.InstantiateEffect(ControlManager.controlTransforms.attackCollider.GetComponent<BoxCollider>());

            if (damageThreshold > baseStatus.HP * 0.65f)
            {
                animator.CrossFade(hitAnimationNameAir, .1f);
                CameraBattle.mInstance.StartCameraShake(2);
                isTakingHit = true;
                blockCounterTimer.Reset();
                blockCounterTimer.Stop();
            }
            else if (damageThreshold > (baseStatus.HP * 0.4f))
            {
                GM.GetRandomNumberWithDifferentValueFromBefore(ref randomNumber, ref currentIndex_Block, blockHeavy.Length);
                animator.CrossFade(blockHeavy[currentIndex_Block].animationName, 0.1f);
                currentBlockAnimation = blockHeavy[currentIndex_Block];
                CameraBattle.mInstance.StartCameraShake(1.5f);
                isBlockImpactBIG = true;
                isBlockImpact = true;
                blockCounterTimer.Restart();
            }
            else
            {
                GM.GetRandomNumberWithDifferentValueFromBefore(ref randomNumber, ref currentIndex_Block, blockHeavy.Length);
                isBlockImpact = true;
                animator.CrossFade(blockLight[currentIndex_Block].animationName, 0.1f);
                currentBlockAnimation = blockHeavy[currentIndex_Block];
                CameraBattle.mInstance.StartCameraShake(0.5f);
                blockCounterTimer.Restart();
            }

            if (CameraBattle.mInstance.targetTransforms.Count == 0 && lastEnemyAttackedYou != null)
            {
                var directionTowardEnemy = (lastEnemyAttackedYou.position - GM.playerPosition).normalized;
                directionTowardEnemy.y = 0;
                ControlManager.controlTransforms.player.rotation = Quaternion.LookRotation(directionTowardEnemy);
                temporaryTarget = lastEnemyAttackedYou;
            }
        }

        public void PlayAnimation_TakingHit(float damageThreshold, Status baseStatus, Status currentStatus)
        {
            isTakingHit = true;

            animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 0.0f);

            if (currentStatus.HP <= 0)
            {
                isDeath = true;

                if (damageThreshold > baseStatus.HP * .4f)
                    animator.CrossFade(deathAirAnimationName, .1f);
                else animator.CrossFade(deathAnimationName, .1f);

                temporaryTarget = null;
                lastEnemyAttackedYou = null;

                return;
            }

            if (damageThreshold > baseStatus.HP * 0.45f)
            {
                animator.CrossFade(hitAnimationNameAir, .1f);
                CameraBattle.mInstance.StartCameraShake(2.0f);
                BigDamageEnter.enabled = true;
                BigDamageExit.enabled = true;
            }
            else if (damageThreshold > baseStatus.HP * 0.25f)
            {
                GM.GetRandomNumberWithDifferentValueFromBefore(ref randomNumber, ref currentIndex_Hit, hitAnimationNamesBIG.Length);
                animator.CrossFade(hitAnimationNamesBIG[currentIndex_Hit], .1f);
                CameraBattle.mInstance.StartCameraShake(1.5f);
            }
            else
            {
                GM.GetRandomNumberWithDifferentValueFromBefore(ref randomNumber, ref currentIndex_Hit, hitAnimationNames.Length);
                animator.CrossFade(hitAnimationNames[currentIndex_Hit], .1f);
                CameraBattle.mInstance.StartCameraShake(1.0f);
            }
        }

        public void PlayAnimation_KickedByEnemy()
        {
            if (isTakingHit) return;

            isTakingHit = true;

            animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 0.0f);

            GM.GetRandomNumberWithDifferentValueFromBefore(ref randomNumber, ref currentIndex_Hit, hitAnimationNamesBIG.Length);
            animator.Play(hitAnimationNamesBIG[currentIndex_Hit]);
        }

        public void PlayAnimation_KnockedAIR()
        {
            isTakingHit = true;
            isTakingHitBIG = true;

            animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 0.0f);

            animator.Play(hitAnimationNameAir);
            BigDamageEnter.enabled = true;
            BigDamageExit.enabled = true;
        }

        //========================================================================================================================//
        //   U T I L I T Y      M E T H O D S                                                                                     //
        //========================================================================================================================//
        private void Damaging()
        {
            if (!damaging)
            {
                isAbleToAttackAgain = true;
                return; 
            }

            if (isTakingHit || isDeath || isRolling || isDodging || isBlockImpact || isBlockImpactBIG)
            {
                damaging = false;
                isAttacking = false;
                return;
            }

            animationTimer = Mathf.MoveTowards(animationTimer, currentAttackAnimation.animationLength, Time.deltaTime);

            if (isFacingToNearbyEnemy)
            {
                var lookRotation = Quaternion.LookRotation(directionToNearbyEnemy);
                ControlManager.controlTransforms.player.rotation = Quaternion.Slerp(ControlManager.controlTransforms.player.rotation, lookRotation, 5 * Time.deltaTime);
            }

            if (animationTimer > currentAttackAnimation.damageTiming && isFirstHit)
            {
                isFirstHit = false;

                isFacingToNearbyEnemy = false;

                DoDamage(currentAttackAnimation);
            }

            if (isSecondHit && animationTimer > currentAttackAnimation.damageTiming2)
            {
                isSecondHit = false;

                isFacingToNearbyEnemy = false;

                DoDamage(currentAttackAnimation);
            }

            if (animationTimer > currentAttackAnimation.GetIsAbleToAttackAgain)
            {
                isAbleToAttackAgain = true;
                damaging = false;
                isAttacking = false;
            }
        }

        private void DoDamage(AttackAnimationSetting attackAnimationSetting)
        {
            var cols = Physics.OverlapSphere(ControlManager.controlTransforms.attackCollider.position, .5f, LayerMask.GetMask("Enemy"));
            if (cols.Length == 0) return;

            for (int x = 0; x < cols.Length; x++)
            {
                var baseAI = cols[x].GetComponent<Tamana.AI.BaseAI>();
                baseAI.TakeDamageFromAttacker(PlayerInfo.mInstance.baseStatus.AT * attackAnimationSetting.attackMultiplier);
                GM.isBattling = true;
                CameraBattle.updateNearestEnemyTimer = 0.0f;
                if (baseAI.isDeath && (CameraBattle.mInstance.targetTransforms.Count > 0 || temporaryTarget != null))
                {
                    if (CameraBattle.mInstance.targetTransforms.Count > 0)
                        CameraBattle.mInstance.LookForEnemy();
                    if (temporaryTarget != null)
                        temporaryTarget = GM.GetNearestEnemy(9.0f, ControlManager.controlTransforms.player.position);
                    if (temporaryTarget == null) GM.isBattling = false;
                }
            }
        }

        private void ReadInputForCounter()
        {
            if (isReadForCounterBlock && (blockCounterTimer.ElapsedMilliseconds / 1000.0f) > counterMakeEnemyStunTimer)
            {
                var cols = Physics.OverlapSphere(ControlManager.controlTransforms.attackCollider.position, 0.3f, LayerMask.GetMask("Enemy"));
                if (cols.Length > 0)
                {
                    for (int x = 0; x < cols.Length; x++)
                    {
                        var baseAI = cols[x].GetComponent<Tamana.AI.BaseAI>();
                        baseAI.PlayBeingKickedByPlayer();
                    }
                }

                isReadForCounterBlock = false;
                blockCounterTimer.Reset();
                blockCounterTimer.Stop();
                return;
            }

            if (!isBlockImpact && !isBlockImpactBIG) return;
            if (isDeath || isRolling || isTakingHit || isTakingHitBIG || isBlockCounter) return;

            var lessThan = (blockCounterTimer.ElapsedMilliseconds / 1000.0f) < currentBlockAnimation.kickTimerEnd;
            var moreThan = (blockCounterTimer.ElapsedMilliseconds / 1000.0f) > currentBlockAnimation.kickTimerStart;

            if (lessThan && moreThan && (Input.GetButtonDown("L2")))
            {
                isReadForCounterBlock = true;
                animator.CrossFade(blockAnimationNameCounter, 0.2f);
                blockCounterTimer.Restart();
                isBlockCounter = true;
            }

        }

        private void FaceTowardNearestEnemyWhenAttacking()
        {
            if (CameraBattle.mInstance.targetTransforms.Count != 0) return;

            temporaryTarget = GM.GetNearestEnemy(9.0f, ControlManager.controlTransforms.player.position);
            if(temporaryTarget != null)
            {
                var direction = (temporaryTarget.position - ControlManager.controlTransforms.player.position).normalized;
                direction.y = 0;
                directionToNearbyEnemy = direction;
                isFacingToNearbyEnemy = true;
            }
        }

        private IEnumerator SwordTrailAutoTurnOFF()
        {
            yield return new WaitForSeconds(.6f);

            swordTrail.enabled = false;
        }
    }

    [System.Serializable]
    public struct AttackAnimationSetting
    {
        public float animationLength;
        public float damageTiming;
        public float damageTiming2;
        public float staminaCost;
        public float attackMultiplier;
        public string animationName;

        public float GetTransitionTime
        {
            get
            {
                if (damageTiming2 != 0) return damageTiming2;
                return damageTiming;
            }
        }

        public float GetIsAbleToAttackAgain
        {
            get
            {
                if (damageTiming2 != 0) return damageTiming2 * 1.25f;
                return damageTiming * 1.25f;
            }
        }
    }

    [System.Serializable]
    public struct BlockAnimationSetting
    {
        public string animationName;
        public float kickTimerStart;
        [HideInInspector]
        public float kickTimerEnd;
    }
}

