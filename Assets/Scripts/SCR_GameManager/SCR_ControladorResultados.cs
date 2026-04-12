using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCR_ControladorResultados : MonoBehaviour
{
    [Header("UI de Estrellas")]
    [Tooltip("El componente Image de tu Canvas donde se verá la foto de las estrellas")]
    [SerializeField] private Image imagenEstrellas;

    [Tooltip("Arrastra aquí tu foto de 1 estrella")]
    [SerializeField] private Sprite sp_1Estrella;
    [Tooltip("Arrastra aquí tu foto de 2 estrellas")]
    [SerializeField] private Sprite sp_2Estrellas;
    [Tooltip("Arrastra aquí tu foto de 3 estrellas")]
    [SerializeField] private Sprite sp_3Estrellas;

    void Start()
    {
        // Nos aseguramos de que existan los dos cerebros antes de calcular
        if (SCR_GestionNiveles.Instancia != null && SCR_ContadorGlobal.Instancia != null)
        {
            CalcularYMostrarEstrellas();
        }
        else
        {
            Debug.LogError("Falta el Gestor de Niveles o el Cronómetro Global en la escena.");
        }
    }

    private void CalcularYMostrarEstrellas()
    {
        // 1. Preguntamos al Gestor de Niveles qué ID acabamos de jugar
        string id = SCR_GestionNiveles.Instancia.idNivelRecienTerminado;

        // 2. Buscamos los datos de ese nivel específico
        DatosNivel datos = SCR_GestionNiveles.Instancia.configuracionNiveles.Find(n => n.idNivel == id);

        // 3. Preguntamos al Reloj cuántos segundos sobraron en el momento de ganar
        float segundosRestantes = SCR_ContadorGlobal.Instancia.ObtenerSegundosActuales();

        if (datos == null)
        {
            Debug.LogError("No se encontraron los datos del nivel: " + id);
            return;
        }

        // 4. Comparamos los segundos con los requisitos y pintamos la imagen
        if (segundosRestantes >= datos.segundosPara3Estrellas)
        {
            imagenEstrellas.sprite = sp_3Estrellas;
        }
        else if (segundosRestantes >= datos.segundosPara2Estrellas)
        {
            imagenEstrellas.sprite = sp_2Estrellas;
        }
        else
        {
            imagenEstrellas.sprite = sp_1Estrella;
        }
    }

    // --- BOTONES DE LA INTERFAZ ---
    // (Asegúrate de asignar estas funciones en el evento OnClick de cada botón)

    public void Boton_Siguiente()
    {
        // Le pasamos la pelota al reloj, él sabe a dónde saltar y qué nuevo tiempo poner
        if (SCR_ContadorGlobal.Instancia != null)
        {
            SCR_ContadorGlobal.Instancia.AvanzarAlSiguienteGrupo();
        }
    }

    public void Boton_Repetir()
    {
        // Le decimos al reloj que reinicie el grupo de niveles que acabamos de terminar
        if (SCR_ContadorGlobal.Instancia != null)
        {
            SCR_ContadorGlobal.Instancia.RepetirGrupoActual();
        }
    }

    public void Boton_Menu()
    {
        // Volvemos directos al menú de inicio
        SceneManager.LoadScene("MainMenu");
    }
}