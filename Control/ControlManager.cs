using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Tamana.PlayerControl
{
    public class ControlManager : MonoBehaviour
    {
        public static ControlTransforms controlTransforms;
        public static JoyStick joyStick;
        public static Animator playerAnimator;
        public static NavMeshAgent navMeshAgent;
        public static PlayerInfo playerInfo;

        [Header("Setting - CameraFollow")]
        public float followSpeed;
        public float yAxisOffset;

        [Header("Setting - Movement")]
        public float playerRotationSpeed;

        [Header("Setting - CameraLookPlayer")]
        public float rotationSpeed;


        private Movement movement;
        private BaseLocomotion baseLocomotion;
        private CameraFollow cameraFollow;
        private CameraLookPlayer cameraLookPlayer;

        private void Awake()
        {
            controlTransforms.thirdPerson = GameObject.FindGameObjectWithTag("3rd Person Camera").transform;
            controlTransforms.thirdPersonBattle = GameObject.FindGameObjectWithTag("3rd Person Battle").transform;
            controlTransforms.battleCamera = GameObject.FindGameObjectWithTag("BattleCamera").transform;
            controlTransforms.offsetCamera = GameObject.FindGameObjectWithTag("CameraOffset").transform;
            controlTransforms.movement = GameObject.FindGameObjectWithTag("MovementManager").transform;
            controlTransforms.leftAnalog = GameObject.FindGameObjectWithTag("AnalogPosition").transform;
            controlTransforms.camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            controlTransforms.player = GameObject.FindGameObjectWithTag("Player").transform;
            controlTransforms.attackCollider = GM.FindChildWithTag("AttackCollider", ControlManager.controlTransforms.player.transform).transform;

            playerAnimator = controlTransforms.player.GetComponent<Animator>();
            playerInfo = controlTransforms.player.GetComponent<PlayerInfo>();

            InstantiateClass();
        }

        // Use this for initialization
        void Start()
        {
            cameraFollow.Start();
        }

        // Update is called once per frame
        void Update()
        {
            movement.Update();
            baseLocomotion.Update();
            cameraFollow.Update();
            cameraLookPlayer.Update();
        }


        private void InstantiateClass()
        {
            movement = new Movement(playerRotationSpeed);
            Movement.mInstance = movement;
            baseLocomotion = new BaseLocomotion(movement);
            cameraFollow = new CameraFollow(followSpeed, yAxisOffset, baseLocomotion);
            cameraLookPlayer = new CameraLookPlayer(rotationSpeed);
        }
    }

    public struct ControlTransforms
    {
        public Transform thirdPerson;
        public Transform battleCamera;
        public Transform thirdPersonBattle;
        public Transform offsetCamera;
        public Transform movement;
        public Transform leftAnalog;
        public Transform camera;
        public Transform player;
        public Transform attackCollider;
    }

    public struct JoyStick
    {
        public float LeftAnalogHorizontal { get { return Input.GetAxis("Analog Left Horizontal"); } }
        public float LeftAnalogVertical { get { return Input.GetAxis("Analog Left Vertical"); } }
        public float RightAnalogHorizontal { get { return Input.GetAxis("Analog Right Horizontal"); } }
        public float RightAnalogVertical { get { return Input.GetAxis("Analog Right Vertical"); } }
        public float AnalogDeadZone { get { return 0.1f; } }
    }
}

