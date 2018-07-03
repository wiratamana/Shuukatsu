using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AI
{
    public class BaseAI_Strafing : MonoBehaviour
    {
        private Animator animator;

        public float strafeHorizontal
        {
            get { return animator.GetFloat("strafeHorizontal"); }
            set { if (value < -2.0f) value = -2.0f; if (value > 2.0f) value = 2.0f; animator.SetFloat("strafeHorizontal", value); }
        }
        public float strafeVertical
        {
            get { return animator.GetFloat("strafeVertical"); }
            set { if (value < -2.0f) value = -2.0f; if (value > 2.0f) value = 2.0f; animator.SetFloat("strafeVertical", value); }
        }

        public void SetUp(BaseAI baseAI)
        {
            animator = baseAI.animator;
        }

        public void Stop()
        {
            strafeHorizontal = 0.0f;
            strafeVertical = 0.0f;
        }
        
        public void MoveForward()
        {
            strafeHorizontal = 0.0f;
            strafeVertical = 1.0f;
        }

        public void MoveBackward()
        {
            strafeHorizontal = 0.0f;
            strafeVertical = -1.0f;
        }

        public void StrafeForward()
        {
            strafeVertical = Mathf.MoveTowards(strafeVertical, 1.0f, Time.deltaTime);
        }

        public void StrafeBackward()
        {
            strafeVertical = Mathf.MoveTowards(strafeVertical, -1.0f, Time.deltaTime);
        }

        public void StrafeRight()
        {
            strafeHorizontal = Mathf.MoveTowards(strafeHorizontal, 1.0f, Time.deltaTime);
        }

        public void StrafeLeft()
        {
            strafeHorizontal = Mathf.MoveTowards(strafeHorizontal, -1.0f, Time.deltaTime);
        }

        public void StopStrafingHorizontally()
        {
            strafeHorizontal = Mathf.MoveTowards(strafeHorizontal, 0.0f, Time.deltaTime);
        }

        public void StopStrafingVertically()
        {
            strafeVertical = Mathf.MoveTowards(strafeVertical, 0.0f, Time.deltaTime);
        }
    }
}

