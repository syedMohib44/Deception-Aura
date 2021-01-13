using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundRender : MonoBehaviour
{
    private RawImage img;
    private WebCamTexture cam;
    private AspectRatioFitter asf;


    void Start()
    {
        asf = GetComponent<AspectRatioFitter>();
        img = GetComponent<RawImage>();
        cam = new WebCamTexture(Screen.width, Screen.height);
        img.texture = cam;
        cam.Play();
      
    }
    void Update()
    {
        if (cam.width < 100) return;
        float cwNeeded = -cam.videoRotationAngle;
        if (cam.videoVerticallyMirrored)
        {
            cwNeeded += 180;
        }
        img.rectTransform.localEulerAngles = new Vector3(0f, 0f, cwNeeded);
        float videoRatio = (float)cam.width / (float)cam.height;
        asf.aspectRatio = videoRatio;


        if (cam.videoVerticallyMirrored)
            img.uvRect = new Rect(1, 0, -1, 1);
        else
            img.uvRect = new Rect(0, 0, 1, 1); 
    }
}
