using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScanQr : MonoBehaviour
{
    private IScanner BarcodeScanner;
    public Text TextHeader;
    public GameObject exitButton;
    public RawImage Image;
    public AudioSource Audio;
    private float RestartTime;
    private Scene_Manager sceneManager;
    private Image backGround;
    private Color backGroundColor;

    // Disable Screen Rotation on that screen
    void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    void Start()
    {
        sceneManager = GameObject.Find("SceneManager").GetComponent<Scene_Manager>();
        backGround = GameObject.Find("Background").GetComponent<Image>();
        backGroundColor = backGround.color;
        exitButton.GetComponent<Button>().onClick.AddListener(ExitApp);
        // Create a basic scanner
        BarcodeScanner = new Scanner();
        BarcodeScanner.Camera.Play();

        // Display the camera texture through a RawImage
        BarcodeScanner.OnReady += (sender, arg) =>
        {
            // Set Orientation & Texture
            Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
            Image.transform.localScale = BarcodeScanner.Camera.GetScale();
            Image.texture = BarcodeScanner.Camera.Texture;

            // Keep Image Aspect Ratio
            var rect = Image.GetComponent<RectTransform>();
            var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

            RestartTime = Time.realtimeSinceStartup;
        };
    }
    private void ExitApp()
    {
        Application.Quit();
    }
    private void StartScanner()
    {
        BarcodeScanner.Scan((barCodeType, barCodeValue) =>
        {
            Debug.Log(barCodeValue.Split('-')[0] + " " + barCodeValue.Split('-')[1]);
            BarcodeScanner.Stop();
            if (TextHeader.text.Length > 250)
            {
                TextHeader.text = "";
            }
            //Type is comming from XZingParser class line 42.
            if (barCodeValue != null && barCodeType == "QR_CODE")
            {
                BarcodeScanner.Camera.Stop();
                sceneManager.LoadScene(barCodeValue.Split('-')[0], barCodeValue.Split('-')[1]);
            }
            TextHeader.text += "Found: " + barCodeType + " / " + barCodeValue.Split('-')[1] + "\n";
            RestartTime += Time.realtimeSinceStartup + 1f;

            // Feedback
            Audio.Play();

#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
        });
    }

    void Update()
    {
        if (BarcodeScanner != null)
        {
            BarcodeScanner.Update();
        }

        // Check if the Scanner need to be started or restarted
        if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
        {
            StartScanner();
            RestartTime = 0;
        }
        float t = Mathf.PingPong(Time.time, 1.0f) / 1.0f;
        backGroundColor = Color.Lerp(Color.red, Color.blue, t);
        backGround.color = backGroundColor;

    }

    #region UI Buttons


    public IEnumerator StopCamera(Action callback)
    {
        // Stop Scanning
        Image = null;
        BarcodeScanner.Destroy();
        BarcodeScanner = null;

        // Wait a bit
        yield return new WaitForSeconds(0.1f);

        callback.Invoke();
    }

    #endregion
}
