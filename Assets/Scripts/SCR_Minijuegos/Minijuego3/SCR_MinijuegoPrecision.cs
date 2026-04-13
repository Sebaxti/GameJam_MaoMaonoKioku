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
    [SerializeField] private List<float> paradasVideo;

    [Header("Ajustes")]
    [SerializeField] private string idNivel = "Nivel3";
    [SerializeField] private float velocidadBarra = 0.6f;

    private float checkpointActual = 0f;
    private bool juegoTerminado = false;
    private bool enEsperaDeSeguridad = false;

    void Start()
    {
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

        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            ActualizarProgreso();
        }
        else
        {
            if (video.isPlaying) video.Pause();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
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

        if (barraPrincipal.value >= 1f)
        {
            StartCoroutine(ProcesoFallo());
        }
    }

    void ValidarIntento()
    {
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
        ActualizarCheckpointDeSeguridad();

        barraPrincipal.value = 0f;
        GenerarNuevaZonaVerde();

        Debug.Log("¡Acierto! Checkpoint de seguridad: " + checkpointActual);
    }

    void ActualizarCheckpointDeSeguridad()
    {
        float tiempoActual = (float)video.time;
        float mejorPunto = 0f;

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

        video.time = checkpointActual;

        barraPrincipal.value = 0f;

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

        if (SCR_GestionNiveles.Instancia != null)
        {
            SCR_GestionNiveles.Instancia.CompletarNivelYContinuar(idNivel);
        }
    }
}