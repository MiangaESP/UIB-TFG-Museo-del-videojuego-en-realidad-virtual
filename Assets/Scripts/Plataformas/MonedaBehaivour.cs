using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonedaBehaivour : MonoBehaviour
{
    public PlataformasBehaviour juego;
    public Animator animMoneda;
    // Start is called before the first frame update
    void Start()
    {
        animMoneda = GetComponent<Animator>();
        juego = GameObject.Find("Plataformas").GetComponent<PlataformasBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Personaje")
        {
            juego.IncrementaMoneda();
            animMoneda.Play("Moneda_cogida");
            Destroy(this.gameObject, 0.6f);
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
