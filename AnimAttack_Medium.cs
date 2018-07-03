using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class AnimAttack_Medium : MonoBehaviour
    {
        public PlayerControl.AttackAnimationSetting[] MediumAttackAnimation;

        public static AnimAttack_Medium mInstance;
        private void Awake()
        {
            mInstance = this;
        }
    }
}

