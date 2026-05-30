using UnityEngine;

public class ElementoQuimico : MonoBehaviour
{
    [Header("Datos del elemento")]
    public string nombreElemento = "Elemento";
    public string simbolo = "??";

    [Header("Posición de reaparición")]
    public Transform puntoRespawn;

    public GameObject prefabPropio;
   
    [HideInInspector] public Vector3 posicionRespawn;

    private void Start()
    {
        if (puntoRespawn != null)
            posicionRespawn = puntoRespawn.position;
    }
}