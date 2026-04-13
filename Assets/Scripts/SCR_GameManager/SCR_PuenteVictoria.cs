using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections.Generic;

// Creamos un bloque para el Inspector
[System.Serializable]
public class DatosVideoVictoria
{
    [Tooltip("El ID del ÚLTIMO minijuego de la receta (Ej: Minijuego3_Sopa)")]
    public string idUltimoNivel;

    [Tooltip("El vídeo que se reproducirá para esta receta")]
    public VideoClip videoCelebracion;
}

public class SCR_PuenteVictoria : MonoBehaviour
{
    [Header("Destino Final")]
    [Tooltip("El nombre de tu escena de estrellas")]
    public string escenaResultados = "SCN_Resultados";

    [Header("Reproductor")]
    [Tooltip("Arrastra aquí el componente Video Player")]
    public VideoPlayer reproductorVideo;

    [Header("Configuración de Vídeos por Receta")]
    [Tooltip("Añade aquí qué vídeo va con qué receta")]
    public List<DatosVideoVictoria> configuracionVideos;

    void Start()
    {
        if (reproductorVideo != null && SCR_GestionNiveles.Instancia != null)
        {
            // 1. Preguntamos al Gestor qué minijuego se acaba de terminar
            string idTerminado = SCR_GestionNiveles.Instancia.idNivelRecienTerminado;

            // 2. Buscamos ese ID en nuestra lista
            DatosVideoVictoria datos = configuracionVideos.Find(v => v.idUltimoNivel == idTerminado);

            // 3. Si lo encontramos, le ponemos su vídeo correspondiente
            if (datos != null && datos.videoCelebracion != null)
            {
                reproductorVideo.clip = datos.videoCelebracion;
            }
            else
            {
                Debug.LogWarning("No se encontró un vídeo configurado para el nivel: " + idTerminado);
                // Si no hay vídeo, saltará un aviso pero reproducirá el que tenga por defecto.
            }

            // 4. Preparamos el salto y le damos al Play
            reproductorVideo.loopPointReached += AlTerminarVideo;
            reproductorVideo.Play();
        }
        else
        {
            Debug.LogError("Falta el VideoPlayer o el Gestor de Niveles.");
        }
    }

    private void AlTerminarVideo(VideoPlayer vp)
    {
        // El vídeo terminó, nos vamos a las estrellas a cobrar
        SceneManager.LoadScene(escenaResultados);
    }
}