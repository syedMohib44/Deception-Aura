using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGame_UI : MonoBehaviour
{
    public GameObject home;
    void Start()
    {
        home.GetComponent<Button>().onClick.AddListener(Home);
    }
    void Home()
    {
        SceneManager.LoadScene(0);
    }
}
