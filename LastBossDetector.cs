using UnityEngine;
using System.Collections;

namespace Tamana
{
    public class LastBossDetector : MonoBehaviour
    {
        public Tamana.AI.BaseAI lastBossAI;
        public GameObject obstacle;
        private readonly WaitForSeconds REPEAT_THREE_TIMES_PER_SECOND = new WaitForSeconds(0.333333f);

        public static LastBossDetector mInstance { private set; get; }

        private void Start()
        {
            mInstance = this;   
        }

        public void BEGIN()
        {
            StartCoroutine(WAIT_UNTIL_PLAYER_ENTERING_BOSS_FIELD());
        }

        private IEnumerator WAIT_UNTIL_PLAYER_ENTERING_BOSS_FIELD()
        {
            while(true)
            {
                yield return REPEAT_THREE_TIMES_PER_SECOND;

                if((GM.playerPosition - lastBossAI.transform.position).sqrMagnitude < 225.0f)
                {
                    Cinematic.mInstance.DESTINATION.SetActive(false);
                    lastBossAI.InstantiateBarHP();

                    lastBossAI.playerTransform = GM.playerTransform;
                    lastBossAI.playerInfo = GM.playerTransform.GetComponent<PlayerInfo>();
                    lastBossAI.playerAttackInstance = PlayerControl.Attack.mInstance;

                    obstacle.SetActive(true);

                    Theme.ChangeBGM(BGM.LastBoss);
                    StartCoroutine(WAIT_FOR_EIGHT_SECOND_BEFORE_WAIT_FOR_PLAYER_DEATH());
                    break;
                }
            }
        }

        private IEnumerator WAIT_FOR_EIGHT_SECOND_BEFORE_WAIT_FOR_PLAYER_DEATH()
        {
            yield return new WaitUntil(() => PlayerControl.Attack.mInstance.isDeath);

            yield return new WaitForSeconds(8.0f);

            obstacle.SetActive(false);
            StartCoroutine(WAIT_UNTIL_PLAYER_ENTERING_BOSS_FIELD());
        }
    }
}

