using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Estructura para cada "hueco" de la galería en el Inspector
[System.Serializable]
public class ElementoGaleria
{
    public string idNivel;           // ID  
    public Image componenteImagen;   // El componente Image del objeto
    public GameObject objetoBloqueo; // El panel negro o candado que tapa la foto
    public Sprite fotoColor;         // La imagen de la receta terminada
}

public class SCR_MenuPrincipal : MonoBehaviour
{
    [Header("Configuración de Galería")]
    [Tooltip("Añade aquí los elementos de la galería y asigna sus componentes.")]
    [SerializeField] private List<ElementoGaleria> nivelesGaleria;

    [Header("Paneles de Interfaz")]
    [SerializeField] private GameObject panelOpciones;

    [Header("Ajustes Visuales")]
    [SerializeField] private Color colorBloqueado = Color.black;
    [SerializeField] private Color colorDesbloqueado = Color.white;

    void Start()
    {
        // 1. Asegurarse de que el panel de opciones esté cerrado al inicio
        if (panelOpciones != null) panelOpciones.SetActive(false);

        // 2. Pintar la galería según el progreso guardado
        RefrescarGaleria();
    }

    // --- LÓGICA DE LA GALERÍA ---

    public void RefrescarGaleria()
    {
        // Recorremos la lista que configuraste en el Inspector
        foreach (var nivel in nivelesGaleria)
        {
            // Consultamos al Gestor de Niveles (el Singleton) si este ID está superado
            bool completado = SCR_GestionNiveles.Instancia.ComprobarDesbloqueo(nivel.idNivel);

            if (completado)
            {
                // Si está ganado: mostramos foto a color y quitamos el bloqueo negro
                nivel.componenteImagen.sprite = nivel.fotoColor;
                nivel.componenteImagen.color = colorDesbloqueado;
                nivel.objetoBloqueo.SetActive(false);

                // Opcional: Si la imagen tiene un componente Button, lo activamos
                Button btn = nivel.componenteImagen.GetComponent<Button>();
                if (btn != null) btn.interactable = true;
            }
            else
            {
                // Si no: foto oscurecida y panel de bloqueo activo
                nivel.componenteImagen.color = colorBloqueado;
                nivel.objetoBloqueo.SetActive(true);

                // Opcional: Desactivamos el botón para que no puedan entrar al nivel
                Button btn = nivel.componenteImagen.GetComponent<Button>();
                if (btn != null) btn.interactable = false;
            }
        }
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    // Carga cualquier escena pasando su nombre exacto como texto
    public void CargarEscena(string nombreEscena)
    {
        Debug.Log("Cargando escena: " + nombreEscena);
        SceneManager.LoadScene(nombreEscena);
    }

    // Abre o cierra el panel de opciones según el "check" del Inspector
    public void AlternarOpciones(bool estado)
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(estado);
        }
    }

    // Cierra el juego (solo funciona en el .exe o .apk final)
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}