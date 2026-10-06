using UnityEngine;

public class Bike_CameraOrbit : MonoBehaviour
{
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private float returnDelay = 1f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 45f;
    [SerializeField, Min(0f)] private float reverseSpeedThreshold = 0.5f;

    private float currentYaw;
    private float currentPitch;

    private float originalYaw;
    private float originalPitch;

    private float idleTimer;
    private float reverseYawOffset;
    private Rigidbody bikeRigidbody;

    private void Start()
    {
        bikeRigidbody = GetComponentInParent<Rigidbody>();

        Vector3 rotation = transform.localEulerAngles;

        originalYaw = rotation.y;
        originalPitch = NormalizeAngle(rotation.x);

        currentYaw = originalYaw;
        currentPitch = originalPitch;
    }

    private void LateUpdate()
    {
        bool isUIOpen = Cursor_Controller.Instance != null &&
                        Cursor_Controller.Instance.IsUIOpen;

        if (!isUIOpen)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            bool mouseMoving =
                Mathf.Abs(mouseX) > 0.01f ||
                Mathf.Abs(mouseY) > 0.01f;

            if (mouseMoving)
            {
                currentYaw += mouseX * sensitivity;
                currentPitch -= mouseY * sensitivity;

                currentPitch = Mathf.Clamp(
                    currentPitch,
                    minPitch,
                    maxPitch
                );

                idleTimer = 0f;
            }
            else
            {
                idleTimer += Time.deltaTime;

                if (idleTimer >= returnDelay)
                {
                    currentYaw = Mathf.LerpAngle(
                        currentYaw,
                        originalYaw,
                        returnSpeed * Time.deltaTime
                    );

                    currentPitch = Mathf.Lerp(
                        currentPitch,
                        originalPitch,
                        returnSpeed * Time.deltaTime
                    );
                }
            }
        }

        if (IsReversing())
        {
            reverseYawOffset = 180f;
        }
        else
        {
            reverseYawOffset = Mathf.LerpAngle(
                reverseYawOffset,
                0f,
                returnSpeed * Time.deltaTime
            );

            if (Mathf.Abs(reverseYawOffset) < 0.01f)
                reverseYawOffset = 0f;
        }

        transform.localRotation = Quaternion.Euler(
            currentPitch,
            currentYaw + reverseYawOffset,
            0f
        );
    }

    private bool IsReversing()
    {
        if (bikeRigidbody == null)
            return false;

        float forwardSpeed = Vector3.Dot(
            bikeRigidbody.linearVelocity,
            bikeRigidbody.transform.forward
        );

        return forwardSpeed < -reverseSpeedThreshold;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}