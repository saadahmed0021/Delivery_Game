using UnityEngine;
using UnityEngine.AI;

public class Bike_NavMeshPathfinder : MonoBehaviour
{
    public enum NavigationState
    {
        None,
        ToRestaurant,
        ToCustomer
    }

    [Header("Navigation")]
    [SerializeField] private float sampleDistance = 5f;
    [SerializeField] private int areaMask = NavMesh.AllAreas;

    [Header("Path Refresh")]
    [SerializeField] private bool refreshPathAutomatically = true;
    [SerializeField] private float refreshInterval = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool showPathGizmos = true;

    private NavMeshPath currentPath;
    private Vector3 restaurantPosition;
    private Vector3 customerPosition;

    private NavigationState navigationState = NavigationState.None;

    private float nextRefreshTime;

    public NavigationState CurrentState => navigationState;

    public bool HasActiveRoute =>
        navigationState != NavigationState.None &&
        currentPath != null &&
        currentPath.status == NavMeshPathStatus.PathComplete;

    public bool IsGoingToRestaurant =>
        navigationState == NavigationState.ToRestaurant;

    public bool IsGoingToCustomer =>
        navigationState == NavigationState.ToCustomer;

    public Vector3 CurrentDestination
    {
        get
        {
            return navigationState switch
            {
                NavigationState.ToRestaurant => restaurantPosition,
                NavigationState.ToCustomer => customerPosition,
                _ => transform.position
            };
        }
    }

    public NavMeshPath CurrentPath => currentPath;

    private void Awake()
    {
        currentPath = new NavMeshPath();
    }

    private void Update()
    {
        if (!refreshPathAutomatically)
            return;

        if (navigationState == NavigationState.None)
            return;

        if (Time.time < nextRefreshTime)
            return;

        CalculateCurrentPath();

        nextRefreshTime = Time.time + refreshInterval;
    }

    public bool StartRestaurantRoute(Vector3 restaurant)
    {
        restaurantPosition = restaurant;
        navigationState = NavigationState.ToRestaurant;

        return CalculateCurrentPath();
    }

    public bool StartCustomerRoute(Vector3 customer)
    {
        customerPosition = customer;
        navigationState = NavigationState.ToCustomer;

        return CalculateCurrentPath();
    }

    public void ClearRoute()
    {
        navigationState = NavigationState.None;

        currentPath.ClearCorners();

        nextRefreshTime = 0f;
    }

    public bool CalculateCurrentPath()
    {
        if (navigationState == NavigationState.None)
        {
            currentPath.ClearCorners();
            return false;
        }

        Vector3 destination = CurrentDestination;

        if (!TryGetNearestNavMeshPosition(
                transform.position,
                out Vector3 startPosition))
        {
            currentPath.ClearCorners();
            return false;
        }

        if (!TryGetNearestNavMeshPosition(
                destination,
                out Vector3 destinationPosition))
        {
            currentPath.ClearCorners();
            return false;
        }

        bool pathFound = NavMesh.CalculatePath(
            startPosition,
            destinationPosition,
            areaMask,
            currentPath
        );

        if (!pathFound)
        {
            currentPath.ClearCorners();
            return false;
        }

        return currentPath.status == NavMeshPathStatus.PathComplete;
    }

    public bool IsDestinationReached(float distance)
    {
        Vector3 destination = CurrentDestination;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = destination;

        currentPosition.y = 0f;
        targetPosition.y = 0f;

        return Vector3.Distance(
            currentPosition,
            targetPosition
        ) <= distance;
    }

    public bool HasReachedRestaurant(float distance = 4f)
    {
        if (!IsGoingToRestaurant)
            return false;

        return IsDestinationReached(distance);
    }

    public bool HasReachedCustomer(float distance = 4f)
    {
        if (!IsGoingToCustomer)
            return false;

        return IsDestinationReached(distance);
    }

    public Vector3 GetNextPathPoint()
    {
        if (!HasActiveRoute)
            return transform.position;

        Vector3[] corners = currentPath.corners;

        if (corners.Length < 2)
            return CurrentDestination;

        return corners[1];
    }

    public Vector3 GetDirectionToNextPathPoint()
    {
        Vector3 direction =
            GetNextPathPoint() - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return Vector3.zero;

        return direction.normalized;
    }

    public float GetDistanceToDestination()
    {
        if (navigationState == NavigationState.None)
            return 0f;

        Vector3 currentPosition = transform.position;
        Vector3 destination = CurrentDestination;

        currentPosition.y = 0f;
        destination.y = 0f;

        return Vector3.Distance(
            currentPosition,
            destination
        );
    }

    public Vector3[] GetPathCorners()
    {
        if (!HasActiveRoute)
            return System.Array.Empty<Vector3>();

        return currentPath.corners;
    }

    private bool TryGetNearestNavMeshPosition(
        Vector3 position,
        out Vector3 navMeshPosition)
    {
        if (NavMesh.SamplePosition(
                position,
                out NavMeshHit hit,
                sampleDistance,
                areaMask))
        {
            navMeshPosition = hit.position;
            return true;
        }

        navMeshPosition = position;
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!showPathGizmos)
            return;

        if (!Application.isPlaying)
            return;

        if (!HasActiveRoute)
            return;

        Vector3[] corners = currentPath.corners;

        if (corners.Length < 2)
            return;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Gizmos.DrawLine(
                corners[i],
                corners[i + 1]
            );
        }

        Gizmos.DrawSphere(
            CurrentDestination,
            0.5f
        );
    }
}