using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviour
{

    PhotonView view;
    public float mouseSensitivityX = 8f;
    public float mouseSensitivityY = 0.5f;
    float mouseX, mouseY;

    Transform playerCamera;
    public float xClamp = 85f;
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        playerCamera = this.gameObject.transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (view != null)
        {
            if (view.IsMine)
            {
                transform.Rotate(Vector3.up, mouseX);

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
                Vector3 playerRotation = transform.eulerAngles;
                playerRotation.x = xRotation;
                playerCamera.eulerAngles = playerRotation;
            }
        }
        
    }
    public void ReceiveInput(Vector2 mouseInput)
    {
        if (view != null)
        {
            if (view.IsMine)
            {
                mouseX = mouseInput.x * mouseSensitivityX;
                mouseY = mouseInput.y * mouseSensitivityY;
            }
        }
    }
}
