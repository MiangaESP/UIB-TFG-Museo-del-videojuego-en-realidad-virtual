using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using UnityEngine.UI;

public class JugadorVRBehaviour : MonoBehaviour
{
    const float triggerTimer = 1f;
    float triggerTimerCounter;

    GameObject camara;
    PhotonView view;
    private XROrigin XROrigin;
    private CharacterController characterController;
    private CharacterControllerDriver driver;
    private GameObject locomotion;

    public GameObject fadePanel;

    public Animator rightHandAnimator;
    public Animator leftHandAnimator;
    public bool manos = false; //Desactivamos las manos que se generan en el instantiate

    //Punteros a los gameobject de ambas manos
    public GameObject manoDer;
    public GameObject manoIzq;

    //Variables que usaremos para guardar el mando que cojamos y variables de interes
    public Transform mandoParent = null;
    public Vector3 mandoPos;
    public Quaternion mandoRot;
    public Transform mandoTransform = null;

    //Booleano que indica si estamos jugando
    public bool jugando;
    //Raycast que se encargará de detectar si estamos apuntando a una pantalla, consola y mando
    RaycastHit hit;

    void Start()
    {
        XROrigin = GetComponent<XROrigin>();
        characterController = GetComponent<CharacterController>();
        driver = GetComponent<CharacterControllerDriver>();
        view = GetComponent<PhotonView>();
        camara = transform.Find("Camera Offset").gameObject.transform.Find("Main Camera").gameObject;
        locomotion = transform.Find("Locomotion System").gameObject;
        if (view.IsMine)
        {
            fadePanel = this.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
            var image = fadePanel.GetComponent<Image>();
            var tempColor = image.color;
            tempColor.a = 0;
            image.color = tempColor;
            camara.SetActive(true);
            manoDer=this.transform.Find("Camera Offset/RightHand Controller").gameObject;
            manoIzq = this.transform.Find("Camera Offset/LeftHand Controller").gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerTimerCounter >= 0)
        {
            triggerTimerCounter -= Time.deltaTime;
        }
        if (!manos)
        {
            /*this.transform.Find("Camera Offset/LeftHand Controller/Custom Left Hand Model(Clone)").gameObject.SetActive(false);
            this.transform.Find("Camera Offset/RightHand Controller/Custom Right Hand Model(Clone)").gameObject.SetActive(false);*/
            
            manos = true;
        }
        if (view.IsMine)
        {
            UpdateCharacterController();
            if (!jugando)
            {
                UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
                UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
            }
            UpdateRaycastIzq();
            UpdateRaycastDer();
        }
    }

    void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {

            if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                handAnimator.SetFloat("Trigger", triggerValue);
            }
            else
            {
                handAnimator.SetFloat("Trigger", 0);
            }
            
            if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                handAnimator.SetFloat("Grip", gripValue);
            }
            else
            {
                handAnimator.SetFloat("Grip", 0);
            }
        
    }

    protected virtual void UpdateCharacterController()
    {
        if (XROrigin == null || characterController == null)
            return;

        var height = Mathf.Clamp(XROrigin.CameraInOriginSpaceHeight, driver.minHeight, driver.maxHeight);

        Vector3 center = XROrigin.CameraInOriginSpacePos;
        center.y = height / 2f + characterController.skinWidth;

        characterController.height = height;
        characterController.center = center;
    }
    void UpdateRaycastIzq()
    {
        bool auxGrip = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.grip, out float gripValue);
        bool auxTrigger = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        //Miramos si el laser ha impactado algo siempre que no estemos jugando, cogemos el mando si hemos pulsado el boton grip
        if (Physics.Raycast(manoIzq.transform.position, manoIzq.transform.forward, out hit, 3f) && !jugando)
        {
            if (hit.transform.gameObject.tag == "Pantalla" && auxTrigger && triggerValue > 0.5 && triggerTimerCounter < 0)
            {

                VideoManager pantalla = hit.transform.gameObject.GetComponent<VideoManager>();
                triggerTimerCounter = triggerTimer;
                if (pantalla.video.isPlaying)
                {
                    pantalla.StopVideo();
                }
                else
                {
                    pantalla.StartVideo();
                }
                if(this.gameObject.name!="XR Rig Unirse")
                {
                    view.RPC("ActualizarColorLaser", RpcTarget.OthersBuffered, true, true);
                }
                
            }
            else if (hit.transform.gameObject.tag == "Consola" && auxTrigger && triggerValue > 0.5 && triggerTimerCounter < 0)
            {
                InfoManager info = hit.transform.gameObject.GetComponent<InfoManager>();
                if (hit.transform.Find("GloboInfo").gameObject.activeSelf)
                {
                    info.HideInfo();
                }
                else
                {
                    info.ShowInfo();
                }
                if (this.gameObject.name != "XR Rig Unirse")
                {
                    view.RPC("ActualizarColorLaser", RpcTarget.OthersBuffered, true, true);
                }
            }
            //Si el Raycast detecta un mando, a parte de cambiar el puntero debemos mirar si se desea cogerlo.
            else if (hit.transform.gameObject.tag == "Mando" && auxTrigger && triggerValue > 0.5)
            {
                if (this.gameObject.name != "XR Rig Unirse")
                {
                    view.RPC("ActualizarColorLaser", RpcTarget.OthersBuffered, true, true);
                }
                if (mandoParent == null && !hit.transform.GetComponent<PersonajeBehaivour>().mandoCogido)
                {
                    //Guardamos la informacion inicial del mando para luego devolverlo a su sitio
                    mandoTransform = hit.transform;
                    mandoParent = hit.transform.parent;

                    //Actualizamos las variables iniciales del juego al que vamos a jugar
                    mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = true;
                    mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = view;
                    mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = mandoTransform.name;
                    mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoEntrada(manoIzq,true);

                    //Cogemos el mando y nos colocamos en la posicion
                    jugando = true;
                    //locomotion.GetComponent<ActionBasedSnapTurnProvider>().enabled = false;
                    locomotion.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
                    mandoPos = mandoTransform.position;
                    mandoRot = mandoTransform.rotation;
                    mandoTransform.parent = this.manoIzq.transform;
                    mandoTransform.position = this.manoIzq.transform.Find("MandoPos").position; 
                    this.transform.position = mandoTransform.GetComponent<PersonajeBehaivour>().posicionJugadorVR;
                    mandoTransform.localScale = new Vector3(mandoTransform.localScale.x / 2, mandoTransform.localScale.y / 2, mandoTransform.localScale.z / 2);

                    //Hacemos desaparecer el modelo de la mano y el raycast
                    this.manoIzq.transform.Find("Custom Left Hand Model").gameObject.SetActive(false);
                    this.manoDer.GetComponent<XRInteractorLineVisual>().enabled = false;
                    this.manoIzq.GetComponent<XRInteractorLineVisual>().enabled = false;
                    view.RPC("ActualizarEntradaMando", RpcTarget.OthersBuffered, mandoPos, mandoRot, mandoTransform.name, mandoParent.name, true);
                }
            }
            else
            {
                if (this.gameObject.name != "XR Rig Unirse")
                {
                    view.RPC("ActualizarColorLaser", RpcTarget.OthersBuffered, true, false);
                }
            }
        }
        //Si tenemos un mando y pulsamos el trigger, dejamos el mando
        if (auxGrip && jugando && gripValue > 0.5)
        {
            if (mandoParent != null)
            {
                mandoTransform.position = mandoPos;
                mandoTransform.parent = mandoParent;
                mandoTransform.rotation = mandoRot;

                mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = false;
                mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoSalida(false);
                mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = null;
                mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = null;
                mandoTransform.localScale = new Vector3(mandoTransform.localScale.x * 2, mandoTransform.localScale.y * 2, mandoTransform.localScale.z * 2);
                jugando = false;
                locomotion.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;
                locomotion.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
                mandoParent = null;
                mandoTransform = null;

                this.manoIzq.transform.Find("Custom Left Hand Model").gameObject.SetActive(true);
                this.manoIzq.GetComponent<XRInteractorLineVisual>().enabled = true;
                this.manoDer.transform.Find("Custom Right Hand Model").gameObject.SetActive(true);
                this.manoDer.GetComponent<XRInteractorLineVisual>().enabled = true;

                view.RPC("ActualizarSalidaMando", RpcTarget.OthersBuffered);
            }
        }
    }

    void UpdateRaycastDer()
    {
        bool auxGrip = InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.grip, out float gripValue);
        bool auxTrigger = InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        //Miramos si el laser ha impactado algo siempre que no estemos jugando, cogemos el mando si hemos pulsado el boton grip
        if (Physics.Raycast(manoDer.transform.position, manoDer.transform.forward, out hit, 3f) && !jugando)
        {
            if (hit.transform.gameObject.tag == "Pantalla" && auxTrigger && triggerValue > 0.5 && triggerTimerCounter < 0)
            {

                VideoManager pantalla = hit.transform.gameObject.GetComponent<VideoManager>();
                triggerTimerCounter = triggerTimer;
                if (pantalla.video.isPlaying)
                {
                    pantalla.StopVideo();
                }
                else
                {
                    pantalla.StartVideo();
                }
                if (this.gameObject.name != "XR Rig Unirse")
                {
                    view.RPC("ActualizarColorLaser", RpcTarget.Others, true, true);
                }
            }
            else if (hit.transform.gameObject.tag == "Consola" && auxTrigger && triggerValue > 0.5 && triggerTimerCounter < 0)
            {
                InfoManager info = hit.transform.gameObject.GetComponent<InfoManager>();
                triggerTimerCounter = triggerTimer;
                if (hit.transform.Find("GloboInfo").gameObject.activeSelf)
                {
                    info.HideInfo();
                }
                else
                {
                    info.ShowInfo();
                }
                if (this.gameObject.name != "XR Rig Unirse")
                {
                    view.RPC("ActualizarColorLaser", RpcTarget.Others, true, true);
                }
            }
            //Si el Raycast detecta un mando, a parte de cambiar el puntero debemos mirar si se desea cogerlo.
            else if (hit.transform.gameObject.tag == "Mando" && auxTrigger && triggerValue > 0.5)
            {
                if (this.gameObject.name != "XR Rig Unirse")
                {
                    view.RPC("ActualizarColorLaser", RpcTarget.Others, false, true);
                }
                if (mandoParent == null && !hit.transform.GetComponent<PersonajeBehaivour>().mandoCogido)
                {
                    //Guardamos la informacion inicial del mando para luego devolverlo a su sitio
                    mandoTransform = hit.transform;
                    mandoParent = hit.transform.parent;

                    //Actualizamos las variables iniciales del juego al que vamos a jugar
                    mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = true;
                    mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = view;
                    mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = mandoTransform.name;
                    mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoEntrada(manoDer,false);

                    //Cogemos el mando y nos colocamos en la posicion
                    jugando = true;
                    //locomotion.GetComponent<ActionBasedSnapTurnProvider>().enabled = false;
                    locomotion.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
                    mandoPos = mandoTransform.position;
                    mandoRot = mandoTransform.rotation;
                    mandoTransform.parent = this.manoDer.transform;
                    mandoTransform.position = this.manoDer.transform.Find("MandoPos").position;
                    mandoTransform.rotation = mandoRot;
                    this.transform.position = mandoTransform.GetComponent<PersonajeBehaivour>().posicionJugadorVR;
                    mandoTransform.localScale = new Vector3(mandoTransform.localScale.x / 2, mandoTransform.localScale.y / 2, mandoTransform.localScale.z / 2);

                    this.manoDer.transform.Find("Custom Right Hand Model").gameObject.SetActive(false);
                    this.manoDer.GetComponent<XRInteractorLineVisual>().enabled = false;
                    this.manoIzq.GetComponent<XRInteractorLineVisual>().enabled = false;
                    view.RPC("ActualizarEntradaMando", RpcTarget.OthersBuffered, mandoPos,mandoRot, mandoTransform.name, mandoParent.name, false);
                }
            }
            else
            {
                if (this.gameObject.name != "XR Rig Unirse")
                {
                    view.RPC("ActualizarColorLaser", RpcTarget.Others, false, false);
                }
            }
        }
        //Si tenemos un mando y pulsamos el trigger, dejamos el mando
        if (auxGrip && jugando && gripValue > 0.5)
        {
            if (mandoParent != null)
            {
                mandoTransform.position = mandoPos;
                mandoTransform.rotation = mandoRot;
                mandoTransform.parent = mandoParent;

                mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = false;
                mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoSalida(false);
                mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = null;
                mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = null;
                mandoTransform.localScale = new Vector3(mandoTransform.localScale.x * 2, mandoTransform.localScale.y * 2, mandoTransform.localScale.z * 2);
                jugando = false;
                locomotion.GetComponent<ActionBasedSnapTurnProvider>().enabled = true;
                locomotion.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
                mandoParent = null;
                mandoTransform = null;

                this.manoIzq.transform.Find("Custom Left Hand Model").gameObject.SetActive(true);
                this.manoIzq.GetComponent<XRInteractorLineVisual>().enabled = true;
                this.manoDer.transform.Find("Custom Right Hand Model").gameObject.SetActive(true);
                this.manoDer.GetComponent<XRInteractorLineVisual>().enabled = true;
                view.RPC("ActualizarSalidaMando", RpcTarget.OthersBuffered);
            }
        }
    }

    public void OnPhotonInstantiate(Photon.Pun.PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
        ExitGames.Client.Photon.Hashtable vrProperty = new ExitGames.Client.Photon.Hashtable() { { "vr", true } };
        info.Sender.SetCustomProperties(vrProperty);
    }

    [PunRPC]
    void ActualizarEntradaMando(Vector3 mandoPos, Quaternion mandoRot, string mandoTransform, string mandoParent, bool izq)
    {
        this.mandoPos = mandoPos;
        this.mandoRot = mandoRot;
        this.mandoParent = GameObject.Find(mandoParent).transform;
        this.mandoTransform = GameObject.Find(mandoTransform).transform;

        this.mandoTransform.localScale = new Vector3(this.mandoTransform.localScale.x / 2, this.mandoTransform.localScale.y / 2, this.mandoTransform.localScale.z / 2);
        this.mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = this.mandoTransform.name;
        this.mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = true;
        this.mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = this.view;
        
        if (izq)
        {
            this.mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoEntrada(manoIzq,true);
            this.manoIzq.transform.Find("Custom Left Hand Model").gameObject.SetActive(false);
            this.manoDer.GetComponent<XRInteractorLineVisual>().enabled = false;
            this.manoIzq.GetComponent<XRInteractorLineVisual>().enabled = false;
            this.mandoTransform.parent = this.manoIzq.transform;
            this.mandoTransform.position = this.manoIzq.transform.Find("MandoPos").position;
        }
        else
        {
            this.mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoEntrada(manoDer,false);
            this.manoDer.transform.Find("Custom Right Hand Model").gameObject.SetActive(false);
            this.manoDer.GetComponent<XRInteractorLineVisual>().enabled = false;
            this.manoIzq.GetComponent<XRInteractorLineVisual>().enabled = false;
            this.mandoTransform.parent = this.manoDer.transform;
            this.mandoTransform.position = this.manoDer.transform.Find("MandoPos").position;
        }
    }
    [PunRPC]
    void ActualizarSalidaMando()
    {
        this.mandoTransform.parent = mandoParent;
        this.mandoTransform.position = mandoPos;
        this.mandoTransform.rotation = mandoRot;
        this.mandoTransform.localScale = new Vector3(mandoTransform.localScale.x * 2, mandoTransform.localScale.y * 2, mandoTransform.localScale.z * 2);
        this.mandoTransform.GetComponent<PersonajeBehaivour>().mandoCogido = false;
        mandoTransform.GetComponent<PersonajeBehaivour>().ActualizarEstadoSalida(false);
        this.mandoTransform.GetComponent<PersonajeBehaivour>().nombreMando = null;
        this.mandoTransform.GetComponent<PersonajeBehaivour>().viewJugador = null;
        this.mandoParent = null;
        this.mandoTransform = null;

        this.manoIzq.transform.Find("Custom Left Hand Model").gameObject.SetActive(true);
        this.manoIzq.GetComponent<XRInteractorLineVisual>().enabled = true;
        this.manoDer.transform.Find("Custom Right Hand Model").gameObject.SetActive(true);
        this.manoDer.GetComponent<XRInteractorLineVisual>().enabled = true;
    }

    [PunRPC]
    void ActualizarColorLaser(bool izq, bool detect)
    {
        Gradient gradient = new Gradient();
        
        float alpha = 1.0f;
        
        
        if (detect)
        {
            Color startColor = Color.white;
            Color endColor = Color.white;
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }
        else
        {
            Color startColor = Color.red;
            Color endColor = Color.red;
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        }
        if (izq)
        {
            this.manoIzq.GetComponent<LineRenderer>().colorGradient = gradient;
        }
        else
        {
            this.manoDer.GetComponent<LineRenderer>().colorGradient = gradient;
        }
    }
}
