using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinoAux : MonoBehaviour
{
    public bool quieto;
    private CongelarPinos congelarBolos;
    // Start is called before the first frame update
    void Start()
    {
        congelarBolos = GameObject.Find("CongelarBolos").GetComponent<CongelarPinos>();
    }

    // Update is called once per frame
    void Update()
    {
        quieto = congelarBolos.quietos;
        if (quieto)
        {
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}
