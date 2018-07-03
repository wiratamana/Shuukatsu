using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AI
{
    public enum Role { Sleep, Defensive, Offensive, LoneWolf, Death }
    public enum Direction { Forward, Backward, Left, Right }
    public class AI_TwoMan : AI_Brain
    {
        public BaseAI partner;
        public Role myRole;
        public AI_TwoMan partnerBrain;

        public bool isBeingTargetedByPlayer { get; private set; }
        public bool isPlayerCameraLockedToMe { get; private set; }

        public Vector3 directionFromPlayerToMe { get { return ai.transform.position - ai.playerTransform.position; } }
        public Vector3 directionFromPlayerToPartner { get { return partner.transform.position - ai.playerTransform.position; } }
        public float getAngleBetweenMyPartner
        {
            get
            {
                var dir2me = directionFromPlayerToMe.normalized;
                var dir2partner = directionFromPlayerToPartner.normalized;
                dir2me.y = 0;
                dir2partner.y = 0;

                return Vector3.Angle(dir2me, dir2partner);
            }
        }
        public float getAngleBetweenMyPartner_LastFrame { get; private set; }
        public float getAngleBetweenMyPartner_Delta { get; private set; }
        public float distanceFromMyPartner { get { return (partner.transform.position - ai.transform.position).sqrMagnitude; } }
        public float distanceFromMyPartner_LastFrame { get; private set; }
        public float distanceFromMyPartner_Delta { get; private set; }

        private AI_TwoManOffensive offensive;
        private AI_TwoManDefensive defensive;

        public override void aiStart()
        {
            base.aiStart();

            var collider = Physics.OverlapSphere(ai.transform.position, 10.0f, LayerMask.GetMask("Enemy"));
            
            for(int i = 0; i < collider.Length; i++)
            {
                var baseAI = collider[i].GetComponent<BaseAI>();
                if (baseAI == null) continue;
                if (baseAI.transform == ai.transform) continue;

                partner = baseAI;
                partnerBrain = (baseAI.brain as AI_TwoMan);
                break;
            }

            offensive = new AI_TwoManOffensive(this);
            defensive = new AI_TwoManDefensive(this);
        }

        public override void aiUpdate()
        {
            if (myRole == Role.Death) return;

            if (ai.playerTransform == null)
            {
                ai.aiStrafing.Stop();
                return;
            }

            if (ai.playerAttackInstance.isDeath)
            {
                ai.playerTransform = null;
                ai.DestroyBarHP();
                myRole = Role.Sleep;
                return;
            }

            CheckIfIAmDead();

            GetDistanceFromMyPartner();
            GetAngleBetweenMyPartner();

            GetIsBeingTargetedByPlayer();

            CheckIfPlayerHittingMe();

            AwakeWhenPlayerGetNearOfMe();

            CheckIfBeingTargettedByPlayer();

            DoesMyPartnerDead();

            offensive.UpdateLogic();
            defensive.UpdateLogic();
        }

        private void CheckIfBeingTargettedByPlayer()
        {
            if (myRole == Role.Sleep) return;

            if(isPlayerCameraLockedToMe && myRole == Role.Offensive && !partner.isDeath)
            {
                if (partnerBrain == null)
                    partnerBrain = partner.brain as AI_TwoMan;

                myRole = Role.Defensive;
                partnerBrain.myRole = Role.Offensive;
            }
        }

        private void AwakeWhenPlayerGetNearOfMe()
        {
            if(myRole == Role.Sleep && ai.playerTransform != null)
            {
                partner.InstantiateBarHP();

                partner.playerTransform = ai.playerTransform;
                partner.playerInfo = ai.playerInfo;
                partner.playerAttackInstance = ai.playerAttackInstance;

                if (isBeingTargetedByPlayer)
                {
                    myRole = Role.Defensive;
                    partnerBrain.myRole = Role.Offensive;
                }
            }
        }

        private void GetIsBeingTargetedByPlayer()
        {
            if (PlayerControl.CameraBattle.mInstance.targetTransforms.Count > 0)
            {
                if (PlayerControl.CameraBattle.mInstance.targetTransforms[0] == ai.cameraTargetLookAt)
                {
                    isBeingTargetedByPlayer = true;
                    isPlayerCameraLockedToMe = true;
                    return;
                }
                else isPlayerCameraLockedToMe = false;
            }
            else isPlayerCameraLockedToMe = false;

            if (ai.playerTransform != null)
            {
                var distanceFromPlayerToMe = ai.distanceFromPlayer;
                var distanceFromPlayerToPartner = partner.distanceFromPlayer;

                if (distanceFromPlayerToMe < 37.5f || distanceFromPlayerToPartner < 37.5f)
                {
                    if (distanceFromPlayerToMe < distanceFromPlayerToPartner)
                    {
                        isBeingTargetedByPlayer = true;
                        return;
                    }
                }
            }

            isBeingTargetedByPlayer = false;
        }

        private void DoesMyPartnerDead()
        {
            if (partner.isDeath && myRole != Role.Offensive)
            {
                myRole = Role.Offensive;
                ai.aiStrafing.Stop();
                return;
            }
        }

        private void GetDistanceFromMyPartner()
        {
            distanceFromMyPartner_Delta = distanceFromMyPartner_LastFrame - distanceFromMyPartner;
            distanceFromMyPartner_LastFrame = distanceFromMyPartner;
        }

        private void GetAngleBetweenMyPartner()
        {
            if (ai.playerTransform == null) return;

            getAngleBetweenMyPartner_Delta = getAngleBetweenMyPartner_LastFrame - getAngleBetweenMyPartner;
            getAngleBetweenMyPartner_LastFrame = getAngleBetweenMyPartner;
        }

        private void CheckIfIAmDead()
        {
            if (ai.isDeath)
                myRole = Role.Death;
        }

        private void CheckIfPlayerHittingMe()
        {
            if((ai.isTakingDamage || ai.isTakingDamageBIG) && myRole == Role.Offensive && !partner.isDeath)
            {
                myRole = Role.Defensive;
                partnerBrain.myRole = Role.Offensive;
            }
        }
    }
}

