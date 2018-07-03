using UnityEngine;
using System.Collections;

namespace Tamana.AI
{
    public enum State
    {
        Whirlwind,
        ForwardCounter,
        ParryKickHitHitHit,
        ThreeCombo,
        DodgeCounter,
    }
    public class ComponentAI_State : MonoBehaviour
    {
        public State myState;
    }
}

