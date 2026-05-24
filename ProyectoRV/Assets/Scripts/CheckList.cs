using System.Collections.Generic;
using UnityEngine;

public class Checklist : MonoBehaviour
{
    public static Checklist Instancia;

    [Header("Conectar desde el Inspector")]
    public MesaMezcla mesaDeMezcla; // La mesa que se desbloquea al final

    [Header("Nombres exactos de los items")]
    // Deben coincidir con el campo 'nombreItem' de cada EquipamientoItem
    public List<string> itemsRequeridos = new List<string> { "Gafas", "Bata", "Guantes" };

    // Lista interna de lo que ya equipó el jugador
    private List<string> _itemsEquipados = new List<string>();

    private void Awake()
    {
        Instancia = this;

        // La mesa empieza BLOQUEADA
        if (mesaDeMezcla != null)
            mesaDeMezcla.enabled = false;
    }

    public void MarcarItem(string nombreItem)
    {
        // Si ya lo tenía, ignoramos
        if (_itemsEquipados.Contains(nombreItem)) return;

        _itemsEquipados.Add(nombreItem);
        Debug.Log($"[Checklist] Equipados: {_itemsEquipados.Count}/{itemsRequeridos.Count}");

        // ¿Ya tiene todo?
        if (_itemsEquipados.Count >= itemsRequeridos.Count)
            DesbloquearMesa();
    }

    private void DesbloquearMesa()
    {
        Debug.Log("[Checklist] ¡Equipamiento completo! Mesa desbloqueada");

        if (mesaDeMezcla != null)
            mesaDeMezcla.enabled = true;
    }
}