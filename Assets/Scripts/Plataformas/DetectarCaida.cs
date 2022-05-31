using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarCaida : MonoBehaviour
{
    public PersonajePlataformasBehaviour personaje;
    public GameObject camara;

    public Vector3 posicionReaparecer;
    //Si el personaje entra en el trigger del detector de caidas, este reaparece en una posición segura
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Personaje")
        {
            personaje.cuerpoPersonaje.transform.position = posicionReaparecer;
            camara.transform.position = new Vector3(personaje.cuerpoPersonaje.transform.position.x, personaje.cuerpoPersonaje.transform.position.y,camara.transform.position.z);
            personaje.QuitarVida();
        }
    }
}
