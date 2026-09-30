using UnityEngine;

public class Bike_Lean : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bodyPivot;
    [SerializeField] private Bike_Movement movement;
    [SerializeField] private Bike_Steering steering;
    [SerializeField] private Bike_Data data;

    [Header("Lean Settings")]
    [SerializeField] private bool useSpeedLimit = true;

    private Quaternion initialBodyRotation;
    private float currentLean;
    private bool isLeaning;

    private void Awake()
    {
        if (movement == null)
        {
            movement = GetComponent<Bike_Movement>();
        }

        if (steering == null)
        {
            steering = GetComponent<Bike_Steering>();
        }

        if (bodyPivot == null ||
            movement == null ||
            steering == null ||
            data == null)
        {
            enabled = false;
            return;
        }

        initialBodyRotation = bodyPivot.localRotation;
    }

    private void LateUpdate()
    {
        UpdateLeanState();
        UpdateLean();
    }

    private void UpdateLeanState()
    {
        if (!useSpeedLimit)
        {
            isLeaning = true;
            return;
        }

        float speed = Mathf.Abs(movement.CurrentSpeed);

        isLeaning =
            speed > data.leanActivationSpeed;
    }

    private void UpdateLean()
    {
        float targetLean = 0f;

        if (isLeaning)
        {
            targetLean = CalculateTargetLean();
        }

        float smoothSpeed =
            1f -
            Mathf.Exp(
                -data.leanSmoothness *
                Time.deltaTime
            );

        currentLean = Mathf.Lerp(
            currentLean,
            targetLean,
            smoothSpeed
        );

        ApplyLean();
    }

    private float CalculateTargetLean()
    {
        float speed =
            Mathf.Abs(movement.CurrentSpeed);

        float leanRange =
            Mathf.Max(
                data.leanSpeedReference -
                data.leanActivationSpeed,
                0.01f
            );

        float speedFactor =
            Mathf.Clamp01(
                (speed - data.leanActivationSpeed) /
                leanRange
            );

        float targetLean =
            -steering.SteeringNormalized *
            data.maxLeanAngle *
            speedFactor;

        return targetLean;
    }

    private void ApplyLean()
    {
        bodyPivot.localRotation =
            initialBodyRotation *
            Quaternion.AngleAxis(
                currentLean,
                Vector3.forward
            );
    }

    private void OnDisable()
    {
        if (bodyPivot != null)
        {
            bodyPivot.localRotation =
                initialBodyRotation;
        }
    }
}