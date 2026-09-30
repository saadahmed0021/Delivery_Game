using UnityEngine;

public class Bike_Movement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_Input input;
    [SerializeField] private Bike_Data data;
    [SerializeField] private Bike_Steering steering;

    [Header("Lateral Grip")]
    [SerializeField] private float lateralGrip = 12f;

    [Header("Debug")]
    [SerializeField] private float currentSpeed;

    public float CurrentSpeed => currentSpeed;

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
            enabled = false;
            return;
        }

        if (data == null)
        {
            enabled = false;
            return;
        }

        if (steering == null)
        {
            steering = GetComponent<Bike_Steering>();
        }

        if (steering == null)
        {
            enabled = false;
            return;
        }

        bikeRigidbody.mass = data.mass;
    }

    private void FixedUpdate()
    {
        ApplyThrottle();
        ApplyBraking();
        ApplyRollingResistance();
        ApplyAerodynamicDrag();
        ApplySteering();
        ApplyLateralGrip();
        LimitForwardSpeed();
        UpdateSpeedDisplay();
    }

    private void ApplyThrottle()
    {
        if (input.Throttle <= 0.001f)
        {
            return;
        }

        float forwardSpeed = GetForwardSpeed();

        if (forwardSpeed < -0.1f)
        {
            return;
        }

        if (forwardSpeed >= data.maxSpeed)
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

    private void ApplyBraking()
    {
        if (input.Brake <= 0.001f)
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

        float brakeForce =
            data.brakeForce *
            input.Brake;

        bikeRigidbody.AddForce(
            -transform.forward *
            brakeDirection *
            brakeForce,
            ForceMode.Acceleration
        );
    }

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

    private void ApplySteering()
    {
        float steeringAngle =
            steering.CurrentSteeringAngle;

        float forwardSpeed =
            GetForwardSpeed();

        if (Mathf.Abs(forwardSpeed) <= 0.01f)
        {
            return;
        }

        if (Mathf.Abs(steeringAngle) <= 0.01f)
        {
            return;
        }

        float speed =
            Mathf.Abs(forwardSpeed);

        float speedFactor =
            Mathf.Clamp01(
                speed /
                data.steeringSpeedReference
            );

        float turnRate =
            Mathf.Lerp(
                data.steeringSpeed * 0.35f,
                data.steeringSpeed,
                speedFactor
            );

        float turnAmount =
            steeringAngle /
            Mathf.Max(
                data.maxSteeringAngle,
                0.001f
            );

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

        Quaternion rotation =
            Quaternion.Euler(
                0f,
                rotationAmount,
                0f
            );

        bikeRigidbody.MoveRotation(
            bikeRigidbody.rotation *
            rotation
        );

        Vector3 velocity =
            bikeRigidbody.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        Vector3 newVelocity =
            rotation *
            horizontalVelocity;

        bikeRigidbody.linearVelocity =
            new Vector3(
                newVelocity.x,
                velocity.y,
                newVelocity.z
            );
    }

    private void ApplyLateralGrip()
    {
        Vector3 localVelocity =
            transform.InverseTransformDirection(
                bikeRigidbody.linearVelocity
            );

        localVelocity.x =
            Mathf.Lerp(
                localVelocity.x,
                0f,
                lateralGrip *
                Time.fixedDeltaTime
            );

        bikeRigidbody.linearVelocity =
            transform.TransformDirection(
                localVelocity
            );
    }

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

        Vector3 limitedVelocity =
            horizontalVelocity.normalized *
            data.maxSpeed;

        bikeRigidbody.linearVelocity =
            new Vector3(
                limitedVelocity.x,
                velocity.y,
                limitedVelocity.z
            );
    }

    private float GetForwardSpeed()
    {
        return Vector3.Dot(
            bikeRigidbody.linearVelocity,
            transform.forward
        );
    }

    private void UpdateSpeedDisplay()
    {
        currentSpeed =
            GetForwardSpeed();
    }
}