using UnityEngine;

public class Bike_CameraOrbit : MonoBehaviour
{
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private float returnDelay = 1f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 45f;

    private float currentYaw;
    private float currentPitch;

    private float originalYaw;
    private float originalPitch;

    private float idleTimer;

    private void Start()
    {
        Vector3 rotation = transform.localEulerAngles;

        originalYaw = rotation.y;
        originalPitch = NormalizeAngle(rotation.x);

        currentYaw = originalYaw;
        currentPitch = originalPitch;
    }

    private void LateUpdate()
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

        transform.localRotation = Quaternion.Euler(
            currentPitch,
            currentYaw,
            0f
        );
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}