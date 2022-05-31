using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controles : MonoBehaviour
{
    public GameObject controles;

    public void ShowControles()
    {
        controles.SetActive(true);   
    }
    public void HideControles()
    {
        controles.SetActive(false);
    }
}
