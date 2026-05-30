using System.Collections;
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
    public TextMeshPro textoResultado;
    public TextMeshPro textoPuntuacion;
    public TextMeshPro textoTiempo;

    [Header("Pantalla de resultados")]
    public GameObject canvasResultados;
    public GameObject canvasTablero;
    public TextMeshPro textoPuntuacionFinal;
    public TextMeshPro textoEstrellas;
    public TextMeshPro textoMensaje;

    [Header("Timer")]
    public float tiempoTotal = 120f;

    [Header("Efectos VR")]
    public Transform puntoEfectos;

    private ElementoQuimico _elementoA;
    private ElementoQuimico _elementoB;
    private int _puntos = 0;
    private float _tiempoRestante;
    private bool _juegoActivo = false;

    private void Start()
    {
        slotA.selectEntered.AddListener(args =>
            _elementoA = args.interactableObject.transform.GetComponent<ElementoQuimico>());
        slotA.selectExited.AddListener(args => _elementoA = null);

        slotB.selectEntered.AddListener(args =>
            _elementoB = args.interactableObject.transform.GetComponent<ElementoQuimico>());
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

    private void ActualizarTimer()
    {
        if (textoTiempo == null) return;
        int minutos  = Mathf.FloorToInt(_tiempoRestante / 60f);
        int segundos = Mathf.FloorToInt(_tiempoRestante % 60f);
        textoTiempo.text  = $"{minutos:00}:{segundos:00}";
        textoTiempo.color = _tiempoRestante < 30f ? Color.red : Color.white;
    }

    private void Mezclar()
    {
        if (_elementoA == null || _elementoB == null)
        {
            MostrarTexto("Faltan elementos en la mesa.");
            return;
        }

        // Posición donde aparecerán los efectos
        Vector3 posEfecto = puntoEfectos != null
            ? puntoEfectos.position
            : transform.position;

        string resultado = Recetas.Instancia.VerificarReaccion(
            _elementoA.simbolo,
            _elementoB.simbolo
        );

        // Guardamos datos de respawn ANTES de destruir
        GameObject prefabA = _elementoA.prefabPropio;
        GameObject prefabB = _elementoB.prefabPropio;
        Vector3 posA = _elementoA.puntoRespawn != null
            ? _elementoA.puntoRespawn.position
            : _elementoA.posicionRespawn;
        Vector3 posB = _elementoB.puntoRespawn != null
            ? _elementoB.puntoRespawn.position
            : _elementoB.posicionRespawn;

        if (resultado != null)
        {
            int bonusMision = Recetas.Instancia.VerificarMision(resultado);

            if (bonusMision > 0)
            {
                EfectosReceta.Instancia?.ReproducirEfectoMisionCompleta(
                    bonusMision, posEfecto);

                _puntos += bonusMision;
                ActualizarPuntos();
                MostrarTexto($"¡MISION COMPLETADA!\n{resultado}\n+{bonusMision} puntos");
            }
            else
            {
                EfectosReceta.Instancia?.ReproducirEfectoExito(posEfecto);

                _puntos += 100;
                ActualizarPuntos();
                MostrarTexto($"¡Reacción exitosa!\n{resultado}\n+100 puntos");
            }
        }
        else
        {
            EfectosReceta.Instancia?.ReproducirEfectoFallo(posEfecto);

            _puntos -= 25;
            ActualizarPuntos();
            MostrarTexto($"Combinación inválida:\n{_elementoA.simbolo} + {_elementoB.simbolo}\n-25 puntos");

            SistemaAlertaDEA.Instancia.RegistrarError(() =>
            {
                _juegoActivo = false;
                MostrarResultados();
            });
        }

        Destroy(_elementoA.gameObject);
        Destroy(_elementoB.gameObject);
        _elementoA = null;
        _elementoB = null;

        ElementoQuimico nuevoA = Instantiate(prefabA, posA, Quaternion.identity)
            .GetComponent<ElementoQuimico>();
        ElementoQuimico nuevoB = Instantiate(prefabB, posB, Quaternion.identity)
            .GetComponent<ElementoQuimico>();

        nuevoA.posicionRespawn = posA;
        nuevoB.posicionRespawn = posB;
        nuevoA.prefabPropio    = prefabA;
        nuevoB.prefabPropio    = prefabB;
    }

    private void MostrarTexto(string mensaje)
    {
        if (textoResultado != null)
            textoResultado.text = mensaje;
    }

    private void MostrarResultados()
    {
        if (canvasTablero    != null) canvasTablero.SetActive(false);
        if (canvasResultados != null) canvasResultados.SetActive(true);

        if (textoPuntuacionFinal != null)
            textoPuntuacionFinal.text = $"Puntuación final:\n{_puntos} puntos";

        int estrellas = 0;
        if      (_puntos >= 300) estrellas = 3;
        else if (_puntos >= 150) estrellas = 2;
        else if (_puntos >= 50)  estrellas = 1;

        if (textoEstrellas != null)
            textoEstrellas.text = estrellas switch
            {
                3 => "*** ¡Brillante!",
                2 => "** ¡Bien hecho!",
                1 => "*  Sigue practicando",
                _ => "Inténtalo de nuevo"
            };

        if (textoMensaje != null)
            textoMensaje.text = _puntos > 0
                ? $"Completaste {_puntos / 100} reacciones exitosas.\n¡Eres un gran científico!"
                : "No lograste ninguna reacción.\n¡Inténtalo de nuevo!";
    }

    private void ActualizarPuntos()
    {
        if (textoPuntuacion != null)
            textoPuntuacion.text = $"Puntos: {_puntos}";
    }
}