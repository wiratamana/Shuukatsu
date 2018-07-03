using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public static class GM 
    {
        public static bool isBattling;
        private static StatusInit statusInit;
        public static Vector3 playerPosition { get { return PlayerControl.ControlManager.controlTransforms.player.position; } }
        public static Transform playerTransform { get { return PlayerControl.ControlManager.controlTransforms.player; } }
        public static Camera mainCamera { get { return AI.BaseAI.mainCamera; } }
        public static Canvas canvas { get { return AI.BaseAI.canvas; } }
        public static Animator playerAnimator { get { return PlayerControl.Attack.mInstance.animator; } }

        public static GameObject FindChildWithTag(string tag, Transform parent)
        {
            for(int x = 0; x < parent.childCount; x++)
            {
                if (parent.GetChild(x).tag == tag)
                    return parent.GetChild(x).gameObject;
            }

            return null;
        }

        public static GameObject FindWithTag(string tag)
        {
            var a = GameObject.FindWithTag(tag);

            if(a == null)
            {
                Debug.Log("Can't find the object with tag '" + tag + "'.");
                return null;
            }

            return a;
        }

        public static GameObject LoadResources(string path)
        {
            var obj = Resources.Load(path) as GameObject;
            if (obj == null)
            {
                Debug.LogError("Cannot find object from 'Resources' folder. Path : " + path);
                return null;
            }

            return obj;
        }

        public static void GetRandomNumberWithDifferentValueFromBefore(ref int randomVar, ref int currentVar, int arrayLength)
        {
            randomVar = Random.Range(0, arrayLength);
            while (randomVar == currentVar)
                randomVar = Random.Range(0, arrayLength);

            currentVar = randomVar;
        }

        public static StatusInit StatusInit
        {
            get
            {
                if (statusInit == null)
                    statusInit = FindWithTag("StatusInit").GetComponent<StatusInit>();

                return statusInit;
            }
        }

        public static Transform GetNearestEnemy(Collider[] colliders, Vector3 myPosition)
        {
            var distance = 1000000.0f;
            Transform transform = null;

            for(int i = 0; i < colliders.Length; i++)
            {
                var baseAI = colliders[i].GetComponent<Tamana.AI.BaseAI>();
                if (baseAI.isDeath) continue;
                var sqrMagnitude = (colliders[i].transform.position - myPosition).sqrMagnitude;
                if (sqrMagnitude < distance && !baseAI.isDeath)
                {
                    distance = sqrMagnitude;
                    transform = colliders[i].transform;
                }
            }

            if (distance <= 10000.0f)
                return transform;

            return null;
        }
        
        public static Transform GetNearestEnemy(float radius, Vector3 myPosition)
        {
            var colliders = Physics.OverlapSphere(myPosition, radius, LayerMask.GetMask("Enemy"));
            if (colliders.Length == 0) return null;

            var distance = 1000000.0f;
            Transform transform = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                var baseAI = colliders[i].GetComponent<Tamana.AI.BaseAI>();
                var sqrMagnitude = (colliders[i].transform.position - myPosition).sqrMagnitude;
                if (sqrMagnitude < distance && !baseAI.isDeath)
                {
                    distance = sqrMagnitude;
                    transform = colliders[i].transform;
                }
            }

            if (distance <= 10000.0f)
                return transform;

            return null;
        }

        public static void SetAnimatorLayerWeight(string layerName, float value)
        {
            if (value < 0.0f) value = 0.0f;
            if (value > 1.0f) value = 1.0f;

            Tamana.PlayerControl.ControlManager.playerAnimator.SetLayerWeight(Tamana.PlayerControl.ControlManager.playerAnimator.GetLayerIndex(layerName), value);
        }

        public static void SetAnimatorLayerWeight(string layerName, float value, Animator animator)
        {
            if (value < 0.0f) value = 0.0f;
            if (value > 1.0f) value = 1.0f;

            animator.SetLayerWeight(animator.GetLayerIndex(layerName), value);
        }

        public static Tamana.AnimatorManager.SetBool GetSetBool(string stateName, Animator animator)
        {
            var StateMachineBehaviours = animator.GetBehaviours<Tamana.AnimatorManager.SetBool>();

            for(int i = 0; i < StateMachineBehaviours.Length; i++)
            {
                if ((StateMachineBehaviours[i] as Tamana.AnimatorManager.SetBool).stateName == stateName)
                {
                    return StateMachineBehaviours[i] as Tamana.AnimatorManager.SetBool;
                }
            }

            Debug.Log("Message = GetSetBool return null. From = " + animator.transform.name + ", when trying to get '" + stateName + "'.");
            return null;
        }

        public static PlayerInfo GetPlayerInfo(Vector3 point)
        {
            var cols = Physics.OverlapSphere(point, 0.3f, LayerMask.GetMask("Player"));
            if (cols.Length == 0) return null;

            for(int i = 0; i < cols.Length; i++)
            {
                var playerInfo = cols[i].GetComponent<PlayerInfo>();
                if (playerInfo != null)
                    return playerInfo;
            }

            return null;
        }

        public static PlayerInfo GetPlayerInfo(Vector3 point, float radius)
        {
            var cols = Physics.OverlapSphere(point, radius, LayerMask.GetMask("Player"));
            if (cols.Length == 0) return null;

            for (int i = 0; i < cols.Length; i++)
            {
                var playerInfo = cols[i].GetComponent<PlayerInfo>();
                if (playerInfo != null)
                    return playerInfo;
            }

            return null;
        }

        public static Tamana.PlayerControl.AttackAnimationSetting GetAnimationData(string originalAnimationName)
        {
            var datas = AnimAttack_Light.mInstance.LightAttackAnimation;
            for (int i = 0; i < datas.Length; i++)
                if (datas[i].animationName == originalAnimationName)
                    return datas[i];

            datas = AnimAttack_Medium.mInstance.MediumAttackAnimation;
            for(int i = 0; i < datas.Length; i++)
                if (datas[i].animationName == originalAnimationName)
                    return datas[i];

            datas = AnimAttack_Heavy.mInstance.HeavyAttackAnimations;
            for (int i = 0; i < datas.Length; i++)
                if (datas[i].animationName == originalAnimationName)
                    return datas[i];

            Debug.LogWarning("Couldn't find animation with name '" + originalAnimationName + "'.");
            return datas[0];
        }

        public static void SlerpRotation(Transform transform, Vector3 targetPosition, float t)
        {
            var directionTowardTarget = (targetPosition - transform.position).normalized;
            directionTowardTarget.y = 0;
            var lookRotation = Quaternion.LookRotation(directionTowardTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, t * Time.deltaTime);
        }

        public static void SlerpRotation(Transform transform, Vector3 targetPosition)
        {
            var directionTowardTarget = (targetPosition - transform.position).normalized;
            directionTowardTarget.y = 0;
            var lookRotation = Quaternion.LookRotation(directionTowardTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 40 * Time.deltaTime);
        }

        public static float GetAngleBetweenMeAndPlayer(Transform transform)
        {
            var directionForward = transform.forward;
            var directionTowardPlayer = (playerPosition - transform.position).normalized;
            directionForward.y = 0; directionTowardPlayer.y = 0;
            return Vector3.Angle(directionForward, directionTowardPlayer);
        }

        /// <summary>
        /// 戻り値が150.0f以上場合、プレイーが俺のお方向に向いている。
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static float GetAngleBetweenMyForwardAndPlayerForward(Transform transform)
        {
            var playerForward = playerTransform.forward;
            var MyForward = transform.forward;
            playerForward.y = 0; MyForward.y = 0;
            return Vector3.Angle(MyForward, playerForward);
        }

        public static bool isPlayerLookingTowardMe(Transform transform)
        {
            var forward = playerTransform.forward;
            var directionTowardPlayer = (transform.position - playerPosition).normalized;
            forward.y = 0; directionTowardPlayer.y = 0;

            return Vector3.Angle(forward, directionTowardPlayer) < 45.0f;
        }

        public static void isVisibleByCamera(ref bool isVisible, Transform transform)
        {
            var cameraForward = mainCamera.transform.forward;
            var directionTowardTarget = (transform.position - mainCamera.transform.position).normalized;
            cameraForward.y = 0; directionTowardTarget.y = 0;

            if (Vector3.Angle(cameraForward, directionTowardTarget) > 90)
                isVisible = false;
            else isVisible = true;
        }

        public static bool isVisibleByCamera(Transform transform)
        {
            var cameraForward = mainCamera.transform.forward;
            var directionTowardTarget = (transform.position - mainCamera.transform.position).normalized;
            cameraForward.y = 0; directionTowardTarget.y = 0;

            if (Vector3.Angle(cameraForward, directionTowardTarget) > 90)
                return false;
            return true;
        }

        public static float isVisibleByCamera_Float(Transform transform, bool zeroAxisY)
        {
            var cameraForward = mainCamera.transform.forward;
            var directionTowardTarget = (transform.position - mainCamera.transform.position).normalized;
            if(zeroAxisY)
                cameraForward.y = 0; directionTowardTarget.y = 0;

            return Vector3.Angle(cameraForward, directionTowardTarget);
        }
    }
}

