using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.XR;

public class PongBehaivour : MonoBehaviour
{
    public bool vr  = false;
    //Boleanos que controlan un par de estados del juego
    public bool vsIA=true;
    public bool jugando = false;
    public bool ganado = false;

    //Punteros al mando de los jugadores
    public GameObject mandoDos;
    public GameObject mandoUno;

    //Punteros a la raqueta 2 por si hay que usar la IA y a la bola
    public GameObject bola;
    public GameObject jugadorDos;

    //Punteros a objetos de la interfaz y a el timer de la ventana ganador
    public GameObject elegirModo;
    public GameObject puntuacion;
    public GameObject ventanaGanador;
    public float tiempoVentana;
    public float tiempoVentanaTimer;
    
    //Photonview vinculado a todo el pong, utilizado principalmente para las funciones RPC
    public PhotonView view;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Primero de todo, comprobamos que un jugador ha cogido el mando numero uno, además de que el juego aun no esta en marcha
        if (mandoUno.GetComponent<RaquetaBehaivour>().viewJugador != null && !jugando)
        {
            //Si el PhotonView es el del jugador 1
            if (mandoUno.GetComponent<RaquetaBehaivour>().viewJugador.IsMine)
            {
                bool aux = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickValue);
                bool aux2 = InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressed);
                //Si se pulsa la tecla W, se marca la opción de la IA
                if (Input.GetKey(KeyCode.W) || (vr && joystickValue.y>0))
                {
                    vsIA = true;
                    elegirModo.transform.Find("SelectIA").gameObject.SetActive(true);
                    elegirModo.transform.Find("Select2J").gameObject.SetActive(false);
                    view.RPC("ActualizarIA", RpcTarget.OthersBuffered, true);
                    view.RPC("ActualizarSelec", RpcTarget.OthersBuffered, true);
                }
                //Si se pulsa la tecla S, se marca la opción de 2J
                else if (Input.GetKey(KeyCode.S) || (vr && joystickValue.y < 0))
                {
                    vsIA = false;
                    elegirModo.transform.Find("SelectIA").gameObject.SetActive(false);
                    elegirModo.transform.Find("Select2J").gameObject.SetActive(true);
                    view.RPC("ActualizarIA", RpcTarget.OthersBuffered, false);
                    view.RPC("ActualizarSelec", RpcTarget.OthersBuffered, false);
                }
                //Si se pulsa la tecla E, y a demas, por si acaso, nos aseguramos de que el jugador 1 esta listo (esta sujetando el mando)
                else if ((Input.GetKey(KeyCode.E)  || (vr && buttonPressed)) && mandoUno.GetComponent<RaquetaBehaivour>().mandoCogido)
                {
                    //Si el jugador dos esta listo y no estamos en el modo vsIA o si estamos en el modo vsIA, empezamos el juego
                    if (vsIA)
                    {
                        //Le damos el Photonview de la raqueta dos al jugador uno para que vea bien el movimiento de la IA
                        mandoDos.GetComponent<RaquetaBehaivour>().raqueta.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);

                        //Ocultamos el selector
                        elegirModo.SetActive(false);
                        
                        //Indicamos a los objetos del juego que el partido empieza
                        jugando = true;
                        bola.GetComponent<BolaBehaivour>().jugando = true;
                        bola.GetComponent<BolaBehaivour>().ReiniciarBola(true);
                        mandoUno.GetComponent<RaquetaBehaivour>().jugando = true;
                        mandoDos.GetComponent<RaquetaBehaivour>().jugando = true;
                        mandoUno.GetComponent<RaquetaBehaivour>().EmpiezaJuego();
                        mandoDos.GetComponent<RaquetaBehaivour>().EmpiezaJuego();
                        
                        view.RPC("ActualizarModo", RpcTarget.OthersBuffered, false);
                        view.RPC("ActualizarJugando", RpcTarget.OthersBuffered, true);                       
                    }
                    else if (mandoDos.GetComponent<RaquetaBehaivour>().mandoCogido && !vsIA)
                    {
                        //Ocultamos el selector
                        elegirModo.SetActive(false);
                        
                        //Indicamos a los objetos del juego que el partido empieza
                        jugando = true;
                        bola.GetComponent<BolaBehaivour>().jugando = true;
                        bola.GetComponent<BolaBehaivour>().ReiniciarBola(true);
                        mandoUno.GetComponent<RaquetaBehaivour>().jugando = true;
                        mandoDos.GetComponent<RaquetaBehaivour>().jugando = true;
                        mandoUno.GetComponent<RaquetaBehaivour>().EmpiezaJuego();
                        mandoDos.GetComponent<RaquetaBehaivour>().EmpiezaJuego();
                        view.RPC("EntregarView", RpcTarget.OthersBuffered);
                        view.RPC("ActualizarModo", RpcTarget.OthersBuffered, false);
                        view.RPC("ActualizarJugando", RpcTarget.OthersBuffered, true);
                    }
                }
            }

        }
        //Si ha ganado algun jugador, y estamos en el entorno del jugador uno
        else if (ganado && mandoUno.GetComponent<RaquetaBehaivour>().viewJugador.IsMine)
        {
            //Si le timer de la ventana ha expirado, ocultamos la ventana de ganador y reiniciamos la pantalla
            if (tiempoVentanaTimer<-1)
            {
                ventanaGanador.SetActive(false);
                ReiniciarPantalla();
            }
            else //Se decrementa el timer hasta que expira el tiempo
            {
                tiempoVentanaTimer = tiempoVentanaTimer - 0.9f * Time.deltaTime;
            }
        }
        //Si se ha seleccionado vsIA y se ha empezado a jugar, el propio script actua como controlador de la IA. Simplemente la raqueta sigue la posicion Y de la bola.
        if (vsIA && jugando)
        {
            if (bola.transform.position.y > jugadorDos.transform.position.y && jugadorDos.transform.position.y < 16)
            {
                jugadorDos.transform.position = new Vector3(jugadorDos.transform.position.x, jugadorDos.transform.position.y + 3.5f * Time.deltaTime, jugadorDos.transform.position.z);
            }
            else if (bola.transform.position.y < jugadorDos.transform.position.y && jugadorDos.transform.position.y > 8.6)
            {
                jugadorDos.transform.position = new Vector3(jugadorDos.transform.position.x, jugadorDos.transform.position.y - 3.5f * Time.deltaTime, jugadorDos.transform.position.z);
            }
        }
        
    }
    //Actualiza la puntuación del juego dependiendo de si se ha marcado punto en la izquierda (+1 al jugador 2) o a la derecha (+1 al jugador 1).
    public void AcutalizarPuntuacion(bool izq)
    {
        //Cogemos el texto del canvas dedicado a las puntuaciones de los dos jugadores.
        Text puntuacionUno = puntuacion.transform.Find("Puntuacion1").gameObject.GetComponent<Text>();
        Text puntuacionDos = puntuacion.transform.Find("Puntuacion2").gameObject.GetComponent<Text>();
        if (izq)
        {
            puntuacionDos.text = (int.Parse(puntuacionDos.text) + 1).ToString();
        }
        else
        {
            puntuacionUno.text = (int.Parse(puntuacionUno.text) + 1).ToString();
        }
        view.RPC("ActualizarPuntuacion", RpcTarget.OthersBuffered, puntuacionUno.text, puntuacionDos.text);
        //Si el jugador 1 llega a 3 puntos. Se activa la ventana de ganador y se pasa el juego para que los jugadores y la bola no se muevan mas.
        if (int.Parse(puntuacionUno.text) >= 3)
        {
            ganado = true;
            ReiniciarJugando();
            ventanaGanador.SetActive(true);
            tiempoVentanaTimer = tiempoVentana;
            view.RPC("ActualizarGanado", RpcTarget.OthersBuffered, true, "Jugador 1 Gana");
            ventanaGanador.transform.Find("Button").Find("Text").GetComponent<Text>().text = "Jugador 1 Gana";
        }
        //Si el jugador 2 llega a 3 puntos. ""
        else if (int.Parse(puntuacionDos.text) >= 3)
        {
            ganado = true;
            ReiniciarJugando();
            ventanaGanador.SetActive(true);
            tiempoVentanaTimer = tiempoVentana;
            view.RPC("ActualizarGanado", RpcTarget.OthersBuffered, true, "Jugador 2 Gana");
            ventanaGanador.transform.Find("Button").Find("Text").GetComponent<Text>().text = "Jugador 2 Gana";
        }
    }
    //Devuelve la pantalla a su origen (canvas elegir modo activo y reinicia la puntuacion).
    public void ReiniciarPantalla()
    {
        vsIA = true;
        ganado = false;
        jugando = false;
        ReiniciarPuntuacion();
        elegirModo.SetActive(true);
        ventanaGanador.SetActive(false);
        elegirModo.transform.Find("SelectIA").gameObject.SetActive(true);
        elegirModo.transform.Find("Select2J").gameObject.SetActive(false);
        view.RPC("ActualizarIA", RpcTarget.OthersBuffered, true);
        view.RPC("ActualizarModo", RpcTarget.OthersBuffered, true);
        view.RPC("ActualizarSelec", RpcTarget.OthersBuffered, true);
        view.RPC("ActualizarJugando", RpcTarget.OthersBuffered, false);
        view.RPC("ActualizarGanado", RpcTarget.OthersBuffered, false, "");
    }

    //Informa a los jugadores y la bola que el juego ya no esta en marcha.
    public void ReiniciarJugando()
    {
        bola.GetComponent<BolaBehaivour>().QuitarJugando();
        mandoUno.GetComponent<RaquetaBehaivour>().QuitarJugando();
        mandoDos.GetComponent<RaquetaBehaivour>().QuitarJugando();
        view.RPC("ReiniciarJugandoRPC", RpcTarget.OthersBuffered);
    }

    //Reinicia la puntuacion (cambia el texto de la puntuación a 0).
    public void ReiniciarPuntuacion() {
        puntuacion.transform.Find("Puntuacion1").gameObject.GetComponent<Text>().text=0.ToString();
        puntuacion.transform.Find("Puntuacion2").gameObject.GetComponent<Text>().text = 0.ToString();
        view.RPC("ActualizarPuntuacion", RpcTarget.OthersBuffered, puntuacion.transform.Find("Puntuacion1").gameObject.GetComponent<Text>().text, 
            puntuacion.transform.Find("Puntuacion2").gameObject.GetComponent<Text>().text);
    }


    //Funciones RPC de Photon


    //Actualiza el valor del booleando vsIa.
    [PunRPC]
    void ActualizarIA(bool vsIA)
    {
        this.vsIA = vsIA;
    }

    //Actualiza el valor del booleano jugando.
    [PunRPC]
    void ActualizarJugando(bool jugando)
    {
        this.jugando = jugando;
    }

    //Actualiza el valor del float tiempoVentanaTimer.
    [PunRPC]
    void ActualizarTiempo(float tiempoVentanaTimer)
    {
        this.tiempoVentanaTimer = tiempoVentanaTimer;
    }
    
    //Actualiza el valor de la variable jugando de los mandos y la bola a false
    [PunRPC]
    void ReiniciarJugandoRPC()
    {
        bola.GetComponent<BolaBehaivour>().QuitarJugando();
        mandoUno.GetComponent<RaquetaBehaivour>().QuitarJugando();
        mandoDos.GetComponent<RaquetaBehaivour>().QuitarJugando();
    }

    //Informa a los jugadores y la bola que el juego esta en marcha.
    [PunRPC]
    void IniciarJugando()
    {
        bola.GetComponent<BolaBehaivour>().jugando = true;
        mandoUno.GetComponent<RaquetaBehaivour>().jugando = true;
        mandoDos.GetComponent<RaquetaBehaivour>().jugando = true;
    }

    //Actualiza el estade del UI elegirModo, el cual indica si se esta seleccionando IA o 2J.
    [PunRPC]
    void ActualizarSelec(bool selec)
    {
        elegirModo.transform.Find("SelectIA").gameObject.SetActive(selec);
        elegirModo.transform.Find("Select2J").gameObject.SetActive(!selec);
    }

    //Actualiza todo el canvas elegirModo para mostrar u ocultar este modo.
    [PunRPC]
    void ActualizarModo(bool modo)
    {
        elegirModo.SetActive(modo);
    }

    //Actualiza el valor del booleano ganado.
    [PunRPC]
    void ActualizarGanado(bool ganado, string text)
    {
        ventanaGanador.SetActive(ganado);
        ventanaGanador.transform.Find("Button").Find("Text").GetComponent<Text>().text = text;
    }

    //Actualiza el texto de las puntuaciones
    [PunRPC]
    void ActualizarPuntuacion(string puntuacion1, string puntuacion2)
    {
        puntuacion.transform.Find("Puntuacion1").gameObject.GetComponent<Text>().text = puntuacion1;
        puntuacion.transform.Find("Puntuacion2").gameObject.GetComponent<Text>().text = puntuacion2;
    }

    //Entrega el Photonview al segundo jugador
    [PunRPC]
    void EntregarView()
    {
        if (mandoDos.GetComponent<RaquetaBehaivour>().viewJugador.IsMine)
        {
            mandoDos.GetComponent<RaquetaBehaivour>().raqueta.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        }

    }

}
