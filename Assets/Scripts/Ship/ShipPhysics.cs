using UnityEngine;

namespace FLFlight {

    public class ShipPhysics : MonoBehaviour {
        public Vector3 linearForce = new Vector3 (100.0f, 100.0f, 100.0f);

        public Vector3 angularForce = new Vector3 (100.0f, 100.0f, 100.0f);

        [Range (0.0f, 1.0f)]
        public float reverseMultiplier = 1.0f;

        public float forceMultiplier = 100.0f;

        [Tooltip ("Extra forward force while boosting.")]
        public float boostMultiplier = 1.8f;

        [Tooltip ("How strongly sideways/vertical drift is cancelled so the ship goes where it points. 0 = pure Newtonian.")]
        public float flightAssist = 1.5f;

        public Rigidbody Rigidbody { get; private set; }

        private Vector3 appliedLinearForce = Vector3.zero;
        private Vector3 appliedAngularForce = Vector3.zero;
        private bool boosting;

        void Awake () {
            Rigidbody = GetComponent<Rigidbody> ();
            if (Rigidbody == null) {
                Debug.LogWarning (name + ": ShipPhysics has no rigidbody.");
            }
        }

        void FixedUpdate () {
            if (Rigidbody == null)
                return;

            Vector3 linear = appliedLinearForce;
            if (boosting && linear.z > 0f)
                linear.z *= boostMultiplier;

            Rigidbody.AddRelativeForce (linear * forceMultiplier, ForceMode.Force);
            Rigidbody.AddRelativeTorque (appliedAngularForce * forceMultiplier, ForceMode.Force);

            ApplyFlightAssist ();
        }

        private void ApplyFlightAssist () {
            if (flightAssist <= 0f)
                return;

            // Bleed off the part of the velocity that isn't along the nose, so turns bite
            // instead of sliding. Strafing still works because it keeps pushing sideways.
            Vector3 localVelocity = transform.InverseTransformDirection (Rigidbody.velocity);
            float keep = Mathf.Exp (-flightAssist * Time.fixedDeltaTime);
            localVelocity.x *= keep;
            localVelocity.y *= keep;
            Rigidbody.velocity = transform.TransformDirection (localVelocity);
        }

        public void SetPhysicsInput (Vector3 linearInput, Vector3 angularInput, bool boost = false) {
            if (linearInput.z < 0f)
                linearInput.z *= reverseMultiplier;

            appliedLinearForce = Vector3.Scale (linearForce, linearInput);
            appliedAngularForce = Vector3.Scale (angularForce, angularInput);
            boosting = boost;
        }

    }
}
