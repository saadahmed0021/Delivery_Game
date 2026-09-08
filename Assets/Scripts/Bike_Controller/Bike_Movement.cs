using UnityEngine;

public class Bike_Movement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_Input input;
    [SerializeField] private Bike_Data data;
    [SerializeField] private Bike_Steering steering;

    [Header("Debug")]
    [SerializeField] private float currentSpeed;

    private Rigidbody bikeRigidbody;

    private void Awake()
    {
        bikeRigidbody = GetComponent<Rigidbody>();

        if (bikeRigidbody == null)
        {
            Debug.LogError(
                "Bike_Movement requires a Rigidbody.",
                this
            );

            enabled = false;
            return;
        }

        if (input == null)
        {
            Debug.LogError(
                "Bike_Input is not assigned in Bike_Movement.",
                this
            );

            enabled = false;
            return;
        }

        if (data == null)
        {
            Debug.LogError(
                "Bike_Data is not assigned in Bike_Movement.",
                this
            );

            enabled = false;
            return;
        }

        if (steering == null)
        {
            steering = GetComponent<Bike_Steering>();
        }

        if (steering == null)
        {
            Debug.LogError(
                "Bike_Movement requires a Bike_Steering component.",
                this
            );

            enabled = false;
            return;
        }

        bikeRigidbody.mass = data.mass;
    }

    private void FixedUpdate()
    {
        ApplyThrottle();
        ApplyEngineBraking();
        ApplyBraking();
        ApplyRollingResistance();
        ApplyAerodynamicDrag();

        ApplySteering();

        LimitForwardSpeed();
        UpdateSpeedDisplay();
    }

    private void UpdateSpeedDisplay()
    {
        currentSpeed = GetForwardSpeed();
    }

    // =========================================================
    // THROTTLE
    // =========================================================

    private void ApplyThrottle()
    {
        if (input.Throttle <= 0f)
        {
            return;
        }

        float forwardSpeed = GetForwardSpeed();

        if (forwardSpeed >= data.maxSpeed)
        {
            return;
        }

        if (forwardSpeed < -0.1f)
        {
            return;
        }

        float speedFactor =
            1f -
            Mathf.Clamp01(
                forwardSpeed / data.maxSpeed
            );

        float acceleration =
            data.acceleration *
            input.Throttle *
            speedFactor;

        bikeRigidbody.AddForce(
            transform.forward * acceleration,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // ENGINE BRAKING
    // =========================================================

    private void ApplyEngineBraking()
    {
        if (input.Throttle > 0.01f)
        {
            return;
        }

        if (input.Brake > 0.01f)
        {
            return;
        }

        float forwardSpeed = GetForwardSpeed();

        if (forwardSpeed <= 0.01f)
        {
            return;
        }

        bikeRigidbody.AddForce(
            -transform.forward *
            data.engineBraking,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // BRAKING
    // =========================================================

    private void ApplyBraking()
    {
        if (input.Brake <= 0.01f)
        {
            return;
        }

        float forwardSpeed = GetForwardSpeed();

        if (Mathf.Abs(forwardSpeed) <= 0.01f)
        {
            return;
        }

        float brakeDirection =
            Mathf.Sign(forwardSpeed);

        float currentSpeed =
            Mathf.Abs(forwardSpeed);

        // Stronger braking at higher speed, but quickly taper off near zero
        // so the bike decelerates smoothly instead of stopping instantly.
        float brakeForce =
            data.brakeForce *
            input.Brake;

        float maxBrakeForce =
            Mathf.Lerp(
                2f,
                12f,
                Mathf.Clamp01(
                    currentSpeed /
                    Mathf.Max(1f, data.maxSpeed)
                )
            );

        brakeForce =
            Mathf.Min(
                brakeForce,
                maxBrakeForce
            );

        bikeRigidbody.AddForce(
            -transform.forward *
            brakeDirection *
            brakeForce,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // STEERING
    // =========================================================

    private void ApplySteering()
    {
        float steeringAngle =
            steering.CurrentSteeringAngle;

        if (Mathf.Abs(steeringAngle) <= 0.01f)
        {
            return;
        }

        Vector3 velocity =
            bikeRigidbody.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        float speed =
            horizontalVelocity.magnitude;

        // Don't rotate the bike while it is essentially stopped.
        if (speed <= 0.1f)
        {
            return;
        }

        float forwardSpeed =
            GetForwardSpeed();

        if (Mathf.Abs(forwardSpeed) <= 0.01f)
        {
            return;
        }

        /*
         * Steering angle controls how quickly the bike
         * changes its heading.
         *
         * This is deliberately speed-dependent:
         * - very slow = sharper turning
         * - faster = smoother turning
         */

        float speedFactor =
            Mathf.Clamp01(
                speed / data.steeringSpeedReference
            );

        float turnRate =
            Mathf.Lerp(
                data.steeringSpeed * 0.35f,
                data.steeringSpeed,
                speedFactor
            );

        float turnAmount =
            steeringAngle /
            data.maxSteeringAngle;

        turnAmount =
            Mathf.Clamp(
                turnAmount,
                -1f,
                1f
            );

        float rotationAmount =
            turnAmount *
            turnRate *
            Mathf.Sign(forwardSpeed) *
            Time.fixedDeltaTime;

        // Rotate the bike's heading.
        Quaternion rotation =
            Quaternion.Euler(
                0f,
                rotationAmount,
                0f
            );

        Vector3 newVelocity =
            rotation *
            horizontalVelocity;

        // Rotate the Rigidbody's velocity to follow
        // the direction of the steering.
        bikeRigidbody.linearVelocity =
            new Vector3(
                newVelocity.x,
                velocity.y,
                newVelocity.z
            );

        // Reverse steering turns the bike's heading in the
        // opposite direction while it moves backward.
        Quaternion targetRotation =
            bikeRigidbody.rotation *
            rotation;

        bikeRigidbody.MoveRotation(
            targetRotation
        );
    }

    // =========================================================
    // ROLLING RESISTANCE
    // =========================================================

    private void ApplyRollingResistance()
    {
        Vector3 velocity =
            bikeRigidbody.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        if (horizontalVelocity.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float forwardSpeed =
            GetForwardSpeed();

        if (Mathf.Abs(forwardSpeed) <= 0.01f)
        {
            return;
        }

        bikeRigidbody.AddForce(
            -transform.forward *
            Mathf.Sign(forwardSpeed) *
            data.rollingResistance,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // AERODYNAMIC DRAG
    // =========================================================

    private void ApplyAerodynamicDrag()
    {
        Vector3 velocity =
            bikeRigidbody.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        float speed =
            horizontalVelocity.magnitude;

        if (speed <= 0.01f)
        {
            return;
        }

        Vector3 dragForce =
            -horizontalVelocity *
            speed *
            data.aerodynamicDrag;

        bikeRigidbody.AddForce(
            dragForce,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // SPEED LIMIT
    // =========================================================

    private void LimitForwardSpeed()
    {
        Vector3 velocity =
            bikeRigidbody.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        float speed =
            horizontalVelocity.magnitude;

        if (speed <= data.maxSpeed)
        {
            return;
        }

        if (horizontalVelocity.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Vector3 limitedDirection =
            horizontalVelocity.normalized;

        bikeRigidbody.linearVelocity =
            new Vector3(
                limitedDirection.x * data.maxSpeed,
                velocity.y,
                limitedDirection.z * data.maxSpeed
            );
    }

    // =========================================================
    // SPEED
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

}