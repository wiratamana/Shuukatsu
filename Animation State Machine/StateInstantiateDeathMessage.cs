using UnityEngine;
using System.Collections;

namespace Tamana.AnimatorManager
{
    public class StateInstantiateDeathMessage : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            PlayerControl.Attack.mInstance.StartCoroutine(Message.WaitAndShowDeathMessage());
        }
    }
}

