using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLFlight {
    public class Bullet : MonoBehaviour {

        void OnTriggerEnter (Collider collider) {
            if (collider.tag != "player") {
                if (collider.tag == "Miner") {
                    if (gameObject.name == "Bullet 1(Clone)") { 
                        GameObject.Find ("Ship1").GetComponent<ShipInput> ().Damage = 5;
                        GameObject.Find ("Ship1").GetComponent<ShipInput> ().Life = 5;
                        GameObject.Find ("Ship1").GetComponent<ShipInput> ().TimeBuff = 5000;
                        Destroy (gameObject);
                        Destroy (collider.gameObject);
                    } else if (gameObject.name == "Bullet 2(Clone)") {
                        GameObject.Find ("Ship2").GetComponent<ShipInput2> ().Damage = 5;
                        GameObject.Find ("Ship2").GetComponent<ShipInput2> ().Life = 5;
                        GameObject.Find ("Ship2").GetComponent<ShipInput2> ().TimeBuff = 5000;
                        Destroy (gameObject);
                        Destroy (collider.gameObject);
                    }
                } else if (collider.tag == "Wall") {
                    Destroy (gameObject);
                }
            }

            if (collider.tag == "player" && gameObject.name == "Bullet 1(Clone)") {
                GameObject.Find ("Ship2").GetComponent<ShipInput2> ().Life = GameObject.Find ("Ship2").GetComponent<ShipInput2> ().Life - GameObject.Find ("Ship1").GetComponent<ShipInput> ().Damage;
            } else if (collider.tag == "player" && gameObject.name == "Bullet 2(Clone)") {
                GameObject.Find ("Ship1").GetComponent<ShipInput> ().Life = GameObject.Find ("Ship1").GetComponent<ShipInput> ().Life - GameObject.Find ("Ship2").GetComponent<ShipInput2> ().Damage;
            }
        }
    }
}