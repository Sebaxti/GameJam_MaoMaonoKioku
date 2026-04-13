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
    [SerializeField] private Image iconoBotonVolumen;
    [SerializeField] private Sprite spriteVolumenNormal;
    [SerializeField] private Sprite spriteVolumenMute; 

    void Start()
    {
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

    public void PulsarBotonVolumen()
    {
        if (SCR_GestorAudioGlobal.Instancia != null)
        {
            SCR_GestorAudioGlobal.Instancia.CiclarVolumenGlobal();
            ActualizarIconoVolumen();
        }
    }

    private void ActualizarIconoVolumen()
    {
        if (iconoBotonVolumen != null)
        {
            iconoBotonVolumen.sprite = (AudioListener.volume <= 0.01f) ? spriteVolumenMute : spriteVolumenNormal;
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