using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] textos;
    public GameObject tutorialPanel;

    private int currentIndex = 0;

    void Start()
    {
        MostrarTexto(currentIndex);
    }

    void MostrarTexto(int index)
    {
        // Oculta todos los textos
        for (int i = 0; i < textos.Length; i++)
        {
            textos[i].SetActive(false);
        }

        // Muestra solo el actual
        textos[index].SetActive(true);
    }

    public void NextPanel()
    {
        if (currentIndex < textos.Length - 1)
        {
            currentIndex++;
            MostrarTexto(currentIndex);
        }
    }

    public void PreviousPanel()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            MostrarTexto(currentIndex);
        }
    }

    public void CerrarTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    
}