using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FLFlight {
    public class GameGUI : MonoBehaviour {

        public Slider j1_slider;
        public Slider j2_slider;
        public List<KeyCode> keys = new List<KeyCode> ();
        public GameObject pauseMenu;
        public GameObject pauseMenu2;
        public GameObject panelEst;
        public GameObject panelEst2;
        public bool pauseGame = false;
        public Text lbl_gameover;
        public Text lbl_gameover2;
        public Ship player1;
        public Ship player2;

        private bool isGameOver = false;

        void Start () {
            // ShipInput.Start may change life based on the selected ship, so wait a frame
            // before reading MaxLife.
            StartCoroutine (InitSliders ());
            pause ();
        }

        IEnumerator InitSliders () {
            yield return null;
            j1_slider.maxValue = player1.Input.MaxLife;
            j2_slider.maxValue = player2.Input.MaxLife;
            setSilderValue ();
        }

        void Update () {
            setSilderValue ();

            if (!isGameOver && (Input.GetKeyDown (keys[0]) || Input.GetKeyDown (keys[1]) || Input.GetKeyDown (keys[2]))) //Pausa
            {
                if (pauseGame) {
                    resume ();
                } else {
                    pause ();
                }
            }

            if (isGameOver)
                return;

            if (player1.Input.IsDead) {
                lbl_gameover.text = "¡JUGADOR 2 GANA!";
                lbl_gameover2.text = "¡JUGADOR 2 GANA!";
                gameOver ();

            } else if (player2.Input.IsDead) {
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
            if (isGameOver)
                return;

            pauseGame = false;
            pauseMenu.SetActive (pauseGame);
            pauseMenu2.SetActive (pauseGame);
            panelEst.SetActive(!pauseGame);
            panelEst2.SetActive(!pauseGame);
            Time.timeScale = 1f; //se reanuda el juego
        }

        public void loadMenu () {
            Time.timeScale = 1f;
            SceneManager.LoadScene ("Interface");
        }

        public void loadGame () {
            Time.timeScale = 1f;
            SceneManager.LoadScene ("Game");
        }

        public void gameOver () {
            isGameOver = true;
            StartCoroutine ("endGame");
        }

        IEnumerator endGame () {
            yield return new WaitForSeconds (0.5f);
            pause ();
        }
    }
}
