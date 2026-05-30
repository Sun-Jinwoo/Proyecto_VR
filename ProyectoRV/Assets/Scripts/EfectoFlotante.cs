using System.Collections;
using UnityEngine;
using TMPro;

public class EfectoFlotante : MonoBehaviour
{
    [Header("Referencia")]
    public TextMeshPro texto;        // TextMeshPro 3D, NO UI

    [Header("Configuración")]
    public float velocidadSubida = 0.6f; // más lento que en pantalla plana
    public float duracion        = 1.5f;
    public float escalaTexto     = 0.004f; // tamaño en metros en el mundo VR

    private Transform _camaraVR;

    public void Iniciar(string mensaje, Color color, Transform camaraVR)
    {
        _camaraVR = camaraVR;

        transform.localScale = Vector3.one * escalaTexto;

        if (texto != null)
        {
            texto.text  = mensaje;
            texto.color = color;
        }

        StartCoroutine(Flotar());
    }

    private IEnumerator Flotar()
    {
        float   t          = 0f;
        Vector3 posInicial = transform.position;

        while (t < duracion)
        {
            t += Time.deltaTime;
            float progreso = t / duracion;

            // Sube suavemente
            transform.position = posInicial + Vector3.up * (velocidadSubida * t);

            // Siempre mira al jugador (billboard)
            if (_camaraVR != null)
            {
                Vector3 dir = transform.position - _camaraVR.position;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(dir);
            }

            // Desvanece en la segunda mitad
            if (texto != null && progreso > 0.5f)
            {
                Color c = texto.color;
                c.a       = Mathf.Lerp(1f, 0f, (progreso - 0.5f) * 2f);
                texto.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}