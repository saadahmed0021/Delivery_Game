using UnityEngine;

public class Minimap_Camera_Follow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height = 100f;

    private void LateUpdate()
    {
        if (target == null)
        return;

    transform.position = new Vector3(
        target.position.x,
        target.position.y + height,
        target.position.z
    );

    transform.rotation = Quaternion.Euler(
        90f,
        target.eulerAngles.y,
        0f
    );
    }
}