using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using TMPro;

public class PlataformasBehaviour : MonoBehaviour
{
    public bool jugando;

    //Monedas que ha recogido el jugador
    public int monedas =0;
    public TextMeshProUGUI numMonedas;

    //Punteros a diferentes elementos de la UI
    public GameObject pantallaInicio;
    public GameObject gameOver;
    public GameObject win;
    public GameObject jugandoUI;
    public TextMeshProUGUI segRestantes;
    public TextMeshProUGUI monedasConseguidas;

    //Timer que tendremos en pantalla
    public TextMeshProUGUI textoTimer;
    public float numeroTimer=100f;

    //Photonview del gestor
    public PhotonView view;

    //Booleano que indica si el primer jugador es vr o no
    public bool vr = false;

    //Punteros a los scripts de los mandos
    public PersonajePlataformasBehaviour mandoUno;


    //Posicion de las monedas, slimes y personaje para reiniciar su posicion
    public Vector3 posicionIniPersonaje;
    public Vector3 posicionIniSlimes;
    public Vector3 posicionIniMonedas;

    //Prefabs de las monedas, slimes y corazones
    public GameObject monedasPrefab;
    public GameObject slimesPrefab;
    public GameObject corazonesPrefab;

    //Timer para mostrar el game over o el texto cuando llegas a la meta
    public float timerFinal;
    public float timerCounterFinal;
    public bool finalizando;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Si no estamos aun jugando , miramos si hay algun input. Si lo hay y es el que le da a play, empezamos a jugar
        if (mandoUno != null && mandoUno.viewJugador != null && !jugando)
        {
            if (mandoUno.viewJugador.IsMine)
            {
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressedDer);
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressedIzq);
                //Si se pulsa la tecla E, iniciamos el juego
                if (Input.GetKey(KeyCode.E) || (buttonPressedIzq || buttonPressedDer))
                {
                    jugando = true;
                    pantallaInicio.SetActive(false);
                    mandoUno.jugando = true;
                    view.RPC("IniciarJugando", RpcTarget.OthersBuffered);
                }
            }
        }
        //Controlamos el counter y si se termina el tiempo o los corazones es game over.
        if (mandoUno != null && mandoUno.viewJugador != null && jugando)
        {
            if (mandoUno.viewJugador.IsMine)
            {
                //Actualizacion del timer
                if (finalizando)
                {
                    timerCounterFinal -= Time.deltaTime;
                }
                else
                {
                    numeroTimer -= Time.deltaTime;
                    textoTimer.text = numeroTimer.ToString("F0");
                    view.RPC("DecrementaTimer", RpcTarget.OthersBuffered);
                    if (numeroTimer <= 0)
                    {
                        GameOver();
                    }
                    if (jugandoUI.transform.GetChild(2).childCount == 0)
                    {
                        GameOver();
                    }

                }
                //Controlamos el timer de cuando llegamos a la meta y llamamos a terminar partida
                if (finalizando && timerCounterFinal < 0)
                {
                    TerminarPartida();
                }
            }


        }
        
    }

    public void IncrementaMoneda()
    {
        monedas += 1;
        numMonedas.text = monedas.ToString();
        if (mandoUno.viewJugador.IsMine)
        {
            view.RPC("RPCIncrementaMoneda", RpcTarget.OthersBuffered);
        }
    }

    public void TerminarPartida()
    {
        if (mandoUno.viewJugador.IsMine)
        {
            view.RPC("RPCTerminarPartida", RpcTarget.OthersBuffered);
        }
        GameObject.Find("Personaje").transform.position = posicionIniPersonaje;

        Destroy(GameObject.Find("Monedas"));
        Destroy(GameObject.Find("Monedas(Clone)"));

        Destroy(GameObject.Find("Slimes"));
        Destroy(GameObject.Find("Slimes(Clone)"));

        Destroy(GameObject.Find("Corazones"));
        Destroy(GameObject.Find("Corazones(Clone)"));
        Destroy(GameObject.Find("Corazones(Clone)"));

        GameObject monedas = Instantiate(monedasPrefab, posicionIniMonedas, Quaternion.identity);
        GameObject slimes = Instantiate(slimesPrefab, posicionIniSlimes, Quaternion.identity);
        GameObject corazones = Instantiate(corazonesPrefab, posicionIniSlimes, Quaternion.identity);

        monedas.transform.parent = GameObject.Find("Grid").transform;
        slimes.transform.parent = GameObject.Find("Grid").transform;
        corazones.transform.position = new Vector3(50, -50, 0);
        corazones.transform.SetParent(jugandoUI.transform,false);
        corazones.transform.SetSiblingIndex(2);

        monedas.transform.localScale=new Vector3(1, 1, 1);
        slimes.transform.localScale = new Vector3(1, 1, 1);
        corazones.transform.localScale = new Vector3(1, 1, 1);
        
        mandoUno.contenedorCorazones = corazones;
        mandoUno.invulnerabilidad = false;
        mandoUno.cuerpoPersonaje.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

        pantallaInicio.SetActive(true);
        jugandoUI.SetActive(true);
        gameOver.SetActive(false);
        win.SetActive(false);

        jugando =false;
        finalizando = false;

        this.monedas = 0;
        numMonedas.text = this.monedas.ToString();

        numeroTimer =100;
        textoTimer.text = numeroTimer.ToString("F0");
    }

    public void GameOver()
    {
        if (mandoUno.viewJugador.IsMine)
        {
            view.RPC("RPCGameOver", RpcTarget.OthersBuffered);
        }
        finalizando = true;
        mandoUno.jugando = false;
        timerCounterFinal = timerFinal;
        gameOver.SetActive(true);
    }

    public void Win()
    {
        if (mandoUno.viewJugador.IsMine)
        {
            view.RPC("RPCWin", RpcTarget.OthersBuffered);
        }
        finalizando = true;
        mandoUno.jugando = false;
        timerCounterFinal = timerFinal;
        segRestantes.text = "Segundos restantes: "+ numeroTimer.ToString("F0");
        monedasConseguidas.text = "Monedas conseguidas: "+monedas.ToString();
        win.SetActive(true);
        jugandoUI.SetActive(false);

    }
    [PunRPC]
    void RPCIncrementaMoneda()
    {
        monedas += 1;
        numMonedas.text = monedas.ToString();
    }

    [PunRPC]
    public void RPCTerminarPartida()
    {
        GameObject.Find("Personaje").transform.position = posicionIniPersonaje;

        Destroy(GameObject.Find("Monedas"));
        Destroy(GameObject.Find("Monedas(Clone)"));

        Destroy(GameObject.Find("Slimes"));
        Destroy(GameObject.Find("Slimes(Clone)"));

        Destroy(GameObject.Find("Corazones"));
        Destroy(GameObject.Find("Corazones(Clone)"));
        Destroy(GameObject.Find("Corazones(Clone)"));

        GameObject monedas = Instantiate(monedasPrefab, posicionIniMonedas, Quaternion.identity);
        GameObject slimes = Instantiate(slimesPrefab, posicionIniSlimes, Quaternion.identity);
        GameObject corazones = Instantiate(corazonesPrefab, posicionIniSlimes, Quaternion.identity);

        monedas.transform.parent = GameObject.Find("Grid").transform;
        slimes.transform.parent = GameObject.Find("Grid").transform;
        corazones.transform.position = new Vector3(50, -50, 0);
        corazones.transform.SetParent(jugandoUI.transform, false);
        corazones.transform.SetSiblingIndex(2);

        monedas.transform.localScale = new Vector3(1, 1, 1);
        slimes.transform.localScale = new Vector3(1, 1, 1);
        corazones.transform.localScale = new Vector3(1, 1, 1);

        mandoUno.contenedorCorazones = corazones;
        mandoUno.invulnerabilidad = false;
        mandoUno.cuerpoPersonaje.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

        pantallaInicio.SetActive(true);
        jugandoUI.SetActive(true);
        gameOver.SetActive(false);
        win.SetActive(false);

        jugando = false;
        finalizando = false;

        this.monedas = 0;
        numMonedas.text = this.monedas.ToString();

        numeroTimer = 100;
        textoTimer.text = numeroTimer.ToString("F0");
    }

    [PunRPC]
    public void RPCGameOver()
    {
        finalizando = true;
        mandoUno.jugando = false;
        timerCounterFinal = timerFinal;
        gameOver.SetActive(true);
    }

    [PunRPC]
    public void RPCWin()
    {
        finalizando = true;
        mandoUno.jugando = false;
        timerCounterFinal = timerFinal;
        segRestantes.text = "Segundos restantes: " + numeroTimer.ToString("F0");
        monedasConseguidas.text = "Monedas conseguidas: " + monedas.ToString();
        win.SetActive(true);
        jugandoUI.SetActive(false);

    }

    [PunRPC]
    public void DecrementaTimer()
    {
        numeroTimer -= Time.deltaTime;
        textoTimer.text = numeroTimer.ToString("F0");
    }

    [PunRPC]
    public void IniciarJugando()
    {
        jugando = true;
        pantallaInicio.SetActive(false);
        mandoUno.jugando = true;
    }
}
