using UnityEngine;

public class Bike_Visuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_Steering steering;
    [SerializeField] private Bike_Data data;

    [Header("Visual Parts")]
    [Tooltip("The transform that visually turns the front wheel/handlebars.")]
    [SerializeField] private Transform frontSteeringPivot;

    [Header("Visual Settings")]
    [Tooltip("How quickly the visual steering follows the physical steering.")]
    [Min(0f)]
    [SerializeField] private float visualSteeringSmoothness = 15f;

    private Quaternion initialSteeringPivotRotation;
    private float visualSteeringAngle;

    private void Awake()
    {
        if (steering == null)
        {
            steering = GetComponent<Bike_Steering>();
        }

        if (steering == null)
        {
            Debug.LogError(
                "Bike_Visuals requires a Bike_Steering reference.",
                this
            );

            enabled = false;
            return;
        }

        if (data == null)
        {
            Debug.LogError(
                "Bike_Data is not assigned in Bike_Visuals.",
                this
            );

            enabled = false;
            return;
        }

        if (frontSteeringPivot == null)
        {
            Debug.LogError(
                "Front Steering Pivot is not assigned in Bike_Visuals.",
                this
            );

            enabled = false;
            return;
        }

        // Store the model's original local rotation.
        // This allows the steering angle to be added without
        // destroying the model's original orientation.
        initialSteeringPivotRotation =
            frontSteeringPivot.localRotation;

        visualSteeringAngle = 0f;
    }

    private void Update()
    {
        UpdateSteeringVisual();
    }

    // =========================================================
    // STEERING VISUAL
    // =========================================================

    private void UpdateSteeringVisual()
    {
        float targetAngle =
            steering.CurrentSteeringAngle;

        // Smooth visual movement.
        visualSteeringAngle =
            Mathf.Lerp(
                visualSteeringAngle,
                targetAngle,
                1f -
                Mathf.Exp(
                    -visualSteeringSmoothness *
                    Time.deltaTime
                )
            );

        ApplySteeringVisual();
    }

    // =========================================================
    // APPLY STEERING
    // =========================================================

    private void ApplySteeringVisual()
    {
        frontSteeringPivot.localRotation =
            initialSteeringPivotRotation *
            Quaternion.AngleAxis(
                visualSteeringAngle,
                Vector3.forward
            );
    }

    // =========================================================
    // RESET
    // =========================================================

    public void ResetVisuals()
    {
        visualSteeringAngle = 0f;

        if (frontSteeringPivot != null)
        {
            frontSteeringPivot.localRotation =
                initialSteeringPivotRotation;
        }
    }
}