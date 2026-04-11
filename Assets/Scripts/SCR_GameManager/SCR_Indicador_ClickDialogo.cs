using UnityEngine;
using UnityEngine.EventSystems; // Requerido para detectar clics

// IPointerClickHandler nos permite detectar clics en elementos de UI
public class SCR_IndicadorClicDialogo : MonoBehaviour, IPointerClickHandler
{
    // Referencia al controlador de diálogos
    [SerializeField] private SCR_ControladorDialogo controladorDialogo;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controladorDialogo != null)
        {
            // Llamamos a la función para avanzar o completar el texto
            controladorDialogo.IntentarAvanzarDialogo();
        }
    }
}