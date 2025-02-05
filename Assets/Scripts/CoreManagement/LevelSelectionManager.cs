using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    public void RegisterLevelButton(Button button, int level)
    {
        if (button != null)
        {
            // Limpiar listeners previos para evitar duplicados
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => StartLevel(level));
        }
    }

    private void StartLevel(int level)
    {
        // Ocultar el panel de selección
        UIManager.Instance.HideLevelSelection();
        
        // Generar el nivel
        GameManager.Instance.LevelManager.GenerateLevel(level);
    }
}