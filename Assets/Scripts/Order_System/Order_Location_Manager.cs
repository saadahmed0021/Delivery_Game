using System.Collections.Generic;
using UnityEngine;

public class Order_Location_Manager : MonoBehaviour
{
    public static Order_Location_Manager Instance { get; private set; }

    private readonly Dictionary<string, Order_Location> locations = new();

    public IReadOnlyDictionary<string, Order_Location> Locations => locations;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        RegisterSceneLocations();
    }

    private void RegisterSceneLocations()
    {
        locations.Clear();

        Order_Location[] sceneLocations =
            FindObjectsByType<Order_Location>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Order_Location location in sceneLocations)
        {
            if (location == null)
                continue;

            RegisterLocation(location);
        }
    }

    public bool RegisterLocation(Order_Location location)
    {
        if (location == null)
            return false;

        string locationID = location.LocationID;

        if (string.IsNullOrWhiteSpace(locationID))
        {
            return false;
        }

        if (locations.TryGetValue(locationID, out Order_Location existingLocation))
        {
            if (existingLocation != location)
            {
                return false;
            }

            return true;
        }

        locations.Add(locationID, location);

        return true;
    }

    public bool UnregisterLocation(Order_Location location)
    {
        if (location == null)
            return false;

        string locationID = location.LocationID;

        if (string.IsNullOrWhiteSpace(locationID))
            return false;

        if (!locations.TryGetValue(locationID, out Order_Location registeredLocation))
            return false;

        if (registeredLocation != location)
            return false;

        return locations.Remove(locationID);
    }

    public Order_Location GetLocation(string locationID)
    {
        if (string.IsNullOrWhiteSpace(locationID))
            return null;

        locations.TryGetValue(
            locationID,
            out Order_Location location
        );

        return location;
    }

    public bool TryGetLocation(
        string locationID,
        out Order_Location location)
    {
        if (string.IsNullOrWhiteSpace(locationID))
        {
            location = null;
            return false;
        }

        return locations.TryGetValue(
            locationID,
            out location
        );
    }

    public bool HasLocation(string locationID)
    {
        return !string.IsNullOrWhiteSpace(locationID) &&
               locations.ContainsKey(locationID);
    }

    public void RefreshLocations()
    {
        RegisterSceneLocations();
    }
}