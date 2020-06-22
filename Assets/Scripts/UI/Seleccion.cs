using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Seleccion : MonoBehaviour
{
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

    int currentSelection;
    int totalCharacters;

    string nave;
    string jugador;

    // Start is called before the first frame update
    void Start()
    {
        Damage_slider.maxValue=120f;
        nave = "nave1";
        jugador = "j1";
        setSilderValue();
        totalCharacters=2;
        currentSelection=2;
    }

    // Update is called once per frame
    void Update()
    {
        setSilderValue();
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxis("6TH")==1){
            currentAngle=transform.eulerAngles;
            targetRot = targetRot+new Vector3(0,180,0);
            currentSelection++;
            naveCheck();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxis("6TH")==-1 ){
            currentAngle=transform.eulerAngles;
            targetRot = targetRot+new Vector3(0,180,0);
            currentSelection--;
            naveCheck();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) ){
             Debug.Log("Selecciono");
        }
        
        currentAngle=new Vector3(0,Mathf.LerpAngle(currentAngle.y,targetRot.y,0.05f));
        transform.eulerAngles=currentAngle;
    }

    public void naveCheck(){
        if( nave == "nave1"){
            nave = "nave2";
            Debug.Log("ahora nave2");
        }else{
            nave = "nave1";
            Debug.Log("actual nave1");
        }
    }

    void setSilderValue()
    {
        if( nave == "nave1"){
            Damage_slider.value = 85;
            Vida_slider.value = 100;
            Velocidad_slider.value = 50;
        }else{
            Damage_slider.value = 65;
            Vida_slider.value = 60;
            Velocidad_slider.value = 100;
        }
        
    }

    public void Selecciona()
    {
        if(jugador== "j1"){
            jugador= "j2";
            tj1.isOn=true;
            txvJugadorA.text = "Jugador eligiendo : jugador2";
        }else{
            tj2.isOn=true;
            txvJugadorA.text = "Listo!";
        }
    }

    public void loadGame()
    {
        if(tj2.isOn && tj1.isOn){
            SceneManager.LoadScene("Game");
        }else{
            alerta.text = "Faltan jugadores por elegir naves";
        }
    }
}
