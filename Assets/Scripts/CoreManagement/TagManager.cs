using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TagManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float snapDistance = 2f;
    [SerializeField] private bool showDebugLogs = true;

    private List<TagSpot> placementSpots = new List<TagSpot>();
    public IReadOnlyList<TagSpot> PlacementSpots => placementSpots;

    public void RegisterSpot(TagSpot spot)
    {
        if (!placementSpots.Contains(spot))
        {
            placementSpots.Add(spot);
            if (showDebugLogs)
                Debug.Log($"Registered spot for tag: {spot.ExpectedTag}");
        }
    }

    public void UnregisterSpot(TagSpot spot)
    {
        placementSpots.Remove(spot);
    }

    public void ClearSpots()
    {
        foreach (var spot in placementSpots)
        {
            spot.ClearTag();
        }
        placementSpots.Clear();
    }

    public bool ValidateTagPlacement(TagObject tag, Vector3 position)
    {
        var nearestSpot = FindNearestValidSpot(tag, position);
        if (nearestSpot == null) return false;

        return nearestSpot.PlaceTag(tag);
    }

    private TagSpot FindNearestValidSpot(TagObject tag, Vector3 position) =>
        placementSpots
            .Where(spot => Vector3.Distance(position, spot.transform.position) <= snapDistance)
            .Where(spot => !spot.IsOccupied || spot.CurrentTag == tag)
            .OrderBy(spot => Vector3.Distance(position, spot.transform.position))
            .FirstOrDefault();

    public bool CheckLevelCompletion() =>
        placementSpots.All(spot => spot.IsPreFilled || spot.IsCorrectlyPlaced);
}