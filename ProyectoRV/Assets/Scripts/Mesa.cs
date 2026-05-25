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
    public TextMeshProUGUI textoResultado;
    public TextMeshProUGUI textoPuntuacion;
    public TextMeshProUGUI textoTiempo;

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
            MostrarTexto($"¡Tiempo agotado!\nPuntuación final: {_puntos}");
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

        if (resultado != null)
        {
            _puntos += 100;
            ActualizarPuntos();
            MostrarTexto($"¡Reacción exitosa!\n{resultado}\n+100 puntos");

            GameObject prefabA = _elementoA.prefabPropio;
            GameObject prefabB = _elementoB.prefabPropio;
            Vector3 posA = _elementoA.puntoRespawn != null ? _elementoA.puntoRespawn.position : _elementoA.posicionRespawn;
            Vector3 posB = _elementoB.puntoRespawn != null ? _elementoB.puntoRespawn.position : _elementoB.posicionRespawn;

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
        else
        {
            _puntos -= 25;
            ActualizarPuntos();
            MostrarTexto($"Combinación inválida:\n{_elementoA.simbolo} + {_elementoB.simbolo}\n-25 puntos");

            GameObject prefabA = _elementoA.prefabPropio;
            GameObject prefabB = _elementoB.prefabPropio;
            Vector3 posA = _elementoA.puntoRespawn != null ? _elementoA.puntoRespawn.position : _elementoA.posicionRespawn;
            Vector3 posB = _elementoB.puntoRespawn != null ? _elementoB.puntoRespawn.position : _elementoB.posicionRespawn;

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
}
