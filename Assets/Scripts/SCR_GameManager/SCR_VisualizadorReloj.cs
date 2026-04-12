using UnityEngine;
using TMPro;

public class SCR_VisualizadorRelojGlobal : MonoBehaviour
{
    public static SCR_VisualizadorRelojGlobal Instancia;
    public GameObject contenedorVisual;
    public TextMeshProUGUI textoEnPantalla;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    void Update()
    {
        if (SCR_ContadorGlobal.Instancia != null)
        {
            bool deberiaMostrarse = SCR_ContadorGlobal.Instancia.ElRelojEstaCorriendo();

            if (deberiaMostrarse)
            {
                if (!contenedorVisual.activeSelf)
                {
                    contenedorVisual.SetActive(true);
                    Debug.Log("Visualizador: Encendiendo reloj.");
                }

                float segundosSobrantes = SCR_ContadorGlobal.Instancia.ObtenerSegundosActuales();
                int minutos = Mathf.FloorToInt(segundosSobrantes / 60);
                int segundos = Mathf.FloorToInt(segundosSobrantes % 60);
                textoEnPantalla.text = string.Format("{0:00}:{1:00}", minutos, segundos);
            }
            else
            {
                if (contenedorVisual.activeSelf)
                {
                    contenedorVisual.SetActive(false);
                    Debug.Log("Visualizador: Apagando reloj.");
                }
            }
        }
    }
}