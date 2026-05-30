using System.Collections;
using UnityEngine;
using TMPro;

public class SistemaAlertaDEA : MonoBehaviour
{
    public static SistemaAlertaDEA Instancia;

    [Header("Configuración")]
    public int erroresParaGameOver = 3; // Errores antes del Game Over

    [Header("Luces rojas")]
    public Light[] lucesRojas; // Arrastra aquí las luces del laboratorio

    [Header("UI Alerta")]
    public TextMeshPro textoNivelAlerta;

    private int _erroresActuales = 0;
    private bool _parpadeando = false;

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;

        // Luces apagadas al inicio
        SetLuces(false);
    }

    // Llamado desde MesaMezcla cuando hay mezcla incorrecta
    public void RegistrarError(System.Action onGameOver)
    {
        _erroresActuales++;
        ActualizarUI();
        Debug.Log($"[DEA] Error #{_erroresActuales}");

        if (_erroresActuales >= erroresParaGameOver)
        {
            // Llegó al máximo — Game Over
            StopAllCoroutines();
            SetLuces(true); // Luces fijas en rojo
            onGameOver?.Invoke();
        }
        else
        {
            // Parpadeo proporcional al nivel de alerta
            if (_parpadeando) StopAllCoroutines();
            float velocidad = 0.5f - (_erroresActuales * 0.1f); // Más rápido cada error
            StartCoroutine(ParpadeaLuces(velocidad));
        }
    }

    private IEnumerator ParpadeaLuces(float intervalo)
    {
        _parpadeando = true;
        float duracion = 3f; // Segundos que parpadea
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            SetLuces(true);
            yield return new WaitForSeconds(intervalo);
            SetLuces(false);
            yield return new WaitForSeconds(intervalo);
            tiempo += intervalo * 2;
        }

        _parpadeando = false;
    }

    private void SetLuces(bool encendidas)
    {
        foreach (Light luz in lucesRojas)
        {
            if (luz == null) continue;
            luz.enabled = encendidas;
            luz.color = Color.red;
        }
    }

    private void ActualizarUI()
    {
        if (textoNivelAlerta == null) return;

        textoNivelAlerta.text = _erroresActuales switch
        {
            0 => "",
            1 => "Alerta DEA: Nivel 1",
            2 => "Alerta DEA: Nivel 2 - ¡Cuidado!",
            _ => "¡LA DEA LLEGÓ!"
        };

        textoNivelAlerta.color = _erroresActuales switch
        {
            1 => Color.yellow,
            2 => new Color(1f, 0.5f, 0f), // Naranja
            _ => Color.red
        };
    }

    public void ResetearAlertas()
    {
        _erroresActuales = 0;
        StopAllCoroutines();
        SetLuces(false);
        ActualizarUI();
        _parpadeando = false;
    }
}