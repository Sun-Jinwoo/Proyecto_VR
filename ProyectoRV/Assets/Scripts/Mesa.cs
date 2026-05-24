using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class MesaMezcla : MonoBehaviour
{
    [Header("Conectar desde el Inspector")]
    public XRSocketInteractor slotA;
    public XRSocketInteractor slotB;
    public XRBaseInteractable boton;

    [Header("Texto resultado")] // NUEVO
    public TextMeshProUGUI textoResultado; // NUEVO

    private ElementoQuimico _elementoA;
    private ElementoQuimico _elementoB;

    private void Start()
    {
        slotA.selectEntered.AddListener(args => _elementoA = args.interactableObject.transform.GetComponent<ElementoQuimico>());
        slotA.selectExited.AddListener(args => _elementoA = null);

        slotB.selectEntered.AddListener(args => _elementoB = args.interactableObject.transform.GetComponent<ElementoQuimico>());
        slotB.selectExited.AddListener(args => _elementoB = null);

        boton.selectEntered.AddListener(args => Mezclar());
    }

    private void Mezclar()
    {
        if (_elementoA == null || _elementoB == null)
        {
            MostrarTexto("Faltan elementos en la mesa.");
            return;
        }

        string resultado = Recetas.Instancia.VerificarReaccion(
            _elementoA.simbolo,
            _elementoB.simbolo
        );

        if (resultado != null)
        {
            MostrarTexto($"¡Reacción exitosa!\n{resultado}"); // NUEVO
            Destroy(_elementoA.gameObject);
            Destroy(_elementoB.gameObject);
            _elementoA = null;
            _elementoB = null;
        }
        else
        {
            MostrarTexto($"Combinación inválida:\n{_elementoA.simbolo} + {_elementoB.simbolo}"); // NUEVO
        }
    }

    private void MostrarTexto(string mensaje) // NUEVO
    {
        if (textoResultado != null)
            textoResultado.text = mensaje;
    }
}