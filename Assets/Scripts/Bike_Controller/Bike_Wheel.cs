using UnityEngine;

public class Bike_Wheel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_Data data;
    [SerializeField] private Bike_Input input;
    [SerializeField] private Bike_Steering steering;
    [SerializeField] private Rigidbody bikeRigidbody;

    [Header("Wheel")]
    [SerializeField] private bool isFrontWheel = false;

    [Tooltip("Transform used for the wheel's forward/right directions.")]
    [SerializeField] private Transform wheelDirection;

    [Header("Visual")]
    [SerializeField] private Transform wheelVisual;

    [Tooltip("The local axis the wheel mesh rotates around.")]
    [SerializeField] private Vector3 visualRotationAxis = Vector3.right;

    [Header("Braking")]
    [SerializeField] private bool canBrake = true;

    [Header("Engine")]
    [Tooltip("Enable ONLY on the rear wheel.")]
    [SerializeField] private bool receivesEnginePower = false;

    public bool IsGrounded { get; private set; }

    public Vector3 GroundPoint { get; private set; }

    public Vector3 GroundNormal { get; private set; }

    public float ForwardSpeed { get; private set; }

    public float SidewaysSpeed { get; private set; }

    private float wheelRotation;

    private Quaternion initialWheelRotation;
    private Vector3 initialWheelLocalPosition;

    private void Awake()
    {
        if (bikeRigidbody == null)
        {
            bikeRigidbody =
                GetComponentInParent<Rigidbody>();
        }

        if (bikeRigidbody == null)
        {
            enabled = false;
            return;
        }

        if (data == null)
        {
            enabled = false;
            return;
        }

        if (input == null)
        {
            input =
                GetComponentInParent<Bike_Input>();
        }

        if (input == null)
        {
            enabled = false;
            return;
        }

        if (isFrontWheel)
        {
            if (steering == null)
            {
                steering =
                    GetComponentInParent<Bike_Steering>();
            }

            if (steering == null)
            {
                enabled = false;
                return;
            }
        }

        if (wheelDirection == null)
        {
            wheelDirection = transform;
        }

        if (wheelVisual != null)
        {
            initialWheelRotation =
                wheelVisual.localRotation;

            initialWheelLocalPosition =
                wheelVisual.localPosition;
        }
    }

    private void FixedUpdate()
    {
        CheckGround();

        CalculateWheelVelocity();

        if (!IsGrounded)
        {
            return;
        }

        ApplyLateralGrip();
        ApplyEnginePower();
        ApplyBrakes();
    }

    private void Update()
    {
        UpdateWheelVisual();
    }

    // =========================================================
    // GROUND
    // =========================================================

    private void CheckGround()
    {
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;

        float suspensionDistance = GetSuspensionDistance();
        float wheelRadius = GetWheelRadius();
        float rayLength = suspensionDistance + wheelRadius + 0.125f;

        IsGrounded = Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            rayLength,
            data.groundLayer,
            QueryTriggerInteraction.Ignore
        );

        Debug.DrawRay(
            origin,
            direction * rayLength,
            IsGrounded ? Color.green : Color.red
        );

        if (!IsGrounded)
        {
            GroundPoint = Vector3.zero;
            GroundNormal = Vector3.up;

            return;
        }

        GroundPoint = hit.point;
        GroundNormal = hit.normal;
    }

    // =========================================================
    // VELOCITY
    // =========================================================

    private void CalculateWheelVelocity()
    {
        Vector3 velocity =
            bikeRigidbody.GetPointVelocity(
                transform.position
            );

        Vector3 forward =
            GetWheelForward();

        Vector3 right =
            GetWheelRight();

        ForwardSpeed =
            Vector3.Dot(
                velocity,
                forward
            );

        SidewaysSpeed =
            Vector3.Dot(
                velocity,
                right
            );
    }

    // =========================================================
    // TIRE GRIP
    // =========================================================

    private void ApplyLateralGrip()
    {
        Vector3 right =
            GetWheelRight();

        float grip =
            isFrontWheel
                ? data.frontTireGrip
                : data.rearTireGrip;

        float maxForce =
            isFrontWheel
                ? data.frontTireMaxLateralForce
                : data.rearTireMaxLateralForce;

        if (grip <= 0f)
        {
            return;
        }

        float force =
            -SidewaysSpeed *
            grip *
            bikeRigidbody.mass;

        force =
            Mathf.Clamp(
                force,
                -maxForce,
                maxForce
            );

        bikeRigidbody.AddForceAtPosition(
            right * force,
            transform.position,
            ForceMode.Force
        );
    }

    // =========================================================
    // ENGINE POWER
    // =========================================================

    private void ApplyEnginePower()
    {
        if (!receivesEnginePower)
        {
            return;
        }

        if (input.Throttle <= 0.001f)
        {
            return;
        }

        float speed =
            Mathf.Max(
                ForwardSpeed,
                0f
            );

        if (speed >= data.maxSpeed)
        {
            return;
        }

        float speedRatio =
            Mathf.Clamp01(
                speed /
                data.maxSpeed
            );

        float throttle =
            Mathf.Clamp01(
                input.Throttle
            );

        float throttleCurve =
            Mathf.Lerp(
                0.2f,
                1f,
                throttle
            );

        float speedFactor =
            1f -
            speedRatio *
            0.9f;

        float force =
            data.acceleration *
            throttleCurve *
            speedFactor *
            bikeRigidbody.mass;

        bikeRigidbody.AddForceAtPosition(
            GetWheelForward() * force,
            transform.position,
            ForceMode.Force
        );
    }

    // =========================================================
    // BRAKES
    // =========================================================

    private void ApplyBrakes()
    {
        if (!canBrake)
        {
            return;
        }

        if (input.Brake <= 0.001f)
        {
            return;
        }

        if (Mathf.Abs(ForwardSpeed) <= 0.01f)
        {
            return;
        }

        float distribution =
            isFrontWheel
                ? data.frontBrakeDistribution
                : data.rearBrakeDistribution;

        float grip =
            isFrontWheel
                ? data.frontBrakeGrip
                : data.rearBrakeGrip;

        float force =
            data.brakeForce *
            input.Brake *
            distribution *
            grip *
            bikeRigidbody.mass;

        float direction =
            Mathf.Sign(
                ForwardSpeed
            );

        bikeRigidbody.AddForceAtPosition(
            -GetWheelForward() *
            direction *
            force,
            transform.position,
            ForceMode.Force
        );
    }

    // =========================================================
    // WHEEL DIRECTIONS
    // =========================================================

    private Vector3 GetWheelForward()
    {
        Vector3 forward =
            wheelDirection.forward;

        forward =
            Vector3.ProjectOnPlane(
                forward,
                GroundNormal
            );

        if (forward.sqrMagnitude < 0.0001f)
        {
            forward =
                Vector3.ProjectOnPlane(
                    transform.forward,
                    GroundNormal
                );
        }

        return forward.normalized;
    }

    private Vector3 GetWheelRight()
    {
        Vector3 right =
            wheelDirection.right;

        right =
            Vector3.ProjectOnPlane(
                right,
                GroundNormal
            );

        if (right.sqrMagnitude < 0.0001f)
        {
            right =
                Vector3.ProjectOnPlane(
                    transform.right,
                    GroundNormal
                );
        }

        return right.normalized;
    }

    // =========================================================
    // WHEEL VISUAL ROTATION
    // =========================================================

    private void UpdateWheelVisual()
    {
        if (wheelVisual == null)
        {
            return;
        }

        float radius =
            GetWheelRadius();

        if (radius <= 0.001f)
        {
            return;
        }

        float rotationDegrees =
            (ForwardSpeed / radius) *
            Mathf.Rad2Deg *
            Time.deltaTime *
            data.wheelVisualSpinMultiplier;

        wheelRotation +=
            rotationDegrees;

        wheelRotation =
            Mathf.Repeat(
                wheelRotation,
                360f
            );

        wheelVisual.localRotation =
            initialWheelRotation *
            Quaternion.AngleAxis(
                wheelRotation,
                visualRotationAxis.normalized
            );

        UpdateWheelSuspensionVisual();
    }

    // =========================================================
    // VISUAL SUSPENSION
    // =========================================================

    private void UpdateWheelSuspensionVisual()
    {
        Vector3 position =
            initialWheelLocalPosition;

        if (IsGrounded)
        {
            float suspensionDistance =
                GetSuspensionDistance();

            float distance =
                Vector3.Distance(
                    transform.position,
                    GroundPoint
                );

            float suspensionLength =
                Mathf.Clamp(
                    distance -
                    GetWheelRadius(),
                    0f,
                    suspensionDistance
                );

            float compression =
                suspensionDistance > 0.001f
                    ? 1f -
                      suspensionLength /
                      suspensionDistance
                    : 0f;

            position.y -=
                compression *
                suspensionDistance;
        }

        wheelVisual.localPosition =
            position;
    }

    // =========================================================
    // DATA
    // =========================================================

    private float GetWheelRadius()
    {
        return isFrontWheel
            ? data.frontWheelRadius
            : data.rearWheelRadius;
    }

    private float GetSuspensionDistance()
    {
        return isFrontWheel
            ? data.frontSuspensionDistance
            : data.rearSuspensionDistance;
    }

    // =========================================================
    // RESET
    // =========================================================

    public void ResetWheelVisual()
    {
        wheelRotation = 0f;

        if (wheelVisual == null)
        {
            return;
        }

        wheelVisual.localRotation =
            initialWheelRotation;

        wheelVisual.localPosition =
            initialWheelLocalPosition;
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

        float rayLength =
            GetSuspensionDistance() +
            GetWheelRadius();

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
            Gizmos.color =
                Color.yellow;

            Gizmos.DrawSphere(
                GroundPoint,
                0.05f
            );
        }
    }
}
