using UnityEngine;
using UnityEngine.EventSystems;


public class SCR_IndicadorClicDialogo : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SCR_ControladorDialogo controladorDialogo;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controladorDialogo != null)
        {
            controladorDialogo.IntentarAvanzarDialogo();
        }
    }
}