using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InfoManager : MonoBehaviour
{
    public GameObject globoInfo;
    public PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowInfo()
    {
        globoInfo.SetActive(true);
        view.RPC("ShowInfoRPC", RpcTarget.OthersBuffered);
    }
    public void HideInfo()
    {
        globoInfo.SetActive(false);
        view.RPC("HideInfoRPC", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    void ShowInfoRPC()
    {
        globoInfo.SetActive(true);
    }
    [PunRPC]
    void HideInfoRPC()
    {
        globoInfo.SetActive(false);
    }
}
