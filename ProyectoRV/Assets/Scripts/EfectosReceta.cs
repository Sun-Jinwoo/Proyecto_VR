using System.Collections;
using UnityEngine;
using TMPro;

public class EfectosReceta : MonoBehaviour
{
    public static EfectosReceta Instancia;

    [Header("Partículas")]
    public ParticleSystem particulasExito;
    public ParticleSystem particulasFallo;
    public ParticleSystem particulasMisionCompleta;

    [Header("Panel 3D Misión Completada")]
    // Panel flotante en el mundo 3D, NO un Canvas de pantalla
    public GameObject panelMision3D;
    public TextMeshPro textoBonus3D;        // TextMeshPro 3D, NO TextMeshProUGUI
    public TextMeshPro textoMisionPanel3D;  // "¡Misión completada!"
    public float duracionPanelMision = 3f;
    public float distanciaAlJugador  = 1.2f; // metros enfrente del jugador
    public float alturaPanel         = 0.3f; // offset vertical sobre la mezcla

    [Header("Texto Flotante")]
    public EfectoFlotante prefabTextoFlotante;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoExito;
    public AudioClip sonidoFallo;
    public AudioClip sonidoMisionCompleta;

    // Referencia a la cámara VR (se busca automáticamente)
    private Transform _camaraVR;

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
    }

    private void Start()
    {
        // Busca la cámara VR automáticamente
        // Funciona con Meta XR, OpenXR y XR Interaction Toolkit
        if (Camera.main != null)
            _camaraVR = Camera.main.transform;
    }

    // ── API Pública ──────────────────────────────────────────────────

    public void ReproducirEfectoExito(Vector3 posicion)
    {
        SpawnParticulas(particulasExito, posicion);
        SpawnTextoFlotante("¡Reacción!", Color.green, posicion);
        ReproducirSonido(sonidoExito);
    }

    public void ReproducirEfectoFallo(Vector3 posicion)
    {
        SpawnParticulas(particulasFallo, posicion);
        SpawnTextoFlotante("Sin reacción...", Color.gray, posicion);
        ReproducirSonido(sonidoFallo);
    }

    public void ReproducirEfectoMisionCompleta(int bonusPuntos, Vector3 posicion)
    {
        SpawnParticulas(particulasMisionCompleta, posicion);
        StartCoroutine(MostrarPanelMision3D(bonusPuntos, posicion));
        SpawnTextoFlotante($"+{bonusPuntos} pts", Color.yellow, posicion);
        ReproducirSonido(sonidoMisionCompleta);
    }

    // ── Coroutines ───────────────────────────────────────────────────

    private IEnumerator MostrarPanelMision3D(int bonus, Vector3 posicionMezcla)
    {
        if (panelMision3D == null) yield break;

        // Posiciona el panel en el espacio 3D, delante del jugador
        // y cerca de donde ocurrió la mezcla
        Vector3 posPanel = CalcularPosicionPanel(posicionMezcla);
        panelMision3D.transform.position = posPanel;
        OrientarHaciaCamara(panelMision3D.transform);

        if (textoBonus3D != null)
            textoBonus3D.text = $"+{bonus} pts";

        if (textoMisionPanel3D != null)
            textoMisionPanel3D.text = "¡Misión completada!";

        // Animación de entrada: escala 0 → 1
        panelMision3D.SetActive(true);
        yield return AnimarEscala3D(panelMision3D.transform, Vector3.zero, Vector3.one * 0.003f, 0.3f);

        // El panel mira al jugador mientras esté activo
        float tiempoEspera = 0f;
        while (tiempoEspera < duracionPanelMision)
        {
            OrientarHaciaCamara(panelMision3D.transform);
            tiempoEspera += Time.deltaTime;
            yield return null;
        }

        // Animación de salida: escala 1 → 0
        yield return AnimarEscala3D(panelMision3D.transform, Vector3.one * 0.003f, Vector3.zero, 0.2f);
        panelMision3D.SetActive(false);
    }

    private IEnumerator AnimarEscala3D(Transform t, Vector3 desde, Vector3 hasta, float duracion)
    {
        float elapsed = 0f;
        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(desde, hasta, elapsed / duracion);
            yield return null;
        }
        t.localScale = hasta;
    }

    // ── Helpers ──────────────────────────────────────────────────────

    private Vector3 CalcularPosicionPanel(Vector3 posicionMezcla)
    {
        // El panel aparece sobre la mezcla, ligeramente hacia el jugador
        Vector3 pos = posicionMezcla + Vector3.up * alturaPanel;

        if (_camaraVR != null)
        {
            // Lo empuja un poco hacia el jugador para mejor visibilidad
            Vector3 dirAlJugador = (_camaraVR.position - posicionMezcla).normalized;
            dirAlJugador.y = 0f;
            pos += dirAlJugador * 0.15f;
        }

        return pos;
    }

    private void OrientarHaciaCamara(Transform t)
    {
        if (_camaraVR == null) return;
        // Rota el panel para mirar siempre al jugador (billboard)
        Vector3 dir = t.position - _camaraVR.position;
        dir.y = 0f; // sin inclinación vertical
        if (dir != Vector3.zero)
            t.rotation = Quaternion.LookRotation(dir);
    }

    private void SpawnParticulas(ParticleSystem prefab, Vector3 posicion)
    {
        if (prefab == null) return;
        ParticleSystem inst = Instantiate(prefab, posicion, Quaternion.identity);
        float vida = inst.main.duration + inst.main.startLifetime.constantMax;
        Destroy(inst.gameObject, vida);
    }

    private void SpawnTextoFlotante(string mensaje, Color color, Vector3 posicion)
    {
        if (prefabTextoFlotante == null) return;
        EfectoFlotante ef = Instantiate(
            prefabTextoFlotante,
            posicion + Vector3.up * 0.1f,
            Quaternion.identity
        );
        ef.Iniciar(mensaje, color, _camaraVR);
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}