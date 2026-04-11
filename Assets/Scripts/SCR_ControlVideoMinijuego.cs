using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class SCR_ControlVideoMinijuego : MonoBehaviour
{
    [Header("Configuración del Video")]
    [SerializeField] private VideoPlayer reproductorDeVideo;

    [Header("Flujo de Escenas (Configurable)")]
    [Tooltip("ID para guardar el progreso (ej: Nivel1)")]
    [SerializeField] private string idDeEsteNivel = "Nivel1";

    [Tooltip("Escribe el nombre de la escena a la que quieres ir al ganar")]
    [SerializeField] private string escenaSiguiente = "MainMenu";

    void Start()
    {
        if (reproductorDeVideo != null)
        {
            reproductorDeVideo.playOnAwake = false;
            reproductorDeVideo.loopPointReached += AlTerminarVideo;
        }
    }

    // Esta función se llamará cuando el jugador haga las cosas bien
    public void RegistrarAcierto()
    {
        if (reproductorDeVideo != null && !reproductorDeVideo.isPlaying)
        {
            reproductorDeVideo.Play();
        }
    }

    // Esta función se llamará cuando el jugador se equivoque
    public void RegistrarFallo()
    {
        if (reproductorDeVideo != null && reproductorDeVideo.isPlaying)
        {
            reproductorDeVideo.Pause();
        }
    }

    // Esta función se ejecuta automáticamente cuando el video llega a su fin
    private void AlTerminarVideo(VideoPlayer vp)
    {
        Debug.Log("Video finalizado. Guardando progreso y cambiando de escena...");

        // 1. Guardamos el progreso en el Gestor de Niveles
        if (SCR_GestionNiveles.Instancia != null)
        {
            SCR_GestionNiveles.Instancia.GuardarProgreso(idDeEsteNivel);
        }

        // 2. Cargamos la escena que tú elijas en el Inspector
        SceneManager.LoadScene(escenaSiguiente);
    }
}