using UnityEngine;

public class Bike_Input : MonoBehaviour
{
    private Bike_Controls_Input controls;

    public float Throttle { get; private set; }
    public float Brake { get; private set; }
    public float Steering { get; private set; }

    public bool Handbrake { get; private set; }
    public bool Reset { get; private set; }

    private void Awake()
    {
        controls = new Bike_Controls_Input();
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Bike_Controls_Input();
        }

        controls.Bike.Enable();
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Bike.Disable();
        }
    }

    private void Update()
    {
        ReadInput();
    }

    private void ReadInput()
    {
        Throttle = Mathf.Clamp01(
            controls.Bike.Throttle.ReadValue<float>()
        );

        Brake = Mathf.Clamp01(
            controls.Bike.Brake.ReadValue<float>()
        );

        Steering = Mathf.Clamp(
            controls.Bike.Steering.ReadValue<float>(),
            -1f,
            1f
        );

        Handbrake =
            controls.Bike.Handbrake.IsPressed();

        Reset =
            controls.Bike.Reset.IsPressed();
    }
}