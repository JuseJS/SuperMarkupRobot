using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;

    [Header("Timer Settings")]
    [SerializeField] private Vector2 timerMargin = new Vector2(30f, 30f);
    [SerializeField] private int fontSize = 36;
    [SerializeField] private Color timerColor = Color.white;
    [SerializeField] private bool showDebugLogs = true;

    [Header("Level Selection References")]
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private Button[] levelButtons;

    [Header("Victory Panel References")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryTimeText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private Canvas canvas;
    private TextMeshProUGUI timerText;
    private LevelSelectionManager levelSelectionManager;
    private GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUI();
            LogDebug("UIManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (levelSelectionPanel != null && canvas != null)
        {
            levelSelectionPanel.transform.SetParent(canvas.transform, false);
        }

        InitializeLevelSelection();
        InitializeVictoryPanel();
        FindPlayer();
        ShowCursor();
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            DisablePlayer();
        }
    }

    private void InitializeUI()
    {
        // Configuración del Canvas
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            LogDebug("No Canvas found, adding one");
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
        }

        // Asegurarnos de tener un GraphicRaycaster
        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
            LogDebug("Added GraphicRaycaster to Canvas");
        }

        if (GetComponent<CanvasScaler>() == null)
        {
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;
        }
    }

    private void InitializeLevelSelection()
    {
        if (levelSelectionPanel != null)
        {
            levelSelectionManager = levelSelectionPanel.GetComponent<LevelSelectionManager>();
            if (levelSelectionManager == null)
            {
                levelSelectionManager = levelSelectionPanel.AddComponent<LevelSelectionManager>();
            }

            // Registrar los botones con el manager
            if (levelButtons != null)
            {
                for (int i = 0; i < levelButtons.Length; i++)
                {
                    if (levelButtons[i] != null)
                    {
                        int levelIndex = i + 1;
                        levelSelectionManager.RegisterLevelButton(levelButtons[i], levelIndex);
                    }
                }
            }
            else
            {
                LogDebug("WARNING: No level buttons assigned!");
            }
        }
        else
        {
            LogDebug("WARNING: Level selection panel not assigned!");
        }
    }

    private void InitializeVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartLevel);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            }
        }
    }

    public void ShowVictoryPanel(float completionTime)
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            ShowCursor();
            DisablePlayer();

            if (victoryTimeText != null)
            {
                TimeSpan time = TimeSpan.FromSeconds(completionTime);
                string timeString = $"{time.Minutes:00}m {time.Seconds:00}s {(time.Milliseconds / 10):00}ms";
                victoryTimeText.text = $"Completado en: {timeString}";
            }
        }
    }

    public void HideVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
            HideCursor();
            EnablePlayer();
        }
    }

    private void RestartLevel()
    {
        HideVictoryPanel();
        GameManager.Instance.LevelManager.GenerateLevel(GameManager.Instance.LevelManager.CurrentLevel);
    }

    private void ReturnToMainMenu()
    {
        HideVictoryPanel();
        ShowLevelSelection();
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void DisablePlayer()
    {
        if (player != null)
        {
            player.SetActive(false);
            if (player.TryGetComponent<CharacterController>(out var controller))
            {
                controller.enabled = false;
            }

            var playerMovement = player.GetComponent<MonoBehaviour>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }
        ShowCursor();
    }

    private void EnablePlayer()
    {
        if (player != null)
        {
            player.SetActive(true);
            if (player.TryGetComponent<CharacterController>(out var controller))
            {
                controller.enabled = true;
            }

            var playerMovement = player.GetComponent<MonoBehaviour>();
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
        }
        HideCursor();
    }

    public void ShowLevelSelection()
    {
        if (levelSelectionPanel != null)
        {
            levelSelectionPanel.SetActive(true);
            DisablePlayer();
        }
    }

    public void HideLevelSelection()
    {
        if (levelSelectionPanel != null)
        {
            levelSelectionPanel.SetActive(false);
            EnablePlayer();
        }
    }

    #region Timer Creation and Management
    public void ShowTimer()
    {
        if (timerText == null)
        {
            CreateTimerText();
        }
        timerText.gameObject.SetActive(true);
        SubscribeToEvents();
    }

    public void HideTimer()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    private void CreateTimerText()
    {
        LogDebug("Creating timer UI elements");

        GameObject timerObj = new GameObject("TimerText");
        timerObj.transform.SetParent(transform);

        timerText = timerObj.AddComponent<TextMeshProUGUI>();
        timerText.fontSize = fontSize;
        timerText.color = timerColor;
        timerText.alignment = TextAlignmentOptions.Left;
        timerText.text = "00:00.00";

        RectTransform rectTransform = timerText.rectTransform;
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.sizeDelta = new Vector2(200, 50);
        rectTransform.anchoredPosition = timerMargin;

        LogDebug("Timer UI elements created successfully");
    }

    private void UpdateTimerDisplay(float currentTime)
    {
        if (timerText == null)
        {
            LogDebug("WARNING: timerText is null!");
            return;
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        string timeString = $"{time.Minutes:00}:{time.Seconds:00}.{(time.Milliseconds / 10):00}";
        timerText.text = timeString;
    }

    private void OnTimerCompleted(float finalTime)
    {
        if (timerText != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(finalTime);
            string timeString = $"{time.Minutes:00}:{time.Seconds:00}.{(time.Milliseconds / 10):00}";
            timerText.text = timeString;
            LogDebug($"Final time displayed: {timeString}");

            ShowVictoryPanel(finalTime);
        }
    }
    #endregion

    #region Timer Events
    private void SubscribeToEvents()
    {
        if (TimerManager.Instance != null)
        {
            LogDebug("Subscribing to TimerManager events");
            TimerManager.Instance.OnTimerUpdated += UpdateTimerDisplay;
            TimerManager.Instance.OnTimerCompleted += OnTimerCompleted;
        }
        else
        {
            LogDebug("WARNING: TimerManager.Instance is null!");
            Invoke(nameof(SubscribeToEvents), 0.1f);
        }
    }

    private void OnEnable()
    {
        ShowCursor();
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.OnTimerUpdated -= UpdateTimerDisplay;
            TimerManager.Instance.OnTimerCompleted -= OnTimerCompleted;
        }
    }

    #endregion

    private void LogDebug(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[UIManager] {message}");
        }
    }
}