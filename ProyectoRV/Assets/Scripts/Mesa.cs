using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MesaMezcla : MonoBehaviour
{
    [Header("Conectar desde el Inspector")]
    public XRSocketInteractor slotA;       // Hueco izquierdo
    public XRSocketInteractor slotB;       // Hueco derecho
    public XRBaseInteractable boton;     // Botón de mezclar

    // Los elementos que están en cada slot
    private ElementoQuimico _elementoA;
    private ElementoQuimico _elementoB;

    private void Start()
    {
        // Escuchamos cuando un elemento entra o sale de cada slot
        slotA.selectEntered.AddListener(args => _elementoA = args.interactableObject.transform.GetComponent<ElementoQuimico>());
        slotA.selectExited.AddListener(args => _elementoA = null);

        slotB.selectEntered.AddListener(args => _elementoB = args.interactableObject.transform.GetComponent<ElementoQuimico>());
        slotB.selectExited.AddListener(args => _elementoB = null);

        // Escuchamos el botón
        boton.selectEntered.AddListener(args => Mezclar());
    }

    private void Mezclar()
    {
        // ¿Hay algo en los dos slots?
        if (_elementoA == null || _elementoB == null)
        {
            Debug.Log("Faltan elementos en la mesa.");
            return;
        }

        // Le preguntamos al SistemaReacciones si esta combinación es válida
        string resultado = Recetas.Instancia.VerificarReaccion(
            _elementoA.simbolo,
            _elementoB.simbolo
        );

        if (resultado != null)
        {
            // ✅ REACCIÓN VÁLIDA
            Debug.Log($"¡Reacción exitosa! Resultado: {resultado}");

            // Eliminamos los frascos usados
            Destroy(_elementoA.gameObject);
            Destroy(_elementoB.gameObject);
            _elementoA = null;
            _elementoB = null;
        }
        else
        {
            // ❌ COMBINACIÓN INVÁLIDA
            Debug.Log($"Combinación inválida: {_elementoA.simbolo} + {_elementoB.simbolo}");
        }
    }
}