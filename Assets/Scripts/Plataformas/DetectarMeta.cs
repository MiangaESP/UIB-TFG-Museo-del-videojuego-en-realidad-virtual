using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarMeta : MonoBehaviour
{
    public PlataformasBehaviour juego;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Personaje"){
            juego.Win();
        }
    }
}
