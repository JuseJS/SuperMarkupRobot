using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float targetHeight;
    public float teleportHeight = 1f;
    
    [Header("Interaction Settings")]
    public KeyCode activationKey = KeyCode.E;
    public float triggerRadius = 1.5f;
    public float cooldownTime = 1f;

    private bool canTeleport = true;
    private GameObject currentPlayer;
    private SphereCollider triggerCollider;
    private bool isInitialized = false;
    private bool isPlayerInTrigger = false;

    private void Awake()
    {
        Debug.Log($"[Teleporter] Awake - Inicializando {gameObject.name}");
        SetupTriggerCollider();
        isInitialized = true;
    }

    private void OnEnable()
    {
        if (isInitialized)
        {
            Debug.Log($"[Teleporter] OnEnable - {gameObject.name} en posición {transform.position}, Altura objetivo: {targetHeight}");
        }
    }

    private void Start()
    {
        Debug.Log($"[Teleporter] Start - {gameObject.name} listo para uso. Usar tecla {activationKey} para activar");
    }

    private void SetupTriggerCollider()
    {
        // Obtener o crear el SphereCollider
        triggerCollider = GetComponent<SphereCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<SphereCollider>();
            Debug.Log($"[Teleporter] Creando nuevo SphereCollider para {gameObject.name}");
        }

        // Configurar el collider
        triggerCollider.isTrigger = true;
        triggerCollider.radius = triggerRadius;
        
        Debug.Log($"[Teleporter] Configuración completada para {gameObject.name}:\n" +
                 $"- Posición: {transform.position}\n" +
                 $"- Radio de detección: {triggerRadius}\n" +
                 $"- Altura objetivo: {targetHeight}");
    }

    private void Update()
    {
        // Solo procesar input si el jugador está realmente en el trigger y podemos teleportar
        if (isPlayerInTrigger && canTeleport && Input.GetKeyDown(activationKey))
        {
            Debug.Log($"[Teleporter] Tecla {activationKey} presionada - Iniciando teleportación");
            TeleportPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[Teleporter] Jugador entró en el área de {gameObject.name}");
            currentPlayer = other.gameObject;
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject == currentPlayer)
        {
            Debug.Log($"[Teleporter] Jugador salió del área de {gameObject.name}");
            currentPlayer = null;
            isPlayerInTrigger = false;
        }
    }

    private void TeleportPlayer()
    {
        if (!isPlayerInTrigger || currentPlayer == null) return;

        Vector3 currentPos = currentPlayer.transform.position;
        Vector3 newPosition = new Vector3(
            currentPos.x,
            targetHeight + teleportHeight,
            currentPos.z
        );

        Debug.Log($"[Teleporter] Iniciando teleportación desde Y: {currentPos.y} a Y: {newPosition.y}");

        bool teleportSuccess = false;
        
        // Intentar teleportar usando CharacterController
        var charController = currentPlayer.GetComponent<CharacterController>();
        if (charController != null)
        {
            try
            {
                charController.enabled = false;
                currentPlayer.transform.position = newPosition;
                charController.enabled = true;
                teleportSuccess = true;
                Debug.Log("[Teleporter] Teleportación exitosa usando CharacterController");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Teleporter] Error al teleportar con CharacterController: {e.Message}");
            }
        }

        // Si falló el CharacterController, intentar con Rigidbody
        if (!teleportSuccess)
        {
            var rb = currentPlayer.GetComponent<Rigidbody>();
            if (rb != null)
            {
                try
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.MovePosition(newPosition);
                    teleportSuccess = true;
                    Debug.Log("[Teleporter] Teleportación exitosa usando Rigidbody");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[Teleporter] Error al teleportar con Rigidbody: {e.Message}");
                }
            }
        }

        // Si todo lo demás falló, usar transform directamente
        if (!teleportSuccess)
        {
            try
            {
                currentPlayer.transform.position = newPosition;
                teleportSuccess = true;
                Debug.Log("[Teleporter] Teleportación exitosa usando Transform directo");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Teleporter] Error al teleportar con Transform: {e.Message}");
            }
        }

        if (teleportSuccess)
        {
            // Forzar la salida del trigger actual
            isPlayerInTrigger = false;
            currentPlayer = null;
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        canTeleport = false;
        Invoke(nameof(ResetTeleport), cooldownTime);
        Debug.Log($"[Teleporter] Iniciando cooldown de {cooldownTime} segundos");
    }

    private void ResetTeleport()
    {
        canTeleport = true;
        Debug.Log($"[Teleporter] {gameObject.name} listo para nuevo uso");
    }

    private void OnDrawGizmos()
    {
        // Visualizar el área de detección en el editor
        Gizmos.color = canTeleport ? Color.cyan : Color.gray;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
        
        // Dibujar una línea que indica la altura objetivo
        Gizmos.color = Color.yellow;
        Vector3 targetPos = transform.position;
        targetPos.y = targetHeight;
        Gizmos.DrawLine(transform.position, targetPos);
    }

    private void OnValidate()
    {
        triggerRadius = Mathf.Max(0.5f, triggerRadius);
        teleportHeight = Mathf.Max(0f, teleportHeight);
        cooldownTime = Mathf.Max(0.1f, cooldownTime);
    }
}