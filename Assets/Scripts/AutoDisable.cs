using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float delay = 15f; 

    private void Start()
    {
        StartCoroutine(DisableAfterDelay());
    }

    private System.Collections.IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}