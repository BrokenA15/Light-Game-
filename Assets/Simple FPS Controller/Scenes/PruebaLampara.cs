using UnityEngine;

public class PruebaLampara : MonoBehaviour
{
    [Header("Referencia de la cámara a seguir")]
    public Transform cameraTransform;

    [Header("Desfase respecto a la cámara")]
    public Vector3 positionOffset = new Vector3(0f, -0.2f, 0.5f); // Ajusta según donde quieras que esté la lámpara
    public Vector3 rotationOffset = Vector3.zero;

    void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        // Hace que la lámpara siga la posición de la cámara con el offset
        transform.position = cameraTransform.position + cameraTransform.rotation * positionOffset;

        // Copia la rotación de la cámara más el offset adicional
        transform.rotation = cameraTransform.rotation * Quaternion.Euler(rotationOffset);
    }
}
