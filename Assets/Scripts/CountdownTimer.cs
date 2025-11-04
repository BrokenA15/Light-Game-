using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [Header("Configuración del temporizador")]
    public float startTime = 240f;
    private float currentTime;

    [Header("UI")]
    public TMP_Text timerText;

    [Header("Objeto que sube")]
    public Transform movingObject;      // Asigna aquí el objeto que subirá
    public float targetHeight = 65.5f;  // Altura final
    private float initialHeight;

    void Start()
    {
        currentTime = startTime;
        if (movingObject != null)
            initialHeight = movingObject.position.y;
        UpdateTimerUI();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0)
                currentTime = 0;

            UpdateTimerUI();
            MoveObject();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void MoveObject()
    {
        if (movingObject == null) return;

        // Calculamos el progreso (0 = inicio, 1 = final)
        float progress = Mathf.InverseLerp(startTime, 0, currentTime);

        // Interpolamos la posición Y
        float newY = Mathf.Lerp(initialHeight, targetHeight, progress);

        // Aplicamos la nueva posición
        Vector3 pos = movingObject.position;
        pos.y = newY;
        movingObject.position = pos;
    }
}
