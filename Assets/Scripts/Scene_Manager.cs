using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    public string inSceneName { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadScene(string sceneName, string inSceneName)
    {
        this.inSceneName = inSceneName;

        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
