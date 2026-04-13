using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class TransicionEntreMJ : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    [SerializeField] private VideoPlayer reproductorVideo;

    [Header("Configuración de Nivel")]
    [SerializeField] private string idNivel = "NivelFinal";

    private bool yaSeHizoClick = false;

    void Start()
    {
        if (reproductorVideo != null)
        {

            reproductorVideo.playOnAwake = false;
            reproductorVideo.Pause();


            reproductorVideo.loopPointReached += AlFinalizarVideo;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!yaSeHizoClick)
        {
            yaSeHizoClick = true;
            IniciarAnimacionFinal();
        }
    }

    private void IniciarAnimacionFinal()
    {
        if (reproductorVideo != null)
        {
            Debug.Log("Iniciando animación final del nivel...");
            reproductorVideo.Play();
        }
    }

    private void AlFinalizarVideo(VideoPlayer vp)
    {
        Debug.Log("Video terminado. Llamando al gestor de niveles...");

        if (SCR_GestionNiveles.Instancia != null)
        {
            SCR_GestionNiveles.Instancia.CompletarNivelYContinuar(idNivel);
        }
        else
        {
            Debug.LogError("Error: No se encontró la Instancia de SCR_GestionNiveles en la escena.");
        }
    }
}
