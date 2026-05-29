using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base de datos global de todas las reacciones químicas posibles.
/// Asignar a un GameObject vacío llamado "GameManager" en la escena.
/// </summary>
public class ReaccionesDB : MonoBehaviour
{
    public static ReaccionesDB Instance { get; private set; }

    // ─── Colores de los materiales resultantes ───────────────────────────────
    [Header("Colores de Elementos Base")]
    public Color colorHidrogeno = new Color(0.27f, 0.71f, 0.90f); // AZUL
    public Color colorOxigeno = new Color(0.18f, 0.72f, 0.25f); // VERDE
    public Color colorCloro = new Color(0.90f, 0.40f, 0.70f); // ROSADO
    public Color colorSodio = new Color(0.88f, 0.88f, 0.88f); // BLANCO

    [Header("Colores de Productos Resultantes")]
    public Color colorAgua = new Color(0.05f, 0.35f, 0.70f); // AZUL OSCURO
    public Color colorAcidoClorhidrico = new Color(0.55f, 0.95f, 0.45f); // VERDE CLARO
    public Color colorHidruroSodio = new Color(0.95f, 0.90f, 0.10f); // AMARILLO
    public Color colorOxidoCloro = new Color(0.50f, 0.30f, 0.10f); // MARRÓN
    public Color colorCloruroSodio = new Color(1.00f, 1.00f, 1.00f); // BLANCO BRILLANTE (emission)

    // ─── Definición interna de una reacción ──────────────────────────────────
    public class Reaccion
    {
        public string nombreProducto;
        public Color colorResultante;
        public bool tieneEmision;   // para el blanco brillante del NaCl
    }

    // Clave del diccionario: lista de símbolos ORDENADA y unida con "_"
    // Ej: "H_H_O"  o  "H_Cl"
    private Dictionary<string, Reaccion> _reacciones;

    // ─── Símbolos de elementos ────────────────────────────────────────────────
    public const string H = "H";
    public const string O = "O";
    public const string Cl = "Cl";
    public const string Na = "Na";

    // ─────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InicializarReacciones();
    }

    private void InicializarReacciones()
    {
        _reacciones = new Dictionary<string, Reaccion>();

        // 2H + O  → Agua (AZUL MÁS OSCURO)
        Registrar(new[] { H, H, O }, "Agua", colorAgua);

        // H + Cl  → Ácido Clorhídrico (VERDE MÁS CLARO)
        Registrar(new[] { H, Cl }, "Acido Clorhidrico", colorAcidoClorhidrico);

        // H + Na  → Hidruro de Sodio (AMARILLO)
        Registrar(new[] { H, Na }, "Hidruro de Sodio", colorHidruroSodio);

        // O + 2Cl → Óxido de Cloro (MARRÓN)
        Registrar(new[] { O, Cl, Cl }, "Oxido de Cloro", colorOxidoCloro);

        // Cl + Na → Cloruro de Sodio / Sal (BLANCO BRILLANTE)
        Registrar(new[] { Cl, Na }, "Cloruro de Sodio", colorCloruroSodio, tieneEmision: true);
    }

    /// <summary>
    /// Registra una reacción en el diccionario usando la clave ordenada.
    /// </summary>
    private void Registrar(string[] simbolos, string nombre, Color color, bool tieneEmision = false)
    {
        string clave = GenerarClave(simbolos);
        _reacciones[clave] = new Reaccion
        {
            nombreProducto = nombre,
            colorResultante = color,
            tieneEmision = tieneEmision
        };
    }

    /// <summary>
    /// Intenta encontrar la reacción para la lista de símbolos dada.
    /// Devuelve null si no hay reacción válida.
    /// </summary>
    public Reaccion BuscarReaccion(List<string> simbolos)
    {
        string clave = GenerarClave(simbolos.ToArray());
        if (_reacciones.TryGetValue(clave, out Reaccion r))
            return r;
        return null;
    }

    /// <summary>
    /// Genera una clave canónica ordenando los símbolos alfabéticamente y uniéndolos con "_".
    /// Así "H_Cl" y "Cl_H" producen la misma clave.
    /// </summary>
    public static string GenerarClave(string[] simbolos)
    {
        var lista = new List<string>(simbolos);
        lista.Sort();
        return string.Join("_", lista);
    }

    /// <summary>
    /// Devuelve el color base de un elemento según su símbolo.
    /// </summary>
    public Color ObtenerColorElemento(string simbolo)
    {
        return simbolo switch
        {
            H => colorHidrogeno,
            O => colorOxigeno,
            Cl => colorCloro,
            Na => colorSodio,
            _ => Color.white
        };
    }
}