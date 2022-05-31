using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BolaBehaivour : MonoBehaviour
{
    public PhotonView view; //PhotonView de la bola, se lo daremos al jugador uno
    public PhotonView viewJugadorUno; //PhotonView del jugador uno, quien se encarga de actualizar la bola
    public PhotonView viewJugadorDos;
    //Floats relacionados con la velocidad de la bola
    public float velocidadX = 1;
    public float velocidadY = 1;
    public float[,] velocidadesDer = new float[, ] { { 1f, 1f }, { 1.5f, 1.0f }, { 1f, -1f }, { 1.5f, -1.0f }, { 1, 0} };
    public float[,] velocidadesIzq = new float[,] { { -1f, -1f }, { -1.5f, -1.0f }, { -1f, 1f }, { -1.5f, 1.0f }, { -1, 0 } };

    //Booleano que indica si el juego de Pong esta en marcha
    public bool jugando;
    public PongBehaivour pong;

    //Posiciones donde reaparece la bola segun si ha marcado en la derecha o la izquierda
    public Transform inicioIzq;
    public Transform inicioDer;

    //Timer para cuando la bola tiene que reaparecer
    public float tiempoReaparecer;
    public float tiempoReaparecerTimer;

    public bool cambioView = false; //Cambia el poseedor del PhotonView de la bola

    public AudioSource audioSource;
    public AudioClip[] clips;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Si el juego esta en marcha
        if (jugando)
        {
            if (tiempoReaparecerTimer > 0) //Animacion reaparicion bola
            {
                if (tiempoReaparecerTimer < 2.5f && tiempoReaparecerTimer > 2f)
                {
                    this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                else if (tiempoReaparecerTimer < 2f && tiempoReaparecerTimer > 1.5f)
                {
                    this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                else if (tiempoReaparecerTimer < 1.5f && tiempoReaparecerTimer > 1f)
                {
                    this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                else if (tiempoReaparecerTimer < 1f && tiempoReaparecerTimer > 0.5f)
                {
                    this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                else if (tiempoReaparecerTimer < 0.5f && tiempoReaparecerTimer > 0.2f)
                {
                    this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                tiempoReaparecerTimer =tiempoReaparecerTimer- 0.9f*Time.deltaTime;

            }
            //Movemos la bola si no tiene que reaparecer, segun su velocidad
            else if (view.IsMine)
            {
                this.transform.position = new Vector3(this.transform.position.x + 5f * velocidadX * Time.deltaTime,
                    this.transform.position.y + 5f * velocidadY * Time.deltaTime, this.transform.position.z);
 
            }
        }
    }

    //Funcion que detecta si el objeto ha colisionado con algun otro objeto
    private void OnCollisionEnter(Collision collision)
    {
        if (viewJugadorUno != null)
        {
            if (viewJugadorUno.IsMine)
            {
                //Si ha colisionado con el jugador1, la bola rebota hacia la derecha
                if (collision.transform.parent.name == "Jugador1")
                {
                    int random = Random.Range(0, 4);
                    velocidadX = velocidadesDer[random, 0];
                    velocidadY = velocidadesDer[random, 1];
                    cambioView = true;
                    audioSource.clip = clips[0];
                    audioSource.Play();
                    view.RPC("PlaySonido", RpcTarget.Others, 0);
                    view.RPC("ActualizarCambioView", RpcTarget.OthersBuffered, true);
                }
                //Si ha colisionado con el jugador2, la bola rebota hacia la izquierda
                else if (collision.transform.parent.name == "Jugador2")
                {
                    int random = Random.Range(0, 4);
                    velocidadX = velocidadesIzq[random, 0];
                    velocidadY = velocidadesIzq[random, 1];
                    view.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                    audioSource.clip = clips[0];
                    audioSource.Play();
                    view.RPC("PlaySonido", RpcTarget.Others, 0);
                    view.RPC("ActualizarCambioView", RpcTarget.OthersBuffered, false);
                }
                //Si ha colisionado con la pared derecha, reaparece en la derecha 
                else if (collision.transform.name == "ParedDer")
                {
                    pong.AcutalizarPuntuacion(false);
                    ReiniciarBola(false);
                    velocidadX = -1;
                    view.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                    audioSource.clip = clips[1];
                    audioSource.Play();
                    view.RPC("PlaySonido", RpcTarget.Others, 1);
                    view.RPC("ActualizarCambioView", RpcTarget.OthersBuffered, false);
                }
                //Si ha colisionado con la pared izquierda, reaparece en la izquierda
                else if (collision.transform.name == "ParedIzq")
                {
                    velocidadX = -1;
                    pong.AcutalizarPuntuacion(true);
                    ReiniciarBola(true);
                    cambioView = true;
                    audioSource.clip = clips[1];
                    audioSource.Play();
                    view.RPC("PlaySonido", RpcTarget.Others, 1);
                    view.RPC("ActualizarCambioView", RpcTarget.OthersBuffered, true);
                }
                //Si colisiona con cualquier otra cosa, es decir, con el techo o con el suelo, invierte la velocidad en Y
                else
                {
                    velocidadY = velocidadY * -1;
                    audioSource.clip = clips[2];
                    audioSource.Play();
                    view.RPC("PlaySonido", RpcTarget.Others, 2);
                }
            }
        }
        
        if (viewJugadorDos != null)
        {
            if (cambioView && viewJugadorDos.IsMine)
            {
                view.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                cambioView = false;
                view.RPC("ActualizarCambioView", RpcTarget.OthersBuffered, false);
            }
        }
        
    }
    //Reinicia la posicion de la bola
    public void ReiniciarBola(bool izq)
    {
        if (izq)
        {
            int random = Random.Range(0, 4);
            velocidadX = velocidadesDer[random, 0];
            velocidadY = velocidadesDer[random, 1];
            this.transform.position = inicioIzq.position;
        }
        else
        {
            int random = Random.Range(0, 4);
            velocidadX = velocidadesIzq[random, 0];
            velocidadY = velocidadesIzq[random, 1]; 
            this.transform.position = inicioDer.position;
        }
        //Inicia el timer para la animacion de reaparecer
        tiempoReaparecerTimer = tiempoReaparecer;
    }

    public void QuitarJugando()
    {
        this.jugando = false;
        view.RPC("ActualizarJugando", RpcTarget.OthersBuffered, false);
    }
    [PunRPC]
    void ActualizarJugando(bool jugando)
    {
        this.jugando = jugando;
    }
    [PunRPC]
    void ActualizarAnimacion(bool anim)
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = anim;
    }
    [PunRPC]
    void ActualizarCambioView(bool cambioView)
    {
        this.cambioView = cambioView;
    }
    [PunRPC]
    void PlaySonido(int clip)
    {
        audioSource.clip = clips[clip];
        audioSource.Play();
    }
}
