using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FLFlight;

public class Seleccion : MonoBehaviour {
    //barras de estadistica
    public Slider Damage_slider;
    public Slider Vida_slider;
    public Slider Velocidad_slider;
    public Text txvJugadorA; // jugador actual
    public Text alerta; // mensaje
    public Button btnSeleccionar;
    public Toggle tj1;
    public Toggle tj2;
    Vector3 targetRot;
    Vector3 currentAngle;
    static AudioSource soundEngine;

    int currentSelection;
    int totalCharacters;

    string nave;
    string jugador;

    // Start is called before the first frame update
    void Start () {
        Damage_slider.maxValue = ShipCatalog.MaxDamage;
        Vida_slider.maxValue = ShipCatalog.MaxLife;
        Velocidad_slider.maxValue = ShipCatalog.MaxSpeedMultiplier;
        nave = "nave1";
        jugador = "j1";
        setSilderValue ();
        totalCharacters = 2;
        currentSelection = 2;
        soundEngine = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update () {
        setSilderValue ();
        if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetAxis ("6TH") == 1) {
            currentAngle = transform.eulerAngles;
            targetRot = targetRot + new Vector3 (0, 180, 0);
            currentSelection++;
            soundEngine.Play();
            naveCheck ();
        }
        if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetAxis ("6TH") == -1) {
            currentAngle = transform.eulerAngles;
            targetRot = targetRot + new Vector3 (0, 180, 0);
            currentSelection--;
            soundEngine.Play();
            naveCheck ();
        }

        currentAngle = new Vector3 (0, Mathf.LerpAngle (currentAngle.y, targetRot.y, 0.05f));
        transform.eulerAngles = currentAngle;
    }

    public void naveCheck () {
        if (nave == "nave1") {
            nave = "nave2";
        } else {
            nave = "nave1";
        }
    }

    int naveIndex () {
        return nave == "nave1" ? 0 : 1;
    }

    void setSilderValue () {
        ShipStats stats = ShipCatalog.Get (naveIndex ());
        Damage_slider.value = stats.damage;
        Vida_slider.value = stats.life;
        Velocidad_slider.value = stats.speedMultiplier;
    }

    public void Selecciona () {
        if (jugador == "j1") {
            MatchSettings.SelectShip (1, naveIndex ());
            jugador = "j2";
            tj1.isOn = true;
            txvJugadorA.text = "Jugador eligiendo : jugador2";
        } else {
            MatchSettings.SelectShip (2, naveIndex ());
            tj2.isOn = true;
            txvJugadorA.text = "Listo!";
        }
    }

    public void loadGame () {
        if (tj2.isOn && tj1.isOn) {
            SceneManager.LoadScene ("Game");
        } else {
            alerta.text = "Faltan jugadores por elegir naves";
        }
    }
}