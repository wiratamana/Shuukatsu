using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AI
{
    public class Idle : MonoBehaviour
    {
        public Animator animator;
        private Transform playerTarget;

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            InvokeRepeating("GetPlayer", 0, 2);
        }

        // Update is called once per frame
        void Update()
        {
            RotateTowardTarget();
        }

        private void RotateTowardTarget()
        {
            if (!playerTarget) return;

            var dir = playerTarget.position - animator.transform.position;
            dir.y = 0;
            var rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5 * Time.deltaTime);
        }

        private void GetPlayer()
        {
            if (playerTarget) return;

            var cols = Physics.OverlapSphere(animator.transform.position, 7, LayerMask.GetMask("Player"));
            if (cols.Length == 0) return;

            playerTarget = cols[0].transform;
        }
    }
}

