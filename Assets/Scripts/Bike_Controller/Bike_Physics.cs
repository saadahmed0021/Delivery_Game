using UnityEngine;

public class Bike_Physics : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_Data data;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheckPoint;

    private Rigidbody bikeRigidbody;

    public bool IsGrounded { get; private set; }

    private float currentBalanceStrength;

    private void Awake()
    {
        bikeRigidbody = GetComponent<Rigidbody>();

        if (bikeRigidbody == null)
        {
            Debug.LogError(
                "Bike_Physics requires a Rigidbody.",
                this
            );

            enabled = false;
            return;
        }

        if (data == null)
        {
            Debug.LogError(
                "Bike_Data is not assigned in Bike_Physics.",
                this
            );

            enabled = false;
            return;
        }

        // Rigidbody configuration.
        bikeRigidbody.mass = data.mass;

        // We use our own gravity handling.
        bikeRigidbody.useGravity = false;

        // Use interpolation for smoother visual motion.
        bikeRigidbody.interpolation =
            RigidbodyInterpolation.Interpolate;

        // Prevent unrealistic physics sleep while balancing.
        bikeRigidbody.sleepThreshold = 0.005f;

        // Apply the configured center of mass.
        ApplyCenterOfMass();
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        if (!IsGrounded)
        {
            ApplyGravity();
        }

    ApplyLowSpeedBalance();
    }

    // =========================================================
    // CENTER OF MASS
    // =========================================================

    private void ApplyCenterOfMass()
    {
        Vector3 centerOfMass =
            data.centerOfMassOffset;

        centerOfMass.y =
            -Mathf.Abs(data.centerOfMassHeight);

        bikeRigidbody.centerOfMass =
            centerOfMass;
    }

    // =========================================================
    // GROUND DETECTION
    // =========================================================

    private void CheckGrounded()
    {
        Vector3 origin;

        if (groundCheckPoint != null)
        {
            origin = groundCheckPoint.position;
        }
        else
        {
            origin = transform.position;
        }

        IsGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            data.groundCheckDistance,
            data.groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    // =========================================================
    // GRAVITY
    // =========================================================

    private void ApplyGravity()
    {
        if (data.gravityMultiplier <= 0f)
        {
            return;
        }

        bikeRigidbody.AddForce(
            Physics.gravity *
            data.gravityMultiplier,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // LOW-SPEED / IDLE BALANCE
    // =========================================================

    private void ApplyLowSpeedBalance()
    {
        if (!IsGrounded)
        {
            return;
        }

        float speed =
            GetHorizontalSpeed();

        // No artificial balance once the bike is properly moving.
        if (speed >= data.lowSpeedBalanceSpeed)
        {
            currentBalanceStrength = 0f;
            return;
        }

        // Calculate how much balance assistance is required.
        if (speed <= data.idleBalanceSpeed)
        {
            currentBalanceStrength =
                data.idleBalanceStrength;
        }
        else
        {
            float t =
                Mathf.InverseLerp(
                    data.lowSpeedBalanceSpeed,
                    data.idleBalanceSpeed,
                    speed
                );

            currentBalanceStrength =
                Mathf.Lerp(
                    0f,
                    data.idleBalanceStrength,
                    t
                );

            currentBalanceStrength =
                Mathf.Max(
                    currentBalanceStrength,
                    data.lowSpeedBalanceStrength
                );
        }

        ApplyUprightTorque(
            currentBalanceStrength
        );
    }

    // =========================================================
    // UPRIGHT TORQUE
    // =========================================================

    private void ApplyUprightTorque(
        float balanceStrength
    )
    {
        Vector3 bikeUp =
            transform.up;

        Vector3 worldUp =
            Vector3.up;

        Vector3 correctionAxis =
            Vector3.Cross(
                bikeUp,
                worldUp
            );

        if (correctionAxis.sqrMagnitude <= 0.000001f)
        {
            ApplyRollDamping();
            return;
        }

        float tiltAmount =
            correctionAxis.magnitude;

        Vector3 correctionTorque =
            correctionAxis.normalized *
            tiltAmount *
            balanceStrength;

        // Only damp roll-related angular velocity.
        Vector3 localAngularVelocity =
            transform.InverseTransformDirection(
                bikeRigidbody.angularVelocity
            );

        float rollAngularVelocity =
            localAngularVelocity.z;

        Vector3 rollDampingTorque =
            -transform.forward *
            rollAngularVelocity *
            data.balanceDamping;

        bikeRigidbody.AddTorque(
            correctionTorque +
            rollDampingTorque,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // ROLL DAMPING
    // =========================================================

    private void ApplyRollDamping()
    {
        Vector3 localAngularVelocity =
            transform.InverseTransformDirection(
                bikeRigidbody.angularVelocity
            );

        float rollAngularVelocity =
            localAngularVelocity.z;

        if (Mathf.Abs(rollAngularVelocity) <= 0.001f)
        {
            return;
        }

        Vector3 dampingTorque =
            -transform.forward *
            rollAngularVelocity *
            data.balanceDamping;

        bikeRigidbody.AddTorque(
            dampingTorque,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // HORIZONTAL SPEED
    // =========================================================

    private float GetHorizontalSpeed()
    {
        Vector3 velocity =
            bikeRigidbody.linearVelocity;

        velocity.y = 0f;

        return velocity.magnitude;
    }

    // =========================================================
    // DEBUG
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (data == null)
        {
            return;
        }

        Vector3 origin;

        if (groundCheckPoint != null)
        {
            origin = groundCheckPoint.position;
        }
        else
        {
            origin = transform.position;
        }

        Gizmos.color =
            IsGrounded
                ? Color.green
                : Color.red;

        Gizmos.DrawLine(
            origin,
            origin +
            Vector3.down *
            data.groundCheckDistance
        );

        Gizmos.DrawSphere(
            origin +
            Vector3.down *
            data.groundCheckDistance,
            0.05f
        );
    }
}