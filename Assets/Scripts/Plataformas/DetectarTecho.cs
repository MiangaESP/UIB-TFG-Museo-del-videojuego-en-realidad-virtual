using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarTecho : MonoBehaviour
{
    public GameObject personaje;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.ToString() == "9")
        {
            Debug.Log("Tocando techo");
            Rigidbody2D rig = personaje.transform.GetComponent<Rigidbody2D>();
            rig.velocity = new Vector2(rig.velocity.x, 0);
        }
    }
}
