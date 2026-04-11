using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum Orador { Izquierda, Derecha }

[System.Serializable]
public class LineaDialogo
{
    public Orador quienHabla;
    public string nombre;
    [TextArea(3, 10)] public string texto;
}

public class SCR_ControladorDialogo : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private RawImage personajeIzquierda;
    [SerializeField] private RawImage personajeDerecha;
    [SerializeField] private GameObject etiquetaNombreIzquierda;
    [SerializeField] private TextMeshProUGUI textoNombreIzquierda;
    [SerializeField] private GameObject etiquetaNombreDerecha;
    [SerializeField] private TextMeshProUGUI textoNombreDerecha;
    [SerializeField] private TextMeshProUGUI textoCuerpo;

    [Header("Configuración de Apariencia")]
    [SerializeField] private float velocidadEscribir = 0.05f;
    [SerializeField] private Color colorDesenfoque = new Color(0.3f, 0.3f, 0.3f, 1f);
    private Color colorEnfoque = Color.white;

    [Header("Conexión con el Gestor (Cerebro)")]
    [Tooltip("El ID que identifica este diálogo en el Gestor de Niveles")]
    [SerializeField] private string idDeEsteNivel = "Dialogo_Intro"; 

    [Header("Conversación")]
    [SerializeField] private List<LineaDialogo> dialogoCompleto;

    private int lineaActualIndex = 0;
    private bool estaEscribiendo = false;
    private bool dialogoTerminado = false;

    void Start()
    {
        etiquetaNombreIzquierda.SetActive(false);
        etiquetaNombreDerecha.SetActive(false);

        if (dialogoCompleto.Count > 0)
        {
            MostrarLinea(dialogoCompleto[lineaActualIndex]);
        }
    }

    private void MostrarLinea(LineaDialogo linea)
    {
        textoCuerpo.text = "";

        if (linea.quienHabla == Orador.Izquierda)
        {
            personajeIzquierda.color = colorEnfoque;
            personajeDerecha.color = colorDesenfoque;
            etiquetaNombreIzquierda.SetActive(true);
            etiquetaNombreDerecha.SetActive(false);
            textoNombreIzquierda.text = linea.nombre;
        }
        else
        {
            personajeIzquierda.color = colorDesenfoque;
            personajeDerecha.color = colorEnfoque;
            etiquetaNombreIzquierda.SetActive(false);
            etiquetaNombreDerecha.SetActive(true);
            textoNombreDerecha.text = linea.nombre;
        }

        StartCoroutine(EscribirTextoCorrutina(linea.texto));
    }

    private IEnumerator EscribirTextoCorrutina(string textoATipiar)
    {
        estaEscribiendo = true;
        foreach (char caracter in textoATipiar.ToCharArray())
        {
            textoCuerpo.text += caracter;
            yield return new WaitForSeconds(velocidadEscribir);
        }
        estaEscribiendo = false;
    }

    public void CompletarTextoInmediatamente()
    {
        if (estaEscribiendo)
        {
            StopAllCoroutines();
            textoCuerpo.text = dialogoCompleto[lineaActualIndex].texto;
            estaEscribiendo = false;
        }
    }

    public void IntentarAvanzarDialogo()
    {
        if (estaEscribiendo)
        {
            CompletarTextoInmediatamente();
        }
        else if (!dialogoTerminado)
        {
            lineaActualIndex++;
            if (lineaActualIndex < dialogoCompleto.Count)
            {
                MostrarLinea(dialogoCompleto[lineaActualIndex]);
            }
            else
            {
                TerminarConversacion();
            }
        }
    }

    private void TerminarConversacion()
    {
        if (dialogoTerminado) return;
        dialogoTerminado = true;

        Debug.Log("Fin del diálogo. Avisando al Gestor de Niveles...");

        // Llamamos al cerebro (Gestor de Niveles) para que guarde progreso y cargue la siguiente escena
        if (SCR_GestionNiveles.Instancia != null)
        {
            // Usamos la función centralizada que ya tienes creada
            SCR_GestionNiveles.Instancia.CompletarNivelYContinuar(idDeEsteNivel);
        }
        else
        {
            Debug.LogError("No se encontró el SCR_GestionNiveles en la escena. Asegúrate de que el objeto GameManager exista.");
        }
    }
}