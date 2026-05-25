using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ElementoQuimico : MonoBehaviour
{
    [Header("Datos del elemento")]
    public string nombreElemento = "Elemento";  // Ej: "Ácido Clorhídrico"
    public string simbolo = "??";               // Ej: "HCl"
    public Transform puntoRespawn;
    public GameObject prefabPropio;
    [HideInInspector] public Vector3 posicionRespawn;

    private void Start()
    {
        // Si tiene puntoRespawn guardamos su posición como Vector3
        if (puntoRespawn != null)
            posicionRespawn = puntoRespawn.position;
    }
}