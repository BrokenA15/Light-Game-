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
    public LayerMask raycastLayers = Physics.DefaultRaycastLayers;

    [Header("Input System")]
    public InputActionAsset inputActions;
    private InputAction m_activateLight;
    private InputAction m_transportLamp;

    [Header("Referencias externas")]
    [SerializeField] private Transform playerTransform;
    public CamaraOrbit cameraOrbit;

    public int currentColorId;
    private IdColorTp lastLamp;

    private Dictionary<int, Color> colorId = new Dictionary<int, Color>
    {
        { 0, Color.yellow },   // Amarillo
        { 1, Color.red },     // Rojo
        { 2, Color.blue },    // Azul
        { 3, Color.purple },   // Morado
        { 4, Color.green },   // Verde 
        { 5, Color.orange }, //Naranja      
        { 6, Color.cyan },   // Cyan
        { 7, Color.deepPink },    // Rosa 
        { 8, Color.white }, // Blanco 
        
    };
    private void Awake()
    {
        m_activateLight = InputSystem.actions.FindAction("ActivateLight");
        m_transportLamp = InputSystem.actions.FindAction("TransportLamp");
        colorMat.color = Color.black;

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

            if (m_transportLamp != null && m_transportLamp.WasPressedThisFrame() && lastLamp != null && lastLamp.isReady)
            {
                 TransportLamp(lastLamp.transform);
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
                    // Cambiar el color de la lámpara según la luz del jugador
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
