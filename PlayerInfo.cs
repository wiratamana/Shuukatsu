using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tamana
{
    public class PlayerInfo : MonoBehaviour
    {
        [HideInInspector]
        public Status rootStatus;
        [HideInInspector]
        public Status baseStatus;
        [HideInInspector]
        public Status currentStatus;

        public StatusPoint statusPoint;
        public bool isDead
        {
            get
            {
                if (currentStatus.HP <= 0)
                {
                    currentStatus.HP = 0;
                    return true;
                }

                return false;
            }
        }
        public int learningPoint { private set; get; }

        [Header("HP & MP")]
        public Image imgHP;
        public Image imgST;
        public Image imgHP_Delay;
        public Image imgST_Delay;

        private float delayHP;
        private float delayST;
        private float delayST_StartRegenerating;
        private bool updatingHP;
        private bool updatingST;
        private bool updatingST_StartRegenerating;
        private bool isHealingHP;

        private float staminaRegenerationRate = 30.0f;

        private float damageThreshold;
        private float healingThreshold;

        public static PlayerInfo mInstance;

        private const int MAX_ALLOCATION_POINT = 11;

        // Use this for initialization
        void Start()
        {
            statusPoint.Init();
            rootStatus = GM.StatusInit.Player;
            baseStatus = GM.StatusInit.Player;
            baseStatus = rootStatus + statusPoint;
            currentStatus += baseStatus;

            learningPoint = 3;

            HPST.Adjust(baseStatus);
            mInstance = this;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateStatus(ref delayHP, ref updatingHP, imgHP_Delay, imgHP);
            UpdateStatus(ref delayST, ref updatingST, imgST_Delay, imgST);
            if (!PlayerControl.Attack.mInstance.isUsingSuperStrongAttack)
                RegenerateStamina();
            RegenerateHP();

            if (Input.GetKeyDown(KeyCode.Space)) DoDamage(10);
        }

        private void UpdateStatus(ref float delay, ref bool type, Image imgDelay, Image img)
        {
            if (type == false) return;
            if (type == updatingHP && isHealingHP) return;

            if (delay != 0)
                delay = Mathf.MoveTowards(delay, 0, Time.deltaTime);
            else
            {
                img.fillAmount = Mathf.MoveTowards(img.fillAmount, imgDelay.fillAmount, .6f * Time.deltaTime);
                if(img.fillAmount == imgDelay.fillAmount) type = false;

                if (img == imgHP) damageThreshold = 0;
            }
        }

        public void RefreshStatusBar()
        {
            imgHP.fillAmount = currentStatus.HP / baseStatus.HP;
            imgHP_Delay.fillAmount = currentStatus.HP / baseStatus.HP;
            imgST.fillAmount = currentStatus.ST / baseStatus.ST;
            imgST_Delay.fillAmount = currentStatus.ST / baseStatus.ST;
        }
        
        private void RegenerateStamina()
        {
            if (!updatingST_StartRegenerating) return;

            if (delayST_StartRegenerating != 0)
                delayST_StartRegenerating = Mathf.MoveTowards(delayST_StartRegenerating, 0, Time.deltaTime);
            else
            {
                currentStatus.ST = Mathf.MoveTowards(currentStatus.ST, baseStatus.ST, (staminaRegenerationRate) * Time.deltaTime);
                //currentStatus.ST = Mathf.MoveTowards(currentStatus.ST, baseStatus.ST, (baseStatus.ST * 1.5f) * Time.deltaTime);
                imgST.fillAmount = currentStatus.ST / baseStatus.ST;
                imgST_Delay.fillAmount = currentStatus.ST / baseStatus.ST;
                if(currentStatus.ST == baseStatus.ST) updatingST_StartRegenerating = false;
            }
        }

        private void RegenerateHP()
        {
            if (!isHealingHP) return;

            if (healingThreshold != 0)
            {
                var threshold = healingThreshold;
                healingThreshold = Mathf.MoveTowards(healingThreshold, 0, ((baseStatus.HP * 0.33f) * 2.0f) * Time.deltaTime);
                currentStatus.HP += Mathf.Abs(threshold - healingThreshold);
                if(currentStatus.HP > baseStatus.HP)
                {
                    currentStatus.HP = baseStatus.HP;
                    healingThreshold = 0.0f;
                    isHealingHP = false;
                }

                imgHP_Delay.fillAmount = currentStatus.HP / baseStatus.HP;
            }
            else
                isHealingHP = false;
        }

        public void DoDamage(float damagePoint)
        {
            var dmg = damagePoint - (baseStatus.DF * Random.Range(0.5f, 1.15f));
            if (dmg < 0) dmg = 10.0f;

            damageThreshold += dmg;
            delayHP = 1;
            updatingHP = true;

            if (PlayerControl.Attack.mInstance.isBlocking)
            {
                PlayerControl.Attack.mInstance.PlayAnimation_BlockImpact(dmg, damageThreshold, baseStatus, ref currentStatus);
                imgHP_Delay.fillAmount = currentStatus.HP / baseStatus.HP;
                imgST_Delay.fillAmount = currentStatus.ST / baseStatus.ST;
                delayST = 1;
                updatingST = true;
                delayST_StartRegenerating = 2;
                updatingST_StartRegenerating = true;
                return;
            }

            currentStatus.HP -= dmg;
            if (currentStatus.HP < 0)
                currentStatus.HP = 0;

            if(healingThreshold > 0)
            {
                var wira = imgHP.fillAmount * baseStatus.HP;
                wira -= dmg;

                imgHP.fillAmount = wira / baseStatus.HP;
            }

            PlayerControl.Attack.mInstance.PlayAnimation_TakingHit(damageThreshold, baseStatus, currentStatus);

            imgHP_Delay.fillAmount = currentStatus.HP / baseStatus.HP;
        }
        public void DoDamageWithoutAnimation(float damagePoint)
        {
            var dmg = damagePoint - (baseStatus.DF * Random.Range(0.5f, 1.15f));
            if (dmg < 0) dmg = 10.0f;

            damageThreshold += dmg;
            delayHP = 1;
            updatingHP = true;

            if (PlayerControl.Attack.mInstance.isBlocking)
            {
                imgHP_Delay.fillAmount = currentStatus.HP / baseStatus.HP;
                imgST_Delay.fillAmount = currentStatus.ST / baseStatus.ST;
                delayST = 1;
                updatingST = true;
                delayST_StartRegenerating = 2;
                updatingST_StartRegenerating = true;
                return;
            }

            currentStatus.HP -= dmg;
            if (currentStatus.HP < 0)
            {
                currentStatus.HP = 0;
                PlayerControl.Attack.mInstance.animator.CrossFade(PlayerControl.Attack.deathAirAnimationName, .1f);
                PlayerControl.Attack.mInstance.isDeath = true;
            }

            if (healingThreshold > 0)
            {
                var wira = imgHP.fillAmount * baseStatus.HP;
                wira -= dmg;

                imgHP.fillAmount = wira / baseStatus.HP;
            }

            imgHP_Delay.fillAmount = currentStatus.HP / baseStatus.HP;
        }

        public void DoHeal(float healPoint)
        {
            isHealingHP = true;

            healingThreshold += healPoint;

            var wira = Mathf.MoveTowards(currentStatus.HP, baseStatus.HP, healingThreshold);

            imgHP.fillAmount = wira / baseStatus.HP;
        }

        public void StaminaManipulator(float staminaUsagePoint, ref bool isCanAttack)
        {
            if (currentStatus.ST < baseStatus.ST * 0.1f)
            {
                isCanAttack = false;
                return;
            }
            else isCanAttack = true;

            currentStatus.ST -= staminaUsagePoint;
            if (currentStatus.ST < 0)
                currentStatus.ST = 0;

            imgST_Delay.fillAmount = currentStatus.ST / baseStatus.ST;
            delayST = 1;
            delayST_StartRegenerating = 2;
            updatingST_StartRegenerating = true;
            updatingST = true;
        }

        public void StaminaRunning(ref bool isAble2Run, float staminaDepleteRate)
        {
            if (currentStatus.ST == 0)
            {
                isAble2Run = false;
                return;
            }
            else isAble2Run = true;

            currentStatus.ST = Mathf.MoveTowards(currentStatus.ST, 0, staminaDepleteRate * Time.deltaTime);
            imgST.fillAmount = currentStatus.ST / baseStatus.ST;
            imgST_Delay.fillAmount = currentStatus.ST / baseStatus.ST;

            delayST_StartRegenerating = 2;
            updatingST_StartRegenerating = true;
        }

        public void ApplyLevelUP()
        {
            baseStatus = rootStatus + statusPoint;
            imgHP.fillAmount = currentStatus.HP / baseStatus.HP;
            imgHP_Delay.fillAmount = currentStatus.HP / baseStatus.HP;

            imgST.fillAmount = currentStatus.ST / baseStatus.ST;
            imgST_Delay.fillAmount = currentStatus.ST / baseStatus.ST;
            HPST.Adjust(baseStatus);
        }

        public void UseLearningPoint(Stats stats)
        {
            if (learningPoint == 0) return;

            switch(stats)
            {
                case Stats.HP:   AddPoint(ref statusPoint.HP); break;
                case Stats.ST:   AddPoint(ref statusPoint.ST); staminaRegenerationRate += 5.0f; break;
                case Stats.AT:   AddPoint(ref statusPoint.AT); PlayerControl.Attack.mInstance.attackSpeedMultiplier += 0.015f; break;
                case Stats.DF:   AddPoint(ref statusPoint.DF); break;
            }

        }

        public void AddLearningPoint()
        {
            learningPoint++;
        }

        private void AddPoint(ref int point)
        {
            if (point >= MAX_ALLOCATION_POINT)
                return;

            point++;
            learningPoint--;
        }
    }

    public enum Stats { HP, ST, AT, DF }
}

