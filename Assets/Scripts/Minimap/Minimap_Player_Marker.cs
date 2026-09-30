using UnityEngine;

public class MinimapPlayerMarker : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Update()
    {
        if (player == null)
            return;

        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            -player.eulerAngles.y
        );
    }
}