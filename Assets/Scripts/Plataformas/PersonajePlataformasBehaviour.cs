using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

public class PersonajePlataformasBehaviour : PersonajeBehaivour
{
    bool vr = false;

    //Puntero al controlador del juego
    public PlataformasBehaviour juego;

    //Puntero objeto que representa al personaje del juego
    public GameObject cuerpoPersonaje;
    public Animator animPersonaje;

    //Booleano que indica si la partida esta en marcha
    public bool jugando = false;

    //Velocidad de salto del personake
    public float velocidadY=0;

    //Objeto que contiene la UI de la vida del jugador
    public GameObject contenedorCorazones;

    //Estado actual de la animacion
    public string estadoActual;

    //Constantes con los nombres de las animaciones
    private static string PERSONAJE_QUIETO = "Personaje_Quieto";
    private static string PERSONAJE_CAMINANDO = "Personaje_Caminando";
    private static string PERSONAJE_SALTO_ABAJO = "Personaje_salto_abajo";
    private static string PERSONAJE_SALTO_ARRIBA = "Personaje_salto_arriba";
    private static string PERSONAJE_MUERTE = "Personaje_muerte";

    //Contador de invulnerabilidad
    public float timerInvulnerabilidad;
    public float timerCounterInvulnerabilidad;
    public bool invulnerabilidad = false;

    //Delay salto
    public float timerSalto;
    public float timerCounterSalto;

    //Contador por si nos quedamos medio bugueados bajo tierra
    /*public float timerBajoTierra;
    public float timerCounterBajoTierra;
    public bool bajoTierra;*/

    protected override void Update()
    {
        //Miramos si el jugador 1 tiene el mando cogido y si el juego de Bolos y si es su turno esta en marcha
        if (viewJugador != null && mandoCogido == true && jugando)
        {
            //Si el PhotonView es el mio y somos el jugador 2 y no esta el modo vsIA o somos el jugador 1 (por lo tanto nuestro mando es el 1º)
            if (viewJugador.IsMine)
            {
                if (invulnerabilidad)
                {
                    timerCounterInvulnerabilidad -= Time.deltaTime;
                }
                if (2.5< timerCounterInvulnerabilidad && timerCounterInvulnerabilidad < 3 && invulnerabilidad)
                {
                    cuerpoPersonaje.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                }
                else if (2 < timerCounterInvulnerabilidad && timerCounterInvulnerabilidad < 2.5 && invulnerabilidad)
                {
                    cuerpoPersonaje.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                }
                else if (1.5 < timerCounterInvulnerabilidad && timerCounterInvulnerabilidad < 2 && invulnerabilidad)
                {
                    cuerpoPersonaje.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                }
                else if (1 < timerCounterInvulnerabilidad && timerCounterInvulnerabilidad < 1.5 && invulnerabilidad)
                {
                    cuerpoPersonaje.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                }
                else if (0.5 < timerCounterInvulnerabilidad && timerCounterInvulnerabilidad < 1 && invulnerabilidad)
                {
                    cuerpoPersonaje.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                }
                else if(invulnerabilidad && timerCounterInvulnerabilidad < 0 && invulnerabilidad)
                {
                    invulnerabilidad = false;
                    cuerpoPersonaje.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                }
                timerCounterSalto -= Time.deltaTime;
                /*if (cuerpoPersonaje.transform.position.y < -1.25 && !bajoTierra)
                {
                    Debug.Log("Estoy bajo tierra");
                    bajoTierra = true;
                    timerCounterBajoTierra = timerBajoTierra;
                }
                if (bajoTierra)
                {
                    timerCounterBajoTierra -= Time.deltaTime;
                }
                if(timerCounterBajoTierra<0 && bajoTierra)
                {
                    cuerpoPersonaje.transform.position = new Vector3(cuerpoPersonaje.transform.position.x, -1.245f, cuerpoPersonaje.transform.position.z);
                    bajoTierra = false;
                }*/

                //Obtenemos booleanos necesarios sobre las colisiones del jugador
                bool tocandoSuelo = cuerpoPersonaje.transform.Find("DetecSuelo").GetComponent<DetectarSuelo>().isGrounded;
                bool tocandoIzquierda= cuerpoPersonaje.transform.Find("DetecIzq").GetComponent<DetectarParedIzq>().isIzqTouch;
                bool tocandoDerecha = cuerpoPersonaje.transform.Find("DetecDer").GetComponent<DetectarParedDer>().isDerTouch;

                //Obtenemos los inputs de los controles vr
                bool aux = InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressedDer);
                bool aux2 = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickValue);

                    
                if (Input.GetKey(KeyCode.D) || joystickValue.x > 0)
                {
                    view.RPC("FlipX", RpcTarget.OthersBuffered, false);
                    cuerpoPersonaje.GetComponent<SpriteRenderer>().flipX = false;
                    if (cuerpoPersonaje.transform.Find("DetecSuelo").GetComponent<DetectarSuelo>().isGrounded)
                    {
                        CambiarAnimacion(PERSONAJE_CAMINANDO);
                    }
                    if (!tocandoDerecha)
                    {
                        cuerpoPersonaje.transform.position =
                        new Vector3(cuerpoPersonaje.transform.position.x + 1f * Time.deltaTime, cuerpoPersonaje.transform.position.y, cuerpoPersonaje.transform.position.z);
                    }
                        
                }
                else if (Input.GetKey(KeyCode.A) || joystickValue.x < 0)
                {
                    cuerpoPersonaje.GetComponent<SpriteRenderer>().flipX = true;
                    view.RPC("FlipX", RpcTarget.OthersBuffered, true);
                    if (cuerpoPersonaje.transform.Find("DetecSuelo").GetComponent<DetectarSuelo>().isGrounded)
                    {
                        CambiarAnimacion(PERSONAJE_CAMINANDO);
                    }
                    if (!tocandoIzquierda)
                    {
                        cuerpoPersonaje.transform.position =
                        new Vector3(cuerpoPersonaje.transform.position.x - 1f * Time.deltaTime, cuerpoPersonaje.transform.position.y, cuerpoPersonaje.transform.position.z);
                    }
                        
                }
                else if (tocandoSuelo && (!Input.GetKeyDown(KeyCode.Space)|| !buttonPressedDer))
                {
                    CambiarAnimacion(PERSONAJE_QUIETO);
                    cuerpoPersonaje.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                }
                if(cuerpoPersonaje.GetComponent<Rigidbody2D>().velocity.y<-0.5f && !tocandoSuelo)
                {
                    CambiarAnimacion(PERSONAJE_SALTO_ABAJO);
                }

                if ((buttonPressedDer || Input.GetKeyDown(KeyCode.Space)) && tocandoSuelo && timerCounterSalto<0)
                {
                    CambiarAnimacion(PERSONAJE_SALTO_ARRIBA);
                    timerCounterSalto = timerSalto;
                    cuerpoPersonaje.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                    cuerpoPersonaje.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, velocidadY), ForceMode2D.Force);
                }
                
            }

        }

    }

    //Actualiza el estado porque un jugador ha cogido el mando
    public override void ActualizarEstadoEntrada(GameObject mano, bool manoIzq)
    {
        if (viewJugador != null && viewJugador.IsMine)
        {
            //Miramos si la entrada del mando se hace desde un dispositivo VR
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            foreach (var xrDisplay in xrDisplaySubsystems)
            {
                if (xrDisplay.running)
                {
                    vr = true;
                }
            }
            view.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            cuerpoPersonaje.transform.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    //Actualiza el estado porque un jugador ha dejado el mando
    public override void ActualizarEstadoSalida(bool desconectado)
    {
        if (viewJugador != null)
        {
            if (viewJugador.IsMine)
            {
                juego.TerminarPartida();

            }
        }
        else if (desconectado)
        {
            juego.TerminarPartida();
        }
        vr = false;
        view.RPC("ActualizarMando", RpcTarget.OthersBuffered, nombreMando, false);
    }



    private void CambiarAnimacion(string nuevoEstado)
    {
        if (viewJugador.IsMine)
        {
            if (estadoActual == nuevoEstado) return;
            animPersonaje.Play(nuevoEstado);
            estadoActual = nuevoEstado;
            view.RPC("RPCActualizarAnim", RpcTarget.OthersBuffered, nuevoEstado);
        }
    }

    public void Salto(int velocidadExtraX, int velocidadExtraY)
    {
        if (viewJugador.IsMine)
        {
            cuerpoPersonaje.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
            cuerpoPersonaje.GetComponent<Rigidbody2D>().AddForce(new Vector3(velocidadExtraX, velocidadY + velocidadExtraY), ForceMode2D.Force);
        }
    }

    public void QuitarVida()
    {
        if (viewJugador.IsMine)
        {
            Destroy(contenedorCorazones.transform.GetChild(0).gameObject);
            invulnerabilidad = true;
            timerCounterInvulnerabilidad = timerInvulnerabilidad;
            view.RPC("RPCQuitarVida", RpcTarget.OthersBuffered);
        }
    }
    public void EmpiezaJuego()
    {
        view.RPC("ActualizarJugando", RpcTarget.OthersBuffered, true);
    }

    public void QuitarJugando()
    {
        this.jugando = false;
        view.RPC("ActualizarJugando", RpcTarget.OthersBuffered, false);
    }



    //Funciones RPC de Photon


    //Actualiza el booleano jugando
    [PunRPC]
    void ActualizarJugando(bool jugando)
    {
        this.jugando = jugando;
    }

    //Actualiza el nombre del mando y el booleano mandoCogido
    [PunRPC]
    void ActualizarMando(string nombreMando, bool mandoCogido)
    {
        this.nombreMando = nombreMando;
        this.mandoCogido = mandoCogido;
    }

    [PunRPC]
    void RPCActualizarAnim(string nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;
        animPersonaje.Play(nuevoEstado);
        estadoActual = nuevoEstado;
    }
    [PunRPC]
    void FlipX(bool flip)
    {
        cuerpoPersonaje.GetComponent<SpriteRenderer>().flipX = flip;
    }

    [PunRPC]
    public void RPCQuitarVida()
    {
        Destroy(contenedorCorazones.transform.GetChild(0).gameObject);
        /* invulnerabilidad = true;
        timerCounterInvulnerabilidad = timerInvulnerabilidad;*/
    }


}
