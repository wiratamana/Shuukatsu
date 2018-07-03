using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class AnimAttack_Custom : MonoBehaviour
    {
        public PlayerControl.AttackAnimationSetting[] animations;

        public static AnimAttack_Custom mInstance;
        

        private void Awake()
        {
            mInstance = this;
        }

        public void LookFor(string animationName, ref PlayerControl.AttackAnimationSetting target)
        {
            for(int i = 0; i < animations.Length; i++)
            {
                if(animationName == animations[i].animationName)
                {
                    target = animations[i];
                    return;
                }
            }

            Debug.Log("Couldn't find target animation");
        }
    }
}

