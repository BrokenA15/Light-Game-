using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraOrbit : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Transform pivot;
    public float distance = 5f;      
    public float heightOffset = 1.5f;

    [Header("Mouse Settings")]
    public float sensitivity = 0.2f;    
    public float zoomSpeed = 2f;     
    public Vector2 zoomLimits = new Vector2(2f, 15f); 

    private bool invertX = false;
    public bool invertY = false;

    [Header("Rotation Limits")]
    public bool useLimits = true;
    public Vector2 xRotationLimits = new Vector2(-50f, 30f); 
    public Vector2 yRotationLimits = new Vector2(-75f, 75f); 

    [Header("Input System")]
    public InputActionReference lookAction; 
    public InputActionReference zoomAction;

    private float currentYRotation = 0f; 
    private float currentXRotation = 0f;


    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No se ha asignado el Target a la cámara.");
            enabled = false;
            return;
        
        }

        if (lookAction != null && !lookAction.action.enabled)
            lookAction.action.Enable();

        if (zoomAction != null && !zoomAction.action.enabled)
            zoomAction.action.Enable();

        if (pivot == null)
        {
            GameObject pivotObj = new GameObject("CameraPivot");
            pivot = pivotObj.transform;
            pivot.position = target.position;
        }

        transform.SetParent(pivot);

        Vector3 offset = transform.position - target.position;
        currentYRotation = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        currentXRotation = 0f;

       
    }

    void LateUpdate()
    {
        if (target == null) return;

        pivot.position = target.position;


        Rotation();
        Zoom();
        FollowTarget();



    }

    private void Rotation()
    {
        if (lookAction != null)
        {
            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
            float mouseX = lookInput.x * sensitivity * (invertX ? -1 : 1);
            float mouseY = lookInput.y * sensitivity * (invertY ? 1 : -1);

            currentYRotation += mouseX;
            currentXRotation += mouseY;

            if (useLimits)
            {
                currentYRotation = Mathf.Clamp(currentYRotation, yRotationLimits.x, yRotationLimits.y);
                currentXRotation = Mathf.Clamp(currentXRotation, xRotationLimits.x, xRotationLimits.y);
            }
        }
    }

    private void FollowTarget()
    {
        Quaternion rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        transform.position = target.position + offset + Vector3.up * heightOffset;
        transform.LookAt(target.position + Vector3.up * heightOffset);
    }

    private void Zoom()
    {
        if (zoomAction != null)
        {
            float scroll = zoomAction.action.ReadValue<float>();
            distance -= scroll * zoomSpeed * Time.deltaTime * 100f;
            distance = Mathf.Clamp(distance, zoomLimits.x, zoomLimits.y);
        }
    }

    public void DisableLimitsAfterTP()
    {
        useLimits = false;
    }


    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        pivot.position = newTarget.position;
    }


}
