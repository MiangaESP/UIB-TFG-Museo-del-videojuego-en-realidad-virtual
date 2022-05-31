using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UnirASala : MonoBehaviourPunCallbacks
{
    public GameObject botonPlay;
    public GameObject botonControles;
    public GameObject panelControles;
    public GameObject panelFallo;
    public void CrearUnirSala()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsOpen = true;
        botonPlay.SetActive(false);
        botonControles.SetActive(false);
        panelControles.SetActive(false);
        PhotonNetwork.JoinOrCreateRoom("Sala",roomOptions,TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Museo");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        panelFallo.SetActive(true);
    }

    public void CerrarPanelFallo()
    {
        botonPlay.SetActive(true);
        botonControles.SetActive(true);
        panelControles.SetActive(true);
    }
}
