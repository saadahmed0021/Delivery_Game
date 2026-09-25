using UnityEngine;

public class Bike_Steering : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_Input input;
    [SerializeField] private Bike_Data data;

    private Rigidbody bikeRigidbody;

    private float currentSteeringAngle;

    /// <summary>
    /// Current front-wheel steering angle in degrees.
    /// Negative = left, positive = right.
    /// </summary>
    public float CurrentSteeringAngle => currentSteeringAngle;

    /// <summary>
    /// Steering angle normalized from -1 to +1.
    /// </summary>
    public float SteeringNormalized
    {
        get
        {
            if (data.maxSteeringAngle <= 0.001f)
            {
                return 0f;
            }

            return Mathf.Clamp(
                currentSteeringAngle /
                data.maxSteeringAngle,
                -1f,
                1f
            );
        }
    }

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
            enabled = false;
            return;
        }

        if (data == null)
        {
            enabled = false;
            return;
        }
    }

    private void FixedUpdate()
    {
        UpdateSteeringAngle();
    }

    // =========================================================
    // STEERING
    // =========================================================

    private void UpdateSteeringAngle()
    {
        float targetAngle =
            CalculateTargetSteeringAngle();

        float steeringRate =
            GetSteeringRate();

        currentSteeringAngle =
            Mathf.MoveTowards(
                currentSteeringAngle,
                targetAngle,
                steeringRate *
                Time.fixedDeltaTime
            );
    }

    private float CalculateTargetSteeringAngle()
    {
        float speed =
            GetForwardSpeed();

        float absoluteSpeed =
            Mathf.Abs(speed);

        // At very low speed the handlebars can reach
        // their full steering angle.
        if (absoluteSpeed <= data.steeringSpeedReference)
        {
            return input.Steering *
                   data.maxSteeringAngle *
                   data.lowSpeedSteeringMultiplier;
        }

        // Reduce steering angle progressively as speed increases.
        float speedFactor =
            Mathf.Clamp01(
                absoluteSpeed /
                data.steeringSpeedReference
            );

        float steeringMultiplier =
            Mathf.Lerp(
                data.lowSpeedSteeringMultiplier,
                data.highSpeedSteeringMultiplier,
                speedFactor
            );

        return input.Steering *
               data.maxSteeringAngle *
               steeringMultiplier;
    }

    // =========================================================
    // STEERING SPEED
    // =========================================================

    private float GetSteeringRate()
    {
        float speed =
            Mathf.Abs(GetForwardSpeed());

        // Faster steering response at low speed.
        // Slightly slower response at higher speed.
        float speedFactor =
            Mathf.Clamp01(
                speed /
                data.steeringSpeedReference
            );

        float responseMultiplier =
            Mathf.Lerp(
                1f,
                0.65f,
                speedFactor
            );

        return data.steeringSpeed *
               responseMultiplier;
    }

    // =========================================================
    // FORWARD SPEED
    // =========================================================

    private float GetForwardSpeed()
    {
        Vector3 velocity =
            bikeRigidbody.linearVelocity;

        return Vector3.Dot(
            velocity,
            transform.forward
        );
    }

    // =========================================================
    // RESET
    // =========================================================

    public void ResetSteering()
    {
        currentSteeringAngle = 0f;
    }
}