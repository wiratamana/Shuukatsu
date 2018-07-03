using UnityEngine;
using System.Collections;

namespace Tamana
{
    public class LastBoss_Destructor : MonoBehaviour
    {
        private readonly WaitUntil UNTIL_PLAYER_IS_DEATH = new WaitUntil(() => PlayerControl.Attack.mInstance.isDeath);

        // Use this for initialization
        void Start()
        {
            StartCoroutine(WAITING_UNTIL_PLAYER_IS_DEATH());
        }

        private IEnumerator WAITING_UNTIL_PLAYER_IS_DEATH()
        {
            yield return UNTIL_PLAYER_IS_DEATH;

            Destroy(gameObject);
        }
    }
}

