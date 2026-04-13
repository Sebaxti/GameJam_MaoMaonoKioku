using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCR_ControladorResultados : MonoBehaviour
{
    [Header("UI de Estrellas")]
    [SerializeField] private Image imagenEstrellas;

    [SerializeField] private Sprite sp_1Estrella;
    [SerializeField] private Sprite sp_2Estrellas;
    [SerializeField] private Sprite sp_3Estrellas;

    void Start()
    {
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
        string id = SCR_GestionNiveles.Instancia.idNivelRecienTerminado;

        DatosNivel datos = SCR_GestionNiveles.Instancia.configuracionNiveles.Find(n => n.idNivel == id);

        float segundosRestantes = SCR_ContadorGlobal.Instancia.ObtenerSegundosActuales();

        if (datos == null)
        {
            Debug.LogError("No se encontraron los datos del nivel: " + id);
            return;
        }

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


    public void Boton_Siguiente()
    {
        if (SCR_ContadorGlobal.Instancia != null)
        {
            SCR_ContadorGlobal.Instancia.AvanzarAlSiguienteGrupo();
        }
    }

    public void Boton_Repetir()
    {

        if (SCR_ContadorGlobal.Instancia != null)
        {
            SCR_ContadorGlobal.Instancia.RepetirGrupoActual();
        }
    }

    public void Boton_Menu()
    {

        SceneManager.LoadScene("MainMenu");
    }
}