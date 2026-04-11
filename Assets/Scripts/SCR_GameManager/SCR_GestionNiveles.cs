using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;

// Esta clase guarda la configuración de cada nivel
[System.Serializable]
public class DatosNivel
{
    [Tooltip("El ID del nivel (ej: Nivel1, Nivel2)")]
    public string idNivel;
    [Tooltip("El nombre exacto de la escena que carga al completar este ID")]
    public string escenaSiguiente;
}

public class SCR_GestionNiveles : MonoBehaviour
{
    public static SCR_GestionNiveles Instancia;

    [Header("Rutas de Niveles (Sistema Central)")]
    [Tooltip("Añade aquí el orden de tus niveles.")]
    public List<DatosNivel> configuracionNiveles;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    public bool ComprobarDesbloqueo(string idNivel)
    {
        return PlayerPrefs.GetInt(idNivel, 0) == 1;
    }

    public void GuardarProgreso(string idNivel)
    {
        PlayerPrefs.SetInt(idNivel, 1);
        PlayerPrefs.Save();
    }


    public void CompletarNivelYContinuar(string idNivelCompletado)
    {
        // 1. Guardamos que el nivel se ha superado
        GuardarProgreso(idNivelCompletado);

        // 2. Buscamos en la lista cuál es la escena que le sigue
        foreach (var nivel in configuracionNiveles)
        {
            if (nivel.idNivel == idNivelCompletado)
            {
                if (!string.IsNullOrEmpty(nivel.escenaSiguiente))
                {
                    Debug.Log("Cargando la siguiente escena centralizada: " + nivel.escenaSiguiente);
                    SceneManager.LoadScene(nivel.escenaSiguiente);
                }
                else
                {
                    Debug.LogWarning("El nivel está en la lista, pero no le asignaste una escena siguiente.");
                }
                return;
            }
        }

        Debug.LogWarning("No se encontró el ID '" + idNivelCompletado + "' en el Gestor de Niveles.");
    }
}
