using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using TMPro;

public class BolosBehaviour : MonoBehaviour
{
    //Reglas de los bolos: Cada juego constará de 5 rondas. Los jugadores tendrán dos oportunidades de tirar en cada ronda.
    //Si un jugador tira todos los bolos en su primer intento, obtendrá un pleno, lo cual le proporcionará 10 puntos extras a los bolos tirados.
    //Si un jugador tira todos los bolos en su segundo intento, obtendrá un semipleno, lo cual le proporcionará 5 puntos extras a los bolos tirados.
    //Si el jugador no tira todos los bolos, su puntuacion esa ronda será el numero de bolos que ha tirado.
    //Cuando pasen las 5 rondas, se calculará la puntuación total, y ganará el jugador con mas puntuación.

    //Booleano que indica si el primer jugador es vr o no
    public bool vr = false;

    //PhotonView del gameobject
    public PhotonView view;

    //Gameobject que contiene el canvas de selección de juego
    public GameObject seleccionModo;

    //Gameobject que contiene el canvas de la tabla de puntuaciones
    public GameObject tablaPuntuacion;

    //Gameobject que contiene el canvas de las ventanas de espera a un jugador
    public GameObject ventanaEsperaLanzamiento;
    public GameObject ventanaEsperaPresentacion;

    public bool esperandoUno;
    public bool esperandoDos;

    //Camaras que utilizamos para visualizar la presentacion del jugador y el lanzamiento
    public Camera camaraLanzamiento;
    public Camera camaraPresentacion;

    //RenderTexture de la pantalla que se muestra en la wii
    public RenderTexture texturaTele;
    public RenderTexture nullTexture;

    //Timer para la presentacion del jugador
    public float timerCounterPresentacion;
    public float timerPresentacion;

    //Timer para el final del juego
    public float timerCounterFinal;
    public float timerFinal;

    //Booleano auxiliar para saber si ya hemos cambiado la camara despues del timer
    private bool camaraCambiada = true;

    //Punteros a los scripts de los mandos
    public MandoBolosBehaviour mandoUno;
    public MandoBolosBehaviour mandoDos;

    //Gameobject de los dos personajes del juego
    public GameObject personajeUno;
    public GameObject personajeDos;

    //Puntero al script que controla la bola
    public BolaBolosBehaivour bola;

    //Gameobject prefab del conjunto de pinos
    public GameObject conjuntoPinos;

    //Posiciones donde se colocan el lanzador y el que espera
    public Vector3 posicionJugadorLanzar;
    public Vector3 posicionJugadorEspera;

    //Posicion de los bolos
    public Vector3 posicionBolos;

    //Posicion de la bola cuando el juego esta inactivo
    public Vector3 posicionBola;

    //Puntuación actual del jugador que tiene el turno
    public int puntuacionActual=0;

    //Booleano que indica si el jugador ha vuelto o no a tirar en su turno
    public bool reintento = false;

    //Booleano que indica si la partida ha sido iniciada y en que modo se ha iniciado (solo==un jugador)
    private bool jugando = false;
    public bool solo = true;

    //Entero que indica el turno del jugador, siendo el turno 1 el del primer jugador y el turno 2 el del segundo
    private int turno = 1;

    //Enteros que indican cuantos turnos han pasado dentro de la ronda y cuantos turnos tienen que pasar en una ronda
    private int turnosPasados = 0;
    private const int turnosRonda = 2;

    //Enteros que indican cuantas rondas han pasado y cuantas rondas hay en total en el juego
    private int rondaActual = 0;
    private const int rondas = 5;

    //Booleano para saber si el jugador ha vuelto a tirar. Esto nos ayudará a la hora de puntuar el pleno y semipleno
    private bool volverATirar = false;

    //Array de las puntuaciones de los jugadores
    private int[] puntuacionUno = new int[5];
    private int[] puntuacionDos = new int[5];

    //Punteros a los Gameobjects de la tabla de puntuaciones
    public GameObject puntuacionTablaUno;
    public GameObject puntuacionTablaDos;

    public GameObject textoGanador;

    public CongelarPinos congelarPinos;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (5 > timerCounterPresentacion && timerCounterPresentacion > 0 && jugando)
        {
            timerCounterPresentacion -= Time.deltaTime;
            seleccionModo.SetActive(false);
            if (mandoUno != null && mandoUno.viewJugador != null && mandoUno.viewJugador.IsMine)
            {
                view.RPC("RPCSeleccionarModo", RpcTarget.OthersBuffered, false);
            }
            bola.transform.localPosition = new Vector3(0, -0.5f, 0);
            bola.transform.localScale = new Vector3(2 / 3f, 2 / 3f, 2 / 3f);
        }
        else if (!camaraCambiada && jugando)
        {
            if (!solo)
            {
                if (turno == 1)
                {
                    PersonajeSemitransparente(personajeUno);
                    PersonajeVisible(personajeDos);
                    mandoUno.jugando = true;
                    mandoDos.jugando = false;
                    view.RPC("ActualizarJugandoJugadores", RpcTarget.OthersBuffered, true, false);
                }
                else
                {
                    PersonajeSemitransparente(personajeDos);
                    PersonajeVisible(personajeUno);
                    mandoUno.jugando = false;
                    mandoDos.jugando = true;
                    view.RPC("ActualizarJugandoJugadores", RpcTarget.OthersBuffered, false,true);
                }
            }
            else
            {
                mandoUno.jugando = true;
            }
            
            CambiarCamara(false);
            tablaPuntuacion.SetActive(false);
            if (mandoUno != null && mandoUno.viewJugador != null && mandoUno.viewJugador.IsMine)
            {
                view.RPC("RPCTablaPuntuaciones", RpcTarget.OthersBuffered, false);
            }
            camaraCambiada = true;
            bola.transform.localPosition = new Vector3(0, -0.5f, 0);
            bola.transform.localScale = new Vector3(2 / 3f, 2 / 3f, 2 / 3f);
        }
        if (5> timerCounterFinal && timerCounterFinal > 0)
        {
            timerCounterFinal -= Time.deltaTime;
        }
        else if (jugando && timerCounterFinal <= 0)
        {
            FinalizarJuego();
        }
        if (mandoUno!=null && mandoUno.viewJugador != null && !jugando)
        {
            
            if (mandoUno.viewJugador.IsMine)
            {
                
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickValue);
                InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressedDer);
                InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressedIzq);
                //Si se pulsa la tecla A, se marca la opción de un jugador
                if (Input.GetKey(KeyCode.A) || (joystickValue.x < 0))
                {
                    
                    seleccionModo.transform.Find("Panel").Find("SeleccionIzq").gameObject.SetActive(true);
                    seleccionModo.transform.Find("Panel").Find("SeleccionDer").gameObject.SetActive(false);
                    view.RPC("RPCActualizarSelec", RpcTarget.OthersBuffered, true);
                    solo = true;
                    view.RPC("ActualizarSolo", RpcTarget.OthersBuffered, true);
                }
                //Si se pulsa la tecla D, se marca la opción de dos jugadores
                else if (Input.GetKey(KeyCode.D) || (joystickValue.x > 0))
                {
                    
                    seleccionModo.transform.Find("Panel").Find("SeleccionIzq").gameObject.SetActive(false);
                    seleccionModo.transform.Find("Panel").Find("SeleccionDer").gameObject.SetActive(true);
                    view.RPC("RPCActualizarSelec", RpcTarget.OthersBuffered, false);
                    solo = false;
                    view.RPC("ActualizarSolo", RpcTarget.OthersBuffered, false);
                }
                //Si se pulsa la tecla E, iniciamos el juego
                else if (Input.GetKey(KeyCode.E) || (buttonPressedIzq || buttonPressedDer))
                {
                    
                    //Si el jugador dos esta listo y no estamos en el modo un jugador, o si estamos en el modo un jugador, empezamos el juego
                    if (solo)
                    {
                        jugando = true;
                        seleccionModo.SetActive(false);
                        view.RPC("RPCSeleccionarModo", RpcTarget.OthersBuffered,false);
                        MoverPersonajes();
                        camaraCambiada = false;
                        CambiarCamara(true);
                        tablaPuntuacion.SetActive(true);
                        view.RPC("RPCTablaPuntuaciones", RpcTarget.OthersBuffered, true);
                        timerCounterPresentacion = timerPresentacion;
                        bola.transform.localPosition = new Vector3(0, -0.5f, 0);
                        bola.transform.localScale = new Vector3(2 / 3f, 2 / 3f, 2 / 3f);
                        Destroy(GameObject.Find("ConjuntoPinos"));
                        Destroy(GameObject.Find("ConjuntoPinos(Clone)"));
                        view.RPC("BorrarPinos", RpcTarget.OthersBuffered);
                        GameObject pinos = PhotonView.Instantiate(conjuntoPinos, posicionBolos, Quaternion.identity);
                        pinos.transform.parent = this.transform;
                        congelarPinos.quietos = true;
                    }
                    else if (mandoDos!=null && mandoDos.mandoCogido && !solo)
                    {
                        jugando = true;
                        seleccionModo.SetActive(false);
                        view.RPC("RPCSeleccionarModo", RpcTarget.OthersBuffered,false);
                        MoverPersonajes();
                        camaraCambiada = false;
                        CambiarCamara(true);
                        tablaPuntuacion.SetActive(true);
                        view.RPC("RPCTablaPuntuaciones", RpcTarget.OthersBuffered, true);
                        timerCounterPresentacion = timerPresentacion;
                        Destroy(GameObject.Find("ConjuntoPinos"));
                        Destroy(GameObject.Find("ConjuntoPinos(Clone)"));
                        view.RPC("BorrarPinos", RpcTarget.OthersBuffered);
                        GameObject pinos = Instantiate(conjuntoPinos, posicionBolos, Quaternion.identity);
                        pinos.transform.parent = this.transform;
                        congelarPinos.quietos = true;
                    }
                    mandoUno.vrPressed = false;
                    mandoDos.vrPressed = false;
                    textoGanador.SetActive(false);
                    view.RPC("RPCTextoGanador", RpcTarget.OthersBuffered, false, "x");
                }
            }
        }
        if ((esperandoUno || esperandoDos) && mandoUno.mandoCogido && mandoDos.mandoCogido)
        {
            Debug.Log("Cerrando ventana porque los dos mandos estan conectados");
            esperandoUno = false;
            esperandoDos = false;
            view.RPC("ActualizarEsperando", RpcTarget.OthersBuffered,false,false);
            OcultarVentanaEspera();
            
            if (turno == 1)
            {
                mandoUno.jugando = true;
            }
            else
            {
                mandoDos.jugando = true;
            }
        }
    }


    private void PasarTurno()
    {
        if (mandoUno != null && mandoUno.viewJugador != null && mandoUno.viewJugador.IsMine)
        {
            Debug.Log("Pasando turno");
            if (!solo)
            {
                GameObject personajeVisible;
                GameObject personajeSemitransparente;
                if (turno == 1)
                {
                    mandoUno.tirado = false;
                    puntuacionUno[rondaActual] = puntuacionActual;
                    puntuacionTablaUno.transform.GetChild(rondaActual).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacionActual.ToString();
                    view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 2, puntuacionActual.ToString(), true);
                    turno = 2;
                    view.RPC("ActualizarTurno", RpcTarget.OthersBuffered, 2);
                    personajeVisible = personajeDos;
                    personajeSemitransparente = personajeUno;
                    PersonajeVisible(personajeVisible);
                    PersonajeSemitransparente(personajeSemitransparente);
                }
                else
                {
                    mandoDos.tirado = false;
                    puntuacionDos[rondaActual] = puntuacionActual;
                    puntuacionTablaDos.transform.GetChild(rondaActual).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacionActual.ToString();
                    view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 2, puntuacionActual.ToString(), false);
                    turno = 1;
                    view.RPC("ActualizarTurno", RpcTarget.OthersBuffered, 1);
                    personajeVisible = personajeUno;
                    personajeSemitransparente = personajeDos;
                    PersonajeVisible(personajeVisible);
                    PersonajeSemitransparente(personajeSemitransparente);
                }
                MoverPersonajes();
                turnosPasados++;
                if (turnosPasados >= turnosRonda)
                {
                    rondaActual++;
                    turnosPasados = 0;
                }
            }
            else
            {
                Debug.Log("Mover Personajes");
                MoverPersonajes();
                puntuacionTablaUno.transform.GetChild(rondaActual).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacionActual.ToString();
                view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 2, puntuacionActual.ToString(), true);
                puntuacionUno[rondaActual] = puntuacionActual;
                rondaActual++;

            }
            if (rondaActual >= rondas)
            {
                puntuacionTablaUno.transform.GetChild(rondaActual).gameObject.GetComponent<TextMeshProUGUI>().text = CalcularPuntuacionTotal(puntuacionUno).ToString();
                puntuacionTablaDos.transform.GetChild(rondaActual).gameObject.GetComponent<TextMeshProUGUI>().text = CalcularPuntuacionTotal(puntuacionDos).ToString();
                timerCounterFinal = timerFinal;
                tablaPuntuacion.SetActive(true);
                textoGanador.SetActive(true);
                if (CalcularPuntuacionTotal(puntuacionUno) > CalcularPuntuacionTotal(puntuacionDos))
                {
                    textoGanador.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "El jugador 1 ha ganado";
                    view.RPC("RPCTextoGanador", RpcTarget.OthersBuffered, true,"El jugador 1 ha ganado");
                }
                else if (CalcularPuntuacionTotal(puntuacionUno) == CalcularPuntuacionTotal(puntuacionDos))
                {
                    textoGanador.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Es un empate";
                    view.RPC("RPCTextoGanador", RpcTarget.OthersBuffered, true,"Es un empate");
                }
                else
                {
                    textoGanador.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "El jugador 2 ha ganado";
                    view.RPC("RPCTextoGanador", RpcTarget.OthersBuffered, true,"El jugador 2 ha ganado");
                }
                
                view.RPC("RPCTablaPuntuaciones", RpcTarget.OthersBuffered, true);
                view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 3, CalcularPuntuacionTotal(puntuacionUno).ToString(), true);
                view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 3, CalcularPuntuacionTotal(puntuacionDos).ToString(), false);
            }
            congelarPinos.quietos = true;
            puntuacionActual = 0;
            camaraCambiada = false;
            volverATirar = false;


            CambiarCamara(true);
            tablaPuntuacion.SetActive(true);
            view.RPC("RPCTablaPuntuaciones", RpcTarget.OthersBuffered, true);
            timerCounterPresentacion = timerPresentacion;
            Destroy(GameObject.Find("ConjuntoPinos"));
            Destroy(GameObject.Find("ConjuntoPinos(Clone)"));
            view.RPC("BorrarPinos", RpcTarget.OthersBuffered);
            GameObject pinos = Instantiate(conjuntoPinos, posicionBolos, Quaternion.identity);
            pinos.transform.parent = this.transform;
            congelarPinos.quietos = true;
            bola.transform.localPosition = new Vector3(0, -0.5f, 0);
            bola.transform.localScale = new Vector3(2 / 3f, 2 / 3f, 2 / 3f);
        }
    }
    public void BolaTrigger()
    {

            if (mandoUno != null && mandoUno.viewJugador != null && mandoUno.viewJugador.IsMine)
            {
                Debug.Log("Calculando puntuacion");
                if (turno == 1)
                {
                    mandoUno.vrPressed = false;
                    personajeUno.transform.GetChild(2).localPosition = new Vector3(0.7f, 0, 0);
                    personajeUno.transform.GetChild(3).localPosition = new Vector3(-0.7f, 0, 0);
                }
                else
                {
                    mandoDos.vrPressed = false;
                    personajeDos.transform.GetChild(2).localPosition = new Vector3(0.7f, 0, 0);
                    personajeDos.transform.GetChild(3).localPosition = new Vector3(-0.7f, 0, 0);
                }
                if (puntuacionActual < 10 && !volverATirar)
                {
                    if (turno == 1)
                    {
                        puntuacionTablaUno.transform.GetChild(rondaActual).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacionActual.ToString();
                        view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 0, puntuacionActual.ToString(), true);
                    }
                    else
                    {
                        puntuacionTablaDos.transform.GetChild(rondaActual).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacionActual.ToString();
                        view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 0, puntuacionActual.ToString(), false);
                    }
                    volverATirar = true;
                    VolverATirar();
                }
                else if (!volverATirar)
                {
                    if (turno == 1)
                    {
                        puntuacionTablaUno.transform.GetChild(rondaActual).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "P";
                        view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 0, "P", true);
                    }
                    else
                    {
                        puntuacionTablaDos.transform.GetChild(rondaActual).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "P";
                        view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 0, "P", false);
                    }
                    puntuacionActual = 20;
                    PasarTurno();
                }
                else if (puntuacionActual >= 10)
                {
                    if (turno == 1)
                    {
                        puntuacionTablaUno.transform.GetChild(rondaActual).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "SP";
                        view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 1, "SP", true);
                    }
                    else
                    {
                        puntuacionTablaDos.transform.GetChild(rondaActual).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "SP";
                        view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 1, "SP", false);
                    }
                    puntuacionActual = 15;
                    PasarTurno();
                }
                else
                {
                    if (turno == 1)
                    {
                        puntuacionTablaUno.transform.GetChild(rondaActual).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacionActual.ToString();
                        view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 1, puntuacionActual.ToString(), true);
                    }
                    else
                    {
                        puntuacionTablaDos.transform.GetChild(rondaActual).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacionActual.ToString();
                        view.RPC("RPCActualizarTabla", RpcTarget.OthersBuffered, rondaActual, 1, puntuacionActual.ToString(), false);
                    }
                    PasarTurno();
                }
             }
        
        
    }
    private void CambiarCamara(bool presentacion)
    {
        if (mandoUno!= null && mandoUno.viewJugador != null && mandoUno.viewJugador.IsMine)
        {
            view.RPC("RPCCambiarCamara", RpcTarget.OthersBuffered, presentacion);
        }
            if (presentacion)
            {
                Debug.Log("He cambiado la camara");
                camaraPresentacion.targetTexture = texturaTele;
                camaraLanzamiento.targetTexture = nullTexture;
                camaraPresentacion.gameObject.SetActive(true);
                camaraLanzamiento.gameObject.SetActive(false);
                PersonajeVisible(personajeUno);
                PersonajeVisible(personajeDos);
            }
            else
            {
                Debug.Log("He cambiado la camara");
                camaraLanzamiento.targetTexture = texturaTele;
                camaraPresentacion.targetTexture = nullTexture;
                camaraPresentacion.gameObject.SetActive(false);
                camaraLanzamiento.gameObject.SetActive(true);
                if (turno == 1)
                {
                    PersonajeVisible(personajeDos);
                    PersonajeSemitransparente(personajeUno);
                }
                else
                {
                    PersonajeVisible(personajeUno);
                    PersonajeSemitransparente(personajeDos);
                }
            }
        
    }
    private void MoverPersonajes()
    {
        if (mandoUno != null && mandoUno.viewJugador!=null && mandoUno.viewJugador.IsMine)
        {
            view.RPC("RPCMoverPersonaje", RpcTarget.OthersBuffered);
            if (turno == 1)
            {
                personajeUno.transform.position = posicionJugadorLanzar;
                personajeDos.transform.position = posicionJugadorEspera;
                mandoUno.CogerBola();
                view.RPC("CogerBola", RpcTarget.OthersBuffered, true);
                mandoUno.jugando = false;
                mandoUno.tirado = false;

            }
            else
            {
                personajeDos.transform.position = posicionJugadorLanzar;
                personajeUno.transform.position = posicionJugadorEspera;
                mandoDos.CogerBola();
                view.RPC("CogerBola", RpcTarget.OthersBuffered, false);
                mandoDos.jugando = false;
                mandoDos.tirado = false;
            }
            bola.transform.localPosition = new Vector3(0, -0.5f, 0);
            bola.transform.localScale = new Vector3(2 / 3f, 2 / 3f, 2 / 3f);
        }
    }

    private void VolverATirar()
    {
        Debug.Log("Volviendo a tirar 1");
        if (mandoUno != null && mandoUno.viewJugador!=null && mandoUno.viewJugador.IsMine)
        {
            Debug.Log("Volviendo a tirar 2");
            congelarPinos.quietos = true;
            if (turno == 1)
            {
                Debug.Log("Volviendo a tirar 3");
                mandoUno.CogerBola();
                view.RPC("CogerBola", RpcTarget.OthersBuffered, true);
                mandoUno.tirado = false;
            }
            else
            {
                Debug.Log("Volviendo a tirar 3");
                mandoDos.CogerBola();
                view.RPC("CogerBola", RpcTarget.OthersBuffered, false);
                mandoDos.tirado = false;
            }
            bola.transform.localPosition = new Vector3(0, -0.5f, 0);
            bola.transform.localScale = new Vector3(2 / 3f, 2 / 3f, 2 / 3f);
        }
    }
    private void PersonajeSemitransparente(GameObject objeto)
    {
        for (int i = 0; i < objeto.transform.childCount; i++) {
            GameObject child= objeto.transform.GetChild(i).gameObject;
            if (child.GetComponent<MeshRenderer>() != null && child.name!="Flecha")
            {
                PersonajeSemitransparente(child);
                Material childMaterial = child.GetComponent<MeshRenderer>().material;
                childMaterial.SetColor("_Color", new Color(childMaterial.color.r, childMaterial.color.g, childMaterial.color.b, 0.3f));
            }
            
        }
        if (turno == 1)
        {
            mandoUno.GetComponent<MandoBolosBehaviour>().ActualizarFlecha(true);
        }
        else
        {
            mandoDos.GetComponent<MandoBolosBehaviour>().ActualizarFlecha(true);
        }
    }
    private void PersonajeVisible(GameObject objeto)
    {
        for (int i = 0; i < objeto.transform.childCount; i++)
        {
            
            GameObject child = objeto.transform.GetChild(i).gameObject;
            if (child.GetComponent<MeshRenderer>() != null)
            {
                PersonajeVisible(child);
                Material childMaterial = child.GetComponent<MeshRenderer>().material;
                childMaterial.SetColor("_Color", new Color(childMaterial.color.r, childMaterial.color.g, childMaterial.color.b, 1f));
            }
        }
        if (turno == 1)
        {
            mandoUno.GetComponent<MandoBolosBehaviour>().ActualizarFlecha(false);
        }
        else
        {
            mandoDos.GetComponent<MandoBolosBehaviour>().ActualizarFlecha(false);
        }
        
    }

    public void FinalizarJuego()
    {
        
        Debug.Log("Finalizando Juego");
        ResetVariables();
        VaciarTabla();
        OcultarVentanaEspera();
        seleccionModo.SetActive(true);
        tablaPuntuacion.SetActive(false);
        textoGanador.SetActive(false);
        camaraLanzamiento.targetTexture = texturaTele;
        camaraPresentacion.targetTexture = nullTexture;
        camaraPresentacion.gameObject.SetActive(false);
        camaraLanzamiento.gameObject.SetActive(true);

        seleccionModo.transform.Find("Panel").Find("SeleccionIzq").gameObject.SetActive(true);
        seleccionModo.transform.Find("Panel").Find("SeleccionDer").gameObject.SetActive(false);
        view.RPC("RPCActualizarSelec", RpcTarget.OthersBuffered, true);

        bola.transform.position = posicionBola;
        bola.GetComponent<BolaBolosBehaivour>().PararBola();
        
        Destroy(GameObject.Find("ConjuntoPinos"));
        Destroy(GameObject.Find("ConjuntoPinos(Clone)"));
        GameObject pinos = PhotonView.Instantiate(conjuntoPinos, posicionBolos, Quaternion.identity);
        pinos.transform.parent = this.transform;
        congelarPinos.quietos = true;
        view.RPC("RPCFinalizarJuego", RpcTarget.OthersBuffered);
    }

    private void ResetVariables()
    {
        puntuacionActual = 0;

        reintento = false;

        jugando = false;
        solo = true;

        turno = 1;

        turnosPasados = 0;

        rondaActual = 0;

        volverATirar = false;

        puntuacionUno = new int[5];
        puntuacionDos = new int[5];

        mandoUno.vrPressed = false;
        mandoDos.vrPressed = false;

        personajeUno.transform.GetChild(2).localPosition = new Vector3(0.7f, 0, 0);
        personajeUno.transform.GetChild(3).localPosition = new Vector3(-0.7f, 0, 0);

        personajeDos.transform.GetChild(2).localPosition = new Vector3(0.7f, 0, 0);
        personajeDos.transform.GetChild(3).localPosition = new Vector3(-0.7f, 0, 0);

        timerCounterFinal = 5;
        timerCounterPresentacion = 5;
    }

    private void VaciarTabla()
    {
        for(int i = 0; i < rondas; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                puntuacionTablaUno.transform.GetChild(i).GetChild(j).gameObject.GetComponent<TextMeshProUGUI>().text = "";
                puntuacionTablaDos.transform.GetChild(i).GetChild(j).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            }
            
        }
        puntuacionTablaUno.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>().text = "";
        puntuacionTablaDos.transform.GetChild(5).gameObject.GetComponent<TextMeshProUGUI>().text = "";
    }

    private int CalcularPuntuacionTotal(int[] puntuacion)
    {
        int aux=0;

        for(int i = 0; i < puntuacion.Length; i++)
        {
            aux += puntuacion[i];
        }

        return aux;
    }

    public void VentanaEspera(bool jugadorUno)
    {
        Debug.Log("Abriendo ventana espera original");
        mandoUno.jugando = false;
        mandoDos.jugando = false;
        ventanaEsperaLanzamiento.SetActive(true);
        ventanaEsperaPresentacion.SetActive(true);
        if (jugadorUno)
        {
            ventanaEsperaLanzamiento.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                "Esperando a que el jugador uno se reconecte...";
            ventanaEsperaPresentacion.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                "Esperando a que el jugador uno se reconecte...";
        }
        else
        {
            ventanaEsperaLanzamiento.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                "Esperando a que el jugador dos se reconecte...";
            ventanaEsperaPresentacion.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                "Esperando a que el jugador dos se reconecte...";
        }
        view.RPC("RPCVentanaEspera", RpcTarget.OthersBuffered, jugadorUno);

    }
    public void OcultarVentanaEspera()
    {
        if (turno == 1)
        {
            mandoUno.jugando = true;
            mandoDos.jugando = false;
        }
        else
        {
            mandoUno.jugando = false;
            mandoDos.jugando = true;
        }
        
        ventanaEsperaLanzamiento.SetActive(false);
        ventanaEsperaPresentacion.SetActive(false);
        view.RPC("RPCOcultarVentanaEspera", RpcTarget.OthersBuffered);
    }
    //Actualiza el nombre del mando y el booleano mandoCogido
    [PunRPC]
    void RPCActualizarSelec(bool selecIzq)
    {
        if (selecIzq)
        {
            seleccionModo.transform.Find("Panel").Find("SeleccionIzq").gameObject.SetActive(true);
            seleccionModo.transform.Find("Panel").Find("SeleccionDer").gameObject.SetActive(false);
            solo = true;
        }
        else
        {
            seleccionModo.transform.Find("Panel").Find("SeleccionIzq").gameObject.SetActive(false);
            seleccionModo.transform.Find("Panel").Find("SeleccionDer").gameObject.SetActive(true);
            solo = false;
        }
    }

    [PunRPC]
    void RPCActualizarTabla(int ronda,int tiro, string puntuacion,bool jugadorUno)
    {
        if (tiro == 3)
        {
            if (jugadorUno)
            {
                puntuacionTablaUno.transform.GetChild(ronda).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacion;
            }
            else
            {
                puntuacionTablaDos.transform.GetChild(ronda).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacion;
            }
            
        }
        else
        {
            if (jugadorUno)
            {
                puntuacionTablaUno.transform.GetChild(ronda).GetChild(tiro).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacion;
            }
            else
            {
                puntuacionTablaDos.transform.GetChild(ronda).GetChild(tiro).gameObject.GetComponent<TextMeshProUGUI>().text = puntuacion;
            }
            
        }
    }

    [PunRPC]
    void RPCFinalizarJuego()
    {
        ResetVariables();
        VaciarTabla();
        seleccionModo.SetActive(true);
        tablaPuntuacion.SetActive(false);
        textoGanador.SetActive(false);
        camaraLanzamiento.targetTexture = texturaTele;
        camaraPresentacion.targetTexture = nullTexture;
        camaraPresentacion.gameObject.SetActive(false);
        camaraLanzamiento.gameObject.SetActive(true);

        bola.transform.position = posicionBola;
        bola.GetComponent<BolaBolosBehaivour>().PararBola();

        Destroy(GameObject.Find("ConjuntoPinos"));
        Destroy(GameObject.Find("ConjuntoPinos(Clone)"));
        GameObject pinos = Instantiate(conjuntoPinos, posicionBolos, Quaternion.identity);
        pinos.transform.parent = this.transform;
        congelarPinos.quietos = true;
    }
    [PunRPC]
    void RPCOcultarVentanaEspera()
    {
        if (turno == 1)
        {
            mandoUno.jugando = true;
            mandoDos.jugando = false;
        }
        else
        {
            mandoUno.jugando = false;
            mandoDos.jugando = true;
        }

        ventanaEsperaLanzamiento.SetActive(false);
        ventanaEsperaPresentacion.SetActive(false);
    }
    [PunRPC]
    void RPCVentanaEspera(bool jugadorUno)
    {
        Debug.Log("Abriendo ventana espera");
        mandoUno.jugando = false;
        mandoDos.jugando = false;
        ventanaEsperaLanzamiento.SetActive(true);
        ventanaEsperaPresentacion.SetActive(true);
        if (jugadorUno)
        {
            ventanaEsperaLanzamiento.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                "Esperando a que el jugador uno se reconecte...";
            ventanaEsperaPresentacion.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                "Esperando a que el jugador uno se reconecte...";
        }
        else
        {
            ventanaEsperaLanzamiento.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                "Esperando a que el jugador dos se reconecte...";
            ventanaEsperaPresentacion.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =
                "Esperando a que el jugador dos se reconecte...";
        }
    }

    [PunRPC]
    void RPCMoverPersonaje()
    {
        if (turno == 1)
        {
            personajeUno.transform.position = posicionJugadorLanzar;
            personajeDos.transform.position = posicionJugadorEspera;
            mandoUno.CogerBola();
            view.RPC("CogerBola", RpcTarget.OthersBuffered,true);
            mandoUno.jugando = false;
            mandoUno.tirado = false;

        }
        else
        {
            personajeDos.transform.position = posicionJugadorLanzar;
            personajeUno.transform.position = posicionJugadorEspera;
            mandoDos.CogerBola();
            view.RPC("CogerBola", RpcTarget.OthersBuffered, false);
            mandoDos.jugando = false;
            mandoDos.tirado = false;
        }
    }
    [PunRPC]
    void RPCCambiarCamara(bool presentacion)
    {
        CambiarCamara(presentacion);
    }
    [PunRPC]
    void ActualizarTurno(int turno)
    {
        this.turno = turno;
    }

    [PunRPC]
    void ActualizarJugandoJugadores(bool unoJugando, bool dosJugando)
    {
        mandoUno.jugando = unoJugando;
        mandoDos.jugando = dosJugando;
    }
    [PunRPC]
    void ActualizarEsperando(bool esperaUno, bool esperaDos)
    {
        this.esperandoUno = esperaUno;
        this.esperandoDos = esperaDos;
    }
    [PunRPC]
    void ActualizarSolo(bool solo)
    {
        this.solo = solo;
    }

    [PunRPC]
    void CogerBola(bool uno)
    {
        if (uno)
        {
            mandoUno.CogerBola();
        }
        else
        {
            mandoDos.CogerBola();
        }
    }
    [PunRPC]
    void BorrarPinos()
    {
        Destroy(GameObject.Find("ConjuntoPinos"));
        Destroy(GameObject.Find("ConjuntoPinos(Clone)"));
        GameObject pinos = Instantiate(conjuntoPinos, posicionBolos, Quaternion.identity);
        pinos.transform.parent = this.transform;
        congelarPinos.quietos = true;
    }

    [PunRPC]
    void RPCSeleccionarModo(bool mostrar)
    {
        if (mostrar)
        {
            seleccionModo.SetActive(true);
        }
        else
        {
            seleccionModo.SetActive(false);
        }
    }
    [PunRPC]
    void RPCTablaPuntuaciones(bool mostrar)
    {
        if (mostrar)
        {
            tablaPuntuacion.SetActive(true);
        }
        else
        {
            tablaPuntuacion.SetActive(false);
        }
    }
    [PunRPC]
    void RPCTextoGanador(bool mostrar, string texto)
    {
        if (mostrar)
        {
            textoGanador.SetActive(true);
        }
        else
        {
            textoGanador.SetActive(false);
        }
        textoGanador.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = texto;
    }
}
