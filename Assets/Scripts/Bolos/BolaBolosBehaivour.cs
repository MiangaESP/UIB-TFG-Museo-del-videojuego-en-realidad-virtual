using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BolaBolosBehaivour : MonoBehaviour
{
    private float velocidad=0;
    public float velocidadX = 0;
    public float velocidadY = 0;
    public float velocidadZ = 0;

    public bool bolaLanzada = false;

    public PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, velocidad);
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(velocidadX, -velocidad * 2, velocidad);
        }
        else if (!bolaLanzada)
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            this.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        }
        if (this.transform.parent.parent!=null)
        {
            if (this.transform.parent.parent.name == "PersonajeBolosJ1" || this.transform.parent.parent.name == "PersonajeBolosJ2")
            {
                this.transform.localPosition = new Vector3(0, -0.5f, 0);
            }
        }
        
    }
    public void LanzarBola(float velocidadX,bool vr)
    {
        bolaLanzada = true;
        this.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        velocidad = 4;
        this.velocidadX = velocidadX*3;
        if (vr)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(velocidadX * 15, velocidadY, velocidadZ), ForceMode.Impulse);
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(velocidadX * 15 - 200, velocidadY, velocidadZ), ForceMode.Impulse);
        }
        view.RPC("RPCLanzarBola", RpcTarget.OthersBuffered);
    }
    public void PararBola()
    {
        bolaLanzada = false;
        velocidad = 0;
        velocidadX = 0;
        this.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        view.RPC("RPCPararBola", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    void RPCPararBola()
    {
        velocidad = 0;
        velocidadX = 0;
        this.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
    }

    [PunRPC]
    void RPCLanzarBola()
    {
        bolaLanzada = true;
    }

}
