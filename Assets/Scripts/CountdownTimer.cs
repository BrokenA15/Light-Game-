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
    public Transform movingObject;     
    public Transform Tornado1;
    public Transform Tornado2;
    public Transform Tornado3;
    public float targetHeight = 65.5f;  
    private float initialHeight;
    private float initialHeightT1;
    private float initialHeightT2;
    private float initialHeightT3;


    void Start()
    {
        currentTime = startTime;
        if (movingObject != null)
            initialHeight = movingObject.position.y;
        initialHeightT1 = Tornado1.position.y;
        initialHeightT2 = Tornado2.position.y;
        initialHeightT3 = Tornado3.position.y;
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
            MoveTornados();
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

        float progress = Mathf.InverseLerp(startTime, 0, currentTime);

        float newY = Mathf.Lerp(initialHeight, targetHeight, progress);

        Vector3 pos = movingObject.position;
        pos.y = newY;
        movingObject.position = pos;
    }

    void MoveTornados()
    {
        float progress = Mathf.InverseLerp(startTime, 0, currentTime);

        float newY1 = Mathf.Lerp(initialHeightT1, targetHeight, progress);
        float newY2 = Mathf.Lerp(initialHeightT2, targetHeight, progress);
        float newY3 = Mathf.Lerp(initialHeightT3, targetHeight, progress);

        Vector3 pos1 = Tornado1.position;
        pos1.y = newY1;
        Vector3 pos2 = Tornado2.position;
        pos2.y = newY2;
        Vector3 pos3 = Tornado3.position;
        pos3.y = newY3;
        Tornado1.position = pos1;
        Tornado2.position = pos2;
        Tornado3.position = pos3;
    }
}
