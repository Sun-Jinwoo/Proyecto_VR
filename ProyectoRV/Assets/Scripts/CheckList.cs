using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors; // Necesario para detectar los Sockets

public class Checklist : MonoBehaviour
{
    public static Checklist Instancia;

    [Header("Conectar desde el Inspector")]
    public MesaMezcla mesaDeMezcla; // La mesa que se desbloquea al final

    [Header("Sockets del Equipamiento")]
    public XRSocketInteractor[] socketsEquipamiento; // El arreglo con tus 3 sockets

    private void Awake()
    {
        Instancia = this;

        // La mesa empieza BLOQUEADA
        if (mesaDeMezcla != null)
            mesaDeMezcla.enabled = false;
    }

    void Update()
    {
        // Empezamos asumiendo que todo está listo
        bool todoEquipado = true;

        // El "detective" revisa cada socket del arreglo uno por uno
        foreach (XRSocketInteractor socket in socketsEquipamiento)
        {
            // Si encuentra UN SOLO socket que esté vacío...
            if (socket != null && !socket.hasSelection)
            {
                todoEquipado = false; // Ya no están todos listos
            }
        }

        // Si todos los sockets tienen un objeto puesto, desbloqueamos la mesa
        if (todoEquipado == true)
        {
            DesbloquearMesa();
        }
        else
        {
            BloquearMesa(); // Si el jugador quita un objeto del socket, se vuelve a bloquear
        }
    }

    private void DesbloquearMesa()
    {
        if (mesaDeMezcla != null && mesaDeMezcla.enabled == false)
        {
            Debug.Log("[Checklist] ¡Los 3 sockets están llenos! Mesa de mezcla DESBLOQUEADA.");
            mesaDeMezcla.enabled = true;
        }
    }

    private void BloquearMesa()
    {
        if (mesaDeMezcla != null && mesaDeMezcla.enabled == true)
        {
            Debug.Log("[Checklist] Falta algún elemento en los sockets. Mesa de mezcla BLOQUEADA.");
            mesaDeMezcla.enabled = false;
        }
    }
}