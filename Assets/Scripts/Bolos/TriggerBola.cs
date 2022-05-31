using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBola : MonoBehaviour
{
    public BolosBehaviour bolosControl;
    public float timerBola;
    public float timerBolaCounter=4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerBolaCounter < 0)
        {
            bolosControl.BolaTrigger();
            timerBolaCounter = 4;
        }
        else if(0<timerBolaCounter && timerBolaCounter<=1)
        {
            timerBolaCounter = timerBolaCounter -Time.deltaTime;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bola")
        {
            timerBolaCounter = timerBola;
        }
    }
}
