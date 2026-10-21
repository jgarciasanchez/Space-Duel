using UnityEngine;
using UnityEngine.UI;

namespace FLFlight.UI {
    /// <summary>
    /// Shows throttle and speed of the player ship.
    /// </summary>
    public class SpeedUI : MonoBehaviour {
        private Text text;
        public Ship player;

        private void Awake () {
            text = GetComponent<Text> ();
        }

        // Update is called once per frame
        void Update () {
            if (text != null && player != null) {
                text.text = string.Format ("THR: {0}\nSPD: {1}\nPWR: {2}\nBST: {3}",
                    (player.Input.Throttle * 100.0f).ToString ("000"),
                    player.Velocity.magnitude.ToString ("000"),
                    player.Input.damage,
                    (player.Input.BoostEnergy * 100.0f).ToString ("000"));
            }
        }
    }
}
