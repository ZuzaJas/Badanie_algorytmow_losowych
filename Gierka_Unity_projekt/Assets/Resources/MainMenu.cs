using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public void GenerateDungeon()
    {
        StartCoroutine(LoadSceneAsync(1));
    }

    public void LoadDungeon()
    {
        StartCoroutine(LoadSceneAsync(2));
    }

    public void GenerateStats()
    {
        StartCoroutine(LoadSceneAsync(3));
    }

    public void GoExit()
    {
        Application.Quit();
        Debug.Log("Quit game");
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);

        // opcjonalnie: blokujemy auto-switch (dla loading screen)
        // op.allowSceneActivation = false;

        while (!op.isDone)
        {
            yield return null;
        }
    }
}