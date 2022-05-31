using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

public class ScenePlayers : MonoBehaviourPunCallbacks
{
    public GameObject playerPCPrefab;
    public GameObject playerVRPrefab;
    public Transform spawnvr;
    public Transform spawnpc;

    private void Start()
    {
        bool vr = false;
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                vr = true;
            }
        }
        if (vr)
        {
            PhotonNetwork.Instantiate(playerVRPrefab.name, spawnvr.position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate(playerPCPrefab.name, spawnpc.position, Quaternion.identity);
        }
    }
        void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // e.g. store this gameobject as this player's charater in Player.TagObject
        info.Sender.TagObject = gameObject;
    }
        
}

