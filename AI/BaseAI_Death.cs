using UnityEngine;
using System.Collections;

namespace Tamana.AI
{
    public class BaseAI_Death : MonoBehaviour
    {
        private BaseAI baseAI;
        private string deathAir = "Death Air";
        private string[] death = { "Death1", "Death2" };

        public void SetUp(BaseAI baseAI)
        {
            this.baseAI = baseAI;
        }

        public void PlayDeathAir()
        {
            baseAI.isBlocking = false;
            baseAI.animator.CrossFade(deathAir, 0.1f);
        }

        public void PlayDeath()
        {
            var randomNumber = Random.Range(0, death.Length);
            baseAI.animator.CrossFade(death[randomNumber], 0.1f);
        }
    }
}

