using UnityEngine;
using System.Collections;

namespace Tamana.AI
{
    public class BaseAI_Block : MonoBehaviour
    {
        private BaseAI baseAI;
        private const string blockStartName = "Longs_BlockStart";
        private readonly string[] blockImpactName = { "Longs_Block_L", "Longs_Block_L2", "Longs_Block_D", "Longs_Block_R2", "Longs_Block_R", };
        private const string blockImpactNameAIR = "BlockAir";

        private int currentIndex;
        private int randomNumber;

        public void SetUp(BaseAI baseAI)
        {
            this.baseAI = baseAI;
        }

        public void StartBlocking()
        {
            if (baseAI.isBlocking) return;

            baseAI.isBlocking = true;
            baseAI.animator.CrossFade(blockStartName, .1f);
        }

        public void StopBlocking()
        {
            if (!baseAI.isBlocking) return;

            baseAI.isBlocking = false;
        }

        public void PlayBlockImpact()
        {
            baseAI.isBlockImpact = true;

            GM.GetRandomNumberWithDifferentValueFromBefore(ref randomNumber, ref currentIndex, blockImpactName.Length);
            baseAI.animator.CrossFade(blockImpactName[currentIndex], 0.1f);
        }

        public void PlayBlockImpactBIG()
        {
            baseAI.isBlockImpact = true;
            baseAI.isBlockImpactBIG = true;

            GM.GetRandomNumberWithDifferentValueFromBefore(ref randomNumber, ref currentIndex, blockImpactName.Length);
            baseAI.animator.CrossFade(blockImpactName[currentIndex], 0.1f);
        }

        public void PlayBlockImpactAIR()
        {
            baseAI.isBlockImpact = true;
            baseAI.isBlockImpactBIG = true;
            baseAI.isBlockImpactAIR = true;
            baseAI.isBlocking = false;

            baseAI.animator.CrossFade(blockImpactNameAIR, 0.1f);
        }
    }
}

