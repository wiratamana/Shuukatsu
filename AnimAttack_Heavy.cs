using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class AnimAttack_Heavy : MonoBehaviour
    {
        public PlayerControl.AttackAnimationSetting[] HeavyAttackAnimations;

        public static AnimAttack_Heavy mInstance;
        private void Awake()
        {
            mInstance = this;
        }
    }
}

