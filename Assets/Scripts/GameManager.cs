using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Scene_Manager sm;

    void Start()
    {
        sm = GameObject.Find("SceneManager").GetComponent<Scene_Manager>();

        GameObject[] allObjects;
        allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        if (allObjects.Length > 0)
        {
            foreach (GameObject objects in allObjects)
            {
                if (objects.hideFlags == HideFlags.None)
                {
                    if (objects.name == sm.inSceneName)
                    {
                        objects.SetActive(true);
                        break;
                    }
                }
            }
        }
    }
}
