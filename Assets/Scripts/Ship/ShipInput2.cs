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
        public int damage = 10;
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
        private bool Controller2 = false;

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
                if (Controller) {
                    Debug.Log (Controller);
                    SetStickCommandsUsingAutopilot ();
                    UpdateKeyboardThrottle (keys[0], keys[1]);
                    if (Input.GetMouseButtonDown (0)) {
                        Rigidbody t = Instantiate (bulletPrefab);
                        t.position = transform.position + transform.forward * 10;
                        t.velocity = transform.forward * 300;
                        sound.PlayOneShot (audios[0]);
                    }
                }

                if (time > 0) {
                    Debug.Log(time);
                    time = time - 1;
                } else {
                    damage = 10;
                }

                if (names[0].Length > 1) {
                    Controller = true;
                } else if (names[0].Length < 1) {
                    Controller = false;
                }
                if (names.Length > 1) {
                    Debug.Log("aaaa");
                    Controller2 = true;
                }

                if (yaw > 0.15 || yaw < -0.15) {

                } else {
                    yaw = 0;
                }

                if (pitch > 0.15 || pitch < -0.15) {

                } else {
                    pitch = 0;
                }

            }

        }

        private void SetStickCommandsUsingAutopilot () {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1000f;
            Vector3 gotoPos = Camera.allCameras[1].ScreenToWorldPoint (mousePos);

            TurnTowardsPoint (gotoPos);
            BankShipRelativeToUpVector (mousePos, Camera.allCameras[1].transform.up);
            if (Controller2 == true) {
                controll (gotoPos);
            }
        }

        private void BankShipRelativeToUpVector (Vector3 mousePos, Vector3 upVector) {
            float bankInfluence = (mousePos.x - (Screen.width * 0.5f)) / (Screen.width * 0.5f);
            bankInfluence = Mathf.Clamp (bankInfluence, -1f, 1f);
            bankInfluence *= throttle;
            float bankTarget = bankInfluence * bankLimit;

            float bankError = Vector3.SignedAngle (transform.up, upVector, transform.forward);
            bankError = bankError - bankTarget;

            bankError = Mathf.Clamp (bankError * 0.1f, -1f, 1f);

            roll = bankError * rollSensitivity;
            if (Controller2 == true) {
                roll = (Input.GetAxis (LAxis) * -1);
            }

        }

        private void TurnTowardsPoint (Vector3 gotoPos) {
            Vector3 localGotoPos = transform.InverseTransformVector (gotoPos - transform.position).normalized;
            Debug.Log(localGotoPos);

            pitch = Mathf.Clamp (-localGotoPos.y * pitchSensitivity, -1f, 1f);
            yaw = Mathf.Clamp (localGotoPos.x * yawSensitivity, -1f, 1f);
        }

        private void controll (Vector3 gotoPos) {
            Vector3 localGotoPos = transform.InverseTransformVector (gotoPos - transform.position).normalized;
            pitch = Mathf.Clamp (Input.GetAxis (yAxis) * pitchSensitivity, -1f, 1f);

            yaw = Mathf.Clamp (Input.GetAxis (xAxis) * yawSensitivity, -1f, 1f);
        }

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
    }
}