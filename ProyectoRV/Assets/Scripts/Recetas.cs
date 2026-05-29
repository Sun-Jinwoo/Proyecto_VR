using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Receta
{
    public string simboloA;
    public string simboloB;
    public string nombreResultado;
    public int bonusPuntos = 200;
    public string descripcion;
}

public class Recetas : MonoBehaviour
{
    public static Recetas Instancia;

    [Header("Recetas / Misiones")]
    public List<Receta> recetas = new List<Receta>();

    [Header("UI Tablero")]
    public TextMeshProUGUI textoMisionActual;
    public TextMeshProUGUI textoMisionesCompletadas;

    private Receta _misionActual;
    private int _misionesCompletadas = 0;
    private int _indiceActual = 0;

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;

        recetas.Add(new Receta
        {
            simboloA = "HCl",
            simboloB = "NaOH",
            nombreResultado = "Agua y Sal",
            bonusPuntos = 200,
            descripcion = "El cartel necesita: Agua y Sal"
        });
        recetas.Add(new Receta
        {
            simboloA = "H2",
            simboloB = "O2",
            nombreResultado = "Agua",
            bonusPuntos = 300,
            descripcion = "Misión urgente: sintetiza Agua pura"
        });
        recetas.Add(new Receta
        {
            simboloA = "Na",
            simboloB = "Cl",
            nombreResultado = "Sal de Mesa",
            bonusPuntos = 150,
            descripcion = "Entrega pendiente: Sal de Mesa"
        });
    }

    private void Start()
    {
        AsignarNuevaMision();
    }

    // Antes estaba en Recetas.cs
    public string VerificarReaccion(string simboloA, string simboloB)
    {
        foreach (Receta receta in recetas)
        {
            bool coincide =
                (receta.simboloA == simboloA && receta.simboloB == simboloB) ||
                (receta.simboloA == simboloB && receta.simboloB == simboloA);

            if (coincide)
                return receta.nombreResultado;
        }
        return null;
    }

    // Verifica si el resultado coincide con la misión activa
    public int VerificarMision(string resultado)
    {
        if (_misionActual == null) return 0;

        if (resultado == _misionActual.nombreResultado)
        {
            _misionesCompletadas++;
            int bonus = _misionActual.bonusPuntos;
            AsignarNuevaMision();
            return bonus;
        }

        return 0;
    }

    public void AsignarNuevaMision()
    {
        if (recetas.Count == 0) return;
        _misionActual = recetas[_indiceActual % recetas.Count];
        _indiceActual++;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoMisionActual != null)
            textoMisionActual.text = $"Misión:\n{_misionActual.descripcion}";

        if (textoMisionesCompletadas != null)
            textoMisionesCompletadas.text = $"Completadas: {_misionesCompletadas}";
    }

    public Receta GetMisionActual() => _misionActual;
    public int GetMisionesCompletadas() => _misionesCompletadas;
}