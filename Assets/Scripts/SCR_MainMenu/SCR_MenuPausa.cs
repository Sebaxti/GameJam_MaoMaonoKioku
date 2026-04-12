using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SCR_MenuPausa : MonoBehaviour
{
    public static SCR_MenuPausa Instancia;

    [Header("Referencias UI")]
    [SerializeField] private GameObject panelPausa;

    [Header("Configuración de Volumen")]
    [SerializeField] private Image iconoBotonVolumen;
    [SerializeField] private Sprite spriteVolumenNormal;
    [SerializeField] private Sprite spriteVolumenMute;

    [Header("Restricciones de Escenas")]
    [Tooltip("Arrastra aquí los archivos de las escenas donde NO quieres que funcione la pausa.")]
    [SerializeField] private List<Object> escenasProhibidasAssets;

    private List<string> nombresEscenasProhibidas = new List<string>();
    private bool juegoPausado = false;
    private VideoPlayer[] videosEnEscena;
    private bool[] estadoVideos;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            PrepararListaEscenas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Convertimos los Assets de escena a Strings una sola vez para evitar "hardcoding"
    void PrepararListaEscenas()
    {
        foreach (Object objeto in escenasProhibidasAssets)
        {
            if (objeto != null) nombresEscenasProhibidas.Add(objeto.name);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string escenaActual = SceneManager.GetActiveScene().name;

            // Verificamos si la escena actual está en la lista de prohibidas
            if (!nombresEscenasProhibidas.Contains(escenaActual))
            {
                if (juegoPausado) ReanudarJuego();
                else PausarJuego();
            }
        }
    }

    public void PausarJuego()
    {
        juegoPausado = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f;

        // CORRECCIÓN MÉTODO OBSOLETO: Usamos FindObjectsByType
        videosEnEscena = Object.FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
        estadoVideos = new bool[videosEnEscena.Length];

        for (int i = 0; i < videosEnEscena.Length; i++)
        {
            estadoVideos[i] = videosEnEscena[i].isPlaying;
            if (estadoVideos[i]) videosEnEscena[i].Pause();
        }

        ActualizarIconoVolumen();
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1f;

        if (videosEnEscena != null)
        {
            for (int i = 0; i < videosEnEscena.Length; i++)
            {
                if (videosEnEscena[i] != null && estadoVideos[i])
                    videosEnEscena[i].Play();
            }
        }
    }

    public void PulsarBotonVolumen()
    {
        float vol = AudioListener.volume;
        // Ciclo de volumen: 0 -> 0.25 -> 0.50 -> 0.75 -> 1.0 -> 0
        float nuevoVol = (vol >= 1f) ? 0f : vol + 0.25f;
        AudioListener.volume = nuevoVol;

        ActualizarIconoVolumen();
    }

    private void ActualizarIconoVolumen()
    {
        if (iconoBotonVolumen != null)
        {
            iconoBotonVolumen.sprite = (AudioListener.volume <= 0.01f) ? spriteVolumenMute : spriteVolumenNormal;
        }
    }

    public void VolverAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        juegoPausado = false;
        panelPausa.SetActive(false);

        // Usamos la primera escena de la lista de prohibidas (que debería ser el MainMenu)
        // para evitar escribir el nombre a mano aquí también.
        if (nombresEscenasProhibidas.Count > 0)
            SceneManager.LoadScene(nombresEscenasProhibidas[0]);
        else
            Debug.LogError("No hay escenas configuradas en la lista de restricciones.");
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }
}