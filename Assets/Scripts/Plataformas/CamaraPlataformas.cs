using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraPlataformas : MonoBehaviour
{
    public Transform personajePrincipal;


    // Update is called once per frame
    void Update()
    {
        //Sigue al personaje salvo cuando cae por un agujero
        if (transform.position.y > -1.35f)
        {
            this.transform.position = new Vector3(personajePrincipal.position.x, personajePrincipal.transform.position.y, transform.position.z);
        }
        
    }
}
