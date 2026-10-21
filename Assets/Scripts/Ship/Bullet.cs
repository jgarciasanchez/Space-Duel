using UnityEngine;

namespace FLFlight {
    public class Bullet : MonoBehaviour {
        [Tooltip ("Seconds before a bullet that hit nothing is removed.")]
        public float lifetime = 5f;

        /// <summary>The ship that fired this bullet. Set when it is fired.</summary>
        public ShipInput Shooter { get; set; }

        void Start () {
            Destroy (gameObject, lifetime);
        }

        void OnTriggerEnter (Collider collider) {
            ShipInput hitShip = collider.GetComponentInParent<ShipInput> ();
            if (hitShip != null) {
                // Bullets leave the shooter's own collider right after spawning.
                if (hitShip == Shooter)
                    return;

                if (Shooter != null)
                    hitShip.TakeDamage (Shooter.damage);
                Destroy (gameObject);
            } else if (collider.CompareTag ("Miner")) {
                if (Shooter != null)
                    Shooter.ApplyMinerBuff ();
                Destroy (collider.gameObject);
                Destroy (gameObject);
            } else if (collider.CompareTag ("wall") || collider.CompareTag ("Asteroid")) {
                Destroy (gameObject);
            }
        }
    }
}
