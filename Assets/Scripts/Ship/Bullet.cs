using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLFlight {
    public class Bullet : MonoBehaviour {

        void OnTriggerEnter (Collider collider) {
            if (collider.tag != "player") {
                if (collider.tag == "Miner") {
                    if (gameObject.name == "Bullet 1(Clone)") {
                        GameObject.Find ("Ship1").GetComponent<ShipInput> ().damage = GameObject.Find ("Ship1").GetComponent<ShipInput> ().damage + 5;
                        GameObject.Find ("Ship1").GetComponent<ShipInput> ().life = GameObject.Find ("Ship1").GetComponent<ShipInput> ().life + 5;
                        GameObject.Find ("Ship1").GetComponent<ShipInput> ().time = GameObject.Find ("Ship1").GetComponent<ShipInput> ().time + 2500;
                        Destroy (gameObject);
                        Destroy (collider.gameObject);
                    } else if (gameObject.name == "Bullet 2(Clone)") {
                        GameObject.Find ("Ship2").GetComponent<ShipInput2> ().damage = GameObject.Find ("Ship2").GetComponent<ShipInput2> ().damage + 5;
                        GameObject.Find ("Ship2").GetComponent<ShipInput2> ().life = GameObject.Find ("Ship2").GetComponent<ShipInput2> ().life + 5;
                        GameObject.Find ("Ship2").GetComponent<ShipInput2> ().time = GameObject.Find ("Ship2").GetComponent<ShipInput2> ().time + 2500;
                        Destroy (gameObject);
                        Destroy (collider.gameObject);
                    }
                } else if (collider.tag == "wall" || collider.tag == "Asteroid") {
                    Destroy (gameObject);
                }
            }

            if (collider.tag == "Player" && gameObject.name == "Bullet 1(Clone)" && collider.gameObject.name != "Ship1") {
                GameObject.Find ("Ship2").GetComponent<ShipInput2> ().life -= GameObject.Find ("Ship1").GetComponent<ShipInput> ().damage;
            } else if (collider.tag == "Player" && gameObject.name == "Bullet 2(Clone)" && collider.gameObject.name != "Ship2") {
                GameObject.Find ("Ship1").GetComponent<ShipInput> ().life -= GameObject.Find ("Ship2").GetComponent<ShipInput2> ().damage;
            }
        }
    }
}