using UnityEngine;
using UnityEngine.Video;

public class SCR_CreditosVideo : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto que tiene el componente Video Player")]
    [SerializeField] private VideoPlayer reproductorVideo;

    [Header("Configuración de Nivel")]
    [Tooltip("El ID de esta escena (Ej: Creditos_Finales) para que el Gestor sepa qué hacer")]
    [SerializeField] private string idNivel = "Creditos_Finales";

    void Start()
    {
        if (reproductorVideo != null)
        {
            // 1. Le decimos que nos avise cuando el vídeo termine su reproducción natural
            reproductorVideo.loopPointReached += AlFinalizarVideo;

            // 2. Nos aseguramos de que el vídeo comience a reproducirse al entrar a la escena
            reproductorVideo.Play();
        }
        else
        {
            Debug.LogError("¡Falta asignar el VideoPlayer en el script de Créditos!");
        }
    }

    private void AlFinalizarVideo(VideoPlayer vp)
    {
        Debug.Log("Vídeo de créditos terminado. Llamando al Gestor de Niveles...");

        // Desconectamos el evento por seguridad
        reproductorVideo.loopPointReached -= AlFinalizarVideo;

        // Le pasamos el ID al gestor para que él decida a dónde ir ahora
        if (SCR_GestionNiveles.Instancia != null)
        {
            SCR_GestionNiveles.Instancia.CompletarNivelYContinuar(idNivel);
        }
    }
}