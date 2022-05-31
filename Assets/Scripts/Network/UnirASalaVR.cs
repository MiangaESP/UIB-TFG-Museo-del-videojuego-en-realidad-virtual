using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class UnirASalaVR : MonoBehaviourPunCallbacks
{
    public GameObject mensajeFallo;
    public void CrearUnirSala()
    {
        GameObject fadePanel = GameObject.Find("FadePanel");
        var image = fadePanel.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 1;
        image.color = tempColor;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom("Sala", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        
        PhotonNetwork.LoadLevel("Museo");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        mensajeFallo.SetActive(true);
    }
}
