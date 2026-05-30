using System.Collections.Generic;
using UnityEngine;

public class ChecklistEquipamiento : MonoBehaviour
{
    public static ChecklistEquipamiento Instancia;

    [Header("Conectar desde el Inspector")]
    public MesaMezcla mesaDeMezcla; 

    [Header("Nombres exactos de los items")]
    public List<string> itemsRequeridos = new List<string> { "Gafas", "Bata", "Guantes" };

    private List<string> _itemsEquipados = new List<string>();

    private void Awake()
    {
        Instancia = this;

        if (mesaDeMezcla != null)
            mesaDeMezcla.enabled = false;
    }

    public void MarcarItem(string nombreItem)
    {
        if (_itemsEquipados.Contains(nombreItem)) return;

        _itemsEquipados.Add(nombreItem);
        Debug.Log($"[Checklist] Equipados: {_itemsEquipados.Count}/{itemsRequeridos.Count}");

        if (_itemsEquipados.Count >= itemsRequeridos.Count)
            DesbloquearMesa();
    }

    private void DesbloquearMesa()
    {
        Debug.Log("[Checklist] ¡Equipamiento completo! Mesa desbloqueada ??");

        if (mesaDeMezcla != null)
            mesaDeMezcla.enabled = true;
    }
}