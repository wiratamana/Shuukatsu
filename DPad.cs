using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public enum DPadDirection { Up, Down, Left, Right }
    public class DPad : MonoBehaviour
    {
        private static bool up;
        private static bool down;
        private static bool left;
        private static bool right;

        private static bool lastFrame_up;
        private static bool lastFrame_down;
        private static bool lastFrame_left;
        private static bool lastFrame_right;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            left = Input.GetAxis("DPadX") < 0.0f;
            right = Input.GetAxis("DPadX") > 0.0f;
            up = Input.GetAxis("DPadY") > 0.0f;
            down = Input.GetAxis("DPadY") < 0.0f;
        }

        private void LateUpdate()
        {
            KeyReaderUp(ref lastFrame_left, left);
            KeyReaderUp(ref lastFrame_down, down);
            KeyReaderUp(ref lastFrame_right, right);
            KeyReaderUp(ref lastFrame_up, up);
        }

        private static void KeyReaderUp(ref bool _r, bool dir)
        {
            if (_r && !dir)
                _r = false;
        }

        private static void KeyReaderDown(ref bool _r, bool dir)
        {
            if (!_r && dir)
                _r = true;
        }

        public static bool GetKeyDown(DPadDirection dPadDirection)
        {
            if(dPadDirection == DPadDirection.Left && lastFrame_left != left && !lastFrame_left)
            {
                lastFrame_left = true;
                return true;
            }

            if (dPadDirection == DPadDirection.Right && lastFrame_right != right && !lastFrame_right)
            {
                lastFrame_right = true;
                return true;
            }

            if (dPadDirection == DPadDirection.Up && lastFrame_up != up && !lastFrame_up)
            {
                lastFrame_up = true;
                return true;
            }

            if (dPadDirection == DPadDirection.Down && lastFrame_down != down && !lastFrame_down)
            {
                lastFrame_down = true;
                return true;
            }

            return false;
        }

        public static bool GetKeyUp(DPadDirection dPadDirection)
        {
            if (dPadDirection == DPadDirection.Left && lastFrame_left != left && lastFrame_left)
            {
                lastFrame_left = false;
                return true;
            }

            if (dPadDirection == DPadDirection.Right && lastFrame_right != right && lastFrame_right)
            {
                lastFrame_right = false;
                return true;
            }

            if (dPadDirection == DPadDirection.Up && lastFrame_up != up && lastFrame_up)
            {
                lastFrame_up = false;
                return true;
            }

            if (dPadDirection == DPadDirection.Down && lastFrame_down != down && lastFrame_down)
            {
                lastFrame_down = false;
                return true;
            }

            return false;
        }
    }
}

