// =============================================
//  ElementoQuimico.cs
//  ¿Qué es? Le dice a Unity que este objeto
//  es un elemento químico que se puede agarrar.
// =============================================

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// RequireComponent = Unity agrega automáticamente
// XRGrabInteractable si no está. Eso lo hace agarrable en VR.
[RequireComponent(typeof(XRGrabInteractable))]
public class ElementoQuimico : MonoBehaviour
{
    [Header("Datos del elemento")]
    public string nombreElemento = "Elemento";  // Ej: "Ácido Clorhídrico"
    public string simbolo = "??";               // Ej: "HCl"
}