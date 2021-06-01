using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using UnityEngine;
using System.Net;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class ScanQr : MonoBehaviour
{
    private IScanner BarcodeScanner;
    [SerializeField]
    private TextMeshProUGUI errorText;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private RawImage Image;
    [SerializeField]
    private AudioSource Audio;
    private float RestartTime;
    private Scene_Manager sceneManager;
    private Image backGround;
    private Color backGroundColor;

    [SerializeField]
    private GameObject progressBar;
    private Slider progressBarSlider;
    private Text progressBarText;

    // Disable Screen Rotation on that screen
    void Awake()
    {
        progressBarSlider = progressBar.transform.Find("Slider").GetComponent<Slider>();
        progressBarText = progressBarSlider.transform.Find("Text").GetComponent<Text>();
        progressBar.SetActive(false);
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
    void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    private bool IsActive(string productName, string campaingName)
    {
        try
        {
            Debug.Log(productName + "    " + campaingName);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://deception-aura.herokuapp.com/api/campaing/{0}/{1}", productName, campaingName));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                errorText.SetText(productName + "    " + campaingName);
                return false;
            }
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            errorText.SetText(e.Message);
            return false;
        }
    }

    private void StartScanner()
    {
        BarcodeScanner.Scan((barCodeType, barCodeValue) =>
        {
            string[] scannedString = barCodeValue.Split('-');
            if (!IsActive(scannedString[0], scannedString[1]))
                return;

            BarcodeScanner.Stop();
            if (errorText.text.Length > 250)
            {
                errorText.SetText("");
            }
            //Type is comming from XZingParser class line 42.
            if (barCodeValue != null && barCodeType == "QR_CODE")
            {
                BarcodeScanner.Camera.Stop();
                sceneManager.LoadScene(barCodeValue.Split('-')[0], barCodeValue.Split('-')[1]);
            }
            errorText.SetText("Found: " + barCodeType + " / " + barCodeValue.Split('-')[1] + "\n");
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
        backGroundColor = Color.Lerp(new Color(1.000f, 0.922f, 0.516f), new Color(0.0f, 1.0f, 0.5f), t);
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
