using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class JugadorFPPCBehaivour : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    //Character controller del jugador
    public CharacterController controller;

    //Velocidad horizontal del jugador
    public float speed = 11f;

    //Gameobject de la camara
    GameObject camara;

    //Photonview del jugador
    PhotonView view;

    // Vector para el movimiento horizontal
    Vector2 horizontalInput;

    //Raycast que se encargará de detectar si estamos apuntando a una pantalla, consola y mando
    RaycastHit hit;

    //Variables que usaremos para guardar el mando que cojamos y variables de interes
    public Transform mandoParent =null;
    public Vector3 mandoPos;
    public Quaternion mandoRot;
    public Transform mandoTransform = null;

    //Booleano que indica si estamos jugando algun juego
    public bool jugando;
    public bool escaped;
    void Start()
    {
        //Inicializa los componentes que estan ocultos para que no interferiese con otros jugadores y oculta el cursor
        Cursor.visible = false;
        view = GetComponent<PhotonView>();
        Cursor.lockState = CursorLockMode.Locked;
        camara = transform.Find("Camera").gameObject;

        if (view.IsMine)
        {
            escaped = false;
            jugando = false;
            camara.SetActive(true);
            this.transform.Find("Puntero").gameObject.SetActive(true);
        }
    }

    //Funcion que recibe el input del usuario con el nuevo sistema de input
    public void ReceiveInput (Vector2 _horizontalInput)
    {
        if (view != null)
        {
            if (view.IsMine)
            {
                horizontalInput = _horizontalInput;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (view != null)
        {
            //Si el Photonview es el del jugador
            if (view.IsMine)
            {
                //Si se esta jugando, el jugador no se puede mover
                if (!jugando)
                {
                    Vector3 velocidadHorizontal = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * speed;
                    controller.Move(velocidadHorizontal * Time.deltaTime);
                }

                //Raycast cuyo objetivo es identificar si se esta apuntando a una pantalla, consola o mando. Se cambia el puntero si es necesario.
                if (Physics.Raycast(camara.transform.position, camara.transform.forward, out hit, 4f))
                {
                    if (hit.transform.gameObject.tag == "Pantalla" && !jugando)
                    {
                        this.transform.Find("Puntero").Find("PunteroGrande").gameObject.SetActive(true);
                        this.transform.Find("Puntero").Find("PunteroPequeño").gameObject.SetActive(false);
                        if (Input.GetMouseButtonDown(0)) //Miramos si se ha pulsado el click izquierdo
                        {
                            
                            VideoManager pantalla = hit.transform.gameObject.GetComponent<VideoManager>();
                            if (pantalla.video.isPlaying)
                            {
                                pantalla.StopVideo();
                            }
                            else
                            {
                                pantalla.StartVideo();
                            }
                        }
                    }
                    else if (hit.transform.gameObject.tag == "Consola" && !jugando)
                    {
                        this.transform.Find("Puntero").Find("PunteroGrande").gameObject.SetActive(true);
                        this.transform.Find("Puntero").Find("PunteroPequeño").gameObject.SetActive(false);
                        if (Input.GetMouseButtonDown(0)) //Miramos si se ha pulsado el click izquierdo
                        {
                            InfoManager info = hit.transform.gameObject.GetComponent<InfoManager>();
                            if (hit.transform.Find("GloboInfo").gameObject.activeSelf)
                            {
                                info.HideInfo();
                            }
                            else
                            {
                                info.ShowInfo();
                            }
                        }
                    }
                    //Si el Raycast detecta un mando, a parte de cambiar el puntero debemos mirar si se desea cogerlo.
                    else if (hit.transform.gameObject.tag == "Mando" && !jugando)
                    {
                        this.transform.Find("Puntero").Find("PunteroGrande").gameObject.SetActive(true);
                        this.transform.Find("Puntero").Find("PunteroPequeño").gameObject.SetActive(false);

                        if (Input.GetMouseButtonDown(0)) //Miramos si se ha pulsado el click izquierdo
                        {
                            if (mandoParent == null && !hit.transform.GetComponent<PersonajeBehaivour>().mandoCogido)
                            {
                                //Guardamos la informacion inicial del mando para luego devolverlo a su sitio
                                mandoTransform = hit.transform;
                                mandoParent = hit.transform.parent;
                                
                                //Actualizamos las variables iniciales del juego al que vamos a jugar
                                mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = true;
                                mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = view;
                                mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = mandoTransform.name;
                                mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoEntrada(null,false);

                                //Cogemos el mando y nos colocamos en la posicion
                                jugando = true;
                                mandoPos = mandoTransform.position;
                                mandoRot = mandoTransform.GetComponent<PersonajeBehaivour>().rotacionMando;
                                mandoTransform.parent = this.transform;
                                mandoTransform.position = this.transform.Find("MandoPosition").position;
                                this.transform.position = mandoTransform.GetComponent<PersonajeBehaivour>().posicionJugador;
                                this.transform.rotation = mandoTransform.GetComponent<PersonajeBehaivour>().rotacionJugador;
                                this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);
                                mandoTransform.localScale = new Vector3(mandoTransform.localScale.x / 2, mandoTransform.localScale.y / 2, mandoTransform.localScale.z / 2);
                                mandoTransform.rotation = mandoRot;

                                //Ocultamos el puntero
                                this.transform.Find("Puntero").Find("PunteroGrande").gameObject.SetActive(false);
                                this.transform.Find("Puntero").Find("PunteroPequeño").gameObject.SetActive(false);
                                view.RPC("ActualizarEntradaMando", RpcTarget.OthersBuffered, mandoPos,mandoRot, mandoTransform.name, mandoParent.name);
                            }
                        }
                    }
                    else if (!jugando)
                    {
                        this.transform.Find("Puntero").Find("PunteroPequeño").gameObject.SetActive(true);
                        this.transform.Find("Puntero").Find("PunteroGrande").gameObject.SetActive(false);
                    }
                }
                else if (!jugando)
                {
                    this.transform.Find("Puntero").Find("PunteroPequeño").gameObject.SetActive(true);
                    this.transform.Find("Puntero").Find("PunteroGrande").gameObject.SetActive(false);
                }
                //Si pulsamos Q y tenemos un mando cogido, lo devolvemos a su lugar correspondiente
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (mandoParent != null)
                    {
                        mandoTransform.position = mandoPos;
                        mandoTransform.parent = mandoParent;

                        mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = false;
                        mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoSalida(false);
                        mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = null;
                        mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = null;
                        mandoTransform.localScale = new Vector3(mandoTransform.localScale.x * 2, mandoTransform.localScale.y * 2, mandoTransform.localScale.z * 2);
                        mandoTransform.rotation = mandoRot;
                        jugando = false;
                        mandoParent = null;
                        mandoTransform = null;
                        
                        //Reactivamos el puntero
                        this.transform.Find("Puntero").Find("PunteroPequeño").gameObject.SetActive(true);
                        this.transform.Find("Puntero").Find("PunteroGrande").gameObject.SetActive(false);

                        view.RPC("ActualizarSalidaMando", RpcTarget.OthersBuffered);
                    }
                }
                if (escaped==false && Input.GetKeyDown(KeyCode.Escape))
                {
                    escaped = true;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else if(escaped == true && Input.GetKeyDown(KeyCode.Escape))
                {
                    escaped = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
    }

    public void Destruir()
    {
        Debug.Log("Me voy");
        PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
        ExitGames.Client.Photon.Hashtable vrProperty = new ExitGames.Client.Photon.Hashtable() { { "vr", false} };
        info.Sender.SetCustomProperties(vrProperty);
    }

    [PunRPC]
    void ActualizarEntradaMando(Vector3 mandoPos,Quaternion mandoRot, string mandoTransform, string mandoParent)
    {
        this.mandoPos = mandoPos;
        this.mandoRot = mandoRot;
        this.mandoParent = GameObject.Find(mandoParent).transform;
        this.mandoTransform = GameObject.Find(mandoTransform).transform;
        this.mandoTransform.parent = this.transform;

        this.mandoTransform.localScale = new Vector3(this.mandoTransform.localScale.x / 2, this.mandoTransform.localScale.y / 2, this.mandoTransform.localScale.z / 2);
        this.mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = this.mandoTransform.name;
        this.mandoTransform.position = this.transform.Find("MandoPosition").position;
        this.mandoTransform.rotation = mandoRot;
        this.mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = true;
        this.mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = this.view;
        this.mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoEntrada(null,false);
    }
    [PunRPC]
    void ActualizarSalidaMando()
    {
        this.mandoTransform.parent = mandoParent;
        this.mandoTransform.position = mandoPos;
        this.mandoTransform.rotation = mandoRot;
        this.mandoTransform.localScale = new Vector3(mandoTransform.localScale.x * 2, mandoTransform.localScale.y * 2, mandoTransform.localScale.z * 2);
        this.mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = false;
        mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoSalida(false);
        this.mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = null;
        this.mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = null;
        this.mandoParent = null;
        this.mandoTransform = null;
    }

}
