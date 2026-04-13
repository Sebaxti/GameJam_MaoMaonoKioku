using UnityEngine;

public class SCR_LanzadorMusica : MonoBehaviour
{
    [Tooltip("Arrastra aquí el archivo MP3/WAV de la canción para esta escena")]
    public AudioClip musicaDeEstaEscena;

    void Start()
    {
        // Al cargar la escena, le decimos al cerebro global que ponga esta canción
        if (SCR_GestorAudioGlobal.Instancia != null && musicaDeEstaEscena != null)
        {
            SCR_GestorAudioGlobal.Instancia.ReproducirMusica(musicaDeEstaEscena);
        }
    }
}