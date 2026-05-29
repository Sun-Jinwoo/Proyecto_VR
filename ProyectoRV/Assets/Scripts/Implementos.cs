//using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.XR.Interaction.Toolkit.Interactables;

//[RequireComponent(typeof(XRGrabInteractable))]
//public class EquipamientoItem : MonoBehaviour
//{
//    [Header("¿Qué es este objeto?")]
//    public string nombreItem = "Gafas";

//    private XRGrabInteractable _grab;
//    private bool _yaEquipado = false;

//    private void Awake()
//    {
//        _grab = GetComponent<XRGrabInteractable>();
//        _grab.selectEntered.AddListener(AlAgarrar);
//    }

//    private void AlAgarrar(SelectEnterEventArgs args)
//    {
//        if (_yaEquipado) return; // Si ya se agarró antes, no cuenta doble

//        _yaEquipado = true;
//        Debug.Log($"[Checklist] {nombreItem} equipado");

//        // Le avisamos al checklist
//        Checklist.Instancia.(nombreItem);
//    }
//}