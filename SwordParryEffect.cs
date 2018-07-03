using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tamana
{
    public class SwordParryEffect : MonoBehaviour
    {
        private static GameObject prefab;

        private void Awake()
        {
            prefab = GM.LoadResources("TANGTANG Effect");
        }

        public static GameObject InstantiateEffect(BoxCollider attackCollider)
        {
            var obj = Instantiate(prefab);
            var posX = Random.Range(attackCollider.transform.position.x - attackCollider.size.x * 0.5f, attackCollider.transform.position.x + attackCollider.size.x * 0.5f);
            var posY = Random.Range(attackCollider.transform.position.y - attackCollider.size.y * 0.5f, attackCollider.transform.position.y + attackCollider.size.y * 0.5f);
            var posZ = Random.Range(attackCollider.transform.position.z - attackCollider.size.z * 0.5f, attackCollider.transform.position.z + attackCollider.size.z * 0.5f);
            obj.transform.position = new Vector3(posX, posY, posZ);

            return obj;
        }
    }
}

