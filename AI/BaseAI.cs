using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tamana.AI
{
    public class BaseAI : MonoBehaviour
    {
        public AI_Brain brain;
        public Image imgHP;
        public Animator animator;
        public BooleanComparator takingHitComparator;
        public string currentHitAnimationName;

        [Header("Animation Name")]
        public string[] lightAttack;
        public string[] mediumAttack;
        public string[] heavyAttack;

        public float delayHP;
        public bool updatingHP;
        public float damageThreshold;
        public float delayBeforeDestroyUI;
        public BaseAI_Strafing aiStrafing;
        public BaseAI_Attack aiAttack;
        public BaseAI_Death aiDeath;
        public BaseAI_Block aiBlock;
        public Vector3 startPosition;
        public Transform playerTransform;
        public Transform cameraTargetLookAt { get; private set; }
        public PlayerInfo playerInfo;
        public PlayerControl.Attack playerAttackInstance;

        public float distanceFromPlayer;
        public float lastDistanceFromPlayer;
        public float distanceDelta;

        private Quaternion lookRotation;
        private Vector3 directionTowardPlayer;
        private Transform enemyHPOffset;
        [HideInInspector]
        public Status currentStatus;
        [HideInInspector]
        public Status baseStatus;

        private Image enemyHP;
        private Image enemyHP_Delay;
        public bool isVisibleToPlayer;
        private const string ATTACKSTOP = "ATTACKSTOP";
        private const string DODGE = "Dodge";

        public static Canvas canvas { get; private set; }
        public static Camera mainCamera { get; private set; }
        public static short[] attackType = { 1, 1, 1, 1, 1, 1, 2, 2, 2, 3 };

        public bool isForceToLookPlayer;
        private bool isLastBoss;

        public bool isTakingDamage
        {
            get { return animator.GetBool("isTakingDamage"); }
            set { animator.SetBool("isTakingDamage", value); }
        }
        public bool isTakingDamageBIG
        {
            get { return animator.GetBool("isTakingDamageBIG"); }
            set { animator.SetBool("isTakingDamageBIG", value); }
        }
        public bool isAttacking
        {
            get { return animator.GetBool("isAttacking"); }
            set { animator.SetBool("isAttacking", value); }
        }
        public bool isUsingSpecialAttack
        {
            get { return animator.GetBool("isUsingSpecialAttack"); }
            set { animator.SetBool("isUsingSpecialAttack", value); }
        }
        public bool isDeath
        {
            get { return animator.GetBool("isDeath"); }
            set { animator.SetBool("isDeath", value); }
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
        public bool isBlockImpactAIR
        {
            get { return animator.GetBool("isBlockImpactAIR"); }
            set { animator.SetBool("isBlockImpactAIR", value); }
        }
        public bool isBlocking
        {
            get { return animator.GetBool("isBlocking"); }
            set { animator.SetBool("isBlocking", value); }
        }
        public bool isInteruptedByPlayer
        {
            get
            {
                return isTakingDamage || isTakingDamageBIG || isBlockImpact || isBlockImpactAIR || isBlockImpactBIG || isRolling || isDodging;
            }
        }
        public bool isInOffensiveState
        { get { if (isAttacking) return true; if (isUsingSpecialAttack) return true; return false; } }

        private void Awake()
        {
            brain = Instantiate(brain.gameObject).GetComponent<AI_Brain>();
            brain.name = brain.name + " " + transform.name;

            cameraTargetLookAt = GM.FindChildWithTag("TargetLookAt", transform).transform;
        }

        // Use this for initialization
        void Start()
        {
            InvokeRepeating("LookForPlayer", 0, 2);
            StartCoroutine(WaitUntilPlayerIsDeath());
            startPosition = transform.position;
            canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            enemyHPOffset = GM.FindChildWithTag("EnemyHP", transform).transform;
            animator = GetComponent<Animator>();
            takingHitComparator = GetComponent<BooleanComparator>();

            if (brain is Tamana.AI.AI_LastBoss) isLastBoss = true;

            if (brain != null) brain.ai = this;

            aiStrafing = gameObject.AddComponent<BaseAI_Strafing>();
            aiStrafing.SetUp(this);
            aiAttack = gameObject.AddComponent<BaseAI_Attack>();
            aiAttack.SetUp(this, lightAttack, mediumAttack, heavyAttack);
            aiDeath = gameObject.AddComponent<BaseAI_Death>();
            aiDeath.SetUp(this);
            aiBlock = gameObject.AddComponent<BaseAI_Block>();
            aiBlock.SetUp(this);

            baseStatus = new Status(100, 100, 50, 20);
            currentStatus = baseStatus;

            StartCoroutine(AddLearningPointWhenDeath());

            brain.aiStart();
        }

        // Update is called once per frame
        void Update()
        {
            RotateTowardPlayer();
            ForceToLookPlayer();

            UpdatePositionImageHP();

            GetDistanceFromPlayerToMe();
            ReturnToStartPositionAfterWalkingTooFar();

            //if (distanceFromPlayer < 3.14f && !isAttacking)
            //    aiAttack.PlayAttackAnimation(aiAttack.lightAttacks, ref aiAttack.isCanAttack);

            if (brain != null) brain.aiUpdate();

            aiAttack.Update();
        }

        protected void UpdateStatus(ref float delay, ref bool type, Image imgDelay, Image img)
        {
            if (type == false) return;

            if (delay != 0)
                delay = Mathf.MoveTowards(delay, 0, Time.deltaTime);
            else
            {
                img.fillAmount = Mathf.MoveTowards(img.fillAmount, imgDelay.fillAmount, .6f * Time.deltaTime);
                if (img.fillAmount == imgDelay.fillAmount)
                {
                    type = false;

                    if (imgDelay == enemyHP_Delay)
                        damageThreshold = 0;
                }
            }
        }

        public void Attack()
        {
            var rand = Random.Range(0, attackType.Length);

            switch(rand)
            {
                case 1: aiAttack.PlayAttackAnimation(aiAttack.lightAttacks, ref aiAttack.isCanAttack); break;
                case 2: aiAttack.PlayAttackAnimation(aiAttack.mediumAttacks, ref aiAttack.isCanAttack); break;
                case 3: aiAttack.PlayAttackAnimation(aiAttack.heavyAttacks, ref aiAttack.isCanAttack); break;
            }
        }

        public void TakeDamageFromAttacker(float damagePoint)
        {
            if (isDeath || isTakingDamageBIG || isBlockImpactAIR) return;
            Damage.InstantiateDamage(damagePoint, transform.position + Vector3.up);
            if (isLastBoss)
                (brain as AI_LastBoss).TakeDamageFromPlayer();

            isUsingSpecialAttack = false;
            isAttacking = false;
            animator.Play("ATTACKSTOP");

            damageThreshold += damagePoint;
            bool reCheck = false;
            DestinationFromBlockToDirectDamage_BecausePlayerAttackedFromBehindOrSide:

            if (isBlocking && !reCheck)
            {
                if (Mathf.Abs(GM.GetAngleBetweenMeAndPlayer(transform)) > 105)
                {
                    reCheck = true;
                    goto DestinationFromBlockToDirectDamage_BecausePlayerAttackedFromBehindOrSide;
                }

                Tamana.FlameSword.InstantiateParrySE();
                SwordParryEffect.InstantiateEffect(aiAttack.attackCollider);

                var blockThreshold = damageThreshold - (baseStatus.HP * 0.35f);
                if (blockThreshold > 0)
                {
                    currentStatus.HP -= blockThreshold;
                }

                if (currentStatus.HP <= 0)
                {
                    currentStatus.HP = 0;
                    delayBeforeDestroyUI = 2.0f;
                    playerTransform = null;
                    isDeath = true;
                    Minimap.DestroyEnemy(transform);

                    if (damageThreshold > baseStatus.HP * .5f)
                        aiDeath.PlayDeathAir();
                    else aiDeath.PlayDeath();

                    goto Apply;
                }

                if(brain is AI_Strong)
                {
                    if((brain as AI_Strong).isParryAndCounter)
                    {
                        if (damageThreshold < baseStatus.HP * 0.3f)
                        {
                            var aiStrong = (brain as AI_Strong);
                            aiStrong.PlayParryAndCounter(aiStrong.parryAndCounterEnter, aiStrong.parryAndCounterExit);
                            damageThreshold = 0;
                            return;
                        }
                    }
                }

                if (damageThreshold > baseStatus.HP * 0.3f)
                    aiBlock.PlayBlockImpactAIR();
                else if (damageThreshold > baseStatus.HP * 0.1f)
                    aiBlock.PlayBlockImpactBIG();
                else aiBlock.PlayBlockImpact();
            }
            else // こいつをブロックしなければー
            {
                currentStatus.HP -= damagePoint;

                if (currentStatus.HP <= 0)
                {
                    currentStatus.HP = 0;
                    delayBeforeDestroyUI = 2.0f;
                    playerTransform = null;
                    isDeath = true;
                    Minimap.DestroyEnemy(transform);

                    if (damageThreshold > baseStatus.HP * .5f)
                        aiDeath.PlayDeathAir();
                    else aiDeath.PlayDeath();
                }
                else
                {
                    if(isLastBoss && (brain as AI_LastBoss).isTakingBigDamage)
                    {

                    }
                    else
                    {
                        if (damageThreshold > baseStatus.HP * .5f)
                        {
                            isTakingDamageBIG = true;
                            isBlocking = false;
                            HitAnimation.mInstance.PlayAir(animator, ref currentHitAnimationName, takingHitComparator);
                        }
                        else if (damageThreshold > baseStatus.HP * .3f)
                        {
                            isTakingDamage = true;
                            HitAnimation.mInstance.PlayHeavy(animator, ref currentHitAnimationName, takingHitComparator);
                        }
                        else
                        {
                            isTakingDamage = true;
                            HitAnimation.mInstance.PlayLight(animator, ref currentHitAnimationName, takingHitComparator);
                        } 
                    }
                }
            }

            Apply:

            if (isAttacking) isAttacking = false;
            animator.SetLayerWeight(animator.GetLayerIndex("Attack"), 0.0f);

            enemyHP_Delay.fillAmount = currentStatus.HP / baseStatus.HP;

            delayHP = 1.5f;
            updatingHP = true;
        }

        public void PlayBeingKickedByPlayer()
        {
            if (isAttacking) isAttacking = false;
            if (isUsingSpecialAttack) isUsingSpecialAttack = false;

            animator.Play(ATTACKSTOP);

            isTakingDamage = true;
            HitAnimation.mInstance.PlayHeavy(animator, ref currentHitAnimationName, takingHitComparator);
        }

        protected void UpdatePositionImageHP()
        {
            if (enemyHP == null) return;

            if(!isLastBoss)
            {
                isVisibleToPlayer = GM.isVisibleByCamera(transform);
                if (isVisibleToPlayer)
                    enemyHP.rectTransform.position = mainCamera.WorldToScreenPoint(enemyHPOffset.position);
                else enemyHP.rectTransform.position = Vector3.one * 100000.0f;
            }

            UpdateStatus(ref delayHP, ref updatingHP, enemyHP_Delay, enemyHP);

            if (isDeath)
            {
                delayBeforeDestroyUI = Mathf.MoveTowards(delayBeforeDestroyUI, 0, Time.deltaTime);
                if (delayBeforeDestroyUI == 0)
                    Destroy(enemyHP.gameObject);
            }
        }

        protected void RotateTowardPlayer()
        {
            if (isAttacking || isUsingSpecialAttack) return;
            if (isDeath) return;
            if (isTakingDamage || isTakingDamageBIG) return;
            if (playerTransform == null) return;

            directionTowardPlayer = playerTransform.position - transform.position;
            directionTowardPlayer.y = 0;
            lookRotation = Quaternion.LookRotation(directionTowardPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5 * Time.deltaTime);
        }

        /// <summary>
        /// 自分の位置からプレイヤーの位置までの距離を獲得する。
        /// </summary>
        private void GetDistanceFromPlayerToMe()
        {
            if (playerTransform == null) return;

            var myPosition = transform.position;
            var playerPosition = playerTransform.position;
            myPosition.y = 0;
            playerPosition.y = 0;

            distanceFromPlayer = (myPosition - playerPosition).sqrMagnitude;
            distanceDelta = lastDistanceFromPlayer - distanceFromPlayer;
            lastDistanceFromPlayer = distanceFromPlayer;
        }

        public void LookForPlayer()
        {
            if (isDeath) return;
            if (playerTransform != null) return;

            float radius = 7;

            if (brain is AI_Strong)
                radius = 15;
            
            var colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Player"));
            if (colliders.Length == 0 || PlayerControl.Attack.mInstance.isDeath) return;

            InstantiateBarHP();

            if (brain is AI_Strong)
            { Theme.ChangeBGM(BGM.Bridge); }
            else Theme.ChangeBGM(BGM.Fight);

            playerTransform = colliders[0].transform;
            playerInfo = playerTransform.GetComponent<PlayerInfo>();
            playerAttackInstance = PlayerControl.Attack.mInstance;
        }

        public void InstantiateBarHP()
        {
            if (enemyHP == null && !isLastBoss)
            {
                enemyHP = Instantiate(imgHP.gameObject).GetComponent<Image>();
                enemyHP.transform.SetParent(canvas.transform);
                enemyHP.rectTransform.position = mainCamera.WorldToScreenPoint(enemyHPOffset.position);
                enemyHP.rectTransform.localScale = new Vector3(0.35f, 0.35f, 0.5f);
                enemyHP_Delay = enemyHP.transform.GetChild(0).GetComponent<Image>();

                Minimap.AddEnemy(transform);
            }

            if (enemyHP == null && isLastBoss)
            {
                var obj = Instantiate(LastBossBarHP.Prefab());
                obj.transform.SetParent(canvas.transform);
                LastBossBarHP.AdjustToFitScreen(obj, ref enemyHP, ref enemyHP_Delay);
                (brain as AI_LastBoss).SetNewBarHP(obj);
            }
        }

        public void DestroyBarHP()
        {
            if (enemyHP != null)
            {
                if (!isLastBoss)
                    Destroy(enemyHP.gameObject);
                else Destroy(enemyHP.gameObject.transform.parent.gameObject);
                Minimap.DestroyEnemy(transform);
                RecoverEnemyHP();
                Theme.ChangeBGM(BGM.Field);
            }
        }

        public void RecoverEnemyHP()
        {
            currentStatus = baseStatus;
        }

        public void Dodge()
        {
            if (isDeath || isDodging || isAttacking || isUsingSpecialAttack || isBlockImpact || isBlockImpactAIR || isBlockImpactBIG || isRolling || isTakingDamage || isTakingDamageBIG)
                return;

            animator.Play(DODGE);
            isDodging = true;
        }

        public void ForceToLookPlayer()
        {
            if(isForceToLookPlayer)
                GM.SlerpRotation(transform, GM.playerPosition, 5);
        }

        public void ReturnToStartPositionAfterWalkingTooFar()
        {
            if (isLastBoss) return;
            if (playerTransform == null) return;

            var distanceFromStart = (startPosition - transform.position).sqrMagnitude;
            var distanceFromPlayer = (playerTransform.position - transform.position).sqrMagnitude;
            var condition = distanceFromPlayer > 900.0f || distanceFromStart > 900.0f || Cinematic.isCinematicON;

            if(condition)
            {
                UnityEngine.AI.NavMeshAgent agent = transform.GetComponent<UnityEngine.AI.NavMeshAgent>();
                agent.enabled = false;
                transform.position = startPosition;
                agent.enabled = true;
                aiStrafing.strafeHorizontal = 0;
                aiStrafing.strafeVertical = 0;
                playerTransform = null;
                DestroyBarHP();
            }
        }

        private IEnumerator AddLearningPointWhenDeath()
        {
            yield return new WaitUntil(() => isDeath);
            playerInfo.AddLearningPoint();
            yield return new WaitForSeconds(5.0f);
            enabled = false;
            Destroy(brain.gameObject);
            Theme.ChangeBGM(BGM.Field);
        }

        private IEnumerator WaitBeforeReturnToStartPosition()
        {
            yield return new WaitForSeconds(8.0f);

            transform.position = startPosition;
            StartCoroutine(WaitUntilPlayerIsDeath());
        }

        private IEnumerator WaitUntilPlayerIsDeath()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => PlayerControl.Attack.mInstance.isDeath);

            StartCoroutine(WaitBeforeReturnToStartPosition());
        }
    }
}

