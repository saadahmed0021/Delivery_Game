using UnityEngine;

public class Pickup_Arrow_Animation : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 startPosition;
    private float currentYRotation;

    private void Start()
    {
        startPosition = transform.position;
        currentYRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        Float();
        Rotate();
    }

    private void Float()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        Vector3 position = startPosition;
        position.y += offset;

        transform.position = position;
    }

    private void Rotate()
    {
        currentYRotation += rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(
            0f,
            currentYRotation,
            90f
        );
    }
}