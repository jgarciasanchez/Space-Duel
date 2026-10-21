using System.Collections.Generic;
using UnityEngine;

namespace FLFlight {
    public enum ControlScheme {
        Gamepad, // sticks + gatillos, dispara con "A"
        MouseKeyboard, // apunta con el raton, acelera con teclas; usa un segundo mando si hay uno
    }

    /// <summary>
    /// Input, health and weapon state for one player ship. Both players use this
    /// class; playerNumber and controlScheme decide how it is driven.
    /// </summary>
    public class ShipInput : MonoBehaviour {
        [Header ("Player")]
        [Range (1, 2)]
        [SerializeField] private int playerNumber = 1;
        [SerializeField] private ControlScheme controlScheme = ControlScheme.Gamepad;

        [Header ("Handling")]
        [SerializeField] private float bankLimit = 35f;
        [SerializeField] private float pitchSensitivity = 2.5f;
        [SerializeField] private float yawSensitivity = 2.5f;
        [SerializeField] private float rollSensitivity = 1f;
        [SerializeField] private float deadzone = 0.2f;

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

        [Header ("Axes (Input Manager)")]
        public string xAxis;
        public string yAxis;
        public string tRAxis;
        public string tLAxis;
        public string LAxis;
        public string fireButton = "A";
        [Tooltip ("Increase / decrease throttle keys (MouseKeyboard scheme).")]
        public List<KeyCode> keys = new List<KeyCode> ();
        [Tooltip ("Camera used to aim with the mouse. Falls back to the second camera in the scene.")]
        [SerializeField] private Camera aimCamera;

        [Header ("Combat")]
        public Rigidbody bulletPrefab;
        public float bulletSpeed = 300f;
        public float fireCooldown = 0.15f;
        public int damage = 25;
        public int life = 100;
        [Tooltip ("Seconds each Miner pickup adds to the damage buff.")]
        public float buffDuration = 10f;

        public List<AudioClip> audios = new List<AudioClip> ();

        // How quickly the throttle reacts to input.
        private const float THROTTLE_SPEED = 0.5f;

        private AudioSource audioSource;
        private int baseDamage;
        private float buffTimeRemaining;
        private float nextFireTime;

        public int PlayerNumber { get { return playerNumber; } }
        public int MaxLife { get; private set; }
        public bool IsDead { get { return life <= 0; } }
        public float BuffTimeRemaining { get { return buffTimeRemaining; } }

        public float Pitch { get { return pitch; } }
        public float Yaw { get { return yaw; } }
        public float Roll { get { return roll; } }
        public float Strafe { get { return strafe; } }
        public float Throttle { get { return throttle; } }

        void Start () {
            audioSource = GetComponent<AudioSource> ();

            ShipStats stats;
            if (MatchSettings.TryGetShip (playerNumber, out stats)) {
                damage = stats.damage;
                life = stats.life;
                GetComponent<ShipPhysics> ().forceMultiplier *= stats.speedMultiplier;
            }

            baseDamage = damage;
            MaxLife = life;

            if (aimCamera == null && controlScheme == ControlScheme.MouseKeyboard)
                aimCamera = Camera.allCameras.Length > 1 ? Camera.allCameras[1] : Camera.main;
        }

        private void Update () {
            if (IsDead) {
                pitch = yaw = roll = strafe = throttle = 0f;
                return;
            }

            UpdateBuff ();

            int gamepads = ConnectedGamepads ();
            if (controlScheme == ControlScheme.Gamepad) {
                if (gamepads >= 1) {
                    SetStickCommands ();
                    UpdateThrottle (false);
                    if (Input.GetButtonDown (fireButton))
                        Fire ();
                }
            } else {
                SetMouseCommands (gamepads >= 2);
                UpdateThrottle (true);
                if (Input.GetMouseButtonDown (0))
                    Fire ();
            }

            if (Mathf.Abs (yaw) <= deadzone)
                yaw = 0;
            if (Mathf.Abs (pitch) <= deadzone)
                pitch = 0;

            if (audioSource != null)
                audioSource.volume = throttle;
        }

        public void TakeDamage (int amount) {
            life = Mathf.Max (0, life - amount);
        }

        /// <summary>Reward for destroying a Miner.</summary>
        public void ApplyMinerBuff () {
            damage += 5;
            life = Mathf.Min (MaxLife, life + 5);
            buffTimeRemaining += buffDuration;
        }

        private void UpdateBuff () {
            if (buffTimeRemaining > 0f) {
                buffTimeRemaining -= Time.deltaTime;
            } else {
                buffTimeRemaining = 0f;
                damage = baseDamage;
            }
        }

        private void Fire () {
            if (Time.time < nextFireTime || bulletPrefab == null)
                return;
            nextFireTime = Time.time + fireCooldown;

            Rigidbody body = Instantiate (bulletPrefab, transform.position + transform.forward * 10, bulletPrefab.transform.rotation);
            body.velocity = transform.forward * bulletSpeed;
            Bullet bullet = body.GetComponent<Bullet> ();
            if (bullet != null)
                bullet.Shooter = this;

            if (audioSource != null && audios.Count > 0)
                audioSource.PlayOneShot (audios[0]);
        }

        private static int ConnectedGamepads () {
            int count = 0;
            foreach (string name in Input.GetJoystickNames ()) {
                if (!string.IsNullOrEmpty (name))
                    count++;
            }
            return count;
        }

        private void SetStickCommands () {
            pitch = Mathf.Clamp (Input.GetAxis (yAxis) * pitchSensitivity, -1f, 1f);
            yaw = Mathf.Clamp (Input.GetAxis (xAxis) * yawSensitivity, -1f, 1f);
            roll = -Input.GetAxis (LAxis);
        }

        private void SetMouseCommands (bool useGamepad) {
            if (aimCamera == null)
                return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1000f;
            Vector3 gotoPos = aimCamera.ScreenToWorldPoint (mousePos);

            TurnTowardsPoint (gotoPos);
            BankShipRelativeToUpVector (mousePos, aimCamera.transform.up);

            // A second gamepad overrides the mouse for this player.
            if (useGamepad)
                SetStickCommands ();
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
        }

        private void TurnTowardsPoint (Vector3 gotoPos) {
            Vector3 localGotoPos = transform.InverseTransformVector (gotoPos - transform.position).normalized;

            pitch = Mathf.Clamp (-localGotoPos.y * pitchSensitivity, -1f, 1f);
            yaw = Mathf.Clamp (localGotoPos.x * yawSensitivity, -1f, 1f);
        }

        private void UpdateThrottle (bool useKeys) {
            float target = throttle;
            bool increase = Input.GetAxis (tLAxis) == 1 || (useKeys && keys.Count > 0 && Input.GetKey (keys[0]));
            bool decrease = Input.GetAxis (tRAxis) == 1 || (useKeys && keys.Count > 1 && Input.GetKey (keys[1]));

            if (increase) {
                target = 1.0f;
                if (audioSource != null && !audioSource.isPlaying)
                    audioSource.Play ();
            } else if (decrease) {
                target = 0.0f;
            }

            throttle = Mathf.MoveTowards (throttle, target, Time.deltaTime * THROTTLE_SPEED);
        }
    }
}
