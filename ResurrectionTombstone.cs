using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public enum ParticleSystemStatus { Play, Stop, Pause }
    
    public class ResurrectionTombstone : MonoBehaviour
    {
        public static GameObject checkpoints { private set; get; }
        public static List<ResurrectionTombstone> tombstones = new List<ResurrectionTombstone>();
        private static bool runOnlyOneTime;
        public static bool isPlayerNearby { get; private set; }
        public static ResurrectionTombstone currentActiveTombstone { private set; get; }
        public static GameObject activeTombstone { private set; get; }

        public TombstoneIcon tombstoneIcon; 
        public bool isActive { private set; get; }
        public ParticleSystem mParticle { private set; get; }
        public Light mLight { private set; get; }
        public Transform resurrectionPosition { private set; get; }

        public static WaitForSeconds delay { private set; get; }

        private void Awake()
        {
            GetAllComponent();

            if (!runOnlyOneTime)
            {
                checkpoints = GM.FindWithTag("Checkpoints");

                for (int i = 0; i < checkpoints.transform.childCount; i++)
                    tombstones.Add((checkpoints.transform.GetChild(i).GetComponent<ResurrectionTombstone>()));

                activeTombstone = GameObject.FindWithTag("ActiveTombstone");
                activeTombstone.SetActive(false);
                activeTombstone.GetComponent<RectTransform>().localPosition = new Vector2(
                    Screen.width * 0.5f - 229.1f, Screen.height * -0.5f + 56.6f);

                runOnlyOneTime = true;
            }

            delay = new WaitForSeconds(0.33f);
        }

        private void Start()
        {
            StartCoroutine(CheckForPlayerDistance());
            Active(tombstones[0]);
        }

        private void Update()
        {
            var isPlayerFacingMe = GM.isPlayerLookingTowardMe(transform);
            var playerDistance = (transform.position - GM.playerPosition).sqrMagnitude;

            if (isPlayerFacingMe && playerDistance < 2.0f && !isActive)
            {
                isPlayerNearby = true;
                activeTombstone.SetActive(true);
            }
            else
            {
                isPlayerNearby = false;
                activeTombstone.SetActive(false);
            }

            ReadInput();
        }

        public static void Active(ResurrectionTombstone tombstone)
        {
            for(int i = 0; i < tombstones.Count; i++)
            {
                if (tombstones[i] == tombstone)
                    tombstones[i].SetActive();
                else tombstones[i].SetDeactive();
            }

            currentActiveTombstone = tombstone;
            activeTombstone.SetActive(false);
        }

        //=========================================================================================================================
        //   A C T I V E     A N D     D E A C T I V E
        //=========================================================================================================================
        public void SetActive()
        {
            isActive = true;

            tombstoneIcon.Switch(TombstoneIcon.ONOFF.ON);
            SetParticleEffectStatus(ParticleSystemStatus.Play);
            TurnLight(true);
        }

        public void SetDeactive()
        {
            isActive = false;

            tombstoneIcon.Switch(TombstoneIcon.ONOFF.OFF);
            SetParticleEffectStatus(ParticleSystemStatus.Stop);
            TurnLight(false);
        }

        //=========================================================================================================================
        //   L I G H T ,    P A R T I C L E    S Y S T E M    C O M P O N E N T S
        //=========================================================================================================================
        private void GetAllComponent()
        {
            mParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
            if (mParticle == null) { Debug.Log("mParticle == null"); }
            mLight = transform.GetChild(0).GetChild(1).GetComponent<Light>();
            if (mLight == null) { Debug.Log("mLight == null"); }
            resurrectionPosition = transform.GetChild(1);
        }

        public void TurnLight(bool value)
        {
            mLight.enabled = value;
        }

        public void SetParticleEffectStatus(ParticleSystemStatus status)
        {
            switch(status)
            {
                case ParticleSystemStatus.Stop:     mParticle.Stop();   break;
                case ParticleSystemStatus.Play:     mParticle.Play();   break;
                case ParticleSystemStatus.Pause:    mParticle.Pause();  break;
            }
        }


        //=========================================================================================================================
        //   U T I L I T Y     M E T H O D S
        //=========================================================================================================================
        private IEnumerator CheckForPlayerDistance()
        {
            while(true)
            {
                if ((transform.position - GM.playerPosition).sqrMagnitude < 16.0f)
                    enabled = true;
                else enabled = false;

                yield return delay;
            }
        }

        private void ReadInput()
        {
            if(Input.GetButtonDown("Circle"))
            {
                if (!isPlayerNearby) return;

                Active(this);
            }
        }

        public static void ResurrectPlayer()
        {
            var navMesh = GM.playerTransform.GetComponent<UnityEngine.AI.NavMeshAgent>();
            navMesh.enabled = false;
            GM.playerTransform.position = currentActiveTombstone.resurrectionPosition.position;
            PlayerControl.Attack.mInstance.isDeath = false;
            PlayerInfo.mInstance.currentStatus = PlayerInfo.mInstance.baseStatus;

            var tombstoneDirection = currentActiveTombstone.transform.forward;
            tombstoneDirection.y = 0.0f;
            GM.playerTransform.rotation = Quaternion.LookRotation(tombstoneDirection);

            PlayerInfo.mInstance.RefreshStatusBar();
            PlayerControl.ControlManager.controlTransforms.thirdPerson.position = GM.playerPosition;
            PlayerControl.ControlManager.controlTransforms.thirdPersonBattle.position = GM.playerPosition;
            PlayerControl.ControlManager.controlTransforms.thirdPerson.rotation = Quaternion.LookRotation(-tombstoneDirection);
            PlayerControl.ControlManager.controlTransforms.thirdPersonBattle.rotation = Quaternion.LookRotation(-tombstoneDirection);
            PlayerControl.CameraBattle.mInstance.ReleaseBattleCamera();

            LevelUP.mInstance.StartCoroutine(WaitBeforeAbleToUseLevelUPMenuAgain());

            Theme.ChangeBGM(BGM.Field);

            navMesh.enabled = true;
        }

        private static IEnumerator WaitBeforeAbleToUseLevelUPMenuAgain()
        {
            yield return new WaitForSeconds(10.0f);

            LevelUP.isAbleToOpenLevelUPMenu = true;
            LevelUP.mInstance.StartCoroutine(LevelUP.mInstance.WaitUntilPlayerDeath());
            Cinematic.mInstance.DESTINATION.SetActive(true);
        }
    } 
}
