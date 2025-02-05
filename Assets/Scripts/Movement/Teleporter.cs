using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Teleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float targetHeight;
    public float teleportHeight = 1f;
    
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode activationKey = KeyCode.E;
    [SerializeField] private float triggerRadius = 1.5f;
    [SerializeField] private float cooldownTime = 1f;

    private bool canTeleport = true;
    private GameObject currentPlayer;
    private SphereCollider triggerCollider;
    private bool isPlayerInTrigger;

    private void Awake()
    {
        SetupCollider();
    }

    private void SetupCollider()
    {
        triggerCollider = GetComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = triggerRadius;
    }

    private void Update()
    {
        if (isPlayerInTrigger && canTeleport && Input.GetKeyDown(activationKey))
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (!isPlayerInTrigger || currentPlayer == null) return;

        Vector3 newPosition = new Vector3(
            currentPlayer.transform.position.x,
            targetHeight + teleportHeight,
            currentPlayer.transform.position.z
        );

        if (TryTeleport(newPosition))
        {
            StartCooldown();
        }
    }

    private bool TryTeleport(Vector3 newPosition)
    {
        if (currentPlayer.TryGetComponent<CharacterController>(out var charController))
        {
            charController.enabled = false;
            currentPlayer.transform.position = newPosition;
            charController.enabled = true;
            return true;
        }
        
        currentPlayer.transform.position = newPosition;
        return true;
    }

    private void StartCooldown()
    {
        canTeleport = false;
        isPlayerInTrigger = false;
        currentPlayer = null;
        Invoke(nameof(ResetTeleport), cooldownTime);
    }

    private void ResetTeleport()
    {
        canTeleport = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.gameObject;
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject == currentPlayer)
        {
            currentPlayer = null;
            isPlayerInTrigger = false;
        }
    }

    private void OnValidate()
    {
        triggerRadius = Mathf.Max(0.5f, triggerRadius);
        teleportHeight = Mathf.Max(0f, teleportHeight);
        cooldownTime = Mathf.Max(0.1f, cooldownTime);
    }
}