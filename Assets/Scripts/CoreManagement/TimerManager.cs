using UnityEngine;
using System;

public class TimerManager : MonoBehaviour
{
    private static TimerManager instance;
    public static TimerManager Instance => instance;

    public event Action<float> OnTimerUpdated;
    public event Action<float> OnTimerCompleted;

    [SerializeField] private bool showDebugLogs = true;

    private float currentTime;
    private bool isTimerRunning;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LogDebug("TimerManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Asegurarse de que el timer empieza en 0
        UpdateTimer(0f);
    }

    private void OnEnable()
    {
        EventSystem.OnLevelStarted += StartTimer;
        EventSystem.OnLevelCompleted += StopTimer;
        LogDebug("Events subscribed");
    }

    private void OnDisable()
    {
        EventSystem.OnLevelStarted -= StartTimer;
        EventSystem.OnLevelCompleted -= StopTimer;
        LogDebug("Events unsubscribed");
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimer(currentTime);
        }
    }

    private void UpdateTimer(float time)
    {
        LogDebug($"Updating timer: {time:F2}");
        OnTimerUpdated?.Invoke(time);
    }

    private void StartTimer(int level)
    {
        LogDebug($"Starting timer for level {level}");
        currentTime = 0f;
        isTimerRunning = true;
        UpdateTimer(currentTime);
    }

    private void StopTimer()
    {
        isTimerRunning = false;
        UpdateTimer(currentTime); // Asegurar última actualización
        OnTimerCompleted?.Invoke(currentTime);
        LogDebug($"Timer stopped. Final time: {currentTime:F2} seconds");
    }

    public float GetCurrentTime() => currentTime;

    private void LogDebug(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[TimerManager] {message}");
        }
    }

#if UNITY_EDITOR
    // Métodos para debugging
    public void ForceStartTimer()
    {
        LogDebug("Force starting timer");
        StartTimer(0);
    }

    public void ForceStopTimer()
    {
        LogDebug("Force stopping timer");
        StopTimer();
    }
#endif
}