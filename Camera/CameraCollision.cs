using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class CameraCollision : MonoBehaviour
    {
        private static int LAYER ;
        private static RaycastHit hit;
        public static bool isCameraColliding { private set; get; }

        private static Vector3 directionToCamera
        { get { return (GM.mainCamera.transform.position - PlayerControl.ControlManager.controlTransforms.thirdPerson.position).normalized; } }

        private void Awake()
        {
            LAYER = LayerMask.GetMask("Scene");
        }

        private void Update()
        {
            if(Physics.Raycast(PlayerControl.ControlManager.controlTransforms.thirdPerson.position,
                directionToCamera, out hit, Vector3.Distance(PlayerControl.ControlManager.controlTransforms.thirdPerson.position, PlayerControl.ControlManager.controlTransforms.offsetCamera.position), LAYER))
            {
                var directionToCamera = (PlayerControl.ControlManager.controlTransforms.thirdPerson.position - GM.mainCamera.transform.position).normalized;
                isCameraColliding = true;
                GM.mainCamera.transform.position = Vector3.Lerp(GM.mainCamera.transform.position, (hit.point + directionToCamera * 0.25f), 5 * Time.deltaTime);
            }
            else
            {
                isCameraColliding = false;
                GM.mainCamera.transform.localPosition = Vector3.Lerp(GM.mainCamera.transform.localPosition, Vector3.zero, 5 * Time.deltaTime);
            }
        }

    }
}

