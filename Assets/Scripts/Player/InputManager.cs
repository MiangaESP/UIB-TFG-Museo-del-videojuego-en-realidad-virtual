using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InputManager : MonoBehaviour
{
    PhotonView view;

    JugadorFPPCBehaivour jugador;
    MouseLook mouseLook;

    PlayerControls controles;
    PlayerControls.MovimientoActions movimiento;

    Vector2 horizonatlInput;
    Vector2 mouseInput;
    // Start is called before the first frame update
    public void Start()
    {
        view = GetComponent<PhotonView>();
        mouseLook = GetComponent<MouseLook>();
        jugador = GetComponent<JugadorFPPCBehaivour>();
        
    }
    private void Awake()
    {
        controles = new PlayerControls();
        movimiento = controles.Movimiento;
        movimiento.Horizontal.performed += ctx => horizonatlInput = ctx.ReadValue<Vector2>();

        movimiento.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        movimiento.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
    }
    private void Update()
    {
        if (view.IsMine)
        {
            jugador.ReceiveInput(horizonatlInput);
            mouseLook.ReceiveInput(mouseInput);
        }
    }
    private void OnEnable()
    {
        controles.Enable();
    }
    private void OnDisable()
    {
        controles.Disable(); 
    }
}
