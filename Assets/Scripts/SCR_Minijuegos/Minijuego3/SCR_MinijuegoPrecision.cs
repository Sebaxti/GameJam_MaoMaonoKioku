using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;

public class SCR_MinijuegoPrecision : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VideoPlayer video;
    [SerializeField] private Slider barraPrincipal;
    [SerializeField] private RectTransform zonaObjetivo; // El cuadradito verde
    [SerializeField] private GameObject indicadorVisual; // La barrita que se mueve

    [Header("Configuración de Tramos (Segundos)")]
    [Tooltip("Puntos donde el video se detendrá para el minijuego")]
    [SerializeField] private List<float> paradasVideo;

    [Header("Ajustes")]
    [SerializeField] private string idNivel = "Nivel3";
    [SerializeField] private float velocidadBarra = 0.5f;

    private int indiceActual = 0;
    private bool juegoActivo = false;
    private bool tramoSuperado = false;
    private float tiempoInicioTramo = 0f;

    void Start()
    {
        video.Pause();
        ConfigurarNuevoTramo();
    }

    void Update()
    {
        if (indiceActual >= paradasVideo.Count) return;

        // MANTENER PRESIONADO
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            if (!tramoSuperado)
            {
                AvanzarMinijuego();
            }
        }

        // SOLTAR
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            ValidarIntento();
        }

        // Si el tramo ya fue superado, el video corre hasta la siguiente parada
        if (tramoSuperado)
        {
            if (video.time >= paradasVideo[indiceActual])
            {
                LlegadaAParada();
            }
        }
    }

    void ConfigurarNuevoTramo()
    {
        tramoSuperado = false;
        video.Pause();

        // Resetear la barra al inicio
        barraPrincipal.value = 0;

        // Mover la zona verde a un lugar aleatorio de la barra (0.2 a 0.8 para que no esté en los bordes)
        float posicionAzar = Random.Range(0.2f, 0.8f);
        zonaObjetivo.anchorMin = new Vector2(posicionAzar - 0.05f, 0);
        zonaObjetivo.anchorMax = new Vector2(posicionAzar + 0.05f, 1);

        juegoActivo = true;
    }

    void AvanzarMinijuego()
    {
        // 1. El video avanza mientras presionas
        if (!video.isPlaying) video.Play();

        // 2. La barra se llena
        barraPrincipal.value += velocidadBarra * Time.deltaTime;

        // 3. Si llega al final y no soltaste, es FALLO automático
        if (barraPrincipal.value >= 1f)
        {
            Fallar();
        }
    }

    void ValidarIntento()
    {
        if (!juegoActivo) return;

        // Calculamos la posición de la barra vs la zona verde
        float centroVerde = zonaObjetivo.anchorMin.x + 0.05f;
        float margen = 0.06f; // El ancho de tu zona de acierto

        if (Mathf.Abs(barraPrincipal.value - centroVerde) < margen)
        {
            Aceptar();
        }
        else
        {
            Fallar();
        }
    }

    void Aceptar()
    {
        Debug.Log("¡Acierto! El video continúa.");
        tramoSuperado = true;
        juegoActivo = false;
        video.Play();
    }

    void Fallar()
    {
        Debug.Log("Fallo. Reiniciando tramo.");
        video.Pause();
        video.time = tiempoInicioTramo; // Vuelve al inicio del intervalo actual
        barraPrincipal.value = 0;
        ConfigurarNuevoTramo();
    }

    void LlegadaAParada()
    {
        video.Pause();
        tiempoInicioTramo = (float)video.time;
        indiceActual++;

        if (indiceActual < paradasVideo.Count)
        {
            ConfigurarNuevoTramo();
        }
        else
        {
            FinalizarNivel();
        }
    }

    void FinalizarNivel()
    {
        if (SCR_GestionNiveles.Instancia != null)
            SCR_GestionNiveles.Instancia.GuardarProgreso(idNivel);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}