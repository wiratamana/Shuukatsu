using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.PlayerControl
{
    public class CameraLookPlayer
    {
        [Header("Adjustment")]
        public float rotationSpeed;

        private Vector3 direction { get { return ControlManager.controlTransforms.thirdPerson.position - ControlManager.controlTransforms.camera.position; } }
        private Quaternion targetLookAt { get { return Quaternion.LookRotation(direction); } }

        public CameraLookPlayer(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;
        }

        // Update is called once per frame
        public void Update()
        {
            if (CameraBattle.mInstance.targetTransforms.Count != 0 || Attack.temporaryTarget != null) return;

            //ControlManager.controlTransforms.camera.localPosition = Vector3.Lerp(ControlManager.controlTransforms.camera.localPosition, Vector3.zero, 5 * Time.deltaTime);
            //ControlManager.controlTransforms.camera.rotation = Quaternion.Slerp(ControlManager.controlTransforms.camera.rotation, targetLookAt, rotationSpeed);
        }
    }
}

