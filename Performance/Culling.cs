using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.Performance
{
    public class Culling : MonoBehaviour
    {
        public Light[] lights;
        public Transform player;
        public BoxCollider boxCollider;

        private CullingInstances CI;

        // Use this for initialization
        void Start()
        {
            CI = new CullingInstances(lights, boxCollider, player);
        }

        // Update is called once per frame
        void Update()
        {
            CI.Update();
        }
    }
}

