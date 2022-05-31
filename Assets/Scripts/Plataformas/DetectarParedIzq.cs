using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarParedIzq : MonoBehaviour
{
    public bool isIzqTouch=false;
    public PersonajePlataformasBehaviour personaje;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.ToString() == "9")
        {
            isIzqTouch = true;
        }
        if (collision.gameObject.layer.ToString() == "10")
        {
            Debug.Log("Me he pegado con un slime");
            if (!personaje.invulnerabilidad)
            {
                personaje.QuitarVida();
                personaje.Salto(100, -200);
            }

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer.ToString() == "9")
        {
            isIzqTouch = false;
        }
    }
}
