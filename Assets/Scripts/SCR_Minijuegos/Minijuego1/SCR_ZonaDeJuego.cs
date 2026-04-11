using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class SCR_ZonaDeJuego : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencias")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Configuración de Segmentos")]
    [Tooltip("El video se detendrá en múltiplos de este número (ej: 2 para segundos pares)")]
    [SerializeField] private float intervaloSegundos = 2f;

    [Header("Configuración del Cursor")]
    [SerializeField] private Texture2D cursorPersonalizado;

    private bool estaEsperandoCorte = false;
    private double tiempoObjetivo = 0;
    private Vector2 hotspot = Vector2.zero;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += (vp) => { vp.Pause(); };
        }
    }

    // --- LÓGICA DE SEGUIMIENTO EN TIEMPO REAL ---
    void Update()
    {
        // Si el jugador soltó el clic y estamos esperando llegar al segundo par...
        if (estaEsperandoCorte && videoPlayer.isPlaying)
        {
            if (videoPlayer.time >= tiempoObjetivo)
            {
                videoPlayer.Pause();
                videoPlayer.time = tiempoObjetivo; // Ajuste exacto para evitar desfases
                estaEsperandoCorte = false;
                Debug.Log("Video detenido en punto de control: " + tiempoObjetivo);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (videoPlayer != null)
        {
            estaEsperandoCorte = false; // Cancelamos la detención si vuelve a pulsar
            videoPlayer.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            // CALCULAMOS EL SIGUIENTE SEGUNDO PAR (o múltiplo del intervalo)
            // Ejemplo: si el tiempo es 1.2 y el intervalo es 2, el objetivo es 2.0
            // Ejemplo: si el tiempo es 2.1 y el intervalo es 2, el objetivo es 4.0
            float tiempoActual = (float)videoPlayer.time;
            tiempoObjetivo = Mathf.Ceil(tiempoActual / intervaloSegundos) * intervaloSegundos;

            // Si por alguna razón el cálculo da el mismo segundo actual (muy raro), sumamos un intervalo
            if (tiempoObjetivo <= videoPlayer.time)
            {
                tiempoObjetivo += intervaloSegundos;
            }

            estaEsperandoCorte = true;
            Debug.Log("Soltaste el clic. El video se detendrá en el segundo: " + tiempoObjetivo);
        }
    }

    // --- CURSOR ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cursorPersonalizado != null) Cursor.SetCursor(cursorPersonalizado, hotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}