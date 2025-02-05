using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [Header("Manager Prefabs")]
    [SerializeField] private TagManager tagManagerPrefab;
    [SerializeField] private LevelManager levelManagerPrefab;
    [SerializeField] private UIManager uiManagerPrefab;
    [SerializeField] private TimerManager timerManagerPrefab;

    public TagManager TagManager { get; private set; }
    public LevelManager LevelManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public TimerManager TimerManager { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManagers()
    {
        // Primero inicializamos UI y Timer ya que son necesarios desde el inicio
        UIManager = GetOrCreateManager(uiManagerPrefab);
        TimerManager = GetOrCreateManager(timerManagerPrefab);

        // Los managers de Tag y Level se inicializan pero no generan contenido hasta que se seleccione un nivel
        TagManager = GetOrCreateManager(tagManagerPrefab);
        LevelManager = GetOrCreateManager(levelManagerPrefab);

        // Deshabilitar el TimerManager hasta que se inicie un nivel
        if (TimerManager != null)
        {
            TimerManager.gameObject.SetActive(false);
        }
    }

    private T GetOrCreateManager<T>(T prefab) where T : Component
    {
        T manager = FindObjectOfType<T>();
        if (manager == null && prefab != null)
        {
            manager = Instantiate(prefab, transform);
        }
        else if (manager == null)
        {
            manager = new GameObject(typeof(T).Name).AddComponent<T>();
            manager.transform.SetParent(transform);
        }
        return manager;
    }
}