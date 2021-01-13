using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public TextMeshProUGUI gpsOut;
    private const float SPEED = 5.0f;
    private bool isUpdating, switchTouch = false;
    private float lat, longi;
    //Long = X, Lat = Y
    public GameObject switchTOn, switchTOff;

    private GameObject building, room, backGroundCam;
    private GameObject canvas;
    private Vector3 roomPos = new Vector3(0.23f, -0.5f, -19.0f);
    private Vector3 buildingPos = new Vector3(0, 0, 0);
    void Start()
    {
        canvas = GameObject.Find("World2/Canvas");
        building = GameObject.Find("zoomObj/Buildings");
        room = GameObject.Find("zoomObj/Rooms");
        backGroundCam = GameObject.Find("World2/BackgroundCamera");
        room.SetActive(false);
        switchTOn.GetComponent<Button>().onClick.AddListener(SwitchOff);
        switchTOff.GetComponent<Button>().onClick.AddListener(SwitchOn);
    }

    void SwitchOff()
    {
        switchTOn.SetActive(false);
        switchTOff.SetActive(true);
        switchTouch = false;
    }

    void SwitchOn()
    {
        switchTOn.SetActive(true);
        switchTOff.SetActive(false);
        switchTouch = true;

    }

    void FixedUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            canvas.transform.Find("RawImage").gameObject.SetActive(false);
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);
            if (hit.collider.tag == "Window")
            {
                building.SetActive(false);
                backGroundCam.SetActive(false);
                room.SetActive(true);
                gameObject.transform.parent.position = new Vector3(0.23f, -0.5f, -19.0f);
            }
            else if (hit.collider.tag == "BuildingScene")
            {
                room.SetActive(false);
                backGroundCam.SetActive(true);
                building.SetActive(true);
                gameObject.transform.parent.position = buildingPos;
                gameObject.transform.parent.eulerAngles = new Vector3(0, 0, 0);
            }
        }

#elif UNITY_ANDROID
    if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Window")
            {
                switchTouch = true;
                building.SetActive(false);
                backGroundCam.SetActive(false);
                room.SetActive(true);
                gameObject.transform.parent.position = roomPos;
            }
            else if (hit.collider.tag == "BuildingScene")
            {
                room.SetActive(false);
                backGroundCam.SetActive(true);
                building.SetActive(true);
                gameObject.transform.parent.position = buildingPos;
            }
        }
    }
#endif
        if (!switchTouch)
        {
            Vector3 dir = Vector3.zero;
            dir.z = Input.gyro.userAcceleration.z;
            transform.Translate(-dir * SPEED * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Vector3 dir = Vector3.zero;
                dir.z = 0.1f;
                transform.Translate(dir * SPEED * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
        }
    }
    IEnumerator GetLocation()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
            //yield return new WaitForSeconds(10);
        }

        // Start service before querying location
        Input.location.Start(1, 0.1f);

        // Wait until service initializes
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            gpsOut.text = "Timed out";
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            gpsOut.text = "Unable to determine device location";
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            if (lat != Input.location.lastData.latitude && longi != Input.location.lastData.longitude)
            {
                //if(lat > Input.location.lastData.latitude && longi > Input.location.lastData.longitude)
                //    transform.position += new Vector3(0, 0, 10) * Time.deltaTime;
                //if (lat < Input.location.lastData.latitude && longi < Input.location.lastData.longitude)
                //    transform.position -= new Vector3(0, 0, 5) * Time.deltaTime;
                lat = Input.location.lastData.latitude;
                longi = Input.location.lastData.longitude;
            }
            gpsOut.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + 100f + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        isUpdating = !isUpdating;
        Input.location.Stop();
    }
}
