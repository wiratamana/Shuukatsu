using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class HitAnimation : MonoBehaviour
    {
        public string[] lightAnimation;
        public string[] heavyAnimation;
        public string airAnimation;

        private static int randomNumber;

        public static HitAnimation mInstance;

        // Use this for initialization
        void Start()
        {
            mInstance = this;
        }

        public void PlayLight(Animator animator, ref string currentHitAnimationName)
        {
            randomNumber = Random.Range(0, heavyAnimation.Length);
            while (currentHitAnimationName == lightAnimation[randomNumber])
                randomNumber = Random.Range(0, lightAnimation.Length);
            currentHitAnimationName = lightAnimation[randomNumber];

            animator.Play(lightAnimation[randomNumber]);
        }

        public void PlayLight(Animator animator, ref string currentHitAnimationName, BooleanComparator booleanComparator)
        {
            randomNumber = Random.Range(0, heavyAnimation.Length);
            while (currentHitAnimationName == lightAnimation[randomNumber])
                randomNumber = Random.Range(0, lightAnimation.Length);
            currentHitAnimationName = lightAnimation[randomNumber];

            animator.Play(lightAnimation[randomNumber]);
            booleanComparator.SetValue(lightAnimation[randomNumber], true);
        }

        public void PlayHeavy(Animator animator, ref string currentHitAnimationName)
        {
            randomNumber = Random.Range(0, heavyAnimation.Length);
            while (currentHitAnimationName == heavyAnimation[randomNumber])
                randomNumber = Random.Range(0, heavyAnimation.Length);
            currentHitAnimationName = heavyAnimation[randomNumber];

            animator.Play(heavyAnimation[randomNumber]);
        }

        public void PlayHeavy(Animator animator, ref string currentHitAnimationName, BooleanComparator booleanComparator)
        {
            randomNumber = Random.Range(0, heavyAnimation.Length);
            while (currentHitAnimationName == heavyAnimation[randomNumber])
                randomNumber = Random.Range(0, heavyAnimation.Length);
            currentHitAnimationName = heavyAnimation[randomNumber];

            animator.Play(heavyAnimation[randomNumber]);
            booleanComparator.SetValue(heavyAnimation[randomNumber], true);
        }

        public void PlayAir(Animator animator, ref string currentHitAnimationName)
        {
            if (currentHitAnimationName == airAnimation)
                return;
            currentHitAnimationName = airAnimation;

            animator.Play(airAnimation);
        }

        public void PlayAir(Animator animator, ref string currentHitAnimationName, BooleanComparator booleanComparator)
        {
            if (currentHitAnimationName == airAnimation)
                return;
            currentHitAnimationName = airAnimation;

            animator.Play(airAnimation);
            booleanComparator.SetValue(airAnimation, true);
        }

    }
}

