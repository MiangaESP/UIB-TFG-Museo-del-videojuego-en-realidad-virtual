using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PersonajeBehaivour : MonoBehaviour
{
    public PhotonView view; //PhotonView del mando
    public PhotonView viewJugador = null; //PhotonView del jugador que coge el mando

    public bool mandoCogido; //Booleano para saber si el mando ha sido cogido
    public string nombreMando; //Nombre del mando
    public Vector3 posicionJugador; //Posicion donde estara el jugador PC una vez haya cogido el mando
    public Vector3 posicionJugadorVR; //Posicion donde estara el jugador VR una vez haya cogido el mando
    public Quaternion rotacionMando; //Rotacion que tendra tanto el mando como el personaje para que mire hacia la pantalla
    public Quaternion rotacionJugador; //Rotacion que tendra tanto el mando como el personaje para que mire hacia la pantalla
    // Start is called before the first frame update
    public virtual void ActualizarEstadoEntrada(GameObject mano, bool manoIzq) { }
    public virtual void ActualizarEstadoSalida(bool desconectado) { }
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
