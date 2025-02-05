using UnityEngine;

public class TeleporterSystem : MonoBehaviour
{
    public static Material CreateTeleporterMaterial(bool isUpward)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.color = isUpward ? 
            new Color(0.3f, 0.7f, 1f, 0.8f) : 
            new Color(0.3f, 1f, 0.5f, 0.8f);
        ConfigureTransparentMaterial(material);
        return material;
    }

    private static void ConfigureTransparentMaterial(Material material)
    {
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}