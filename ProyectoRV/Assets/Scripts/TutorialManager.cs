using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] panels; // arrastra tus 10 paneles aquí
    private int currentIndex = 0;

    void Start()
    {
        ShowPanel(currentIndex);
    }

    void ShowPanel(int index)
    {
        // Oculta todos
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        // Activa el actual
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

    public void GoToMenu()
    {
        SceneManager.LoadScene("NewMenu");
    }
}