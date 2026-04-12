using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class DatosNivel
{
    [Tooltip("El ID exacto de este minijuego")]
    public string idNivel;

    [Tooltip("La escena que va después de este minijuego. Si lo dejas vacío, irá a Resultados")]
    public string escenaSiguiente;

    [Header("Estrellas (Segundos sobrantes)")]
    public float segundosPara3Estrellas;
    public float segundosPara2Estrellas;
}

public class SCR_GestionNiveles : MonoBehaviour
{
    public static SCR_GestionNiveles Instancia;

    [Header("Configuración de Todos los Minijuegos")]
    public List<DatosNivel> configuracionNiveles;

    [Tooltip("El nombre exacto de tu escena de estrellas")]
    public string escenaResultados = "SCN_Resultados";

    [HideInInspector]
    public string idNivelRecienTerminado; // Lo usa la escena de resultados para saber qué acabas de jugar

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- LÓGICA DE GUARDADO Y GALERÍA ---

    public bool ComprobarDesbloqueo(string idNivel)
    {
        // Devuelve verdadero si PlayerPrefs guardó un 1 en este ID
        return PlayerPrefs.GetInt(idNivel, 0) == 1;
    }

    public void GuardarProgreso(string idNivel)
    {
        PlayerPrefs.SetInt(idNivel, 1);
        PlayerPrefs.Save();
        Debug.Log("Progreso guardado correctamente para: " + idNivel);
    }

    // --- LÓGICA DE FLUJO DE MINIJUEGOS ---

    public void CompletarNivelYContinuar(string id)
    {
        // 1. Guardamos el ID en la variable invisible para que lo lea la pantalla de resultados
        idNivelRecienTerminado = id;

        // 2. Guardamos la partida
        GuardarProgreso(id);

        // 3. Buscamos qué hacer ahora (Ir a otro minijuego, a un diálogo, o a las estrellas)
        DatosNivel datos = configuracionNiveles.Find(n => n.idNivel == id);

        if (datos != null && !string.IsNullOrEmpty(datos.escenaSiguiente))
        {
            SceneManager.LoadScene(datos.escenaSiguiente);
        }
        else
        {
            SceneManager.LoadScene(escenaResultados);
        }
    }
}