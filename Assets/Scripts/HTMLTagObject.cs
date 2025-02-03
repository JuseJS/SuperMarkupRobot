using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class HTMLTagObject : MonoBehaviour
{
    [Header("Tag Settings")]
    [SerializeField] private string tagText = "<p>";
    [SerializeField] private Color tagColor = Color.white;
    [SerializeField] private float textSize = 1f;
    [SerializeField] private float minWidth = 1f;
    [SerializeField] private float minHeight = 0.6f;
    [SerializeField] private float verticalMargin = 0.1f;

    [Header("Interaction Settings")]
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private float followSpeed = 12f;
    [SerializeField] private Vector3 holdOffset = new Vector3(0f, 1f, 0.5f);
    [SerializeField] private KeyCode pickupKey = KeyCode.C;

    private TextMeshPro[] textMeshes;
    private BoxCollider boxCollider;
    private Rigidbody rb;
    private Transform playerTransform;
    private Transform playerHandTransform;
    private TagPlacementManager placementManager;

    private bool isPickedUp = false;
    private bool isPrefilled = false;
    private bool isPlaced = false;
    private static HTMLTagObject currentlyHeldObject = null;

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        UpdateTagAppearance();
        FindReferences();
    }

    private void InitializeComponents()
    {
        // Configurar componentes físicos
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        rb.mass = 1f;
        rb.drag = 5f;
        rb.angularDrag = 5f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Configurar texto
        SetupTextMeshes();
    }

    private void FindReferences()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerHandTransform = player.transform.Find("Right_Hand") ?? playerTransform;
            }
            else
            {
                Debug.LogError("No se encontró objeto con tag Player");
            }
        }

        if (placementManager == null)
        {
            placementManager = FindObjectOfType<TagPlacementManager>();
            if (placementManager == null)
            {
                Debug.LogError("No se encontró TagPlacementManager en la escena");
            }
        }
    }

    private void SetupTextMeshes()
    {
        if (textMeshes != null && textMeshes.Length > 0) return;

        textMeshes = new TextMeshPro[2];

        // Configuración corregida para ambos lados del texto
        (Vector3 position, Vector3 rotation)[] configurations = new[]
        {
        (new Vector3(0, 0, 0.51f), new Vector3(180, 0, 180)),     // Frente
        (new Vector3(0, 0, -0.51f), new Vector3(0, 0, 0)) // Atrás (rotado 180 en Y y Z)
    };

        for (int i = 0; i < configurations.Length; i++)
        {
            var textObj = new GameObject($"TagText_{i}");
            textObj.transform.SetParent(transform, false);
            textObj.transform.localPosition = configurations[i].position;
            textObj.transform.localRotation = Quaternion.Euler(configurations[i].rotation);

            textMeshes[i] = textObj.AddComponent<TextMeshPro>();
            ConfigureTextMesh(textMeshes[i]);
        }
    }

    private void ConfigureTextMesh(TextMeshPro textMesh)
    {
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = textSize;
        textMesh.enableWordWrapping = false;
        textMesh.overflowMode = TextOverflowModes.Overflow;
        textMesh.rectTransform.sizeDelta = new Vector2(5, 2);
        textMesh.rectTransform.localScale = Vector3.one * 2;

        if (TMP_Settings.defaultFontAsset != null)
        {
            textMesh.font = TMP_Settings.defaultFontAsset;
        }
    }

    private void Update()
    {
        if (isPrefilled) return;

        if (Input.GetKeyDown(pickupKey) &&
            playerTransform != null &&
            Vector3.Distance(transform.position, playerTransform.position) <= pickupRange)
        {
            if (isPickedUp)
            {
                Drop();
            }
            else if (currentlyHeldObject == null)
            {
                TryPickUp();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isPickedUp && playerHandTransform != null)
        {
            Vector3 targetPosition = playerHandTransform.position +
                                   playerHandTransform.TransformDirection(holdOffset);

            rb.MovePosition(Vector3.Lerp(rb.position, targetPosition, followSpeed * Time.fixedDeltaTime));
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, playerTransform.rotation, followSpeed * Time.fixedDeltaTime));
        }
    }

    private void TryPickUp()
    {
        if (currentlyHeldObject != null || isPrefilled) return;

        rb.isKinematic = true;
        boxCollider.isTrigger = true;
        isPickedUp = true;
        currentlyHeldObject = this;

        if (isPlaced && placementManager != null)
        {
            placementManager.RemoveTagFromSpot(this);
            isPlaced = false;
        }
    }

    private void Drop()
    {
        if (!isPickedUp) return;

        Debug.Log($"Soltando tag {tagText} en posición {transform.position}");

        isPickedUp = false;
        currentlyHeldObject = null;
        rb.isKinematic = false;
        boxCollider.isTrigger = false;

        // Verificar colocación en spot
        if (placementManager != null)
        {
            Debug.Log("Verificando colocación con TagPlacementManager");
            if (placementManager.CheckTagPlacement(this, transform.position))
            {
                Debug.Log("Tag colocado correctamente en el spot");
                rb.isKinematic = true;
                isPlaced = true;
            }
            else
            {
                Debug.Log("Tag no se pudo colocar en ningún spot válido");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró TagPlacementManager");
        }
    }

    private void UpdateTagAppearance()
    {
        if (textMeshes == null || textMeshes.Length == 0) return;

        // Actualizar texto y color
        foreach (var textMesh in textMeshes)
        {
            if (textMesh == null) continue;

            textMesh.text = tagText;
            textMesh.color = tagColor;
            textMesh.ForceMeshUpdate();
        }

        // Ajustar tamaño del cubo basado en el texto
        Vector3 maxBounds = Vector3.zero;
        foreach (var textMesh in textMeshes)
        {
            if (textMesh == null) continue;
            maxBounds = Vector3.Max(maxBounds, textMesh.bounds.size);
        }

        // Calcular nueva escala con márgenes
        Vector3 newScale = new Vector3(
            Mathf.Max(maxBounds.x * 3f, minWidth),
            Mathf.Max(maxBounds.y * 2f + verticalMargin, minHeight),
            0.3f
        );

        transform.localScale = newScale;

        if (boxCollider != null)
        {
            boxCollider.size = Vector3.one;
            boxCollider.center = Vector3.zero;
        }
    }

    public void SetTagText(string newText)
    {
        if (string.IsNullOrEmpty(newText)) return;
        tagText = newText;
        if (gameObject.activeInHierarchy)
        {
            UpdateTagAppearance();
        }
    }

    public void SetTagColor(Color newColor)
    {
        tagColor = newColor;
        if (gameObject.activeInHierarchy)
        {
            UpdateTagAppearance();
        }
    }

    public void SetAsPrefilled(bool value)
    {
        isPrefilled = value;
        if (isPrefilled)
        {
            rb.isKinematic = true;
            enabled = false;
        }
    }

    public string GetTagText()
    {
        return tagText;
    }

    private void OnDestroy()
    {
        if (currentlyHeldObject == this)
        {
            currentlyHeldObject = null;
        }
    }

    private void OnEnable()
    {
        FindReferences();
    }
}