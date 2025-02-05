using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class TagObject : MonoBehaviour
{
    [Header("Tag Settings")]
    [SerializeField] private string tagText;
    [SerializeField] private Color tagColor = Color.white;
    [SerializeField] private float textSize = 8f;
    [SerializeField] private float minWidth = 1f;
    [SerializeField] private float minHeight = 0.6f;
    [SerializeField] private float maxWidth = 5f;
    [SerializeField] private float padding = 0.1f;

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
    private bool isPickedUp;
    private bool isPrefilled;
    private static TagObject currentlyHeldObject;

    public void Initialize(string text, bool prefilled)
    {
        tagText = text;
        isPrefilled = prefilled;
        InitializeComponents();
        UpdateTagAppearance();
        FindReferences();

        if (isPrefilled)
        {
            rb.isKinematic = true;
            enabled = false;
        }
    }

    private void InitializeComponents()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        rb.mass = 1f;
        rb.drag = 5f;
        rb.angularDrag = 5f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        SetupTextMeshes();
    }

    private void SetupTextMeshes()
    {
        if (textMeshes != null && textMeshes.Length > 0) return;

        textMeshes = new TextMeshPro[2];
        var configurations = new[]
        {
           (new Vector3(0, 0, 0.51f), new Vector3(180, 0, 180)),
           (new Vector3(0, 0, -0.51f), new Vector3(0, 0, 0))
       };

        for (int i = 0; i < configurations.Length; i++)
        {
            var textObj = new GameObject($"TagText_{i}");
            textObj.transform.SetParent(transform, false);
            textObj.transform.localPosition = configurations[i].Item1;
            textObj.transform.localRotation = Quaternion.Euler(configurations[i].Item2);

            var textMesh = textObj.AddComponent<TextMeshPro>();
            ConfigureTextMesh(textMesh);
            textMeshes[i] = textMesh;
        }
    }

    private void ConfigureTextMesh(TextMeshPro textMesh)
    {
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = textSize;
        textMesh.enableWordWrapping = true;
        textMesh.overflowMode = TextOverflowModes.Ellipsis;
        textMesh.rectTransform.sizeDelta = Vector2.one;
        textMesh.rectTransform.localScale = Vector3.one;
        textMesh.margin = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);
        textMesh.font = TMP_Settings.defaultFontAsset;
    }

    private void Update()
    {
        if (isPrefilled) return;

        if (Input.GetKeyDown(pickupKey) &&
            playerTransform != null &&
            Vector3.Distance(transform.position, playerTransform.position) <= pickupRange)
        {
            if (isPickedUp) Drop();
            else if (currentlyHeldObject == null) TryPickUp();
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
        rb.isKinematic = true;
        boxCollider.isTrigger = true;
        isPickedUp = true;
        currentlyHeldObject = this;
        EventSystem.RaiseTagPickedUp(this);
    }

    private void Drop()
    {
        isPickedUp = false;
        currentlyHeldObject = null;
        rb.isKinematic = false;
        boxCollider.isTrigger = false;

        GameManager.Instance.TagManager.ValidateTagPlacement(this, transform.position);
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
        }
    }

    private void UpdateTagAppearance()
    {
        if (textMeshes == null || textMeshes.Length == 0) return;

        float maxNaturalWidth = 0f;
        float maxWrappedHeight = 0f;

        foreach (var textMesh in textMeshes)
        {
            if (textMesh == null) continue;
            textMesh.text = tagText;
            textMesh.color = tagColor;
            textMesh.ForceMeshUpdate();

            // Calcular ancho natural sin wrapping
            float naturalWidth = textMesh.GetPreferredValues(tagText, Mathf.Infinity, 0f).x;
            maxNaturalWidth = Mathf.Max(maxNaturalWidth, naturalWidth);

            // Calcular alto necesario con wrapping en minWidth
            float wrappedHeight = textMesh.GetPreferredValues(tagText, minWidth, 0f).y;
            maxWrappedHeight = Mathf.Max(maxWrappedHeight, wrappedHeight);
        }

        // Calcular dimensiones finales del padre
        float parentWidth = Mathf.Max(maxNaturalWidth, minWidth);
        float parentHeight = Mathf.Max(maxWrappedHeight, minHeight);

        Vector3 newScale = new Vector3(
            parentWidth + 0.1f,
            parentHeight + 0.1f,
            0.3f
        );

        transform.localScale = newScale;
        boxCollider.size = Vector3.one;
        boxCollider.center = Vector3.zero;

        foreach (var textMesh in textMeshes)
        {
            if (textMesh == null) continue;
            textMesh.rectTransform.sizeDelta = new Vector2(
                0.9f,
                0.9f
            );
            textMesh.fontSize = textSize * Mathf.Min(1, minWidth / parentWidth);
            textMesh.ForceMeshUpdate();
        }
    }

    public string GetTagText() => tagText;
}