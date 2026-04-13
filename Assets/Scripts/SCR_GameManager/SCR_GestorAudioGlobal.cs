using UnityEngine;

public class SCR_GestorAudioGlobal : MonoBehaviour
{
    public static SCR_GestorAudioGlobal Instancia;

    [Header("Reproductor de Música")]
    [Tooltip("Arrastra aquí el AudioSource que reproducirá la música de fondo")]
    public AudioSource fuenteMusica;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            CargarVolumenGuardado();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- CONTROL DE LA MÚSICA ---
    public void ReproducirMusica(AudioClip nuevaMusica)
    {
        if (nuevaMusica == null) return;

        // Si la canción que pide la escena YA está sonando, no la reiniciamos. 
        // Así fluye perfecto si dos niveles seguidos usan la misma música.
        if (fuenteMusica.clip == nuevaMusica && fuenteMusica.isPlaying) return;

        fuenteMusica.clip = nuevaMusica;
        fuenteMusica.Play();
    }

    // --- LÓGICA GENERAL DEL VOLUMEN (25% en 25%) ---
    public void CiclarVolumenGlobal()
    {
        float volumenActual = AudioListener.volume;

        // Subimos de 0.25 en 0.25
        if (volumenActual < 0.1f) AudioListener.volume = 0.25f;
        else if (volumenActual < 0.3f) AudioListener.volume = 0.5f;
        else if (volumenActual < 0.6f) AudioListener.volume = 0.75f;
        else if (volumenActual < 0.8f) AudioListener.volume = 1f;
        else AudioListener.volume = 0f; // Si estaba al 100%, se silencia

        // Guardamos el volumen para la próxima vez que el jugador abra el juego
        PlayerPrefs.SetFloat("VolumenGeneral", AudioListener.volume);
        PlayerPrefs.Save();
    }

    private void CargarVolumenGuardado()
    {
        // Al arrancar el juego, leemos el volumen guardado (por defecto 100%)
        AudioListener.volume = PlayerPrefs.GetFloat("VolumenGeneral", 1f);
    }
}