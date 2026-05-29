using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterTMP : MonoBehaviour
{
    public float velocidad = 0.03f;

    private TextMeshProUGUI texto;
    private string fullText;
    private Coroutine rutina;

    void Awake()
    {
        texto = GetComponent<TextMeshProUGUI>();
        fullText = texto.text; // guardamos el texto UNA sola vez
    }

    void OnEnable()
    {
        StartTyping();
    }

    public void StartTyping()
    {
        if (rutina != null)
            StopCoroutine(rutina);

        texto.text = "";
        rutina = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            texto.text += fullText[i];
            yield return new WaitForSeconds(velocidad);
        }
    }
}