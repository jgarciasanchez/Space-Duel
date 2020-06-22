using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLFlight {
    /// <summary>
    /// Class specifically to deal with input.
    /// </summary>
    public class ShipInput2 : MonoBehaviour {
        [SerializeField] private float bankLimit = 35f;
        [SerializeField] private float pitchSensitivity = 2.5f;
        [SerializeField] private float yawSensitivity = 2.5f;
        [SerializeField] private float rollSensitivity = 1f;
        public int damage = 25;
        public int life = 80;
        public int time = 0;

        [Range (-1, 1)]
        [SerializeField] private float pitch;
        [Range (-1, 1)]
        [SerializeField] private float yaw;
        [Range (-1, 1)]
        [SerializeField] private float roll;
        [Range (-1, 1)]
        [SerializeField] private float strafe;
        [Range (0, 1)]
        [SerializeField] private float throttle;
        [Range (-1, 1)]

        float rotationSpeed = 55;
        Vector3 currentEulerAngles;
        float z;

        private Ship ship;

        public Rigidbody bulletPrefab;
        static AudioSource sound;
        static AudioSource soundEngine;
        public string xAxis;
        public string yAxis;
        public string tRAxis;
        public string tLAxis;
        public string LAxis;
        public List<KeyCode> keys = new List<KeyCode> ();

        public List<AudioClip> audios = new List<AudioClip> ();

        public bool randomRotation = true;

        // How quickly the throttle reacts to input.
        private const float THROTTLE_SPEED = 0.5f;

        private bool Controller = false;

        public float Pitch { get { return pitch; } }
        public float Yaw { get { return yaw; } }
        public float Roll { get { return roll; } }
        public float Strafe { get { return strafe; } }
        public float Throttle { get { return throttle; } }

        void Start () {
            sound = GetComponent<AudioSource> ();
            soundEngine = GetComponent<AudioSource> ();
        }

        private void Update () {

            if (life <= 0) {

            } else {
                string[] names = Input.GetJoystickNames ();
                soundEngine.volume = Throttle;
                SetStickCommandsUsingAutopilot ();
                UpdateMouseWheelThrottle ();
                UpdateKeyboardThrottle (keys[0], keys[1]);

                if (time > 0) {
                    time = time - 1;
                } else {
                    damage = 25;
                }

                if (names.Length > 1) {
                    Controller = true;
                } else {
                    Controller = false;
                }

                if (yaw > 0.15 || yaw < -0.15) {

                } else {
                    yaw = 0;
                }

                if (pitch > 0.15 || pitch < -0.15) {

                } else {
                    pitch = 0;
                }

                if (Input.GetMouseButtonDown (0)) {
                    Rigidbody t = Instantiate (bulletPrefab);
                    t.position = transform.position + transform.forward * 10;
                    t.velocity = transform.forward * 300;
                    sound.PlayOneShot (audios[0]);
                }
            }

        }

        private void SetStickCommandsUsingAutopilot () {
            // Project the position of the mouse on screen out to some distance.
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1000f;
            Vector3 gotoPos = Camera.allCameras[1].ScreenToWorldPoint (mousePos);

            // Use that world position under the mouse as a target point.
            TurnTowardsPoint (gotoPos);
            if (Controller == true) {
                controll (gotoPos);
            }

            // Use the mouse to bank the ship some degrees based on the mouse position.
            BankShipRelativeToUpVector (mousePos, Camera.allCameras[1].transform.up);
        }

        /// <param name="mousePos"></param>
        /// <param name="upVector"></param>
        private void BankShipRelativeToUpVector (Vector3 mousePos, Vector3 upVector) {
            float bankInfluence = (mousePos.x - (Screen.width * 0.5f)) / (Screen.width * 0.5f);
            bankInfluence = Mathf.Clamp (bankInfluence, -1f, 1f);

            // Throttle modifies the bank angle so that when at idle, the ship just flatly yaws.
            bankInfluence *= throttle;
            float bankTarget = bankInfluence * bankLimit;

            // Here's the special sauce. Roll so that the bank target is reached relative to the
            // up of the camera.
            float bankError = Vector3.SignedAngle (transform.up, upVector, transform.forward);
            bankError = bankError - bankTarget;

            // Clamp this to prevent wild inputs.
            bankError = Mathf.Clamp (bankError * 0.1f, -1f, 1f);

            // Roll to minimze error.
            roll = bankError * rollSensitivity;
            if (Controller == true) {
                roll = (Input.GetAxis (LAxis) * -1);
            }

        }

        /// <summary>
        /// Pitches and yaws the ship to look at the passed in world position.
        /// </summary>
        /// <param name="gotoPos">World position to turn the ship towards.</param>
        private void TurnTowardsPoint (Vector3 gotoPos) {
            Vector3 localGotoPos = transform.InverseTransformVector (gotoPos - transform.position).normalized;

            // Note that you would want to use a PID controller for this to make it more responsive.
            pitch = Mathf.Clamp (-localGotoPos.y * pitchSensitivity, -1f, 1f);
            yaw = Mathf.Clamp (localGotoPos.x * yawSensitivity, -1f, 1f);
        }

        private void controll (Vector3 gotoPos) {
            Vector3 localGotoPos = transform.InverseTransformVector (gotoPos - transform.position).normalized;
            // Note that you would want to use a PID controller for this to make it more responsive.
            pitch = Mathf.Clamp (Input.GetAxis (yAxis) * pitchSensitivity, -1f, 1f);

            yaw = Mathf.Clamp (Input.GetAxis (xAxis) * yawSensitivity, -1f, 1f);
        }

        /// <summary>
        /// Uses R and F to raise and lower the throttle.
        /// </summary>
        private void UpdateKeyboardThrottle (KeyCode increaseKey, KeyCode decreaseKey) {
            float target = throttle;

            if (Input.GetKey (increaseKey) || Input.GetAxis (tLAxis) == 1) {
                target = 1.0f;
                soundEngine.Play ();
            } else if (Input.GetKey (decreaseKey) || Input.GetAxis (tRAxis) == 1) {
                target = 0.0f;
            }

            throttle = Mathf.MoveTowards (throttle, target, Time.deltaTime * THROTTLE_SPEED);
        }

        /// <summary>
        /// Uses the mouse wheel to control the throttle.
        /// </summary>
        private void UpdateMouseWheelThrottle () {
            throttle += Input.GetAxis ("Mouse ScrollWheel");
            throttle = Mathf.Clamp (throttle, 0.0f, 1.0f);
        }
    }
}