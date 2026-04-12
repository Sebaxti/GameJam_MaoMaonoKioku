using UnityEngine;
using UnityEngine.Video;

public class SCR_ControlVideoMinijuego : MonoBehaviour
{
    [Header("Configuración del Video")]
    [SerializeField] private VideoPlayer reproductorDeVideo;

    [Header("Identificación (Sistema Central)")]
    [Tooltip("El ID debe coincidir exactamente con el que pusiste en el GestorNiveles del MainMenu")]
    [SerializeField] private string idDeEsteNivel = "Nivel1";

    void Start()
    {
        if (reproductorDeVideo != null)
        {
            reproductorDeVideo.playOnAwake = false;
            reproductorDeVideo.loopPointReached += AlTerminarVideo;
        }
    }

    public void RegistrarAcierto()
    {
        if (reproductorDeVideo != null && !reproductorDeVideo.isPlaying)
        {
            reproductorDeVideo.Play();
        }
    }

    public void RegistrarFallo()
    {
        if (reproductorDeVideo != null && reproductorDeVideo.isPlaying)
        {
            reproductorDeVideo.Pause();
        }
    }


    private void AlTerminarVideo(VideoPlayer vp)
    {
        Debug.Log($"Nivel {idDeEsteNivel} terminado. Avisando al Gestor Central...");

        if (SCR_GestionNiveles.Instancia != null)
        {
            SCR_GestionNiveles.Instancia.CompletarNivelYContinuar(idDeEsteNivel);
        }
        else
        {
            Debug.LogError("¡No se encuentra el SCR_GestionNiveles! ¿Has iniciado el juego desde el Main Menu?");
        }
    }
}