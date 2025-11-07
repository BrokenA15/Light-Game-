using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightOn : MonoBehaviour
{
    [Header("Configuración de luz")]
    public Light lightSettings;
    public Material colorMat;
    [SerializeField]
    private GetColorId getColorId;
    public float intensity = 300f;

    [Header("Raycast Settings")]
    public float rayLength = 10f;
    private float rayRadius = 0.3f;
    public Vector3 rayOffset = Vector3.zero;      
    public bool isLampHit = false;         
    public bool isFloorHit = false;
    public LayerMask raycastLayers = Physics.DefaultRaycastLayers;
    public float floorYOffset = 1.5f;

    [Header("Input System")]
    public InputActionAsset inputActions;
    private InputAction m_activateLight;
    private InputAction m_transportLamp;
    private InputAction m_returnPlayer;

    [Header("Referencias externas")]
    [SerializeField] private Transform playerTransform;
    public CamaraOrbit cameraOrbit;
    public SFPSC_PlayerMovement movement;
    [SerializeField] private LightRotation lightRotation;
    [SerializeField] private Camera lampCamera;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject playerLamp;
    [SerializeField] private Transform playerLampTransform;
    private Quaternion lampRotation;
    private Vector3 lampPosition;
    [SerializeField] private Transform Player;
    [SerializeField] private Transform Light;
    private Vector3 lightScale;
    private RaycastHit floorHitInfo;


    public int currentColorId;
    private IdColorTp lastLamp;

    public ParticleSystem tpReady;
    public ParticleSystem tpNotReady;


    private Dictionary<int, Color> colorId = new Dictionary<int, Color>
    {
        { 0, Color.yellow },   // Amarillo
        { 1, Color.red },     // Rojo
        { 2, Color.blue },    // Azul
        { 3, Color.green },   // Verde 
        { 4, Color.deepPink },    // Rosa 
       
        
    };
    private void Awake()
    {
        m_activateLight = InputSystem.actions.FindAction("ActivateLight");
        m_transportLamp = InputSystem.actions.FindAction("TransportLamp");



        colorMat.color = Color.black;
        lightRotation.enabled = false;
        playerCamera.enabled = true;
        lampCamera.enabled = false;
        playerLamp.SetActive(true);
        tpNotReady.Stop();
        tpReady.Stop();
        lightScale = Light.localScale;
        lampRotation = transform.localRotation;
        lampPosition = transform.localPosition;


    }


    void Start()
    {
        lightSettings = GetComponent<Light>();
        colorMat.color = Color.black;
        colorMat.SetColor("_EmissionColor", Color.black * 0f);
        lightSettings.intensity = 0f;
        currentColorId = getColorId.currentIndex;
        
    }

    public void ChangeColor(int colorCode)
    {
        currentColorId = colorCode;
        if (colorId.ContainsKey(currentColorId))
        {
            lightSettings.color = colorId[currentColorId];
            colorMat.color = colorId[currentColorId];
            colorMat.SetColor("_EmissionColor", colorId[currentColorId] * 20f);
        }
    }

   


    void Update()
    {
      
        if (m_activateLight != null && m_activateLight.IsPressed())
        {
            ActivateLight();
            DoRaycastCheck();

            if (m_transportLamp != null && m_transportLamp.WasPressedThisFrame())
            {
                TransportLampOrReturn();
            }
        }
        else
        {
            DeactivateLight();
        }

    }
    private void DoRaycastCheck()
    {
        isLampHit = false;
        isFloorHit = false;

        Vector3 origin = transform.position + rayOffset;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, rayRadius, direction, out RaycastHit hit, rayLength, raycastLayers))
        {
            var lamp = hit.collider.GetComponent<IdColorTp>();

            if (lamp != null && hit.collider.CompareTag("Lamp"))
            {
                if (lastLamp != null && lastLamp != lamp)
                    lastLamp.OnRayExit();

                lastLamp = lamp;

                if (lamp.colorId == currentColorId)
                {
                    isLampHit = true;
                    lamp.OnMatchedColorHit();
                    lamp.SetColor(colorId[currentColorId]);
                    Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
                }
                else
                {
                    Debug.Log("Color incorrecto");
                    lamp.OnRayExit();
                    Debug.DrawRay(origin, direction * hit.distance, Color.red);
                }
            }
            else if (hit.collider.CompareTag("Floor"))
            {
                isFloorHit = true;
                floorHitInfo = hit;
                Debug.Log("TP a floor listo");
                Debug.DrawRay(origin, direction * hit.distance, Color.green);
            }
            else
            {
                ResetLamp();
                Debug.DrawRay(origin, direction * hit.distance, Color.red);
            }
        }
        else
        {
            ResetLamp();
            Debug.DrawRay(origin, direction * rayLength, Color.red);
        }
    }

    public void TransportLamp(Transform lampTransform)
    {
        if (lampTransform == null) return;

        Vector3 oldPos = transform.position;

        transform.SetParent(lampTransform);
        transform.localPosition = Vector3.zero;
        


        if (cameraOrbit != null)
        {
            cameraOrbit.target = lampTransform;
            cameraOrbit.DisableLimitsAfterTP();
        }

        Debug.Log($"Jugador teletransportado a {lampTransform.name}");

        Vector3 travelDir = lampTransform.position - oldPos;

        IdColorTp lamp = lampTransform.GetComponent<IdColorTp>();
        if (lamp != null)
        {
            lamp.ApplyRecoil(travelDir);
        }

        lightRotation.enabled = true;
        playerCamera.enabled = false;
        lampCamera.enabled = true;
        movement.BlockJump();
        movement.DisableMovement();
        playerLamp.SetActive(false);

    }

    private void TransportLampOrReturn()
    {
        // Prioriza lámpara si la hay y está ready
        if (lastLamp != null && lastLamp.isReady && isLampHit)
        {
            TransportLamp(lastLamp.transform);
            return;
        }

        // Si no hay lámpara válida pero sí piso, regresa al Player
        if (isFloorHit)
        {
            ReturnToPlayer(floorHitInfo.point);
            return;
        }

        // Ningún objetivo válido
        tpNotReady.Play();
        Debug.Log("Ningún objetivo válido para TP");
    }

 

    private void ReturnToPlayer(Vector3 floorPoint)
    {
        if (Player == null) return;

        
        Vector3 targetPos = floorPoint + Vector3.up * floorYOffset;
        Player.SetParent(null);
        Player.position = targetPos;

        if (playerLampTransform != null)
        {
            transform.SetParent(playerLampTransform);
            transform.localPosition = lampPosition;
            transform.localRotation = lampRotation;
            transform.localScale = lightScale;
            playerLamp.SetActive(true);

        }

        transform.SetParent(playerLampTransform);
        transform.localPosition = lampPosition;
        lightRotation.enabled = false;
        playerCamera.enabled = true;
        lampCamera.enabled = false;
        movement.UnblockJump();
        movement.EnableMovement();   
        playerLamp.SetActive(true);
        transform.localRotation = lampRotation;
        transform.localScale = lightScale;





        Debug.Log("Jugador regresó del modo lámpara al piso");
    }


    public Color GetColorById(int id)
    {
        if (colorId.ContainsKey(id))
            return colorId[id];
        return Color.black;
    }

    private void ActivateLight()
    {
        if (!colorId.ContainsKey(currentColorId)) return;

        lightSettings.intensity = intensity;
        lightSettings.color = colorId[currentColorId];

        colorMat.color = colorId[currentColorId];
        colorMat.SetColor("_EmissionColor", colorId[currentColorId] * 20f);

    }

    private void DeactivateLight()
    {
        lightSettings.intensity = 0f;
        colorMat.color = Color.black;
        colorMat.SetColor("_EmissionColor", Color.black * 0f);
        isLampHit = false;
    }


    private void ResetLamp()
    {
        if (lastLamp != null)
        {
            lastLamp.OnRayExit();
            lastLamp = null;
        }
    }
    public bool IsLightActive() =>
       m_activateLight != null && m_activateLight.IsPressed();

}
