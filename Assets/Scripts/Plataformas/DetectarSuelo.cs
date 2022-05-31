using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarSuelo : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isGrounded;

    public PersonajePlataformasBehaviour personaje;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.ToString() == "9")
        {
            isGrounded = true;
        }
        if (collision.gameObject.layer.ToString() == "10")
        {
            collision.gameObject.GetComponent<SlimeBehaivour>().MatarSlime();
            personaje.GetComponent<PersonajePlataformasBehaviour>().Salto(0,-100);
        }
        if (collision.gameObject.layer.ToString() == "11")
        {
            personaje.GetComponent<PersonajePlataformasBehaviour>().Salto(0,100);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.ToString() == "9")
        {
            isGrounded = false;
        }
    }
}
