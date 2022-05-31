//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/Scripts/Player/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Movimiento"",
            ""id"": ""c1847d6d-975a-46c1-af41-70a4af0c9792"",
            ""actions"": [
                {
                    ""name"": ""Horizontal"",
                    ""type"": ""PassThrough"",
                    ""id"": ""9e3814e7-c158-4786-9652-c514715bf673"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseX"",
                    ""type"": ""PassThrough"",
                    ""id"": ""5b659495-3ade-48d7-b76e-9cc0974a4235"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseY"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d7ebac97-0926-4742-98a8-82d5ade8b436"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""821429e4-0398-425a-96e5-ea083ecb0f68"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""052943cd-7cab-4a1a-af91-2fa076c6c082"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""83f8d52b-0d0f-4299-9797-195c8529f90e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""23c72767-2eec-492f-ae67-2f10857eeaeb"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ca90a1c7-6ab5-4278-92cc-91d1bdc4b3ea"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a4326b33-c44e-40b4-9f49-d4df32baefeb"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1defe2f0-cf4d-4836-8fb0-379378b92bbf"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Movimiento
        m_Movimiento = asset.FindActionMap("Movimiento", throwIfNotFound: true);
        m_Movimiento_Horizontal = m_Movimiento.FindAction("Horizontal", throwIfNotFound: true);
        m_Movimiento_MouseX = m_Movimiento.FindAction("MouseX", throwIfNotFound: true);
        m_Movimiento_MouseY = m_Movimiento.FindAction("MouseY", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Movimiento
    private readonly InputActionMap m_Movimiento;
    private IMovimientoActions m_MovimientoActionsCallbackInterface;
    private readonly InputAction m_Movimiento_Horizontal;
    private readonly InputAction m_Movimiento_MouseX;
    private readonly InputAction m_Movimiento_MouseY;
    public struct MovimientoActions
    {
        private @PlayerControls m_Wrapper;
        public MovimientoActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Horizontal => m_Wrapper.m_Movimiento_Horizontal;
        public InputAction @MouseX => m_Wrapper.m_Movimiento_MouseX;
        public InputAction @MouseY => m_Wrapper.m_Movimiento_MouseY;
        public InputActionMap Get() { return m_Wrapper.m_Movimiento; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovimientoActions set) { return set.Get(); }
        public void SetCallbacks(IMovimientoActions instance)
        {
            if (m_Wrapper.m_MovimientoActionsCallbackInterface != null)
            {
                @Horizontal.started -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnHorizontal;
                @Horizontal.performed -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnHorizontal;
                @Horizontal.canceled -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnHorizontal;
                @MouseX.started -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnMouseX;
                @MouseX.performed -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnMouseX;
                @MouseX.canceled -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnMouseX;
                @MouseY.started -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnMouseY;
                @MouseY.performed -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnMouseY;
                @MouseY.canceled -= m_Wrapper.m_MovimientoActionsCallbackInterface.OnMouseY;
            }
            m_Wrapper.m_MovimientoActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Horizontal.started += instance.OnHorizontal;
                @Horizontal.performed += instance.OnHorizontal;
                @Horizontal.canceled += instance.OnHorizontal;
                @MouseX.started += instance.OnMouseX;
                @MouseX.performed += instance.OnMouseX;
                @MouseX.canceled += instance.OnMouseX;
                @MouseY.started += instance.OnMouseY;
                @MouseY.performed += instance.OnMouseY;
                @MouseY.canceled += instance.OnMouseY;
            }
        }
    }
    public MovimientoActions @Movimiento => new MovimientoActions(this);
    public interface IMovimientoActions
    {
        void OnHorizontal(InputAction.CallbackContext context);
        void OnMouseX(InputAction.CallbackContext context);
        void OnMouseY(InputAction.CallbackContext context);
    }
}
