using UnityEngine;

namespace FLFlight {

    [RequireComponent (typeof (Rigidbody))]
    [RequireComponent (typeof (ShipPhysics))]
    [RequireComponent (typeof (ShipInput2))]
    public class Ship2 : MonoBehaviour {
        [Tooltip ("Set this ship to be the player ship. The player ship can always be accessed through the PlayerShip property.")]
        [SerializeField] private bool isPlayer = false;

        public static Ship2 PlayerShip { get; private set; }

        public Vector3 Velocity { get { return Physics.Rigidbody.velocity; } }
        public ShipInput2 Input { get; private set; }
        public ShipPhysics Physics { get; internal set; }

        private void Awake () {
            Input = GetComponent<ShipInput2> ();
            Physics = GetComponent<ShipPhysics> ();
        }

        private void Update () {

            Physics.SetPhysicsInput (new Vector3 (Input.Strafe, 0.0f, Input.Throttle), new Vector3 (Input.Pitch, Input.Yaw, Input.Roll));

            if (isPlayer)
                PlayerShip = this;
        }
    }
}