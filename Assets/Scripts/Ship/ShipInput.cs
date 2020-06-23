using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLFlight {
    public class ShipInput : MonoBehaviour {
        [SerializeField] private float bankLimit = 35f;
        [SerializeField] private float pitchSensitivity = 2.5f;
        [SerializeField] private float yawSensitivity = 2.5f;
        [SerializeField] private float rollSensitivity = 1f;

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
        public float rotate;

        float rotationSpeed = 55;
        Vector3 currentEulerAngles;
        float z;

        private Ship ship;

        public Rigidbody bulletPrefab;
        private bool Controller = false;
        static AudioSource sound;
        static AudioSource soundEngine;
        public int damage = 25;
        public int life = 100;
        public int time = 0;
        public string xAxis;
        public string yAxis;
        public string tRAxis;
        public string tLAxis;
        public string LAxis;
        private bool control = false;

        public List<AudioClip> audios = new List<AudioClip> ();

        public bool randomRotation = true;

        private const float THROTTLE_SPEED = 0.5f;

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
            string[] names = Input.GetJoystickNames ();
            Debug.Log(damage + "nave1");

            if (life <= 0) {

            } else {
                soundEngine.volume = Throttle;
                if (Controller) {
                    SetStickCommandsUsingAutopilot ();
                    UpdateThrottle ();
                    if (Input.GetButtonDown ("A")) {
                        Rigidbody t = Instantiate (bulletPrefab);
                        t.position = transform.position + transform.forward * 10;
                        t.velocity = transform.forward * 300;
                        sound.PlayOneShot (audios[0]);
                    }
                }

                if (names[0].Length > 1) {
                    Controller = true;
                } else if (names[0].Length < 1) {
                    Controller = false;
                }

                if (time > 0) {
                    time = time - 1;
                    Debug.Log (time);
                } else {
                    damage = 25;
                }

                if (yaw > 0.2 || yaw < -0.2) {

                } else {
                    yaw = 0;
                }

                if (pitch > 0.2 || pitch < -0.2) {

                } else {
                    pitch = 0;
                }

            }
        }

        private void SetStickCommandsUsingAutopilot () {
            pitch = Mathf.Clamp (Input.GetAxis (yAxis) * pitchSensitivity, -1f, 1f);

            yaw = Mathf.Clamp (Input.GetAxis (xAxis) * yawSensitivity, -1f, 1f);
            roll = (Input.GetAxis (LAxis) * -1);
        }

        private void UpdateThrottle () {
            float target = throttle;

            if (Input.GetAxis (tLAxis) == 1) {
                target = 1.0f;
                soundEngine.Play ();
            } else if (Input.GetAxis (tRAxis) == 1) {
                target = 0.0f;
            }

            throttle = Mathf.MoveTowards (throttle, target, Time.deltaTime * THROTTLE_SPEED);
        }
    }
}