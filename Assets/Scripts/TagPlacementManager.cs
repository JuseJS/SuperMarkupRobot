using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TagPlacementManager : MonoBehaviour
{
    [System.Serializable]
    public class TagPlacementSpot
    {
        public Transform spotTransform;
        public string expectedTag;
        public bool isPreFilled = false;
        public bool isOccupied = false;
        public HTMLTagObject currentTag;

        public bool IsCorrectlyPlaced => 
            isOccupied && currentTag != null && currentTag.GetTagText() == expectedTag;

        public bool NeedsPlacement => 
            !isPreFilled && (!isOccupied || currentTag == null);
    }

    [Header("Placement Settings")]
    public List<TagPlacementSpot> placementSpots = new List<TagPlacementSpot>();
    public float snapDistance = 2f;

    [Header("Level Settings")]
    public int currentLevel = 1;

    [Header("Debug Settings")]
    public bool showDebugLogs = true;

    private void Start()
    {
        if (showDebugLogs)
        {
            LogLevelInfo();
        }
    }

    private void LogLevelInfo()
    {
        int totalSpots = placementSpots.Count;
        int requiredPlacements = GetRequiredPlacements();
        
        Debug.Log($"Nivel {currentLevel} iniciado - Total spots: {totalSpots}, Por colocar: {requiredPlacements}");
        
        foreach (var spot in placementSpots)
        {
            Debug.Log($"Spot Config - Tag: {spot.expectedTag}, " +
                     $"Posición: {spot.spotTransform.position}, " +
                     $"Prefijado: {spot.isPreFilled}");
        }
    }

    public bool CheckTagPlacement(HTMLTagObject tag, Vector3 position)
    {
        if (tag == null)
        {
            Debug.LogWarning("Tag nulo detectado en CheckTagPlacement");
            return false;
        }

        var nearestSpot = FindNearestValidSpot(tag, position);
        if (nearestSpot == null) return false;

        if (tag.GetTagText() != nearestSpot.expectedTag)
        {
            if (showDebugLogs)
            {
                Debug.Log($"Tag incorrecto. Esperado: {nearestSpot.expectedTag}, Recibido: {tag.GetTagText()}");
            }
            return false;
        }

        PlaceTagInSpot(tag, nearestSpot);
        return true;
    }

    private TagPlacementSpot FindNearestValidSpot(HTMLTagObject tag, Vector3 position)
    {
        return placementSpots
            .Where(spot => Vector3.Distance(position, spot.spotTransform.position) <= snapDistance)
            .Where(spot => !spot.isOccupied || spot.currentTag == tag)
            .OrderBy(spot => Vector3.Distance(position, spot.spotTransform.position))
            .FirstOrDefault();
    }

    private void PlaceTagInSpot(HTMLTagObject tag, TagPlacementSpot spot)
    {
        // Actualizar estado del spot
        spot.isOccupied = true;
        spot.currentTag = tag;

        // Posicionar el tag
        Vector3 spotPosition = spot.spotTransform.position;
        tag.transform.position = new Vector3(spotPosition.x, spotPosition.y + 0.2f, spotPosition.z);
        tag.transform.rotation = spot.spotTransform.rotation;

        if (showDebugLogs)
        {
            Debug.Log($"Tag {tag.GetTagText()} colocado correctamente");
        }

        CheckLevelCompletion();
    }

    public void RemoveTagFromSpot(HTMLTagObject tag)
    {
        var spot = placementSpots.FirstOrDefault(s => s.currentTag == tag);
        if (spot == null) return;

        spot.isOccupied = false;
        spot.currentTag = null;

        if (showDebugLogs)
        {
            Debug.Log($"Tag removido de spot. Progreso: {GetCorrectPlacements()}/{GetRequiredPlacements()}");
        }
    }

    private int GetRequiredPlacements() => 
        placementSpots.Count(spot => !spot.isPreFilled);

    private int GetCorrectPlacements() => 
        placementSpots.Count(spot => !spot.isPreFilled && spot.IsCorrectlyPlaced);

    private void CheckLevelCompletion()
    {
        int correctPlacements = GetCorrectPlacements();
        int requiredPlacements = GetRequiredPlacements();

        if (correctPlacements == requiredPlacements && requiredPlacements > 0)
        {
            Debug.Log($"¡VICTORIA! Nivel {currentLevel} completado correctamente!");
            // Aquí puedes añadir la lógica para pasar al siguiente nivel
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        foreach (var spot in placementSpots)
        {
            if (spot.spotTransform == null) continue;

            // Verde: disponible, Rojo: ocupado
            Gizmos.color = spot.isOccupied ? Color.red : Color.green;
            Gizmos.DrawWireSphere(spot.spotTransform.position, snapDistance);

            // Amarillo: línea al tag actual
            if (spot.currentTag != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(spot.spotTransform.position, spot.currentTag.transform.position);
            }
        }
    }
}