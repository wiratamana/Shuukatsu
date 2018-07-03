using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Tamana.PlayerControl
{
    public class Movement
    {
        public float playerRotationSpeed;

        private Vector3 cameraDirection
        {
            get
            {
                Vector3 direction = ControlManager.controlTransforms.thirdPerson.position - ControlManager.controlTransforms.camera.position;
                direction.y = 0;
                return direction;
            }
        }
        private Vector3 leftAnalogPosition;
        public Vector3 directionToAnalog { get { return ControlManager.controlTransforms.leftAnalog.position - ControlManager.controlTransforms.thirdPerson.position; } }
        private Quaternion lookDirection { get { return Quaternion.LookRotation(cameraDirection); } }
        private Quaternion lookDirectionAnalog { get { return Quaternion.LookRotation(directionToAnalog); } }
        public bool isLeftAnalogMoved
        {
            get
            {
                if (Mathf.Abs(ControlManager.joyStick.LeftAnalogHorizontal) > ControlManager.joyStick.AnalogDeadZone)
                    return true;
                if (Mathf.Abs(ControlManager.joyStick.LeftAnalogVertical) > ControlManager.joyStick.AnalogDeadZone)
                    return true;

                return false;
            }
        }
        public bool isRightAnalogMoved
        {
            get
            {
                if (Mathf.Abs(ControlManager.joyStick.RightAnalogHorizontal) > ControlManager.joyStick.AnalogDeadZone)
                    return true;
                if (Mathf.Abs(ControlManager.joyStick.RightAnalogVertical) > ControlManager.joyStick.AnalogDeadZone)
                    return true;

                return false;
            }
        }

        public Vector3 getLeftAnalogPosition { get { return leftAnalogPosition; } }

        private float delayBeforeAutoRotate;
        private float autoRotateSpeed;

        public bool isRunning
        {
            get
            {
                return ControlManager.playerAnimator.GetBool("isRunning");
            }
            set
            {
                ControlManager.playerAnimator.SetBool("isRunning", value);
            }
        }

        public Movement(float playerRotationSpeed)
        {
            this.playerRotationSpeed = playerRotationSpeed;
        }

        public static Movement mInstance;

        private bool isAble2Run;

        // Update is called once per frame
        public void Update()
        {
            if (LevelUP.isON || Message.activeMessage != null || Cinematic.isCinematicON) return;
            if (Input.GetButtonDown("Pad") && LevelUP.isAbleToOpenLevelUPMenu) LevelUP.Show();

            ControlManager.controlTransforms.movement.position = ControlManager.controlTransforms.thirdPerson.position;
            ControlManager.controlTransforms.movement.rotation = lookDirection;

            GetAnalogPosition();

            ControlManager.controlTransforms.leftAnalog.position = ControlManager.controlTransforms.movement.localPosition + leftAnalogPosition;
            Move();
            BeginSprint();
        }

        private void BeginSprint()
        {
            if (CameraBattle.mInstance.targetTransforms.Count > 0 || ControlManager.playerAnimator.GetBool("isRolling") || ControlManager.playerAnimator.GetBool("isAttacking") || LevelUP.isON)
            {
                isRunning = false;
                return;
            }

            if (CameraBattle.mInstance.targetTransforms.Count == 0 && Attack.temporaryTarget != null)
            {
                Attack.temporaryTarget = null;
            }

            if (isRunning)
            {
                ControlManager.playerInfo.StaminaRunning(ref isAble2Run, 10);
                if (!isAble2Run)
                {
                    isRunning = false;
                    return;
                }

                if (Input.GetButtonUp("Cross") || !isLeftAnalogMoved)
                    isRunning = false;
                else return;
            }

            if (Input.GetButtonDown("Cross") && isLeftAnalogMoved)
            {
                ControlManager.playerInfo.StaminaRunning(ref isAble2Run, 10);
                if (!isAble2Run) return;

                isRunning = true;
                ControlManager.playerAnimator.CrossFade("Longs_SprintStart", .3f);
            }
        }

        private void GetAnalogPosition()
        {
            leftAnalogPosition = Vector3.zero;

            if (Mathf.Abs(ControlManager.joyStick.LeftAnalogHorizontal) > ControlManager.joyStick.AnalogDeadZone)
                leftAnalogPosition += ControlManager.controlTransforms.movement.right * ControlManager.joyStick.LeftAnalogHorizontal;
            if (Mathf.Abs(ControlManager.joyStick.LeftAnalogVertical) > ControlManager.joyStick.AnalogDeadZone)
                leftAnalogPosition += ControlManager.controlTransforms.movement.forward * ControlManager.joyStick.LeftAnalogVertical;
        }

        private void Move()
        {
            if (!isLeftAnalogMoved || ControlManager.playerAnimator.GetBool("isRolling") || ControlManager.playerAnimator.GetBool("isAttacking"))
                return;

            if (Attack.mInstance.isTakingHit || Attack.mInstance.isBlockImpact || Attack.mInstance.isBlockImpactBIG || Attack.mInstance.isDeath)
                return;

            if (Attack.mInstance.isRolling || Attack.mInstance.isDodging)
                return;

            ControlManager.controlTransforms.player.rotation = Quaternion.Slerp(ControlManager.controlTransforms.player.rotation, lookDirectionAnalog, playerRotationSpeed * Time.deltaTime);
            AutoRotateCameraTowardPlayerFacingForward();
        }

        private void AutoRotateCameraTowardPlayerFacingForward()
        {
            if (isRightAnalogMoved)
            {
                delayBeforeAutoRotate = Time.time + 1.0f;
                autoRotateSpeed = 0.0f;
                return; 
            }
            if (Time.time < delayBeforeAutoRotate) return;

            autoRotateSpeed = Mathf.MoveTowards(autoRotateSpeed, 1.0f, Time.deltaTime);

            var playerForward = GM.playerTransform.forward;
            var cameraForward = ControlManager.controlTransforms.thirdPerson.forward;
            var angle = Vector3.Angle(playerForward, cameraForward);
            if (angle < 10 || angle > 75) return;

            var direction = playerForward;
            direction.y = 0;
            ControlManager.controlTransforms.thirdPerson.transform.rotation =
                Quaternion.Slerp(ControlManager.controlTransforms.thirdPerson.transform.rotation, Quaternion.LookRotation(direction),
                autoRotateSpeed *Time.deltaTime);

            Vector3 eulerAngle = ControlManager.controlTransforms.thirdPerson.eulerAngles;
            ControlManager.controlTransforms.thirdPerson.eulerAngles = new Vector3(CameraOrbit.eulerAngleX, eulerAngle.y, 0);
        }
    }
}

