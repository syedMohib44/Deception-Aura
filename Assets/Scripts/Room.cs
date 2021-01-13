using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private GameObject window;
    private Material windowColor;
    void Start()
    {
        window = gameObject.transform.Find("Indicator/Building").gameObject;
        windowColor = window.GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (window != null)
        {
            float t = Mathf.PingPong(Time.time, 1.0f) / 1.0f;
            windowColor.color = Color.Lerp(Color.red, Color.blue, t);
        }
    }
}
