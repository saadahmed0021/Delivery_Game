using UnityEngine;

[CreateAssetMenu(fileName = "Bike_Data", menuName = "Bike/Bike Data")]
public class Bike_Data : ScriptableObject
{
    [Header("Speed")]
    [Min(0f)] public float maxSpeed = 20f;
    [Min(0f)] public float acceleration = 3f;
    [Min(0f)] public float engineBraking = 0.08f;
    [Min(0f)] public float brakeForce = 15f;
    [Min(0f)] public float reverseSpeed = 3f;

    [Header("Steering")]
    [Min(0f)] public float maxSteeringAngle = 30f;
    [Min(0f)] public float steeringSpeed = 120f;
    [Min(0f)] public float lowSpeedSteeringMultiplier = 1f;
    [Min(0f)] public float highSpeedSteeringMultiplier = 0.35f;
    [Min(0.1f)] public float steeringSpeedReference = 20f;

    [Header("Lean")]
    [Tooltip("Bike must be faster than this speed before visual leaning starts.")]
    [Min(0f)] public float leanActivationSpeed = 5f;

    [Tooltip("Maximum visual lean angle in degrees.")]
    [Min(0f)] public float maximumLeanAngle = 30f;

    [Tooltip("How quickly the bike reaches the target lean angle.")]
    [Min(0f)] public float leanSmoothness = 8f;

    [Tooltip("Flip this if the bike leans in the opposite direction.")]
    public bool invertLeanDirection = false;

    [Header("Low Speed Balance")]
    [Min(0f)] public float idleBalanceSpeed = 0.5f;
    [Min(0f)] public float lowSpeedBalanceSpeed = 3f;
    [Min(0f)] public float idleBalanceStrength = 25f;
    [Min(0f)] public float lowSpeedBalanceStrength = 8f;
    [Min(0f)] public float balanceDamping = 4f;

    [Header("Physics")]
    [Min(0.01f)] public float mass = 150f;
    [Min(0f)] public float gravityMultiplier = 1f;
    [Min(0f)] public float centerOfMassHeight = 0.45f;
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.2f, 0f);

    [Header("Tire Grip")]
    [Min(0f)] public float frontTireGrip = 1f;
    [Min(0f)] public float rearTireGrip = 1f;
    [Min(0f)] public float frontTireMaxLateralForce = 2500f;
    [Min(0f)] public float rearTireMaxLateralForce = 2500f;

    [Header("Braking")]
    [Range(0f, 1f)] public float frontBrakeDistribution = 0.7f;
    [Range(0f, 1f)] public float rearBrakeDistribution = 0.3f;
    [Min(0f)] public float frontBrakeGrip = 1f;
    [Min(0f)] public float rearBrakeGrip = 1f;

    [Header("Suspension - Front")]
    [Min(0f)] public float frontSuspensionDistance = 0.2f;
    [Min(0f)] public float frontSuspensionStrength = 5000f;
    [Min(0f)] public float frontSuspensionDamping = 500f;

    [Header("Suspension - Rear")]
    [Min(0f)] public float rearSuspensionDistance = 0.2f;
    [Min(0f)] public float rearSuspensionStrength = 5000f;
    [Min(0f)] public float rearSuspensionDamping = 500f;

    [Header("Wheels")]
    [Min(0.01f)] public float frontWheelRadius = 0.3f;
    [Min(0.01f)] public float rearWheelRadius = 0.3f;
    [Range(0f, 1f)] public float wheelVisualSpinMultiplier = 0.35f;
    [Min(0.1f)] public float wheelbase = 1.4f;

    [Header("Rolling Resistance")]
    [Min(0f)] public float rollingResistance = 0.05f;
    [Min(0f)] public float aerodynamicDrag = 0.003f;

    [Header("Air Control")]
    [Min(0f)] public float airControl = 2f;
    [Min(0f)] public float airRotationSpeed = 3f;

    [Header("Ground Detection")]
    [Min(0.01f)] public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;
}