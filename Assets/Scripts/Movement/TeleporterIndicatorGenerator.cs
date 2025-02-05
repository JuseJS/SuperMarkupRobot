using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TeleporterIndicatorGenerator : MonoBehaviour
{
    [SerializeField] private string savePath = "Assets/Prefabs/";

    [ContextMenu("Generate Teleporter Indicators")]
    public void GenerateIndicators()
    {
        GenerateIndicator(true, "UpwardIndicator", Color.cyan);
        GenerateIndicator(false, "DownwardIndicator", Color.green);
    }

    private GameObject GenerateIndicator(bool isUpward, string name, Color color)
    {
        GameObject indicator = new GameObject(name);
        indicator.AddComponent<TeleporterIndicator>();

        float direction = isUpward ? 1f : -1f;
        GameObject arrow = CreateArrow(color, direction);
        arrow.transform.SetParent(indicator.transform);

        // Ajustar posición para que la base quede alineada con el origen
        arrow.transform.localPosition = new Vector3(0, isUpward ? 0.8f : -0.8f, 0);

#if UNITY_EDITOR
        string fullPath = $"{savePath}{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(indicator, fullPath);
        DestroyImmediate(indicator);
#endif
        return indicator;
    }

    private GameObject CreateArrow(Color color, float direction)
    {
        GameObject arrow = new GameObject("Arrow");

        GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shaft.transform.SetParent(arrow.transform);
        shaft.transform.localScale = new Vector3(0.15f, 1.125f, 0.15f); // 25% más pequeño
        shaft.transform.localPosition = Vector3.up * (0.56f * direction);
        DestroyImmediate(shaft.GetComponent<Collider>());

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        head.transform.SetParent(arrow.transform);
        head.transform.localScale = new Vector3(0.45f, 0.3f, 0.45f); // 25% más pequeño
        head.transform.localPosition = Vector3.up * (1.2f * direction);
        head.transform.localRotation = Quaternion.Euler(direction > 0 ? 0 : 180, 0, 0);
        DestroyImmediate(head.GetComponent<Collider>());

        // Aplicar materiales con colores correctos
        Material shaftMat = new Material(Shader.Find("Standard"));
        Material headMat = new Material(Shader.Find("Standard"));
        
        shaftMat.EnableKeyword("_EMISSION");
        headMat.EnableKeyword("_EMISSION");
        
        Color emissionColor = color * 2f; // Intensificar emisión
        
        shaftMat.SetColor("_Color", color);
        shaftMat.SetColor("_EmissionColor", emissionColor);
        headMat.SetColor("_Color", color);
        headMat.SetColor("_EmissionColor", emissionColor);
        
        shaft.GetComponent<Renderer>().material = shaftMat;
        head.GetComponent<Renderer>().material = headMat;

        return arrow;
    }
}