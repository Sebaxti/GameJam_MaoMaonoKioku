using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class ElementoGaleria
{
    public string idNivel;
    public Image componenteImagen;
    public GameObject objetoBloqueo;
    public Sprite fotoColor;
}

public class SCR_MenuPrincipal : MonoBehaviour
{
    [Header("Configuración de Galería")]
    [SerializeField] private List<ElementoGaleria> nivelesGaleria;

    [Header("Configuración de Volumen")]
    [SerializeField] private Image iconoBotonVolumen; // La imagen del botón que vas a pulsar
    [SerializeField] private Sprite spriteVolumenNormal; // Icono de altavoz con sonido
    [SerializeField] private Sprite spriteVolumenMute;   // Icono de altavoz tachado/silenciado

    void Start()
    {
        // Nos aseguramos de que el tiempo corra normal (por si venimos de un menú de pausa)
        Time.timeScale = 1f;

        RefrescarGaleria();
        ActualizarIconoVolumen();
    }

    public void RefrescarGaleria()
    {
        foreach (var nivel in nivelesGaleria)
        {
            bool completado = SCR_GestionNiveles.Instancia.ComprobarDesbloqueo(nivel.idNivel);

            if (completado)
            {
                nivel.componenteImagen.sprite = nivel.fotoColor;
                nivel.componenteImagen.color = Color.white;
                nivel.objetoBloqueo.SetActive(false);

                Button btn = nivel.componenteImagen.GetComponent<Button>();
                if (btn != null) btn.interactable = true;
            }
            else
            {
                nivel.componenteImagen.color = Color.black;
                nivel.objetoBloqueo.SetActive(true);

                Button btn = nivel.componenteImagen.GetComponent<Button>();
                if (btn != null) btn.interactable = false;
            }
        }
    }

    // --- LÓGICA DE VOLUMEN (25% en 25%) ---
    public void PulsarBotonVolumen()
    {
        float volumenActual = AudioListener.volume;

        // Subimos de 25 en 25 (0 -> 0.25 -> 0.5 -> 0.75 -> 1 -> 0)
        if (volumenActual < 0.1f) AudioListener.volume = 0.25f;
        else if (volumenActual < 0.3f) AudioListener.volume = 0.5f;
        else if (volumenActual < 0.6f) AudioListener.volume = 0.75f;
        else if (volumenActual < 0.8f) AudioListener.volume = 1f;
        else AudioListener.volume = 0f; // Si es 100%, vuelve a 0

        ActualizarIconoVolumen();
    }

    private void ActualizarIconoVolumen()
    {
        if (iconoBotonVolumen != null)
        {
            if (AudioListener.volume == 0f)
                iconoBotonVolumen.sprite = spriteVolumenMute;
            else
                iconoBotonVolumen.sprite = spriteVolumenNormal;
        }
    }

    // --- FUNCIONES BÁSICAS ---
    public void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}