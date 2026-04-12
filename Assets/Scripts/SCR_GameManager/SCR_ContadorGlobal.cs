using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class DatosCronometro
{
    public string idGrupoCronometro;
    public float tiempoTotal;
    public List<string> escenasActivas;
    public string escenaReinicioFallo;
    public string escenaInicialDelGrupo;
    public string idSiguienteGrupo;
    public string escenaSiguienteJuego;
}

public class SCR_ContadorGlobal : MonoBehaviour
{
    public static SCR_ContadorGlobal Instancia;
    public List<DatosCronometro> configuracionesReloj;

    private DatosCronometro configActual;
    private float tiempoActual;
    private bool estaActivo = false;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    void OnEnable() { SceneManager.sceneLoaded += AlCargarEscena; }
    void OnDisable() { SceneManager.sceneLoaded -= AlCargarEscena; }

    public void IniciarGrupoReloj(string idGrupo)
    {
        configActual = configuracionesReloj.Find(c => c.idGrupoCronometro == idGrupo);
        if (configActual != null)
        {
            tiempoActual = configActual.tiempoTotal;
            Debug.Log("<color=green>Reloj configurado para el grupo: " + idGrupo + "</color>");
        }
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (configActual != null)
        {
            estaActivo = configActual.escenasActivas.Contains(escena.name);
            // ESTO APARECERÁ EN TU CONSOLA:
            Debug.Log("Escena actual: " + escena.name + " | ¿Reloj activo aquí?: " + estaActivo);
        }
    }

    void Update()
    {
        if (estaActivo && tiempoActual > 0)
        {
            tiempoActual -= Time.deltaTime;
            if (tiempoActual <= 0) { tiempoActual = 0; ReiniciarPorFallo(); }
        }
    }

    private void ReiniciarPorFallo()
    {
        estaActivo = false;
        tiempoActual = configActual.tiempoTotal;
        SceneManager.LoadScene(configActual.escenaReinicioFallo);
    }

    public float ObtenerSegundosActuales() => tiempoActual;
    public bool ElRelojEstaCorriendo() => estaActivo;

    public void RepetirGrupoActual()
    {
        tiempoActual = configActual.tiempoTotal;
        SceneManager.LoadScene(configActual.escenaInicialDelGrupo);
    }

    public void AvanzarAlSiguienteGrupo()
    {
        if (configActual != null && !string.IsNullOrEmpty(configActual.idSiguienteGrupo))
        {
            string siguienteEscena = configActual.escenaSiguienteJuego;
            IniciarGrupoReloj(configActual.idSiguienteGrupo);
            SceneManager.LoadScene(siguienteEscena);
        }
        else { SceneManager.LoadScene("MainMenu"); }
    }
}