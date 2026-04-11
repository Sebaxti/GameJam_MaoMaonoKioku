using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections.Generic;

public class SCR_MinijuegoMachacar : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoActual;
    [SerializeField] private TextMeshProUGUI textoSiguiente;
    [SerializeField] private GameObject panelContenedor;

    [Header("Configuración Video")]
    [SerializeField] private VideoPlayer reproductor;
    [SerializeField] private float intervaloSegundos = 5f;

    [Header("Mecánica de Machacar")]
    [SerializeField] private int pulsacionesNecesarias = 10;
    [SerializeField] private List<KeyCode> teclasPosibles;
    [SerializeField] private float tiempoCastigo = 2f;
    [SerializeField] private string idNivel = "Nivel2";

    private KeyCode teclaActual;
    private KeyCode teclaSiguiente;
    private double tiempoProximaParada;
    private int contadorPulsaciones = 0;

    private bool esperandoInput = false;
    private bool enCastigo = false;
    private float cronometroCastigo;
    private bool avisoMostrado = false;

    void Start()
    {
        panelContenedor.SetActive(false);

        // Configuramos la transparencia del 50% para la tecla de "predicción"
        if (textoSiguiente != null)
        {
            Color c = textoSiguiente.color;
            c.a = 0.5f;
            textoSiguiente.color = c;
        }

        tiempoProximaParada = intervaloSegundos;
        GenerarTeclasIniciales();

        if (reproductor != null)
        {
            reproductor.loopPointReached += AlFinalizarVideoTotal;
            reproductor.Play();
        }
    }

    void Update()
    {
        if (enCastigo)
        {
            cronometroCastigo -= Time.deltaTime;
            if (cronometroCastigo <= 0) FinCastigo();
            return;
        }

        if (reproductor == null || !reproductor.isPlaying && !esperandoInput) return;

        // --- LÓGICA DE AVISO (1 segundo antes de la parada) ---
        if (!esperandoInput && !avisoMostrado)
        {
            if (reproductor.time >= (tiempoProximaParada - 1.0f))
            {
                avisoMostrado = true;
                panelContenedor.SetActive(true);
                textoSiguiente.gameObject.SetActive(true);
                textoActual.gameObject.SetActive(false); // Aún no toca pulsar
            }
        }

        // --- LÓGICA DE PARADA ---
        if (!esperandoInput && reproductor.time >= tiempoProximaParada)
        {
            PausarYPedirMachacar();
        }

        // --- LÓGICA DE MACHACAR ---
        if (esperandoInput)
        {
            if (Input.GetKeyDown(teclaActual))
            {
                RegistrarPulsacion();
            }
            else if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
            {
                ActivarCastigo();
            }
        }
    }

    void PausarYPedirMachacar()
    {
        reproductor.Pause();
        reproductor.time = tiempoProximaParada;
        esperandoInput = true;

        // Mostramos la tecla actual para machacar
        textoActual.gameObject.SetActive(true);
        ActualizarTextoContador();
    }

    void RegistrarPulsacion()
    {
        contadorPulsaciones++;
        ActualizarTextoContador();

        if (contadorPulsaciones >= pulsacionesNecesarias)
        {
            ContinuarTrasMachacar();
        }
    }

    void ContinuarTrasMachacar()
    {
        esperandoInput = false;
        avisoMostrado = false;
        contadorPulsaciones = 0;
        tiempoProximaParada += intervaloSegundos;

        // La que era "Siguiente" pasa a ser la "Actual"
        teclaActual = teclaSiguiente;
        teclaSiguiente = teclasPosibles[Random.Range(0, teclasPosibles.Count)];

        ActualizarVisualizacionTeclas();
        panelContenedor.SetActive(false);
        reproductor.Play();
    }

    void ActivarCastigo()
    {
        enCastigo = true;
        cronometroCastigo = tiempoCastigo;
        panelContenedor.SetActive(false);
    }

    void FinCastigo()
    {
        enCastigo = false;
        panelContenedor.SetActive(true);
        // Al volver del castigo, el jugador sigue teniendo que machacar la misma tecla
    }

    void GenerarTeclasIniciales()
    {
        teclaActual = teclasPosibles[Random.Range(0, teclasPosibles.Count)];
        teclaSiguiente = teclasPosibles[Random.Range(0, teclasPosibles.Count)];
        ActualizarVisualizacionTeclas();
    }

    void ActualizarVisualizacionTeclas()
    {
        textoActual.text = teclaActual.ToString().Replace("Arrow", "");
        textoSiguiente.text = teclaSiguiente.ToString().Replace("Arrow", "");
    }

    void ActualizarTextoContador()
    {
        // Opcional: Puedes hacer que el texto diga "TECLA (10/10)"
        textoActual.text = teclaActual.ToString().Replace("Arrow", "");
    }

    private void AlFinalizarVideoTotal(VideoPlayer vp)
    {
        if (SCR_GestionNiveles.Instancia != null)
        {
            SCR_GestionNiveles.Instancia.CompletarNivelYContinuar(idNivel);
        }
    }
}