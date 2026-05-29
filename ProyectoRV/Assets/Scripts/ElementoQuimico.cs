using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Se asigna a cada cilindro/elemento en la escena.
/// Define qué elemento químico representa y gestiona su respawn.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class ElementoQuimico : MonoBehaviour
{
    // ─── Inspector ────────────────────────────────────────────────────────────
    [Header("Datos del Elemento")]
    [Tooltip("Nombre largo. Ej: Hidrógeno")]
    public string nombreElemento = "Elemento";

    [Tooltip("Símbolo químico. Usa las constantes de ReaccionesDB: H, O, Cl, Na")]
    public string simbolo = "H";

    [Header("Respawn")]
    public Transform puntoRespawn;

    [Tooltip("Prefab propio del elemento (usado por Mesa.cs para reaparecer)")]
    public GameObject prefabPropio;

    [Header("Visual")]
    [Tooltip("Renderer del cilindro para aplicar el color base")]
    public Renderer rendererElemento;

    // ─── Estado interno ───────────────────────────────────────────────────────
    [HideInInspector] public Vector3 posicionRespawn;
    [HideInInspector] public bool fueDepositado = false; // true cuando ya está dentro de un frasco
    [HideInInspector] public RecipienteQuimico recipienteActual = null;

    private XRGrabInteractable _grab;

    // ─────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        // Guardar posición de respawn
        if (puntoRespawn != null)
            posicionRespawn = puntoRespawn.position;
        else
            posicionRespawn = transform.position;

        // Aplicar color base al renderer
        AplicarColorBase();
    }

    /// <summary>
    /// Pinta el cilindro con su color de elemento según la DB.
    /// </summary>
    public void AplicarColorBase()
    {
        if (rendererElemento == null) return;
        if (ReaccionesDB.Instance == null) return;

        Material mat = rendererElemento.material; // instancia propia
        mat.color = ReaccionesDB.Instance.ObtenerColorElemento(simbolo);
    }

    /// <summary>
    /// Llamado por RecipienteQuimico cuando acepta este elemento.
    /// Desactiva el grab para que no se pueda volver a agarrar mientras está en el frasco.
    /// </summary>
    public void AlSerDepositado(RecipienteQuimico recipiente)
    {
        fueDepositado = true;
        recipienteActual = recipiente;

        // Cancelar interacción si está siendo sostenido
        _grab.enabled = false;

        // Ocultar el cilindro (ya no se ve en el frasco, el líquido lo representa)
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Devuelve el elemento a su posición de respawn y lo resetea.
    /// Llamado cuando se reinicia el frasco.
    /// </summary>
    public void Respawnear()
    {
        fueDepositado = false;
        recipienteActual = null;
        _grab.enabled = true;

        gameObject.SetActive(true);
        transform.position = posicionRespawn;
        transform.rotation = (puntoRespawn != null) ? puntoRespawn.rotation : Quaternion.identity;
    }
}