using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Se asigna a cada frasco/recipiente en la escena.
/// 
/// REQUISITOS en el Inspector:
///  - "liquidoRenderer"  → el Renderer del objeto interior que simula el líquido
///  - "radioDeteccion"   → distancia a la que se "chupa" el elemento (recomendado: 0.15)
///  - El frasco debe tener un Collider (Trigger) O usar la detección por distancia incluida aquí
/// </summary>
public class RecipienteQuimico : MonoBehaviour
{
    // ─── Inspector ────────────────────────────────────────────────────────────
    [Header("Visual del Líquido")]
    [Tooltip("Renderer del objeto interior que representa el contenido del frasco")]
    public Renderer liquidoRenderer;

    [Tooltip("Escala Z inicial del líquido (casi invisible)")]
    public float escalaZInicial = 0.1f;

    [Tooltip("Cuánto crece en Z cada vez que se añade un elemento")]
    public float incrementoZ = 0.12f;

    [Header("Detección de Proximidad")]
    [Tooltip("Radio en metros para detectar elementos cercanos y snappearlos")]
    public float radioDeteccion = 0.18f;

    [Tooltip("Layer de los elementos (opcional, deja 'Everything' si no usas layers)")]
    public LayerMask capaElementos;

    [Header("Capacidad")]
    [Tooltip("Máximo de elementos que acepta este frasco")]
    public int capacidadMaxima = 3;

    // ─── Estado interno ───────────────────────────────────────────────────────
    private List<string> _simbolosContenidos = new List<string>();
    private List<ElementoQuimico> _elementosContenidos = new List<ElementoQuimico>();
    private Material _matLiquido;
    private bool _mezclado = false;   // ya reaccionó, no acepta más

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        if (liquidoRenderer != null)
        {
            _matLiquido = liquidoRenderer.material; // instancia propia
            // Escala Z inicial casi invisible
            Vector3 s = liquidoRenderer.transform.localScale;
            s.z = escalaZInicial;
            liquidoRenderer.transform.localScale = s;

            // Ocultar al inicio (sin color aún)
            _matLiquido.color = Color.clear;
        }
        else
        {
            Debug.LogWarning($"[RecipienteQuimico] '{name}': liquidoRenderer no asignado.", this);
        }
    }

    private void Update()
    {
        if (_mezclado) return;                    // ya reaccionó, ignorar
        if (_simbolosContenidos.Count >= capacidadMaxima) return; // frasco lleno

        BuscarElementoCercano();
    }

    // ─── Detección por distancia ──────────────────────────────────────────────
    private void BuscarElementoCercano()
    {
        // Busca todos los colliders en el radio de detección
        Collider[] hits = Physics.OverlapSphere(transform.position, radioDeteccion, capaElementos);

        float menorDistancia = float.MaxValue;
        ElementoQuimico candidato = null;

        foreach (Collider col in hits)
        {
            ElementoQuimico elem = col.GetComponent<ElementoQuimico>();
            if (elem == null) continue;
            if (elem.fueDepositado) continue; // ya está en otro frasco

            float dist = Vector3.Distance(transform.position, elem.transform.position);
            if (dist < menorDistancia)
            {
                menorDistancia = dist;
                candidato = elem;
            }
        }

        if (candidato != null)
            IntentarAgregarElemento(candidato);
    }

    // ─── Lógica de agregar elemento ───────────────────────────────────────────
    private void IntentarAgregarElemento(ElementoQuimico elemento)
    {
        // ── Validación 1: frasco ya reaccionó ──
        if (_mezclado)
        {
            Debug.LogWarning($"[Frasco '{name}'] Ya tiene una mezcla completada. No acepta más elementos.");
            return;
        }

        // ── Validación 2: frasco lleno ──
        if (_simbolosContenidos.Count >= capacidadMaxima)
        {
            Debug.LogWarning($"[Frasco '{name}'] Capacidad máxima ({capacidadMaxima}) alcanzada.");
            return;
        }

        // ── Validación 3: no mezclar más de 2 elementos DISTINTOS ──
        // (permite repetir el mismo, ej: H+H, pero no H+O+Cl)
        if (_simbolosContenidos.Count > 0)
        {
            var distintos = new HashSet<string>(_simbolosContenidos);
            if (!distintos.Contains(elemento.simbolo) && distintos.Count >= 2)
            {
                Debug.LogError($"[Frasco '{name}'] ERROR: No se pueden combinar más de 2 elementos distintos. " +
                               $"Ya contiene: {string.Join(", ", distintos)}. " +
                               $"Intentaste agregar: {elemento.simbolo}");
                return;
            }
        }

        // ── Todo OK: depositar elemento ──
        AceptarElemento(elemento);
    }

    private void AceptarElemento(ElementoQuimico elemento)
    {
        _simbolosContenidos.Add(elemento.simbolo);
        _elementosContenidos.Add(elemento);

        // Notificar al elemento
        elemento.AlSerDepositado(this);

        Debug.Log($"[Frasco '{name}'] Elemento '{elemento.nombreElemento}' ({elemento.simbolo}) añadido. " +
                  $"Contenido actual: [{string.Join(", ", _simbolosContenidos)}]");

        // Actualizar visual del líquido
        ActualizarLiquido(elemento.simbolo);

        // Intentar mezclar automáticamente
        IntentarMezclar();
    }

    // ─── Visual del líquido ───────────────────────────────────────────────────
    private void ActualizarLiquido(string ultimoSimbolo)
    {
        if (_matLiquido == null || liquidoRenderer == null) return;

        // Color = color del último elemento añadido
        Color colorNuevo = ReaccionesDB.Instance != null
            ? ReaccionesDB.Instance.ObtenerColorElemento(ultimoSimbolo)
            : Color.white;

        _matLiquido.color = colorNuevo;

        // Crecer en Z
        Vector3 escala = liquidoRenderer.transform.localScale;
        escala.z += incrementoZ;
        liquidoRenderer.transform.localScale = escala;
    }

    // ─── Lógica de mezcla ─────────────────────────────────────────────────────
    private void IntentarMezclar()
    {
        if (ReaccionesDB.Instance == null) return;

        ReaccionesDB.Reaccion reaccion = ReaccionesDB.Instance.BuscarReaccion(_simbolosContenidos);

        if (reaccion != null)
        {
            AplicarMezcla(reaccion);
        }
        else
        {
            // Solo mostrar error si ya hay 2+ elementos y la combinación es inválida
            // (no mostrarlo cuando solo hay 1 elemento, porque aún puede añadirse más)
            if (_simbolosContenidos.Count >= 2)
            {
                // Verificar si existe ALGUNA reacción posible con los elementos actuales
                // (puede que falte el tercer elemento, ej: O con 1 Cl esperando otro Cl)
                bool puedeSeguir = PuedeCompletarseConMasElementos();

                if (!puedeSeguir)
                {
                    Debug.LogError($"[Frasco '{name}'] ERROR: La combinación " +
                                   $"[{string.Join(", ", _simbolosContenidos)}] no produce ninguna reacción válida. " +
                                   $"Revisa el orden o los elementos usados.");
                }
                else
                {
                    Debug.Log($"[Frasco '{name}'] Combinación incompleta [{string.Join(", ", _simbolosContenidos)}]. " +
                              $"Esperando más elementos...");
                }
            }
        }
    }

    /// <summary>
    /// Verifica si los símbolos actuales son prefijo de alguna reacción válida.
    /// Esto evita mostrar error cuando el frasco espera un tercer elemento (ej: O+Cl esperando Cl).
    /// </summary>
    private bool PuedeCompletarseConMasElementos()
    {
        if (_simbolosContenidos.Count >= capacidadMaxima) return false;

        // Reacciones que tienen más elementos que los actuales y que los contienen como subconjunto
        // Simplificado: revisamos las claves conocidas manualmente
        var actuales = new List<string>(_simbolosContenidos);
        actuales.Sort();
        string claveActual = string.Join("_", actuales);

        // Prefijos válidos de reacciones de 3 elementos
        // H_H → puede ser H_H_O (Agua)
        // Cl_O → puede ser Cl_Cl_O (Óxido de Cloro)
        string[] prefijosValidos = { "H_H", "Cl_O" };

        foreach (string prefijo in prefijosValidos)
        {
            if (claveActual == prefijo) return true;
        }

        return false;
    }

    private void AplicarMezcla(ReaccionesDB.Reaccion reaccion)
    {
        _mezclado = true;

        if (_matLiquido != null)
        {
            _matLiquido.color = reaccion.colorResultante;

            // Efecto de emisión para el Cloruro de Sodio (blanco brillante)
            if (reaccion.tieneEmision)
            {
                _matLiquido.EnableKeyword("_EMISSION");
                _matLiquido.SetColor("_EmissionColor", reaccion.colorResultante * 2.5f);
            }
        }

        Debug.Log($"[Frasco '{name}'] ✅ REACCIÓN EXITOSA: [{string.Join(" + ", _simbolosContenidos)}] " +
                  $"→ {reaccion.nombreProducto}");
    }

    // ─── Reiniciar frasco ─────────────────────────────────────────────────────
    /// <summary>
    /// Reinicia el frasco y devuelve los elementos a su posición de spawn.
    /// Puedes llamar este método desde un botón en la UI o un trigger de la escena.
    /// </summary>
    public void ReiniciarFrasco()
    {
        // Respawnear todos los elementos que tenía
        foreach (ElementoQuimico elem in _elementosContenidos)
        {
            if (elem != null) elem.Respawnear();
        }

        _simbolosContenidos.Clear();
        _elementosContenidos.Clear();
        _mezclado = false;

        // Resetear visual del líquido
        if (liquidoRenderer != null)
        {
            Vector3 s = liquidoRenderer.transform.localScale;
            s.z = escalaZInicial;
            liquidoRenderer.transform.localScale = s;

            if (_matLiquido != null)
            {
                _matLiquido.color = Color.clear;
                _matLiquido.DisableKeyword("_EMISSION");
            }
        }

        Debug.Log($"[Frasco '{name}'] Reiniciado.");
    }

    // ─── Gizmo de depuración ──────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}