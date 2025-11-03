using UnityEngine;

public class LightRotation : MonoBehaviour
{
    [Header("Cámara a seguir")]
    public Transform cameraToFollow; 

    [Header("Offset opcional")]
    public Vector3 rotationOffset = Vector3.zero; 

    void LateUpdate()
    {
        if (cameraToFollow == null) return;

        transform.rotation = cameraToFollow.rotation * Quaternion.Euler(rotationOffset);
    }
}
