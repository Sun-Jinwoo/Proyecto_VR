using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MesaMezcla : MonoBehaviour
{
    [Header("Conectar desde el Inspector")]
    public XRSocketInteractor slotA;
    public XRSocketInteractor slotB;
    public XRBaseInteractable boton;

    [Header("Tablero")]
    public GameObject canvasTablero;
    public TextMeshProUGUI textoResultado;
    public TextMeshProUGUI textoPuntuacion;
    public TextMeshProUGUI textoTiempo;

    [Header("Pantalla de resultados")]
    public GameObject canvasResultados;         
    public TextMeshProUGUI textoPuntuacionFinal; 
    public TextMeshProUGUI textoEstrellas;       
    public TextMeshProUGUI textoMensaje;         

    public float tiempoTotal = 120f;

    private ElementoQuimico _elementoA;
    private ElementoQuimico _elementoB;
    private int _puntos = 0;
    private float _tiempoRestante; 
    private bool _juegoActivo = false;

    private void Start()
    {
        slotA.selectEntered.AddListener(args => _elementoA = args.interactableObject.transform.GetComponent<ElementoQuimico>());
        slotA.selectExited.AddListener(args => _elementoA = null);

        slotB.selectEntered.AddListener(args => _elementoB = args.interactableObject.transform.GetComponent<ElementoQuimico>());
        slotB.selectExited.AddListener(args => _elementoB = null);

        boton.selectEntered.AddListener(args => Mezclar());

        ActualizarPuntos();

        _tiempoRestante = tiempoTotal;
        _juegoActivo = true;
    }

    private void Update() 
    {
        if (!_juegoActivo) return;

        _tiempoRestante -= Time.deltaTime;
        ActualizarTimer();

        if (_tiempoRestante <= 0f)
        {
            _tiempoRestante = 0f;
            _juegoActivo = false;
            MostrarResultados();
        }
    }

    private void ActualizarTimer() // NUEVO
    {
        if (textoTiempo == null) return;

        int minutos = Mathf.FloorToInt(_tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(_tiempoRestante % 60f);

        textoTiempo.text = $"{minutos:00}:{segundos:00}";

        // Se pone rojo cuando quedan menos de 30 segundos
        textoTiempo.color = _tiempoRestante < 30f ? Color.red : Color.white;
    }


    private void Mezclar()
    {
        if (_elementoA == null || _elementoB == null)
        {
            MostrarTexto("Faltan elementos en la mesa.");
            return;
        }

        string resultado = Recetas.Instancia.VerificarReaccion(
            _elementoA.simbolo,
            _elementoB.simbolo
        );

        GameObject prefabA = _elementoA.prefabPropio;
        GameObject prefabB = _elementoB.prefabPropio;
        Vector3 posA = _elementoA.puntoRespawn != null ? _elementoA.puntoRespawn.position : _elementoA.posicionRespawn;
        Vector3 posB = _elementoB.puntoRespawn != null ? _elementoB.puntoRespawn.position : _elementoB.posicionRespawn;

        if (resultado != null)
        {
            int bonusMision = Recetas.Instancia.VerificarMision(resultado);

            if (bonusMision > 0)
            {
                _puntos += 100 + bonusMision;
                ActualizarPuntos();
                MostrarTexto($"¡MISIÓN COMPLETADA!\n{resultado}\n+{100 + bonusMision} puntos");
            }
            else
            {
                // Reacción válida pero no era la misión
                _puntos += 100;
                ActualizarPuntos();
                MostrarTexto($"¡Reacción exitosa!\n{resultado}\n+100 puntos");
            }
        }
        else
        {
            _puntos -= 25;
            ActualizarPuntos();
            MostrarTexto($"Combinación inválida:\n{_elementoA.simbolo} + {_elementoB.simbolo}\n-25 puntos");
        }

        // Respawn siempre al final, sin importar el resultado
        Destroy(_elementoA.gameObject);
        Destroy(_elementoB.gameObject);
        _elementoA = null;
        _elementoB = null;

        ElementoQuimico nuevoA = Instantiate(prefabA, posA, Quaternion.identity).GetComponent<ElementoQuimico>();
        ElementoQuimico nuevoB = Instantiate(prefabB, posB, Quaternion.identity).GetComponent<ElementoQuimico>();

        nuevoA.posicionRespawn = posA;
        nuevoB.posicionRespawn = posB;
        nuevoA.prefabPropio = prefabA;
        nuevoB.prefabPropio = prefabB;
    }

    private void MostrarTexto(string mensaje)
    {
        if (textoResultado != null)
            textoResultado.text = mensaje;
    }

    private void ActualizarPuntos() 
    {
        if (textoPuntuacion != null)
            textoPuntuacion.text = $"Puntos: {_puntos}";
    }

    private void MostrarResultados()
    {
        if (canvasTablero != null)
            canvasTablero.SetActive(false);
        // Ocultamos el tablero y mostramos la pantalla final
        if (canvasResultados != null)
            canvasResultados.SetActive(true);

        // Puntuación
        if (textoPuntuacionFinal != null)
            textoPuntuacionFinal.text = $"Puntuación final:\n{_puntos} puntos";

        // Estrellas según puntuación
        int estrellas = 0;
        if (_puntos >= 300) estrellas = 3;
        else if (_puntos >= 150) estrellas = 2;
        else if (_puntos >= 50) estrellas = 1;

        if (textoEstrellas != null)
            textoEstrellas.text = estrellas switch
            {
                3 => "*** ¡Brillante!",
                2 => "** ¡Bien hecho!",
                1 => "* Sigue practicando",
                _ => "Inténtalo de nuevo"
            };

        // Mensaje motivacional
        if (textoMensaje != null)
            textoMensaje.text = _puntos > 0
                ? $"Completaste {_puntos / 100} reacciones exitosas.\n¡Eres un gran científico!"
                : "No lograste ninguna reacción.\n¡Inténtalo de nuevo!";

        Debug.Log($"[Resultados] Juego terminado. Puntos: {_puntos}");
    }
}
