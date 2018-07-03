using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class AnimAttack_Light : MonoBehaviour
    {
        public PlayerControl.AttackAnimationSetting[] LightAttackAnimation;

        public static AnimAttack_Light mInstance;
        private void Awake()
        {
            mInstance = this;
        }
    }
}

