using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AnimatorManager
{
    public enum AB { A, B }
    public class isRolling : StateMachineBehaviour
    {
        public AB ab;
        public static AB ROLLING;
    } 
}
