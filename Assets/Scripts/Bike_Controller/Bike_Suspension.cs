using UnityEngine;

public class Bike_Suspension : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_Data data;
    [SerializeField] private Rigidbody bikeRigidbody;

    [Header("Wheel")]
    [SerializeField] private bool isFrontWheel = true;
    [SerializeField] private float wheelRadius = 0.3f;

    [Header("Suspension Settings")]
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }
    public Vector3 GroundPoint { get; private set; }
    public Vector3 GroundNormal { get; private set; }

    /// <summary>
    /// 0 = fully extended
    /// 1 = fully compressed
    /// </summary>
    public float Compression { get; private set; }

    /// <summary>
    /// Current suspension length in meters.
    /// </summary>
    public float SuspensionLength { get; private set; }

    private RaycastHit groundHit;

    private void Awake()
    {
        if (bikeRigidbody == null)
        {
            bikeRigidbody =
                GetComponentInParent<Rigidbody>();
        }

        if (bikeRigidbody == null)
        {
            Debug.LogError(
                "Bike_Suspension requires a Rigidbody in the parent hierarchy.",
                this
            );

            enabled = false;
            return;
        }

        if (data == null)
        {
            Debug.LogError(
                "Bike_Data is not assigned in Bike_Suspension.",
                this
            );

            enabled = false;
            return;
        }

        // Use the appropriate wheel radius from Bike_Data.
        wheelRadius =
            isFrontWheel
                ? data.frontWheelRadius
                : data.rearWheelRadius;

        // Use the same ground layer as the rest of the bike.
        groundLayer =
            data.groundLayer;
    }

    private void FixedUpdate()
    {
        DetectGround();
        ApplySuspensionForce();
    }

    // =========================================================
    // GROUND DETECTION
    // =========================================================

    private void DetectGround()
    {
        Vector3 origin =
            transform.position;

        Vector3 suspensionDirection =
            -transform.up;

        float rayLength =
            GetSuspensionDistance() +
            wheelRadius;

        IsGrounded = Physics.Raycast(
            origin,
            suspensionDirection,
            out groundHit,
            rayLength,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        if (!IsGrounded)
        {
            ResetSuspensionData();
            return;
        }

        GroundPoint =
            groundHit.point;

        GroundNormal =
            groundHit.normal;

        CalculateSuspensionCompression();
    }

    // =========================================================
    // COMPRESSION
    // =========================================================

    private void CalculateSuspensionCompression()
    {
        float distanceToGround =
            groundHit.distance;

        float suspensionLength =
            distanceToGround -
            wheelRadius;

        float maximumSuspensionLength =
            GetSuspensionDistance();

        SuspensionLength =
            Mathf.Clamp(
                suspensionLength,
                0f,
                maximumSuspensionLength
            );

        Compression =
            Mathf.Clamp01(
                1f -
                SuspensionLength /
                maximumSuspensionLength
            );
    }

    // =========================================================
    // SUSPENSION FORCE
    // =========================================================

    private void ApplySuspensionForce()
    {
        if (!IsGrounded)
        {
            return;
        }

        if (Compression <= 0f)
        {
            return;
        }

        Vector3 suspensionDirection =
            transform.up;

        // Velocity of the suspension point.
        Vector3 pointVelocity =
            bikeRigidbody.GetPointVelocity(
                transform.position
            );

        // Velocity along the suspension axis.
        float suspensionVelocity =
            Vector3.Dot(
                pointVelocity,
                suspensionDirection
            );

        float suspensionStrength =
            GetSuspensionStrength();

        float suspensionDamping =
            GetSuspensionDamping();

        // Spring force.
        float springForce =
            Compression *
            suspensionStrength;

        // Damping force.
        float dampingForce =
            suspensionVelocity *
            suspensionDamping;

        // Total force.
        float totalForce =
            springForce -
            dampingForce;

        // Suspension cannot pull the bike toward the ground.
        if (totalForce <= 0f)
        {
            return;
        }

        bikeRigidbody.AddForceAtPosition(
            suspensionDirection *
            totalForce,
            transform.position,
            ForceMode.Force
        );
    }

    // =========================================================
    // RESET
    // =========================================================

    private void ResetSuspensionData()
    {
        GroundPoint = Vector3.zero;
        GroundNormal = Vector3.up;

        Compression = 0f;

        SuspensionLength =
            GetSuspensionDistance();
    }

    // =========================================================
    // DATA
    // =========================================================

    private float GetSuspensionDistance()
    {
        if (isFrontWheel)
        {
            return data.frontSuspensionDistance;
        }

        return data.rearSuspensionDistance;
    }

    private float GetSuspensionStrength()
    {
        if (isFrontWheel)
        {
            return data.frontSuspensionStrength;
        }

        return data.rearSuspensionStrength;
    }

    private float GetSuspensionDamping()
    {
        if (isFrontWheel)
        {
            return data.frontSuspensionDamping;
        }

        return data.rearSuspensionDamping;
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

        float radius =
            isFrontWheel
                ? data.frontWheelRadius
                : data.rearWheelRadius;

        float suspensionDistance =
            isFrontWheel
                ? data.frontSuspensionDistance
                : data.rearSuspensionDistance;

        float rayLength =
            suspensionDistance +
            radius;

        Gizmos.color =
            IsGrounded
                ? Color.green
                : Color.red;

        Gizmos.DrawLine(
            transform.position,
            transform.position -
            transform.up *
            rayLength
        );

        if (IsGrounded)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(
                GroundPoint,
                0.05f
            );

            Gizmos.color = Color.blue;

            Gizmos.DrawRay(
                GroundPoint,
                GroundNormal *
                0.3f
            );
        }
    }
}