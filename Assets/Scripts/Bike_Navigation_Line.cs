using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Bike_Navigation_Line : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bike_NavMeshPathfinder pathfinder;

    [Header("Line")]
    [SerializeField] private float lineHeight = 0.15f;
    [SerializeField] private float lineWidth = 0.2f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
    }

    private void LateUpdate()
    {
        if (pathfinder == null)
        {
            ClearLine();
            return;
        }

        if (!pathfinder.HasActiveRoute)
        {
            ClearLine();
            return;
        }

        UpdateLine();
    }

    private void UpdateLine()
    {
        Vector3[] corners = pathfinder.GetPathCorners();

        if (corners == null || corners.Length < 2)
        {
            ClearLine();
            return;
        }

        lineRenderer.positionCount = corners.Length;

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 position = corners[i];
            position.y += lineHeight;

            lineRenderer.SetPosition(i, position);
        }
    }

    private void ClearLine()
    {
        if (lineRenderer.positionCount > 0)
            lineRenderer.positionCount = 0;
    }

    private void OnDisable()
    {
        ClearLine();
    }
}