using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

public class MandoBolosBehaviour : PersonajeBehaivour
{
    bool vr = false;

    //Boleano que usamos para impedir que un usuario vr lance la bola varias veces
    public bool vrPressed = false;

    //Puntero al script que maneja el comportamiento del juego
    public BolosBehaviour bolos;

    //Puntero objeto que representa al personaje bolero
    public GameObject cuerpoPersonaje;

    //Puntero a las manos del bolero
    public GameObject manoDerBolero;
    public GameObject manoIzqBolero;

    //Mano que tomará la bola, segun hayamos cogido el mando con la mano izquierda o derecha
    public GameObject manoElegida;
    public bool manoIzq;

    //Objeto bola 
    public GameObject bola;

    //Objeto de la mano del jugador para poder tener el tracking de movimiento
    public GameObject manoJugador;

    //Booleano para saber si el juego esta en marcha
    public bool jugando = false;

    //Booleano para saber si el jugador de PC ya ha tirado para no poder volver a tirar
    public bool tirado = false;

    private bool modoMovimiento=false; //true se mueve rotando, false se mueve de lado a lado

    //String que usamos para saber si el nombre del mando actual es el primer mando
    private string nombreMandoUno="MandoWiiJ1";

    public float rotacionInicial=0;
    protected override void Update()
    {
        //Miramos si el jugador 1 tiene el mando cogido y si el juego de Bolos le cede el turno
        if (viewJugador != null && mandoCogido == true && jugando)
        {
            //Si el PhotonView es el mio
            if (viewJugador.IsMine)
            {
                //Si somos un usuario vr
                if (vr)
                {
                    //Cogemos los inputs de los botones principales de la mano izquierda y derecha, dado que podemos tirar con cualquiera de las dos
                    bool aux = InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressedDer);
                    bool aux2 = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressedIzq);

                    //Lanzamos con la mano izquierda
                    if ((buttonPressedIzq || buttonPressedDer) && !vrPressed)
                    {
                        bola.transform.parent = manoElegida.transform.parent.parent;
                        vrPressed = true;
                        float rotacionFinal= manoJugador.transform.localEulerAngles.y-rotacionInicial;
                        if (0< rotacionFinal && rotacionFinal < 180)
                        {
                            bola.GetComponent<BolaBolosBehaivour>().LanzarBola(rotacionFinal / 100, vr);
                        }
                        else if (180 < rotacionFinal && rotacionFinal < 360)
                        {
                            bola.GetComponent<BolaBolosBehaivour>().LanzarBola(-(360 % rotacionFinal) / 100, vr);
                        }
                        else
                        {
                            bola.GetComponent<BolaBolosBehaivour>().LanzarBola(rotacionFinal / 100, vr);
                        }
                        
                    }
                    //Movemos la mano del personaje bolero segun la mano del jugador
                    if (manoIzq)
                    {
                        manoElegida.transform.localPosition = manoJugador.transform.localPosition+new Vector3(-0.8f,-0.8f,0);
                    }
                    else
                    {
                        manoElegida.transform.localPosition = manoJugador.transform.localPosition + new Vector3(0.4f, -0.8f, 0);
                    }
                    
                }
                //Si somos un usuario pc
                else
                {
                    //Modo de movimiento de rotacion
                    if (modoMovimiento)
                    {
                        if (Input.GetKey(KeyCode.D) && cuerpoPersonaje.transform.rotation.y < 0.3)
                        {
                            cuerpoPersonaje.transform.rotation = cuerpoPersonaje.transform.rotation * Quaternion.Euler(0, 0.1f, 0);
                        }
                        else if (Input.GetKey(KeyCode.A) && cuerpoPersonaje.transform.rotation.y > -0.3)
                        {
                            cuerpoPersonaje.transform.rotation = cuerpoPersonaje.transform.rotation * Quaternion.Euler(0, -0.1f, 0);
                        }
                    }
                    //Modo de movimiento de translacion
                    else
                    {
                        if (Input.GetKey(KeyCode.D) && cuerpoPersonaje.transform.position.x < 85)
                        {
                            cuerpoPersonaje.transform.position = 
                                new Vector3(cuerpoPersonaje.transform.position.x + 4f * Time.deltaTime, cuerpoPersonaje.transform.position.y , cuerpoPersonaje.transform.position.z);
                        }
                        else if (Input.GetKey(KeyCode.A) && cuerpoPersonaje.transform.position.x > 83.5)
                        {
                            cuerpoPersonaje.transform.position = 
                                new Vector3(cuerpoPersonaje.transform.position.x - 4f * Time.deltaTime, cuerpoPersonaje.transform.position.y , cuerpoPersonaje.transform.position.z);
                        }
                    }
                    //Tiramos la bola
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (!tirado)
                        {
                            tirado = true;
                            bola.transform.localPosition = new Vector3(0, -0.5f, 0);
                            bola.transform.parent = manoElegida.transform.parent.parent;
                            bola.transform.rotation = cuerpoPersonaje.transform.rotation;
                            bola.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                            view.RPC("RPCLanzarBola", RpcTarget.OthersBuffered, manoElegida.transform.parent.parent.name, cuerpoPersonaje.transform.rotation, bola.transform.position);
                            bola.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                            bola.GetComponent<BolaBolosBehaivour>().LanzarBola(cuerpoPersonaje.transform.rotation.y * 2,vr);
                        }
                    }
                    //Cambiamos el modo de movimiento del jugador en PC
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        modoMovimiento = !modoMovimiento;
                    }
                }

            }

        }

    }

    //Actualiza el estado porque un jugador ha cogido el mando
    public override void ActualizarEstadoEntrada(GameObject mano, bool manoIzq)
    {
        //Cogemos la mano del jugador e indicamos si es la mano izquierda o derecha
        manoJugador = mano;
        this.manoIzq = manoIzq;
        if (mano != null)
        {
            rotacionInicial = mano.transform.localEulerAngles.y;
        }
        
        if (viewJugador!=null && viewJugador.IsMine)
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

            //Indicamos la mano del personaje de bolos
            if (manoIzq)
            {
                manoElegida = cuerpoPersonaje.transform.GetChild(3).gameObject;
            }
            else
            {
                manoElegida = cuerpoPersonaje.transform.GetChild(2).gameObject;
            }
            view.RPC("ActualizarManoElegida", RpcTarget.OthersBuffered, manoIzq);
            view.RPC("ActualizarMando", RpcTarget.OthersBuffered, nombreMando, true);
        }   
    }

    //Actualiza el estado porque un jugador ha dejado el mando
    public override void ActualizarEstadoSalida(bool desconectado)
    {
        if (viewJugador != null)
        {
            if (viewJugador.IsMine)
            {
                //Si tenemos el primer mando
                if (nombreMando==nombreMandoUno)
                {
                    //Si estamos jugando solos, se acaba la partida
                    if (bolos.solo)
                    {
                        bolos.FinalizarJuego();
                    }
                    //Si no estamos jugando solos, pero el segundo jugador ya abandonado, se acaba la partida
                    else if (bolos.esperandoDos)
                    {
                        bolos.FinalizarJuego();
                    }
                    //Si no estamos jugando solos, indicamos que se inicie el proceso de espera de un jugador
                    else
                    {
                        bolos.esperandoUno = true;
                        bolos.VentanaEspera(true);
                        view.RPC("ActualizarEsperando", RpcTarget.OthersBuffered, true,false);
                        view.RPC("VentanaEspera", RpcTarget.OthersBuffered, true);
                    }
                }
                //Si somos el segundo jugador
                else
                {
                    //Si el jugador uno ya ha abandonado, se acaba la partida
                    if (bolos.esperandoUno)
                    {
                        bolos.FinalizarJuego();
                    }
                    //Indicamos que se inicie el proceso de espera de un jugador
                    else if (!bolos.solo)
                    {
                        bolos.esperandoDos = true;
                        bolos.VentanaEspera(false);
                        view.RPC("ActualizarEsperando", RpcTarget.OthersBuffered,false, true);
                        view.RPC("VentanaEspera", RpcTarget.OthersBuffered, false);
                    }
                }
                
            }
        }
        else if (desconectado)
        {
            bolos.FinalizarJuego();
        }
        vr = false;
        view.RPC("ActualizarMando", RpcTarget.OthersBuffered, nombreMando, false);
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
    public void CogerBola()
    {
        
        if (viewJugador!=null && viewJugador.IsMine)
        {
            tirado = false;
            bola.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            bola.transform.parent = manoElegida.transform;
            bola.GetComponent<BolaBolosBehaivour>().PararBola();
            bola.transform.localPosition = new Vector3(0, -0.5f, 0);
            bola.transform.localScale = new Vector3(2/3f, 2 / 3f, 2 / 3f);
            view.RPC("RPCCogerBola", RpcTarget.OthersBuffered, manoIzq);
        }
    }

    public void ActualizarFlecha(bool activo)
    {
        if (!vr)
        {
            cuerpoPersonaje.transform.Find("Flecha").gameObject.SetActive(activo);
            view.RPC("RPCActualizarFlecha", RpcTarget.OthersBuffered, activo);
        }
    }

    //Funciones RPC de Photon


    //Actualiza el booleano jugando
    [PunRPC]
    void ActualizarJugando(bool jugando)
    {
        this.jugando = jugando;
    }

    //Actualiza la mano elegida
    [PunRPC]
    void ActualizarManoElegida(bool manoIzq)
    {
        if (manoIzq)
        {
            manoElegida = manoIzqBolero;
        }
        else
        {
            manoElegida = manoDerBolero;
        }
    }
    //Coge la bola para que el transform padre se actualice en todas las instancias
    [PunRPC]
    void RPCCogerBola(bool manoIzq)
    {
        if (manoIzq)
        {
            bola.transform.parent = manoIzqBolero.transform;
        }
        else
        {
            bola.transform.parent = manoDerBolero.transform;
        }
        bola.transform.localPosition = new Vector3(0, -0.5f, 0);
        bola.transform.localScale = new Vector3(2 / 3f, 2 / 3f, 2 / 3f);
    }
    //Actualiza el estado de espera del juego
    [PunRPC]
    void ActualizarEsperando(bool esperandoUno, bool esperandoDos)
    {
        bolos.esperandoUno = esperandoUno;
        bolos.esperandoDos = esperandoDos;
    }
    //Finaliza el juego en todas las instancias
    [PunRPC]
    void FinalizarJuego()
    {
        bolos.FinalizarJuego();
    }
    //Abre la ventana de espera en todos las instancias
    [PunRPC]
    void VentanaEspera(bool jugador)
    {
        bolos.VentanaEspera(jugador);
        if (jugador)
        {
            bolos.esperandoUno = true;
        }
        else
        {
            bolos.esperandoDos = true;
        }
        
    }

    //Actualiza el nombre del mando y el booleano mandoCogido
    [PunRPC]
    void ActualizarMando(string nombreMando, bool mandoCogido)
    {
        this.nombreMando = nombreMando;
        this.mandoCogido = mandoCogido;
    }

    //Actualiza el padre y otros valores del transform al lanzar la bola en una instancia
    [PunRPC]
    void RPCLanzarBola(string nombrePadre, Quaternion rotacion, Vector3 pos)
    {
        bola.transform.parent = GameObject.Find(nombrePadre).transform;
        bola.transform.rotation = rotacion;
        bola.transform.position = pos;
        bola.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }
    //Muestra u oculta la flecha del personaje bolero en PC
    [PunRPC]
    void RPCActualizarFlecha(bool activo)
    {
        if (!vr)
        {
            cuerpoPersonaje.transform.Find("Flecha").gameObject.SetActive(activo);
        }
    }
}
