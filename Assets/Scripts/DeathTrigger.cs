using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathTrigger : MonoBehaviour
{
    public Transform Jugador;
    public string SceneName;

    public float slowMotionTarget = 0.3f;  // valor final del timescale
    public float slowMotionDuration = 2f;  // tiempo que tarda en llegar a 0.3
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(DeathCount());
        }
    }
    private IEnumerator DeathCount()
    {
        float startScale = Time.timeScale;
        float elapsed = 0f;

        while (elapsed < slowMotionDuration)
        {
            elapsed += Time.unscaledDeltaTime; 
            float t = elapsed / slowMotionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            Time.timeScale = Mathf.Lerp(startScale, slowMotionTarget, t);
            yield return null;
        }

        Time.timeScale = slowMotionTarget;

        yield return new WaitForSecondsRealtime(5f);

        elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / 0.5f;
            t = Mathf.SmoothStep(0f, 1f, t);
            Time.timeScale = Mathf.Lerp(slowMotionTarget, 1f, t);
            yield return null;
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneName);
    }
}
