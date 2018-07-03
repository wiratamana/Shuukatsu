using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.PlayerControl
{
    public class CameraOrbit : MonoBehaviour
    {
        [Header("Adjustment")]
        [Range(1, 100)]
        public float orbitSpeed;
        [Range(1, 100)]
        public float verticalOrbitSpeed;
        public float xAxis_MIN;
        public float xAxis_MAX;
        public bool invertCameraAxisX;

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

        public static float eulerAngleX;
        private float xAxis;
        private float yAxis;

        public static CameraOrbit mInstance;

        private void Awake()
        {
            mInstance = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (Cinematic.isCinematicON) return;

            Orbit();
        }

        private void Orbit()
        {
            if (!isRightAnalogMoved) return;

            yAxis = ControlManager.joyStick.RightAnalogHorizontal * orbitSpeed * Time.deltaTime;

            if(!invertCameraAxisX)
            {
                if (ControlManager.joyStick.RightAnalogVertical > ControlManager.joyStick.AnalogDeadZone && xAxis < xAxis_MAX)
                    xAxis = Mathf.MoveTowards(xAxis, xAxis_MAX, ControlManager.joyStick.RightAnalogVertical * verticalOrbitSpeed * Time.deltaTime);
                if (ControlManager.joyStick.RightAnalogVertical < (ControlManager.joyStick.AnalogDeadZone * -1) && xAxis > xAxis_MIN)
                    xAxis = Mathf.MoveTowards(xAxis, xAxis_MIN, Mathf.Abs(ControlManager.joyStick.RightAnalogVertical * verticalOrbitSpeed * Time.deltaTime));
            }
            else
            {
                if (ControlManager.joyStick.RightAnalogVertical > ControlManager.joyStick.AnalogDeadZone && xAxis > xAxis_MIN)
                    xAxis = Mathf.MoveTowards(xAxis, xAxis_MIN, ControlManager.joyStick.RightAnalogVertical * verticalOrbitSpeed * Time.deltaTime);
                if (ControlManager.joyStick.RightAnalogVertical < (ControlManager.joyStick.AnalogDeadZone * -1) && xAxis < xAxis_MAX)
                    xAxis = Mathf.MoveTowards(xAxis, xAxis_MAX, Mathf.Abs(ControlManager.joyStick.RightAnalogVertical * verticalOrbitSpeed * Time.deltaTime));
            }

            Vector3 eulerAngle = ControlManager.controlTransforms.thirdPerson.eulerAngles;
            ControlManager.controlTransforms.thirdPerson.eulerAngles = new Vector3(xAxis, eulerAngle.y + yAxis, 0);
            eulerAngleX = ControlManager.controlTransforms.thirdPerson.eulerAngles.x;
        }

        public void ResetAxisX()
        {
            xAxis = 0;
            Vector3 eulerAngle = ControlManager.controlTransforms.thirdPerson.eulerAngles;
            ControlManager.controlTransforms.thirdPerson.eulerAngles = new Vector3(xAxis, eulerAngle.y + yAxis, eulerAngle.z);
            eulerAngleX = ControlManager.controlTransforms.thirdPerson.eulerAngles.x;
        }
    }
}

