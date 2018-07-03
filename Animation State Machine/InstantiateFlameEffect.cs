using UnityEngine;
using System.Collections;

namespace Tamana.AnimatorManager
{
    public class InstantiateFlameEffect : StateMachineBehaviour
    {
        public float instantiateAt;
        private bool enabled = true;
        private float timeSinceStarted;
        private float elapsedTime;
        private float insAT;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            insAT = instantiateAt - 0.1f;
            insAT *= animator.GetFloat("specialAttackMultiplier");
            timeSinceStarted = Time.time;
            elapsedTime = 0.0f;
            enabled = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!enabled) return;
            elapsedTime = Time.time - timeSinceStarted;

            if(elapsedTime > insAT)
            {
                enabled = false;
                Tamana.FlameSword.InstantiateFlameSE();
            }
        }

    }
}

