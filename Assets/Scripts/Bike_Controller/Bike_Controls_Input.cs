










using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;


























































public partial class @Bike_Controls_Input: IInputActionCollection2, IDisposable
{



    public InputActionAsset asset { get; }




    public @Bike_Controls_Input()
    {
        asset = InputActionAsset.FromJson(@"{
    ""version"": 1,
    ""name"": ""Bike_Controls_Input"",
    ""maps"": [
        {
            ""name"": ""Bike"",
            ""id"": ""584edb80-2c26-4343-9e1e-3cc99140d7f2"",
            ""actions"": [
                {
                    ""name"": ""Throttle"",
                    ""type"": ""Value"",
                    ""id"": ""e35a6452-158a-4ae4-815f-e8c679047896"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Brake"",
                    ""type"": ""Value"",
                    ""id"": ""0a10f999-f1d1-42f8-9a45-60a4ed9d8a0a"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Steering"",
                    ""type"": ""Value"",
                    ""id"": ""ef3ff14c-e198-4521-a0da-5c4c1cbe9b9c"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Handbrake"",
                    ""type"": ""Button"",
                    ""id"": ""e8fe9ed5-5356-4e9c-9df1-d1bc4e968721"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Reset"",
                    ""type"": ""Button"",
                    ""id"": ""6167bf04-993b-4185-a370-7663238ffd71"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c3865bf8-17e8-4dfa-8135-d29c1e51d748"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throttle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4bc6429c-3101-403b-9763-465937971091"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""234b1ef1-5e77-45c7-a067-2652da277b1a"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""757d1177-b2f2-4b63-bd68-7ae0815129e1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""26526439-d918-4d7c-9a52-d71170984375"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7cccef13-aa87-45fc-9d92-38abef73559f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Handbrake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6cba3ebe-08d1-46d9-ae2d-fe941461a4d3"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");

        m_Bike = asset.FindActionMap("Bike", throwIfNotFound: true);
        m_Bike_Throttle = m_Bike.FindAction("Throttle", throwIfNotFound: true);
        m_Bike_Brake = m_Bike.FindAction("Brake", throwIfNotFound: true);
        m_Bike_Steering = m_Bike.FindAction("Steering", throwIfNotFound: true);
        m_Bike_Handbrake = m_Bike.FindAction("Handbrake", throwIfNotFound: true);
        m_Bike_Reset = m_Bike.FindAction("Reset", throwIfNotFound: true);
    }

    ~@Bike_Controls_Input()
    {
        UnityEngine.Debug.Assert(!m_Bike.enabled, "This will cause a leak and performance issues, Bike_Controls_Input.Bike.Disable() has not been called.");
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


    private readonly InputActionMap m_Bike;
    private List<IBikeActions> m_BikeActionsCallbackInterfaces = new List<IBikeActions>();
    private readonly InputAction m_Bike_Throttle;
    private readonly InputAction m_Bike_Brake;
    private readonly InputAction m_Bike_Steering;
    private readonly InputAction m_Bike_Handbrake;
    private readonly InputAction m_Bike_Reset;



    public struct BikeActions
    {
        private @Bike_Controls_Input m_Wrapper;




        public BikeActions(@Bike_Controls_Input wrapper) { m_Wrapper = wrapper; }



        public InputAction @Throttle => m_Wrapper.m_Bike_Throttle;



        public InputAction @Brake => m_Wrapper.m_Bike_Brake;



        public InputAction @Steering => m_Wrapper.m_Bike_Steering;



        public InputAction @Handbrake => m_Wrapper.m_Bike_Handbrake;



        public InputAction @Reset => m_Wrapper.m_Bike_Reset;



        public InputActionMap Get() { return m_Wrapper.m_Bike; }

        public void Enable() { Get().Enable(); }

        public void Disable() { Get().Disable(); }

        public bool enabled => Get().enabled;



        public static implicit operator InputActionMap(BikeActions set) { return set.Get(); }








        public void AddCallbacks(IBikeActions instance)
        {
            if (instance == null || m_Wrapper.m_BikeActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_BikeActionsCallbackInterfaces.Add(instance);
            @Throttle.started += instance.OnThrottle;
            @Throttle.performed += instance.OnThrottle;
            @Throttle.canceled += instance.OnThrottle;
            @Brake.started += instance.OnBrake;
            @Brake.performed += instance.OnBrake;
            @Brake.canceled += instance.OnBrake;
            @Steering.started += instance.OnSteering;
            @Steering.performed += instance.OnSteering;
            @Steering.canceled += instance.OnSteering;
            @Handbrake.started += instance.OnHandbrake;
            @Handbrake.performed += instance.OnHandbrake;
            @Handbrake.canceled += instance.OnHandbrake;
            @Reset.started += instance.OnReset;
            @Reset.performed += instance.OnReset;
            @Reset.canceled += instance.OnReset;
        }








        private void UnregisterCallbacks(IBikeActions instance)
        {
            @Throttle.started -= instance.OnThrottle;
            @Throttle.performed -= instance.OnThrottle;
            @Throttle.canceled -= instance.OnThrottle;
            @Brake.started -= instance.OnBrake;
            @Brake.performed -= instance.OnBrake;
            @Brake.canceled -= instance.OnBrake;
            @Steering.started -= instance.OnSteering;
            @Steering.performed -= instance.OnSteering;
            @Steering.canceled -= instance.OnSteering;
            @Handbrake.started -= instance.OnHandbrake;
            @Handbrake.performed -= instance.OnHandbrake;
            @Handbrake.canceled -= instance.OnHandbrake;
            @Reset.started -= instance.OnReset;
            @Reset.performed -= instance.OnReset;
            @Reset.canceled -= instance.OnReset;
        }





        public void RemoveCallbacks(IBikeActions instance)
        {
            if (m_Wrapper.m_BikeActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }










        public void SetCallbacks(IBikeActions instance)
        {
            foreach (var item in m_Wrapper.m_BikeActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_BikeActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }



    public BikeActions @Bike => new BikeActions(this);





    public interface IBikeActions
    {






        void OnThrottle(InputAction.CallbackContext context);






        void OnBrake(InputAction.CallbackContext context);






        void OnSteering(InputAction.CallbackContext context);






        void OnHandbrake(InputAction.CallbackContext context);






        void OnReset(InputAction.CallbackContext context);
    }
}
