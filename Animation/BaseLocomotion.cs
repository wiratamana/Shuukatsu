using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.PlayerControl
{
    public class BaseLocomotion
    {
        public Animator animator;
        public PlayerControl.Movement movement;

        public float speed
        {
            get
            {
                if (LevelUP.isON || Message.activeMessage != null || Cinematic.isCinematicON)
                    return 0;

                var playerPosition = ControlManager.controlTransforms.player.position;
                var analogPosition = movement.getLeftAnalogPosition;
                analogPosition += playerPosition;

                var distance = Vector3.Distance(playerPosition, analogPosition);
                if (distance > 1) distance = 1;

                return distance;
            }
        }

        public BaseLocomotion(Movement movement)
        {
            this.movement = movement;
        }

        // Update is called once per frame
        public void Update()
        {
            ControlManager.playerAnimator.SetFloat("MovementSpeed", speed);
        }
    }
}

