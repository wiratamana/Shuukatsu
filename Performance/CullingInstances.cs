using UnityEngine;

namespace Tamana.Performance
{
    public class CullingInstances
    {
        private BoxCollider boxCollider;
        public Light[] lights;
        public Transform player;
        private bool on;

        public CullingInstances(Light[] lights, BoxCollider boxCollider, Transform player)
        {
            this.lights = lights;
            this.boxCollider = boxCollider;
            this.player = player;
        }

        public void Update()
        {
            if (boxCollider.bounds.Contains(player.position))
            {
                if (on) return;

                for (int x = 0; x < lights.Length; x++)
                    lights[x].enabled = false;

                on = true;
            }
            else
            {
                if(on)
                {
                    for (int x = 0; x < lights.Length; x++)
                        lights[x].enabled = true;
                    on = false;
                }
            }
        }
    }
}

