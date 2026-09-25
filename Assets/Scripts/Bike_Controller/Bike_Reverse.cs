using UnityEngine;

public class Bike_Reverse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_Input input;
    [SerializeField] private Bike_Data data;

    [Header("Reverse Settings")]
    [Min(0f)]
    [SerializeField] private float reverseAcceleration = 8f;

    [Min(0f)]
    [SerializeField] private float reverseStartSpeed = 0.1f;

    private Rigidbody bikeRigidbody;

    private void Awake()
    {
        bikeRigidbody = GetComponent<Rigidbody>();

        if (bikeRigidbody == null)
        {
            enabled = false;
            return;
        }

        if (input == null)
        {
            input = GetComponent<Bike_Input>();
        }

        if (input == null)
        {
            enabled = false;
            return;
        }

        if (data == null)
        {
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        float forwardSpeed = GetForwardSpeed();

        if (input.Brake <= 0.001f)
        {
            return;
        }

        // If the bike is still moving forward, keep braking it to a stop.
        if (forwardSpeed > reverseStartSpeed)
        {
            float brakingForce =
                data.brakeForce *
                input.Brake;

            bikeRigidbody.AddForce(
                -transform.forward *
                brakingForce,
                ForceMode.Acceleration
            );
            return;
        }

        // If the bike is already moving backward, limit the reverse speed.
        if (forwardSpeed < -data.reverseSpeed)
        {
            bikeRigidbody.AddForce(
                transform.forward *
                data.brakeForce *
                input.Brake,
                ForceMode.Acceleration
            );
            return;
        }

        // Once the bike is stopped or nearly stopped and the brake remains held,
        // it should auto-shift into reverse like a real bike.
        float reverseForce =
            reverseAcceleration *
            input.Brake;

        bikeRigidbody.AddForce(
            -transform.forward *
            reverseForce,
            ForceMode.Acceleration
        );
    }

    private float GetForwardSpeed()
    {
        return Vector3.Dot(
            bikeRigidbody.linearVelocity,
            transform.forward
        );
    }
}
