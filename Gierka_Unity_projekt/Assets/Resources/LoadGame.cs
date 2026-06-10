using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public LoadSystem LoadSys;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSys.LoadAllSeeds(1);
    }

    public void GoBackButton() {
        SceneManager.LoadSceneAsync(0);
    }
}
