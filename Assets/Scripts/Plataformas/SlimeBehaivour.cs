using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehaivour : MonoBehaviour
{

    //Constantes con los nombres de las animaciones
    private static string SLIME_QUIETO = "Slime_quieto";
    private static string SLIME_CAMINANDO = "Slime_caminando";
    private static string SLIME_MUERTO = "Slime_muerto";

    //Animator del slime
    public Animator animSlime;

    //Booleano para saber si el slime esta quieto o moviendose
    public bool caminando=false;

    //Direccion hacia la que avanza el slime (1 avanza a la derecha, -1 avanza a la izquierda)
    public float direccion=1;

    public float timerQuieto=30;
    public float timerCounterQuieto=30;

    public float timerCaminando=30;
    public float timerCounterCaminando=0;

    private PlataformasBehaviour juego;

    // Start is called before the first frame update
    void Start()
    {
        animSlime = GetComponent<Animator>();
        juego = GameObject.Find("Plataformas").GetComponent<PlataformasBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (juego.jugando)
        {
            if (caminando)
            {
                timerCounterCaminando -= Time.deltaTime;
                this.transform.position = new Vector3(transform.position.x + direccion * Time.deltaTime * Time.deltaTime * 20, -1.23f, transform.position.z);
                //Cuando me quedo sin tiempo de movimiento paramos al enemigo
                if (timerCounterCaminando < 0)
                {
                    this.transform.position = new Vector3(transform.position.x, -1.25f, transform.position.z);
                    caminando = false;
                    animSlime.Play(SLIME_QUIETO);
                    timerCounterQuieto = timerQuieto;
                }
            }
            else
            {
                timerCounterQuieto -= Time.deltaTime;
                //Cuando me quedo sin tiempo de estar parado, arranca al enemigo
                if (timerCounterQuieto < 0)
                {
                    caminando = true;
                    animSlime.Play(SLIME_CAMINANDO);
                    timerCounterCaminando = timerCaminando;
                    direccion = -direccion;
                    gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX;
                }
            }
        }

    }
    public void MatarSlime()
    {
        animSlime.Play(SLIME_MUERTO);
        Destroy(this.gameObject, 0.6f);
        GetComponent<Collider2D>().enabled = false;
    }

}
