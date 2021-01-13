using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private GameObject[] windows;
    private Material[] windowColor;
    private int tempInt;
    void Start()
    {
        tempInt = 0;
        windows = GameObject.FindGameObjectsWithTag("Window");
        windowColor = new Material[windows.Length];
        for (int i = 0; i < windows.Length; i++)
        {
            windowColor[i] = windows[i].GetComponent<Renderer>().material;
        }
    }

    void Update()
    {
        if (windows[tempInt] != null)
        {
            float t = Mathf.PingPong(Time.time, 1.0f) / 1.0f;
            windowColor[tempInt].color = Color.Lerp(Color.red, Color.blue, t);
        }
        tempInt++;
        tempInt %= (windows.Length);
    }
}
