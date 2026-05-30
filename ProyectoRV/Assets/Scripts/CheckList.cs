using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChecklistEquipamiento : MonoBehaviour
{
    public static ChecklistEquipamiento Instancia;

    [Header("Conectar desde el Inspector")]
    public MesaMezcla mesaDeMezcla;

    [Header("Nombres exactos de los items")]
    public List<string> itemsRequeridos = new List<string> { "Gafas", "Bata", "Guantes" };

    [Header("UI Canvas Checklist")]
    public GameObject canvasChecklist;        // El canvas entero
    public TextMeshProUGUI textoTitulo;       // "Equipamiento de seguridad"
    public List<TextMeshProUGUI> textosItems; // Un TMP por cada item (3 en total)

    private List<string> _itemsEquipados = new List<string>();

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;

        if (mesaDeMezcla != null)
            mesaDeMezcla.enabled = false;
    }

    private void Start()
    {
        ActualizarUI();
    }

    public void MarcarItem(string nombreItem)
    {
        if (_itemsEquipados.Contains(nombreItem)) return;

        _itemsEquipados.Add(nombreItem);
        Debug.Log($"[Checklist] Equipados: {_itemsEquipados.Count}/{itemsRequeridos.Count}");

        ActualizarUI();

        if (_itemsEquipados.Count >= itemsRequeridos.Count)
            DesbloquearMesa();
    }

    private void ActualizarUI()
    {
        if (textoTitulo != null)
            textoTitulo.text = $"Equipamiento de seguridad\n{_itemsEquipados.Count}/{itemsRequeridos.Count}";

        for (int i = 0; i < textosItems.Count; i++)
        {
            if (textosItems[i] == null) continue;

            string item = i < itemsRequeridos.Count ? itemsRequeridos[i] : "";
            bool equipado = _itemsEquipados.Contains(item);

            textosItems[i].text = equipado ? $"? {item}" : $"? {item}";
            textosItems[i].color = equipado ? Color.green : Color.white;
        }
    }

    private void DesbloquearMesa()
    {
        Debug.Log("[Checklist] ¡Equipamiento completo! Mesa desbloqueada.");

        // Ocultar el canvas al completar
        if (canvasChecklist != null)
            canvasChecklist.SetActive(false);

        if (mesaDeMezcla != null)
            mesaDeMezcla.enabled = true;
        mesaDeMezcla.canvasTablero.SetActive(true);
    }
}