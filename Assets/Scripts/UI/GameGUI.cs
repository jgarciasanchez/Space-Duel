using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FLFlight {
    public class GameGUI : MonoBehaviour {

        public Slider j1_slider;
        public Slider j2_slider;
        // Start is called before the first frame update
        public List<KeyCode> keys = new List<KeyCode> ();
        public GameObject pauseMenu;
        public GameObject pauseMenu2;
        public GameObject panelEst;
        public GameObject panelEst2;
        public bool pauseGame = false;
        public Text lbl_gameover;
        public Text lbl_gameover2;
        public Ship player1;
        public Ship2 player2;

        void Start () {
            setSilderValue ();
            pause ();

        }

        // Update is called once per frame
        void Update () {
            setSilderValue ();

            if (Input.GetKeyDown (keys[0]) || Input.GetKeyDown (keys[1]) || Input.GetKeyDown (keys[2])) //Pausa
            {
                if (pauseGame) {
                    resume ();
                } else {
                    pause ();
                }
            }

            if (player1.Input.life <= 0) {
                lbl_gameover.text = "¡JUGADOR 2 GANA!";
                lbl_gameover2.text = "¡JUGADOR 2 GANA!";
                gameOver ();

            } else if (player2.Input.life <= 0) {
                lbl_gameover.text = "¡JUGADOR 1 GANA!";
                lbl_gameover2.text = "¡JUGADOR 1 GANA!";
                gameOver ();
            }
        }

        void setSilderValue () {
            j1_slider.value = player1.Input.life;
            j2_slider.value = player2.Input.life;
        }

        public void pause () {
            pauseGame = true;
            pauseMenu.SetActive (pauseGame);
            pauseMenu2.SetActive (pauseGame);
            panelEst.SetActive(!pauseGame);
            panelEst2.SetActive(!pauseGame);
            Time.timeScale = 0f; //se detiene el juego
        }

        public void resume () {
            pauseGame = false;
            pauseMenu.SetActive (pauseGame);
            pauseMenu2.SetActive (pauseGame);
            panelEst.SetActive(!pauseGame);
            panelEst2.SetActive(!pauseGame);
             Time.timeScale = 1f; //se reanuda el juego
        }

        public void loadMenu () {
            SceneManager.LoadScene ("Interface");
        }

        public void loadGame () {
            SceneManager.LoadScene ("Game");
        }

        public void gameOver () {
            StartCoroutine("endGame");
        }

        IEnumerator endGame () {
            yield return new WaitForSeconds (0.5f);
            pause ();
        }
    }
}