using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuVR : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Salir()
    {
        Debug.Log("Botón Salir pulsado");
        Application.Quit();
    }
}