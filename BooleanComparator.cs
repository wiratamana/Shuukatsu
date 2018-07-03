using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class BooleanComparator : MonoBehaviour
    {
        public Bool[] bools;

        public void SetValue(string name, bool value)
        {
            for(int x = 0; x < bools.Length; x++)
            {
                if (bools[x].name == name)
                    bools[x].value = value;
            }
        }
    }

    [System.Serializable]
    public struct Bool
    {
        public string name;
        public bool value;
    }
}

