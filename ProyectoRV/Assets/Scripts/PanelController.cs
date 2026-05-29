using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject panel; 

    // Se llama desde el botón de abrir
    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    // Se llama desde el botón de cerrar
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}