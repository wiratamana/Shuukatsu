using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.PlayerControl
{
    public class CameraBattle : MonoBehaviour
    {
        [Header("Adjustment")]
        [Range(0.1f, 20.0f)]
        public float rotationSpeed;

        public List<Transform> targetTransforms = new List<Transform>();
        private System.Diagnostics.Stopwatch doubleClickTimer = new System.Diagnostics.Stopwatch();
        private bool isReadingForDoubleClick_R3;
        private bool isReadingForSingleClick_R3 = true;
        private bool isReleasingCamera;
        private float timeSinceFirstClick_Millisecond;
        private float cameraShakeTimer;

        public static float updateNearestEnemyTimer;

        public const float doubleClickWaitingTime_Millisecond = 250.0f;
        public const float autoReleaseCameraDistance = 75.0f;

        public static CameraBattle mInstance;

        private void Awake()
        {
            mInstance = this;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateNearestEnemy();
            CameraLock();          
            ReleasingCamera();
            AutoReleaseCamera();
            MakePlayerAlwaysFaceTargetEnemy();
            ControlStrafeMovement();
            ShakeCamera();

            ReadForDoubleClick_R3();

            if (Input.GetButtonDown("R3") && isReadingForSingleClick_R3 && !isReadingForDoubleClick_R3)
            {
                if (!isReadingForDoubleClick_R3) { isReadingForDoubleClick_R3 = true; doubleClickTimer.Restart(); }
            }

        }

        public void CameraLock()
        {
            if (ControlManager.controlTransforms.camera.parent != ControlManager.controlTransforms.battleCamera) return;

            var direction = targetTransforms[0].position - ControlManager.controlTransforms.thirdPersonBattle.position;
            var lookRotation = Quaternion.LookRotation(direction);

            ControlManager.controlTransforms.camera.localPosition = Vector3.Lerp(ControlManager.controlTransforms.camera.localPosition, Vector3.zero, 5 * Time.deltaTime);
            ControlManager.controlTransforms.camera.localEulerAngles = Vector3.Lerp(ControlManager.controlTransforms.camera.localEulerAngles, Vector3.zero, 5 * Time.deltaTime);
            ControlManager.controlTransforms.thirdPersonBattle.rotation = Quaternion.Slerp(ControlManager.controlTransforms.thirdPersonBattle.rotation, lookRotation, 2 * Time.deltaTime);
        }

        public void ReleasingCamera()
        {
            if (!isReleasingCamera) return;

            var rot = Quaternion.Euler(new Vector3(20.0f, 0.0f, 0.0f));
            ControlManager.controlTransforms.camera.localRotation = Quaternion.RotateTowards(ControlManager.controlTransforms.camera.localRotation, rot, 15 * Time.deltaTime);
            ControlManager.controlTransforms.camera.localPosition = Vector3.MoveTowards(ControlManager.controlTransforms.camera.localPosition, Vector3.zero, 3 * Time.deltaTime);

            if (Mathf.Abs((ControlManager.controlTransforms.camera.localEulerAngles.x) - 20) < 1.0f && ControlManager.controlTransforms.camera.localPosition == Vector3.zero)
                isReleasingCamera = false;
        }

        public void UpdateNearestEnemy()
        {
            if (targetTransforms.Count == 0 && Attack.temporaryTarget == null) return;

            if (updateNearestEnemyTimer != 1.0f)
                updateNearestEnemyTimer = Mathf.MoveTowards(updateNearestEnemyTimer, 1.0f, Time.deltaTime);
            else
            {
                updateNearestEnemyTimer = 0.0f;
                Attack.temporaryTarget = GM.GetNearestEnemy(9.0f, ControlManager.controlTransforms.player.position);
                if(Attack.temporaryTarget == null)
                {
                    ReleaseBattleCamera();
                }
            }
        }

        public void LookForEnemy()
        {
            var a = Physics.OverlapSphere(transform.position, 9, LayerMask.GetMask("Enemy"));

            if (a.Length == 0)
            {
                if (targetTransforms.Count > 0)
                {
                    ReleaseBattleCamera();
                }
                if (Attack.temporaryTarget != null)
                    Attack.temporaryTarget = null;

                return;
            }

            if (a.Length == 1)
            {
                if (targetTransforms.Count == 0 && !a[0].GetComponent<Tamana.AI.BaseAI>().isDeath)
                {
                    targetTransforms.Add(GM.FindChildWithTag("TargetLookAt", a[0].transform).transform);
                }
                else
                {
                    ReleaseBattleCamera();
                    if (Attack.temporaryTarget != null)
                        Attack.temporaryTarget = null;
                    return;
                }
            }

            if (a.Length > 1)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    var trans = GM.FindChildWithTag("TargetLookAt", a[i].transform).transform;
                    var baseAI = a[i].GetComponent<Tamana.AI.BaseAI>();

                    if (targetTransforms.Contains(trans))
                    {
                        if (baseAI.isDeath)
                            targetTransforms.Remove(trans);

                        continue;
                    }
                    else
                    {
                        if (baseAI.isDeath)
                            continue;
                    }

                    targetTransforms.Add(trans);
                }

                if (targetTransforms.Count > 0)
                {
                    var firstIndex = targetTransforms[0];
                    targetTransforms.Remove(targetTransforms[0]);
                    targetTransforms.Add(firstIndex);
                }
                else
                {
                    ReleaseBattleCamera();
                    if (Attack.temporaryTarget != null)
                        Attack.temporaryTarget = null;
                    return;
                }

            }

            var directionForward = ControlManager.controlTransforms.thirdPerson.forward;
            directionForward.y = 0;
            ControlManager.controlTransforms.thirdPersonBattle.rotation = Quaternion.LookRotation(directionForward);
            ControlManager.controlTransforms.camera.SetParent(ControlManager.controlTransforms.battleCamera);

            ControlManager.playerAnimator.SetLayerWeight(ControlManager.playerAnimator.GetLayerIndex("Strafing"), 1.0f);
            GM.isBattling = true;
        }

        public void ReleaseBattleCamera()
        {
            targetTransforms.Clear();
            Attack.temporaryTarget = null;
            PlayerControl.CameraOrbit.mInstance.ResetAxisX();
            var directionForward = ControlManager.controlTransforms.thirdPersonBattle.forward;
            directionForward.y = 0;
            ControlManager.controlTransforms.thirdPerson.rotation = Quaternion.LookRotation(directionForward);
            ControlManager.controlTransforms.camera.SetParent(ControlManager.controlTransforms.offsetCamera);
            ControlManager.playerAnimator.SetLayerWeight(ControlManager.playerAnimator.GetLayerIndex("Strafing"), 0.0f);
            isReleasingCamera = true;
            GM.isBattling = false;
        }

        private void MakePlayerAlwaysFaceTargetEnemy()
        {
            if (targetTransforms.Count == 0 && Attack.temporaryTarget == null)
                return;
            if (Attack.mInstance.isDeath || Attack.mInstance.isRolling || Attack.mInstance.isDodging || Attack.mInstance.isTakingHit || Attack.mInstance.isBlockImpact || Attack.mInstance.isBlockImpactBIG)
                return;

            Vector3 facingDirection = Vector3.zero;

            if(targetTransforms.Count > 0)
                facingDirection = targetTransforms[0].position - ControlManager.controlTransforms.player.position;
            else if (Attack.temporaryTarget != null)
                facingDirection = Attack.temporaryTarget.position - ControlManager.controlTransforms.player.position;

            if (facingDirection == Vector3.zero) return;

            facingDirection.y = 0;
            var lookRotation = Quaternion.LookRotation(facingDirection);

            ControlManager.controlTransforms.player.rotation = Quaternion.Lerp(ControlManager.controlTransforms.player.rotation, lookRotation, 10 * Time.deltaTime);
        }

        private void ControlStrafeMovement()
        {
            if (targetTransforms.Count == 0 && Attack.temporaryTarget == null)
            {
                GM.SetAnimatorLayerWeight("Strafing", 0.0f);
                return;
            }

            GM.SetAnimatorLayerWeight("Strafing", 1.0f);

            if (GetAnalogAxis(ControlManager.joyStick.LeftAnalogHorizontal) == 0 && GetAnalogAxis(ControlManager.joyStick.LeftAnalogVertical) == 0)
            {
                ControlManager.playerAnimator.SetFloat("StrafeHorizontal", 0.0f);
                ControlManager.playerAnimator.SetFloat("StrafeVertical", 0.0f);
                return;
            }

            if(Input.GetButton("Cross"))
            {
                ControlManager.playerAnimator.SetFloat("StrafeHorizontal", GetAnalogAxis(ControlManager.joyStick.LeftAnalogHorizontal) * 2);
                ControlManager.playerAnimator.SetFloat("StrafeVertical", GetAnalogAxis(ControlManager.joyStick.LeftAnalogVertical) * 2);
            }
            else
            {
                if(targetTransforms.Count == 0)
                {
                    var directionToAnalog = (ControlManager.controlTransforms.leftAnalog.position - ControlManager.controlTransforms.player.position).normalized;
                    var camPosPlusDirection = ControlManager.controlTransforms.player.position + directionToAnalog;
                    var wira = ControlManager.controlTransforms.player.InverseTransformPoint(camPosPlusDirection) * 2;
                    var horizontal = wira.x;
                    var vertical = wira.z;
                    ControlManager.playerAnimator.SetFloat("StrafeHorizontal", Mathf.MoveTowards(ControlManager.playerAnimator.GetFloat("StrafeHorizontal"), horizontal, 4 * Time.deltaTime));
                    ControlManager.playerAnimator.SetFloat("StrafeVertical", Mathf.MoveTowards(ControlManager.playerAnimator.GetFloat("StrafeVertical"), vertical, 4 * Time.deltaTime));
                }
                else
                {
                    ControlManager.playerAnimator.SetFloat("StrafeHorizontal", GetAnalogAxis(ControlManager.joyStick.LeftAnalogHorizontal));
                    ControlManager.playerAnimator.SetFloat("StrafeVertical", GetAnalogAxis(ControlManager.joyStick.LeftAnalogVertical));
                }
            }
        }

        private float GetAnalogAxis(float direction)
        {
            var dir = Mathf.Abs(direction);
            if (dir < ControlManager.joyStick.AnalogDeadZone)
                return 0;

            return direction;
        }

        private void AutoReleaseCamera()
        {
            if (targetTransforms.Count == 0)
                return;

            if ((targetTransforms[0].position - PlayerControl.ControlManager.controlTransforms.player.position).sqrMagnitude > autoReleaseCameraDistance)
                ReleaseBattleCamera();
        }

        private void ReadForDoubleClick_R3()
        {
            if(!isReadingForSingleClick_R3)
            {
                if (doubleClickTimer.ElapsedMilliseconds - timeSinceFirstClick_Millisecond > doubleClickWaitingTime_Millisecond)
                {
                    isReadingForSingleClick_R3 = true;
                    doubleClickTimer.Stop();
                }
            }


            if (!isReadingForDoubleClick_R3) return;

            if (doubleClickTimer.ElapsedMilliseconds > doubleClickWaitingTime_Millisecond)
            {
                isReadingForDoubleClick_R3 = false;

                LookForEnemy();
            }
            else
            {
                if (Input.GetButtonDown("R3"))
                {
                    ReleaseBattleCamera();
                    timeSinceFirstClick_Millisecond = doubleClickTimer.ElapsedMilliseconds;
                    isReadingForDoubleClick_R3 = false;
                    isReadingForSingleClick_R3 = false;
                }
            }
        }

        private void ShakeCamera()
        {
            if(cameraShakeTimer > 0)
            {
                var pos = ControlManager.controlTransforms.camera.localPosition + ((Random.insideUnitSphere * 0.75f) * cameraShakeTimer);
                ControlManager.controlTransforms.camera.localPosition = Vector3.Lerp(ControlManager.controlTransforms.camera.localPosition, pos, 2.5f * Time.deltaTime);
                cameraShakeTimer = Mathf.MoveTowards(cameraShakeTimer, 0, Time.deltaTime);
            }
            else
            {
                ControlManager.controlTransforms.camera.localPosition = Vector3.Lerp(ControlManager.controlTransforms.camera.localPosition, Vector3.zero, 2.5f * Time.deltaTime);
            }
        }

        public void StartCameraShake(float time)
        {
            cameraShakeTimer = time;
        }
    }
}

