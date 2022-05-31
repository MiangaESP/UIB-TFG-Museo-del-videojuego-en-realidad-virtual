using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer video;
    public PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartVideo()
    {
        video.Play();
        view.RPC("StartVideoRPC", RpcTarget.OthersBuffered);
    }
    public void StopVideo()
    {
        video.Stop();
        view.RPC("StopVideoRPC", RpcTarget.OthersBuffered);
    }
    [PunRPC]
    void StartVideoRPC()
    {
        video.Play();
    }

    [PunRPC]
    void StopVideoRPC()
    {
        video.Stop();
    }

}
