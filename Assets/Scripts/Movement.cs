using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Quaternion rot;
    public bool hit { get; private set; }
    public bool hit1 { get; private set; }
    public bool hit2 { get; private set; }

    GameObject camObject;
    Quaternion rotfix;
    void Start()
    {
        //transform.rotation = GyroToUnity(Input.gyro.attitude);
        rot = Quaternion.Euler(Camera.main.transform.rotation.x - 160, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z);
        camObject = GameObject.Find("BackgroundCamera");
        rotfix = new Quaternion(0, 0, 1, 0);
    }


    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && (hit == false || hit1 == false || hit2 == false))
        {
            hit = true;
            //gameObject.transform.rotation.SetFromToRotation(transform.rotation.eulerAngles, new Vector3(90, 30, -60)); 
        }
        if (hit)
        {
            gameObject.transform.Rotate(250 * Time.deltaTime, 200 * Time.deltaTime, 100 * Time.deltaTime, Space.Self);
            if (transform.rotation.eulerAngles.z > 280)
                hit = false;
        }
        if (hit == false)
        {
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.8f, 0.4f, Camera.main.nearClipPlane * 3.5f));
            Vector3 rotAngle = new Vector3(Camera.main.transform.eulerAngles.x - 160.0f, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            Quaternion quaternionAngle = Quaternion.Euler(rotAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternionAngle, 3 * Time.deltaTime);            //transform.eulerAngles = rotAngle;
        }
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                hit = true;
            }
           //gameObject.transform.rotation.SetFromToRotation(transform.rotation.eulerAngles, new Vector3(90, 30, -60)); 
        }

        if (hit)
        {
            gameObject.transform.Rotate(180 * Time.deltaTime, 100 * Time.deltaTime, -100 * Time.deltaTime, Space.Self);
            //gameObject.transform.Rotate((180 + Camera.main.transform.rotation.x) * Time.deltaTime, (100 + Camera.main.transform.rotation.y) * Time.deltaTime, (-190 + Camera.main.transform.rotation.z) * Time.deltaTime, Space.Self);

            if (transform.rotation.eulerAngles.z > 300)
                hit = false;
        }
        if (hit == false)
        {
            //transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.8f, 0.4f, Camera.main.nearClipPlane * 3.5f));
            ////transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x - 160, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z);
            ////transform.rotation = Quaternion.Slerp(transform.rotation, rot, 3 * Time.deltaTime);
            //Vector3 rotAngle = new Vector3(Camera.main.transform.eulerAngles.x - 180.0f, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            //transform.eulerAngles = rotAngle;

          transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.8f, 0.4f, Camera.main.nearClipPlane * 3.5f));
            Vector3 rotAngle = new Vector3(Camera.main.transform.eulerAngles.x - 180.0f, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            Quaternion quaternionAngle = Quaternion.Euler(rotAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternionAngle, 3 * Time.deltaTime);
        }
#endif
    }
}
