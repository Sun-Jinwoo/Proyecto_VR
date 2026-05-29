using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject panelTutorial;

    public void AbrirTutorial()
    {
        panelTutorial.SetActive(true);
    }

    public void CerrarTutorial()
    {
        panelTutorial.SetActive(false);
    }
}