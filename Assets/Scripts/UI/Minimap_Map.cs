using UnityEngine;

public class Minimap_Map : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform mapImage;

    [Header("World Calibration")]
    [SerializeField] private Vector2 worldCenter = new Vector2(765.18f, -120.85f);
    [SerializeField] private Vector2 worldSize = new Vector2(1000f, 1000f);

    [Header("Map Settings")]
    [SerializeField] private float mapScale = 1f;

    private void LateUpdate()
    {
        if (player == null || mapImage == null)
            return;

        float normalizedX =
            (player.position.x - worldCenter.x) / worldSize.x;

        float normalizedZ =
            (player.position.z - worldCenter.y) / worldSize.y;

        float mapWidth = mapImage.rect.width * mapImage.localScale.x;
        float mapHeight = mapImage.rect.height * mapImage.localScale.y;

        float mapX = normalizedX * mapWidth * mapScale;
        float mapY = normalizedZ * mapHeight * mapScale;

        mapImage.anchoredPosition = new Vector2(
            -mapX,
            -mapY
        );
    }
}