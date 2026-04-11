using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class SCR_MinijuegoPrecision : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VideoPlayer video;
    [SerializeField] private Slider barraPrincipal;
    [SerializeField] private RectTransform zonaObjetivo;

    [Header("Configuración de Checkpoints (Segundos)")]
    [Tooltip("El video volverá a estos segundos exactos si fallas después de haberlos superado.")]
    [SerializeField] private List<float> paradasVideo;

    [Header("Ajustes")]
    [SerializeField] private string idNivel = "Nivel3";
    [SerializeField] private float velocidadBarra = 0.6f;

    private float checkpointActual = 0f;
    private bool juegoTerminado = false;
    private bool enEsperaDeSeguridad = false;

    void Start()
    {
        // 1. Reset absoluto de la interfaz al arrancar
        if (barraPrincipal != null)
        {
            barraPrincipal.minValue = 0f;
            barraPrincipal.maxValue = 1f;
            barraPrincipal.value = 0f;
        }

        if (video != null)
        {
            video.loopPointReached += AlFinalizarVideoTotal;
            video.playOnAwake = false;
            video.Pause();
            video.time = 0;
        }

        GenerarNuevaZonaVerde();
    }

    void Update()
    {
        if (juegoTerminado || enEsperaDeSeguridad) return;

        // MIENTRAS MANTIENES PULSADO: El video avanza y la barra se llena
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            ActualizarProgreso();
        }
        else
        {
            // Si dejas de pulsar, el video se pausa inmediatamente
            if (video.isPlaying) video.Pause();
        }

        // AL SOLTAR: Evaluamos si la barra estaba en la zona verde
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            // Solo validamos si la barra se ha movido un poco (evita fallos por clics accidentales)
            if (barraPrincipal.value > 0.05f)
            {
                ValidarIntento();
            }
        }
    }

    void ActualizarProgreso()
    {
        if (!video.isPlaying) video.Play();

        barraPrincipal.value += velocidadBarra * Time.deltaTime;

        // Si la barra llega al 100% y no has soltado, fallas por tardanza
        if (barraPrincipal.value >= 1f)
        {
            StartCoroutine(ProcesoFallo());
        }
    }

    void ValidarIntento()
    {
        // Calculamos los bordes visuales de la zona verde
        float inicioVerde = zonaObjetivo.anchorMin.x;
        float finVerde = zonaObjetivo.anchorMax.x;

        if (barraPrincipal.value >= inicioVerde && barraPrincipal.value <= finVerde)
        {
            Aceptar();
        }
        else
        {
            StartCoroutine(ProcesoFallo());
        }
    }

    void Aceptar()
    {
        // Guardamos el progreso: buscamos en la lista qué parada hemos superado ya
        ActualizarCheckpointDeSeguridad();

        // Limpiamos la barra y ponemos un nuevo reto para seguir avanzando el video
        barraPrincipal.value = 0f;
        GenerarNuevaZonaVerde();

        Debug.Log("¡Acierto! Checkpoint de seguridad: " + checkpointActual);
    }

    void ActualizarCheckpointDeSeguridad()
    {
        float tiempoActual = (float)video.time;
        float mejorPunto = 0f;

        // Buscamos el tiempo más alto en la lista que sea menor al tiempo actual del video
        foreach (float p in paradasVideo)
        {
            if (p <= tiempoActual) mejorPunto = p;
            else break;
        }
        checkpointActual = mejorPunto;
    }

    IEnumerator ProcesoFallo()
    {
        enEsperaDeSeguridad = true;
        video.Pause();

        // PENALIZACIÓN: El video vuelve al último checkpoint que lograste pasar
        video.time = checkpointActual;

        // RESET DE BARRA: Forzamos que vuelva a 0
        barraPrincipal.value = 0f;

        // Pequeña espera para que el jugador suelte el dedo y se prepare
        yield return new WaitForSeconds(0.3f);

        enEsperaDeSeguridad = false;
        GenerarNuevaZonaVerde();
    }

    void GenerarNuevaZonaVerde()
    {
        float anchoZona = 0.15f;
        float posicionAzar = Random.Range(0.1f, 0.8f);
        zonaObjetivo.anchorMin = new Vector2(posicionAzar, 0);
        zonaObjetivo.anchorMax = new Vector2(posicionAzar + anchoZona, 1);
    }

    private void AlFinalizarVideoTotal(VideoPlayer vp)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        // El nivel se completa SOLO cuando el video llega al final
        if (SCR_GestionNiveles.Instancia != null)
        {
            SCR_GestionNiveles.Instancia.CompletarNivelYContinuar(idNivel);
        }
    }
}