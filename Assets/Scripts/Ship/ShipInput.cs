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
        [Range (-1, 1)]
        [SerializeField] private float throttle;
        [Tooltip ("Throttle change per second. Higher = snappier.")]
        [SerializeField] private float throttleResponse = 1.2f;

        [Header ("Keyboard")]
        [Tooltip ("Increase / decrease throttle keys (MouseKeyboard scheme).")]
        public List<KeyCode> keys = new List<KeyCode> ();
        public KeyCode strafeLeftKey = KeyCode.A;
        public KeyCode strafeRightKey = KeyCode.D;
        public KeyCode boostKey = KeyCode.LeftShift;

        [Header ("Boost")]
        [Tooltip ("Boost energy drained per second while boosting (energy goes from 0 to 1).")]
        public float boostDrain = 0.35f;
        [Tooltip ("Boost energy recovered per second while not boosting.")]
        public float boostRecharge = 0.15f;
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

        private AudioSource audioSource;
        private int baseDamage;
        private float buffTimeRemaining;
        private float nextFireTime;
        private float boostEnergy = 1f;
        private bool boosting;

        public int PlayerNumber { get { return playerNumber; } }
        public int MaxLife { get; private set; }
        public bool IsDead { get { return life <= 0; } }
        public float BuffTimeRemaining { get { return buffTimeRemaining; } }
        public float BoostEnergy { get { return boostEnergy; } }
        public bool Boosting { get { return boosting; } }

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
                boosting = false;
                return;
            }

            UpdateBuff ();

            LogGamepadsIfChanged ();
            bool wantBoost = false;
            if (controlScheme == ControlScheme.Gamepad) {
                // Player 1 takes the first connected gamepad, whatever slot Windows gave it.
                int slot = FindGamepadSlot (1);
                if (slot > 0) {
                    SetStickCommands (slot);
                    UpdateThrottle (slot, false);
                    strafe = GamepadStrafe (slot);
                    wantBoost = Input.GetKey (JoystickButton (slot, 1));
                    if (Input.GetKeyDown (JoystickButton (slot, 0)))
                        Fire ();
                }
            } else {
                // A second gamepad, if any, also drives the mouse/keyboard player.
                int slot = FindGamepadSlot (2);
                bool useGamepad = slot > 0;
                SetMouseCommands (slot);
                UpdateThrottle (slot, true);

                strafe = 0f;
                if (Input.GetKey (strafeLeftKey))
                    strafe -= 1f;
                if (Input.GetKey (strafeRightKey))
                    strafe += 1f;
                wantBoost = Input.GetKey (boostKey);
                if (useGamepad) {
                    strafe = Mathf.Clamp (strafe + GamepadStrafe (slot), -1f, 1f);
                    wantBoost |= Input.GetKey (JoystickButton (slot, 1));
                }

                if (Input.GetMouseButtonDown (0))
                    Fire ();
            }
            UpdateBoost (wantBoost);

            if (Mathf.Abs (yaw) <= deadzone)
                yaw = 0;
            if (Mathf.Abs (pitch) <= deadzone)
                pitch = 0;

            if (audioSource != null)
                audioSource.volume = Mathf.Abs (throttle);
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

        private void UpdateBoost (bool wantBoost) {
            // Boost only helps going forward; it needs some energy left to start.
            boosting = wantBoost && throttle > 0f && boostEnergy > (boosting ? 0f : 0.15f);
            if (boosting)
                boostEnergy = Mathf.Max (0f, boostEnergy - boostDrain * Time.deltaTime);
            else
                boostEnergy = Mathf.Min (1f, boostEnergy + boostRecharge * Time.deltaTime);
        }

        private float GamepadStrafe (int slot) {
            float value = 0f;
            if (Input.GetKey (JoystickButton (slot, 4))) // LB
                value -= 1f;
            if (Input.GetKey (JoystickButton (slot, 5))) // RB
                value += 1f;
            return value;
        }

        private static string JoystickButton (int slot, int button) {
            return "joystick " + slot + " button " + button;
        }

        // Per-slot axes defined in the Input Manager as J1X..J4RT.
        private static string JoystickAxis (int slot, string axis) {
            return "J" + slot + axis;
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

        private const int MaxGamepadSlots = 4;
        private static string lastGamepadList;

        /// <summary>
        /// Returns the 1-based Input Manager slot of the nth connected gamepad, or 0 if
        /// there aren't that many. Windows often leaves empty entries in the list after a
        /// reconnect, so the first gamepad isn't always in slot 1.
        /// </summary>
        private static int FindGamepadSlot (int nth) {
            string[] names = Input.GetJoystickNames ();
            int found = 0;
            for (int i = 0; i < names.Length && i < MaxGamepadSlots; i++) {
                if (!string.IsNullOrEmpty (names[i]) && ++found == nth)
                    return i + 1;
            }
            return 0;
        }

        private static void LogGamepadsIfChanged () {
            string list = string.Join (" | ", Input.GetJoystickNames ());
            if (list == lastGamepadList)
                return;
            lastGamepadList = list;
            Debug.Log ("Mandos detectados (por ranura): [" + list + "] -> J1 usa ranura " + FindGamepadSlot (1) + ", J2 usa ranura " + FindGamepadSlot (2));
        }

        private void SetStickCommands (int slot) {
            pitch = Mathf.Clamp (Input.GetAxis (JoystickAxis (slot, "Y")) * pitchSensitivity, -1f, 1f);
            yaw = Mathf.Clamp (Input.GetAxis (JoystickAxis (slot, "X")) * yawSensitivity, -1f, 1f);
            roll = -Input.GetAxis (JoystickAxis (slot, "RX"));
        }

        private void SetMouseCommands (int gamepadSlot) {
            if (aimCamera == null)
                return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1000f;
            Vector3 gotoPos = aimCamera.ScreenToWorldPoint (mousePos);

            TurnTowardsPoint (gotoPos);
            BankShipRelativeToUpVector (mousePos, aimCamera.transform.up);

            // A second gamepad overrides the mouse for this player.
            if (gamepadSlot > 0)
                SetStickCommands (gamepadSlot);
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

        private void UpdateThrottle (int gamepadSlot, bool useKeys) {
            float target = throttle;
            bool increase = useKeys && keys.Count > 0 && Input.GetKey (keys[0]);
            bool decrease = useKeys && keys.Count > 1 && Input.GetKey (keys[1]);
            if (gamepadSlot > 0) {
                increase |= Input.GetAxis (JoystickAxis (gamepadSlot, "RT")) == 1;
                decrease |= Input.GetAxis (JoystickAxis (gamepadSlot, "LT")) == 1;
            }

            if (increase) {
                target = 1.0f;
                if (audioSource != null && !audioSource.isPlaying)
                    audioSource.Play ();
            } else if (decrease) {
                // Brakes first, then reverses (reverse thrust is scaled by ShipPhysics.reverseMultiplier).
                target = -1.0f;
            }

            throttle = Mathf.MoveTowards (throttle, target, Time.deltaTime * throttleResponse);
        }
    }
}
