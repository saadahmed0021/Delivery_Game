using UnityEngine;

public class Order_Location : MonoBehaviour
{
    [Header("Location")]
    [SerializeField] private string locationName;

    public string LocationName => locationName;
    public Transform Transform => transform;
    public Vector3 Position => transform.position;

    public void SetLocationName(string value)
    {
        locationName = value;
    }
}