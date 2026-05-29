using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] panels; // Arrastra aquí todos los paneles del tutorial
    public GameObject tutorialPanel; // Panel principal del tutorial

    private int currentIndex = 0;

    void Start()
    {
        ShowPanel(currentIndex);
    }

    void ShowPanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        panels[index].SetActive(true);
    }

    public void NextPanel()
    {
        if (currentIndex < panels.Length - 1)
        {
            currentIndex++;
            ShowPanel(currentIndex);
        }
    }

    public void PreviousPanel()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowPanel(currentIndex);
        }
    }

    public void CerrarTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}