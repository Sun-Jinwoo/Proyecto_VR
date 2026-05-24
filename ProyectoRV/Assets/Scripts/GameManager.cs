//using System.Collections;
//using UnityEngine;
//using UnityEngine.Events;

//public enum EstadoJuego
//{
//    Onboarding,    // Tutorial inicial
//    Jugando,       // En plena partida
//    EventoUrgente, // Evento inesperado activo
//    Pausado,
//    Finalizado
//}

//public class GameManager : MonoBehaviour
//{
//    public static GameManager Instancia { get; private set; }

//    [Header("Tiempo")]
//    public float tiempoTotalSegundos = 300f;      // 5 minutos por defecto
//    public float intervalEventoInesperado = 60f;  // Cada 60s puede ocurrir algo

//    [Header("Puntuación")]
//    public int penalizacionPorFallo = 25;         // Puntos que se restan al fallar
//    public float multiplicadorTiempo = 1.5f;      // Bonus si termina rápido

//    [Header("Misión")]
//    public int reaccionesParaGanar = 4;           // Cuántas reacciones hay que hacer

//    [Header("Eventos Unity (conectar UI desde Inspector)")]
//    public UnityEvent<float> OnTiempoActualizado;   // Cada segundo
//    public UnityEvent<int> OnPuntuacionCambiada;  // Al cambiar score
//    public UnityEvent<string> OnMensajeFlotante;     // Para mostrar texto en VR
//    public UnityEvent OnJuegoTerminado;
//    public UnityEvent OnEventoInesperado;

//    private EstadoJuego _estadoActual = EstadoJuego.Onboarding;
//    private float _tiempoRestante;
//    private int _puntuacion;
//    private int _reaccionesCompletadas;
//    private bool _juegoActivo;

//    public EstadoJuego Estado => _estadoActual;
//    public float TiempoRestante => _tiempoRestante;
//    public int Puntuacion => _puntuacion;
//    public int Reacciones => _reaccionesCompletadas;

//    private void Awake()
//    {
//        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
//        Instancia = this;
//    }

//    private void Start()
//    {
//        if (Recetas.Instancia != null)
//        {
//            Recetas.Instancia.OnReaccionExitosa += AlCompletarReaccion;
//            Recetas.Instancia.OnReaccionFallida += AlFallarReaccion;
//        }

//        _tiempoRestante = tiempoTotalSegundos;
//        _puntuacion = 0;
//        _reaccionesCompletadas = 0;

//        Debug.Log("[GM] GameManager iniciado. Esperando onboarding...");
//    }

//    private void Update()
//    {
//        if (!_juegoActivo) return;

//        _tiempoRestante -= Time.deltaTime;
//        OnTiempoActualizado?.Invoke(_tiempoRestante);

//        if (_tiempoRestante <= 0f)
//        {
//            _tiempoRestante = 0f;
//            TerminarJuego(porTiempo: true);
//        }
//    }

//    public void IniciarJuego()
//    {
//        _estadoActual = EstadoJuego.Jugando;
//        _juegoActivo = true;

//        Debug.Log("[GM] ¡Juego iniciado!");
//        MostrarMensaje("¡Bienvenido al Laboratorio! Combina elementos para crear compuestos.");

//        // Iniciamos el sistema de eventos inesperados
//        StartCoroutine(CorrutinaEventosInesperados());
//    }

//    public void TerminarJuego(bool porTiempo = false)
//    {
//        if (_estadoActual == EstadoJuego.Finalizado) return;

//        _juegoActivo = false;
//        _estadoActual = EstadoJuego.Finalizado;

//        // Calculamos bonus si terminó antes del tiempo
//        if (!porTiempo && _tiempoRestante > 0)
//        {
//            int bonusTiempo = Mathf.RoundToInt(_tiempoRestante * multiplicadorTiempo);
//            AgregarPuntos(bonusTiempo);
//            MostrarMensaje($"¡Bonus de tiempo! +{bonusTiempo} puntos");
//        }
//        else if (porTiempo)
//        {
//            MostrarMensaje("¡Tiempo agotado!");
//        }

//        Debug.Log($"Juego terminado. Puntuación final: {_puntuacion}");
//        OnJuegoTerminado?.Invoke();

//        // Mostramos la pantalla de resultados tras 2 segundos
//        Invoke(nameof(MostrarResultados), 2f);
//    }

//    public void AgregarPuntos(int cantidad)
//    {
//        _puntuacion += cantidad;
//        _puntuacion = Mathf.Max(0, _puntuacion); // No puede ser negativo
//        OnPuntuacionCambiada?.Invoke(_puntuacion);
//        Debug.Log($"Puntuación: {_puntuacion} (+{cantidad})");
//    }

//    public void RestarPuntos(int cantidad)
//    {
//        _puntuacion -= cantidad;
//        _puntuacion = Mathf.Max(0, _puntuacion);
//        OnPuntuacionCambiada?.Invoke(_puntuacion);
//        Debug.Log($"Puntuación: {_puntuacion} (-{cantidad})");
//    }

//  /*  private void AlCompletarReaccion(RecetaReaccion receta)
//    {
//        _reaccionesCompletadas++;
//        AgregarPuntos(receta.puntos);
//        MostrarMensaje($"¡{receta.nombreResultado} descubierto! +{receta.puntos} pts");

//        // ¿Completó la misión?
//        if (_reaccionesCompletadas >= reaccionesParaGanar)
//        {
//            MostrarMensaje("¡MISIÓN COMPLETADA!");
//            Invoke(nameof(TerminarJuego), 1.5f);
//        }
//    }*/

//    private void AlFallarReaccion(string simboloA, string simboloB)
//    {
//        RestarPuntos(penalizacionPorFallo);
//        MostrarMensaje($"Combinación inválida: {simboloA} + {simboloB}. -{penalizacionPorFallo} pts");
//    }

//    private IEnumerator CorrutinaEventosInesperados()
//    {
//        while (_juegoActivo)
//        {
//            yield return new WaitForSeconds(intervalEventoInesperado);

//            if (!_juegoActivo) yield break;

//            // Lanzamos el evento inesperado
//            _estadoActual = EstadoJuego.EventoUrgente;
//            Debug.Log("¡EVENTO INESPERADO ACTIVADO!");

//            OnEventoInesperado?.Invoke();
//            MostrarMensaje("¡ALERTA! Hay un compuesto inestable. ¡Estabilízalo rápido!");

//            // El estado de urgencia dura 20 segundos
//            yield return new WaitForSeconds(20f);

//            if (_estadoActual == EstadoJuego.EventoUrgente)
//            {
//                // El jugador no lo estabilizó a tiempo
//                RestarPuntos(50);
//                MostrarMensaje("?? El compuesto explotó. -50 pts");
//                _estadoActual = EstadoJuego.Jugando;
//            }
//        }
//    }

//    public void ResolverEventoInesperado()
//    {
//        if (_estadoActual != EstadoJuego.EventoUrgente) return;

//        _estadoActual = EstadoJuego.Jugando;
//        AgregarPuntos(75);
//        MostrarMensaje("¡Compuesto estabilizado! +75 pts");
//        Debug.Log(" Evento inesperado resuelto por el jugador.");
//    }

//    private void MostrarResultados()
//    {
//        int estrellas = CalcularEstrellas();
//        string mensaje = $"FIN | Puntos: {_puntuacion} | Reacciones: {_reaccionesCompletadas}/{reaccionesParaGanar}";

//        Debug.Log($"RESULTADOS FINALES — {mensaje}");
//        MostrarMensaje(mensaje);
//    }

//    private int CalcularEstrellas()
//    {
//        float porcentajePuntos = (float)_puntuacion / (reaccionesParaGanar * 150f);

//        if (porcentajePuntos >= 0.8f) return 3;
//        if (porcentajePuntos >= 0.5f) return 2;
//        return 1;
//    }

//    public int ObtenerEstrellas() => CalcularEstrellas();
//    public float ObtenerPorcentaje() => (float)_reaccionesCompletadas / reaccionesParaGanar;

//    private void MostrarMensaje(string texto)
//    {
//        OnMensajeFlotante?.Invoke(texto);
//    }
//}