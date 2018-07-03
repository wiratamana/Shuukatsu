using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.PlayerControl
{
    public class CameraFollow
    {
        [Header("References")]
        public BaseLocomotion baseLocomotion;

        [Header("Adjustment")]
        public float followSpeed;
        public float yAxisOffset;

        public CameraFollow(float followSpeed, float yAxisOffset, BaseLocomotion baseLocomotion)
        {
            this.followSpeed = followSpeed;
            this.yAxisOffset = yAxisOffset;
            this.baseLocomotion = baseLocomotion;
        }

        private Vector3 playerPositionOffset
        {
            get
            {
                Vector3 playerPosition = ControlManager.controlTransforms.player.position;
                playerPosition.y += yAxisOffset;
                return playerPosition;
            }
        }

        private float realSpeed { get { return 2 + (baseLocomotion.speed * followSpeed); } }

        public void Start()
        {
            ControlManager.controlTransforms.thirdPerson.position = playerPositionOffset;
        }

        public void Update()
        {
            ControlManager.controlTransforms.thirdPerson.position = 
                Vector3.Lerp(ControlManager.controlTransforms.thirdPerson.position, playerPositionOffset, realSpeed * Time.deltaTime);

            ControlManager.controlTransforms.thirdPersonBattle.position =
                Vector3.Lerp(ControlManager.controlTransforms.thirdPersonBattle.position, playerPositionOffset, realSpeed * Time.deltaTime);
        }
    }
}

