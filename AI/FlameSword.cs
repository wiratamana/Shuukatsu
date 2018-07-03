using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana.AI
{
    public class FlameSword : MonoBehaviour
    {
        private static GameObject DragonBreath;
        private static GameObject FlameHorizontal;
        private static GameObject FlameVertical;
        private static GameObject FlameDiagonal;
        public const float velocity = 7.6f;
        private static bool loadStart = true;

        public Vector3 spawnPosition;
        public Vector3 endPosition;
        public Vector3 direction;
        public float damage;

        private ParticleSystem trail;
        private ParticleSystem explosion;
        private FlameStatus status;

        private bool lightUP;

        private float distanceFromEndPosition { get { return (transform.position - endPosition).sqrMagnitude; } }
        private float distanceFromPlayer { get { return (transform.position - GM.playerPosition).sqrMagnitude; } }

        private void Start()
        {
            trail = transform.GetChild(0).GetComponent<ParticleSystem>();
            explosion = transform.GetChild(1).GetComponent<ParticleSystem>();

            trail.Play();
            explosion.Stop();
        }

        // Update is called once per frame
        void Update()
        {
            MoveTowardPlayer();
            Explosion();

            ExplosionToDestroy();
            if (status == FlameStatus.Destroyed)
                Destroy(gameObject);
        }

        private void MoveTowardPlayer()
        {
            if (status == FlameStatus.MoveTowardPlayer)
                transform.position = transform.position + (direction * velocity * Time.deltaTime);
        }

        private void Explosion()
        {
            if (status != FlameStatus.MoveTowardPlayer) return;

            if(distanceFromEndPosition < 3.0f)
            {
                SwapEffectToExplosion();
            }
            else if (distanceFromPlayer < 3.0f)
            {
                SwapEffectToExplosion();
                var a = (transform.position - GM.playerTransform.position).normalized;
                a.y = 0;
                PlayerInfo.mInstance.DoDamageWithoutAnimation(damage);
                GM.playerTransform.rotation = Quaternion.LookRotation(a);
                Tamana.PlayerControl.Attack.mInstance.PlayAnimation_KnockedAIR();
            }
        }

        private void ExplosionToDestroy()
        {
            if(status == FlameStatus.Explosion)
            {
                Tamana.FlameSword.InstantiateExplosionSE();

                if (explosion.isStopped)
                    Destroy(gameObject);

                status = FlameStatus.Destroyed;
            }
        }

        private void SwapEffectToExplosion()
        {
            status = FlameStatus.Explosion;
            trail.Stop();
            explosion.Play();
        }

        public static void InstantiateFlame(Vector3 spawnPosition, Vector3 direction, float dmg, FlameType type)
        {
            if(loadStart)
                LoadResourcesIFNULL();

            var obj = Instantiate(GetFlamePrefab(type));
            obj.layer = LayerMask.NameToLayer("FlameSword");
            obj.AddComponent<BoxCollider>();

            var flameSword = obj.GetComponent<FlameSword>();
            obj.transform.position = spawnPosition;

            flameSword.spawnPosition = spawnPosition;
            flameSword.damage = dmg;

            direction.y = 0;
            obj.transform.rotation = Quaternion.LookRotation(direction);
            flameSword.direction = direction;
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position, direction, out hit, 100000.0f, LayerMask.GetMask("LastArena")))
                flameSword.endPosition = hit.point;
        }

        private static void LoadResourcesIFNULL()
        {
            DragonBreath = Resources.Load("DragonBreath") as GameObject;
            FlameVertical = Resources.Load("FlameVertical") as GameObject;
            FlameHorizontal = Resources.Load("FlameHorizontal") as GameObject;
            FlameDiagonal = Resources.Load("FlameDiagonal") as GameObject;

            loadStart = false;
        }

        private static GameObject GetFlamePrefab(FlameType type)
        {
            switch(type)
            {
                case FlameType.DragonBreath     :  return DragonBreath;
                case FlameType.FlameDiagonal    :  return FlameDiagonal;
                case FlameType.FlameVertical    :  return FlameVertical;
                case FlameType.FlameHorizontal  :  return FlameHorizontal;
            }

            return null;
        }
    }

    public enum FlameStatus { MoveTowardPlayer, Explosion, Destroyed }
    public enum FlameType { None, DragonBreath, FlameHorizontal, FlameVertical, FlameDiagonal }
}

