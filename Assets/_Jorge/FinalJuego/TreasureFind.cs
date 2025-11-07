using UnityEngine;
using UnityEngine.SceneManagement; // para reiniciar la escena
using UnityEngine.UI;
using TMPro;

public class TreasurePickup : MonoBehaviour
{
    [Header("UI")]
    public GameObject uiPanel;              // Panel que se mostrará al recoger el tesoro
    public TextMeshProUGUI messageText;     // Texto "¡Encontraste el tesoro oculto!"
    public Button restartButton;            // Botón de reiniciar
    public Button quitButton;               // Botón de salir

    private bool found = false;

    void Start()
    {
        // Asegurarse de que la UI está oculta al inicio
        if (uiPanel != null)
            uiPanel.SetActive(false);

        // Configurar botones
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar si el jugador tocó el objeto
        if (!found && other.CompareTag("Player"))
        {
            found = true;
            OnTreasureFound();
        }
    }

    void OnTreasureFound()
    {
        // Ocultar el tesoro (este objeto)
        gameObject.SetActive(false);

        // Mostrar el panel de UI
        if (uiPanel != null)
            uiPanel.SetActive(true);

        // Mostrar el mensaje
        if (messageText != null)
            messageText.text = "¡Acabaste la demo!";

        // Pausar el juego
        Time.timeScale = 0f;
    }

    void RestartGame()
    {
        // Quitar la pausa
        Time.timeScale = 1f;
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void QuitGame()
    {
        // Quitar la pausa
        Time.timeScale = 1f;
        // Salir del juego
        Application.Quit();

        // (En el editor no funciona, pero podemos simularlo)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
