using UnityEngine;

public class TagPlacementSpot : MonoBehaviour
{
    public string expectedTag;
    public bool isPreFilled;
    public Color spotColor = new Color(1f, 1f, 0f, 0.3f);

    private void OnDrawGizmos()
    {
        // Make spots more visible in scene view
        Gizmos.color = isPreFilled ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawCube(transform.position, transform.localScale);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, expectedTag);
#endif
    }

    public void OnValidate()
{
    // Asegurar que el spot siempre tenga una altura mínima
    if (transform.localScale.y < 0.1f)
    {
        Vector3 scale = transform.localScale;
        scale.y = 0.1f;
        transform.localScale = scale;
    }
}
}