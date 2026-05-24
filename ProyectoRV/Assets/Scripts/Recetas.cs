using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Receta
{
    public string simboloA;        // Ej: "HCl"
    public string simboloB;        // Ej: "NaOH"
    public string nombreResultado; // Ej: "Agua y Sal"
}

public class Recetas : MonoBehaviour
{
    public static Recetas Instancia;

    [Header("Lista de reacciones válidas")]
    public List<Receta> recetas = new List<Receta>();

    private void Awake()
    {
        Instancia = this;

        // Cargamos 3 reacciones de ejemplo para empezar
        recetas.Add(new Receta { simboloA = "HCl", simboloB = "NaOH", nombreResultado = "Agua y Sal" });
        recetas.Add(new Receta { simboloA = "H2", simboloB = "O2", nombreResultado = "Agua" });
        recetas.Add(new Receta { simboloA = "Na", simboloB = "Cl", nombreResultado = "Sal de Mesa" });
    }

    public string VerificarReaccion(string simboloA, string simboloB)
    {
        foreach (Receta receta in recetas)
        {
            // El orden no importa: HCl+NaOH = NaOH+HCl
            bool coincide =
                (receta.simboloA == simboloA && receta.simboloB == simboloB) ||
                (receta.simboloA == simboloB && receta.simboloB == simboloA);

            if (coincide)
                return receta.nombreResultado; //Encontró una reacción
        }

        return null; // No hay reacción para esa combinación
    }
}