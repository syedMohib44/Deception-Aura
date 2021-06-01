using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public IEnumerator LoadLevelAsync(string sceneName, string inSceneName, Slider progressBarSlider, Text progressBarText)
    {
        this.inSceneName = inSceneName;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBarSlider.value = progress;
            progressBarText.text = progress.ToString("F0") + "%";

            yield return null;
        }
    }
}
