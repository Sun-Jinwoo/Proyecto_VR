using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject panel;

    public void AbrirPanel()
    {
        panel.SetActive(true);
    }

    public void CerrarPanel()
    {
        panel.SetActive(false);
    }
}