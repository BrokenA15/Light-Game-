using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class GetColorId : MonoBehaviour
{
    [Header("Configuración")]
    public LightOn colorChange;
    public Image[] images; 
    public float normalScale = 0.6f;
    public float selectedScale = 0.7f;

    [Header("Input Actions")]
    public InputActionReference navigateAction; 

    public int currentIndex = 0;
 

    private void OnEnable()
    {
        if (navigateAction != null)
        {
            navigateAction.action.Enable();
            navigateAction.action.performed += OnNavigate;
        }
        else
        {
            Debug.LogError("No se asignó la acción de navegación (navigateAction).");
        }

        UpdateSelection();
    }

    private void OnDisable()
    {
        if (navigateAction != null)
        {
            navigateAction.action.performed -= OnNavigate;
            navigateAction.action.Disable();
        }
    }


    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (colorChange != null && colorChange.IsLightActive())
            return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input.x > 0.5f)
            MoveSelection(1);
        else if (input.x < -0.5f)
            MoveSelection(-1);
        else if (input.y > 0.5f)
            MoveSelection(-3); 
        else if (input.y < -0.5f)
            MoveSelection(3); 
    }

    private void MoveSelection(int change)
    {
        images[currentIndex].transform.localScale = new Vector3(normalScale, normalScale, normalScale);
        currentIndex += change;

        if (currentIndex < 0) currentIndex = 0;
        if (currentIndex >= images.Length) currentIndex = images.Length - 1;

        UpdateSelection();
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < images.Length; i++)
        {

            images[i].transform.localScale = new Vector3(normalScale, normalScale, normalScale);
        }
       

        if (images.Length > 0)
        {

            images[currentIndex].transform.localScale = new Vector3(selectedScale, selectedScale, selectedScale);
            colorChange.ChangeColor(currentIndex);
        }

    }

}
