using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR;

public class Disconnected : MonoBehaviourPunCallbacks
{
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        bool vr = (bool) otherPlayer.CustomProperties["vr"] ;
        Debug.Log(vr);
        SalidaJugador(otherPlayer.TagObject as GameObject, vr);   
    }
    public void OnPhotonPlayerDisconnected(Player otherPlayer)
    {
        bool vr = (bool)otherPlayer.CustomProperties["vr"];
        Debug.Log(vr);
        SalidaJugador(otherPlayer.TagObject as GameObject, vr);
    }

    public void SalidaJugador(GameObject playerObject, bool vr)
    {
        Transform mandoTransform;
        Transform mandoParent;
        Vector3 mandoPos;
        Quaternion mandoRot=new Quaternion();
        
        if (vr)
        {
            mandoTransform = playerObject.GetComponent<JugadorVRBehaviour>().mandoTransform;
            mandoParent = playerObject.GetComponent<JugadorVRBehaviour>().mandoParent;
            mandoPos = playerObject.GetComponent<JugadorVRBehaviour>().mandoPos;
            mandoRot = playerObject.GetComponent<JugadorVRBehaviour>().mandoRot;
        }
        else{
            mandoTransform = playerObject.GetComponent<JugadorFPPCBehaivour>().mandoTransform;
            mandoParent = playerObject.GetComponent<JugadorFPPCBehaivour>().mandoParent;
            mandoPos = playerObject.GetComponent<JugadorFPPCBehaivour>().mandoPos;
        }
        
        if (mandoTransform != null)
        {
            if (vr)
            {
                mandoTransform.rotation = mandoRot;
            }
            mandoTransform.parent = mandoParent;
            mandoTransform.position = mandoPos;
            mandoTransform.localScale = new Vector3(mandoTransform.localScale.x * 2, mandoTransform.localScale.y * 2, mandoTransform.localScale.z * 2);
            mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = false;
            mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoSalida(true);
            mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = null;
        }
        Destroy(playerObject);

    }
        
    

}
