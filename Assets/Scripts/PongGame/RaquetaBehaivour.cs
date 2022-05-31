using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

public class RaquetaBehaivour : PersonajeBehaivour
{
    bool vr = false;
    //Puntero a la bola para coger su photonview
    public GameObject bola;

    //Puntero al script que controla el juego de Pong
    public PongBehaivour pong; 

    //Puntero al objeto 3D que representa la raqueta
    public GameObject raqueta;

    //Booleano que indica si el Pong esta en marcha (jugando)
    public bool jugando = false;

    static string mandoUno = "MandoAtariJ1"; //Nombre del mando nº1

    protected override void Update()
    {
        //Miramos si el jugador 1 tiene el mando cogido y si el juego de Pong esta en marcha
        if (viewJugador != null && mandoCogido == true && jugando)
        {
            //Si el PhotonView es el mio y somos el jugador 2 y no esta el modo vsIA o somos el jugador 1 (por lo tanto nuestro mando es el 1º)
            if (viewJugador.IsMine)
            {
                if (!vr) { 
                    if (mandoUno == nombreMando)
                    {
                        //Si pulsamos la tecla W y la raqueta no esta ya contra el techo, subimos la raqueta
                        if (Input.GetKey(KeyCode.W) && raqueta.transform.position.y < 16)
                        {
                            raqueta.transform.position = new Vector3(raqueta.transform.position.x, raqueta.transform.position.y + 4f * Time.deltaTime, raqueta.transform.position.z);
                        }
                        //Si pulsamos la tecla S y la requeta no esta contra el suelo, bajamos la raqueta
                        else if (Input.GetKey(KeyCode.S) && raqueta.transform.position.y > 8.6)
                        {
                            raqueta.transform.position = new Vector3(raqueta.transform.position.x, raqueta.transform.position.y - 4f * Time.deltaTime, raqueta.transform.position.z);
                        }
                    }
                    else if (nombreMando != mandoUno && !pong.vsIA)
                    {
                        if (Input.GetKey(KeyCode.W) && raqueta.transform.position.y < 16)
                        {
                            raqueta.transform.position = new Vector3(raqueta.transform.position.x, raqueta.transform.position.y + 4f * Time.deltaTime, raqueta.transform.position.z);
                        }
                        //Si pulsamos la tecla S y la requeta no esta contra el suelo, bajamos la raqueta
                        else if (Input.GetKey(KeyCode.S) && raqueta.transform.position.y > 8.6)
                        {
                            raqueta.transform.position = new Vector3(raqueta.transform.position.x, raqueta.transform.position.y - 4f * Time.deltaTime, raqueta.transform.position.z);
                        }
                    }
                }
                else
                {
                    bool aux = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickValue);
                    if (mandoUno == nombreMando)
                    {
                        
                        if (joystickValue.y > 0 && raqueta.transform.position.y < 16)
                        {
                                raqueta.transform.position = new Vector3(raqueta.transform.position.x, raqueta.transform.position.y + 4f * Time.deltaTime, raqueta.transform.position.z);
                        }
                        else if (joystickValue.y < 0 && raqueta.transform.position.y > 8.6)
                        {
                                raqueta.transform.position = new Vector3(raqueta.transform.position.x, raqueta.transform.position.y - 4f * Time.deltaTime, raqueta.transform.position.z);
                        }
                        
                    }
                    else if (nombreMando != mandoUno && !pong.vsIA)
                    {
                        if (joystickValue.y > 0 && raqueta.transform.position.y < 16)
                        {
                            raqueta.transform.position = new Vector3(raqueta.transform.position.x, raqueta.transform.position.y + 4f * Time.deltaTime, raqueta.transform.position.z);
                        }
                        else if (joystickValue.y < 0 && raqueta.transform.position.y > 8.6)
                        {
                            raqueta.transform.position = new Vector3(raqueta.transform.position.x, raqueta.transform.position.y - 4f * Time.deltaTime, raqueta.transform.position.z);
                        }
                    }
                }
                
            }
            
        }
        
    }

    //Actualiza el estado porque un jugador ha cogido el mando
    public override void ActualizarEstadoEntrada(GameObject mano, bool manoIzq)
    {
        if (viewJugador.IsMine)
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
            if (nombreMando == mandoUno)
            {
                bola.GetComponent<BolaBehaivour>().viewJugadorUno = viewJugador;
                bola.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                pong.vr = vr;

            }
            else
            {
                bola.GetComponent<BolaBehaivour>().viewJugadorDos = viewJugador;
            }
            raqueta.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            
            
            
        }
    }

    //Actualiza el estado porque un jugador ha dejado el mando
    public override void ActualizarEstadoSalida(bool desconectado)
    {
        if (viewJugador != null)
        {
            if (viewJugador.IsMine)
            {
                if (nombreMando == mandoUno)
                {
                    jugando = false;
                    bola.GetComponent<BolaBehaivour>().viewJugadorUno = null;
                    pong.ReiniciarJugando();
                    pong.ReiniciarPantalla();
                }
                else
                {
                    if (pong.vsIA == false)
                    {
                        jugando = false;
                        bola.GetComponent<BolaBehaivour>().viewJugadorDos = null;
                        pong.ReiniciarJugando();
                        pong.ReiniciarPantalla();
                    }
                }
            }
        }
        else if (desconectado)
        {
            if (nombreMando == mandoUno)
            {
                jugando = false;
                bola.GetComponent<BolaBehaivour>().viewJugadorUno = null;
                pong.ReiniciarJugando();
                pong.ReiniciarPantalla();
            }
            else
            {
                if (pong.vsIA == false)
                {
                    jugando = false;
                    bola.GetComponent<BolaBehaivour>().viewJugadorDos = null;
                    pong.ReiniciarJugando();
                    pong.ReiniciarPantalla();
                }
            }
        }
        vr = false;
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
}
