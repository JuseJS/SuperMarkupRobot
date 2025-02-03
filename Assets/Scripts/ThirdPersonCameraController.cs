using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Altura objetivo de la cámara")]
    public float TargetHeight = 1.375f;
    
    [Tooltip("Velocidad de rotación de la cámara")]
    public float RotationSpeed = 2f;

    [Tooltip("Sensibilidad del ratón")]
    public float MouseSensitivity = 0.8f;

    [Header("Cinemachine Settings")]
    [Tooltip("Distancia deseada de la cámara")]
    public float CameraDistance = 5f;

    [Tooltip("Suavizado del movimiento")]
    public float Damping = 0.5f;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer framingTransposer;
    private CinemachineComposer composer;
    private Transform playerCameraRoot;
    private GameObject player;
    private float targetRotation;
    private float currentRotation;

    private void Start()
    {
        // Obtener referencias
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        
        if (virtualCamera == null)
        {
            Debug.LogError("No se encontró CinemachineVirtualCamera");
            return;
        }

        // Configurar Cinemachine
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();

        if (framingTransposer != null)
        {
            framingTransposer.m_CameraDistance = CameraDistance;
            framingTransposer.m_XDamping = Damping;
            framingTransposer.m_YDamping = Damping;
            framingTransposer.m_ZDamping = Damping;
        }

        // Encontrar el player y su camera root
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCameraRoot = player.transform.Find("PlayerCameraRoot");
            if (playerCameraRoot == null)
            {
                Debug.LogError("No se encontró PlayerCameraRoot en el Player");
                return;
            }

            // Asignar follow y lookAt
            virtualCamera.Follow = playerCameraRoot;
            virtualCamera.LookAt = playerCameraRoot;
        }
        else
        {
            Debug.LogError("No se encontró objeto con tag Player");
        }

        // Configurar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (playerCameraRoot == null || player == null) return;

        // Obtener input del ratón
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

        // Actualizar rotación
        targetRotation += mouseX * RotationSpeed;
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * RotationSpeed * 3f);

        // Aplicar rotación
        player.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        
        // Actualizar posición del camera root
        playerCameraRoot.position = player.transform.position + Vector3.up * TargetHeight;
    }

    private void OnValidate()
    {
        if (framingTransposer != null)
        {
            framingTransposer.m_CameraDistance = CameraDistance;
            framingTransposer.m_XDamping = Damping;
            framingTransposer.m_YDamping = Damping;
            framingTransposer.m_ZDamping = Damping;
        }
    }
}