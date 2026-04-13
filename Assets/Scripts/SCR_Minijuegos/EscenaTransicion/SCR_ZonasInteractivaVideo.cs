using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class DatosZonaInteractiva
{
    public GameObject objetoZona;
    public float segundoDestino;
}

public class SCR_ZonasInteractivaVideo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VideoPlayer reproductorVideo;

    [Header("Iconos del Ratón")]
    [Tooltip("La foto del ratón cuando pasas por la zona correcta (Ej: Una manita)")]
    public Texture2D cursorInteractivo;
    [Tooltip("Déjalo vacío para usar la flecha de Windows/Mac normal, o pon tu propia flecha")]
    public Texture2D cursorNormal;

    [Header("Zonas Clicables (El orden importa)")]
    [Tooltip("El jugador tendrá que pulsarlas exactamente en el orden de esta lista.")]
    public List<DatosZonaInteractiva> listaDeZonas;

    [Header("Configuración de Nivel")]
    [SerializeField] private string idNivel = "NivelFinal";

    private double tiempoObjetivo;
    private bool estaAvanzando = false;

    // Aquí llevamos la cuenta de por qué paso vamos
    private int indiceZonaQueTocaPulsar = 0;

    void Start()
    {
        if (reproductorVideo != null)
        {
            reproductorVideo.playOnAwake = false;
            reproductorVideo.Pause();
            reproductorVideo.loopPointReached += AlFinalizarVideo;
        }

        // Configuramos todas las zonas con un bucle "for" para saber su número de lista
        for (int i = 0; i < listaDeZonas.Count; i++)
        {
            int indiceDeEstaZona = i; // Guardamos el número para no perderlo
            GameObject zona = listaDeZonas[i].objetoZona;

            if (zona != null)
            {
                EventTrigger disparador = zona.GetComponent<EventTrigger>();
                if (disparador == null) disparador = zona.AddComponent<EventTrigger>();

                // 1. Detectar el CLIC
                EventTrigger.Entry entradaClick = new EventTrigger.Entry();
                entradaClick.eventID = EventTriggerType.PointerClick;
                entradaClick.callback.AddListener((data) => { PulsarZona(indiceDeEstaZona); });
                disparador.triggers.Add(entradaClick);

                // 2. Detectar cuando el ratón ENTRA a la zona
                EventTrigger.Entry entradaRaton = new EventTrigger.Entry();
                entradaRaton.eventID = EventTriggerType.PointerEnter;
                entradaRaton.callback.AddListener((data) => { AlEntrarRaton(indiceDeEstaZona); });
                disparador.triggers.Add(entradaRaton);

                // 3. Detectar cuando el ratón SALE de la zona
                EventTrigger.Entry salidaRaton = new EventTrigger.Entry();
                salidaRaton.eventID = EventTriggerType.PointerExit;
                salidaRaton.callback.AddListener((data) => { AlSalirRaton(); });
                disparador.triggers.Add(salidaRaton);
            }
        }
    }

    // --- AL PASAR EL RATÓN ---
    private void AlEntrarRaton(int indice)
    {
        // Solo cambia el icono si la zona por la que pasamos es la que toca pulsar ahora y el vídeo está pausado
        if (indice == indiceZonaQueTocaPulsar && !estaAvanzando)
        {
            if (cursorInteractivo != null)
            {
                // Cambia el cursor. La posición Vector2.zero es la punta del ratón.
                Cursor.SetCursor(cursorInteractivo, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    private void AlSalirRaton()
    {
        // Vuelve al cursor normal
        Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.Auto);
    }

    // --- AL HACER CLIC ---
    private void PulsarZona(int indicePulsado)
    {
        // Comprobamos si nos estamos adelantando
        if (indicePulsado > indiceZonaQueTocaPulsar)
        {
            Debug.Log("¡Aún no! Primero debes completar el paso anterior.");
            return; // Bloqueamos la acción
        }

        // Si es la zona correcta y el vídeo no está avanzando ya...
        if (indicePulsado == indiceZonaQueTocaPulsar && !estaAvanzando && reproductorVideo != null)
        {
            float segundoDestino = listaDeZonas[indicePulsado].segundoDestino;

            if (reproductorVideo.time < segundoDestino)
            {
                tiempoObjetivo = segundoDestino;
                estaAvanzando = true;
                reproductorVideo.Play();

                // Sumamos 1 para que ahora toque pulsar la siguiente zona
                indiceZonaQueTocaPulsar++;

                // Reseteamos el ratón inmediatamente por si cambia de escena
                AlSalirRaton();

                if (tiempoObjetivo >= reproductorVideo.length)
                    Debug.Log("Paso correcto. El vídeo irá hasta el final.");
                else
                    Debug.Log("Paso correcto. Avanzando hasta " + tiempoObjetivo + "s.");
            }
        }
    }

    // --- CONTROL DEL TIEMPO ---
    void Update()
    {
        if (estaAvanzando && reproductorVideo != null && reproductorVideo.isPlaying)
        {
            if (tiempoObjetivo < reproductorVideo.length)
            {
                if (reproductorVideo.time >= tiempoObjetivo)
                {
                    reproductorVideo.Pause();
                    reproductorVideo.time = tiempoObjetivo;
                    estaAvanzando = false;
                }
            }
        }
    }

    private void AlFinalizarVideo(VideoPlayer vp)
    {
        estaAvanzando = false;
        AlSalirRaton(); // Por seguridad, devolvemos el cursor a la normalidad

        if (SCR_GestionNiveles.Instancia != null)
        {
            SCR_GestionNiveles.Instancia.CompletarNivelYContinuar(idNivel);
        }
    }

    // Por seguridad: si sales del minijuego de repente, el cursor se resetea
    void OnDisable()
    {
        AlSalirRaton();
    }
}