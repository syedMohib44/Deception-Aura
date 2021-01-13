using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroCamera : MonoBehaviour
{
    private Gyroscope gyroScope;
    private bool gyroSuppoted = false;
    private Quaternion rotfix;

    [SerializeField]
    private Transform worldObj;
    private float startY;
    [SerializeField]
    private Transform zoomObj;

    void Start()
    {
        gyroSuppoted = SystemInfo.supportsGyroscope;
        GameObject camParent = new GameObject("camParent");
        camParent.transform.position = transform.position;
        transform.parent = camParent.transform;
        //GameObject.Find("Player").transform.parent = gameObject.transform;

        gyroScope = Input.gyro;
        if (gyroSuppoted)
        {
            gyroScope = Input.gyro;
            gyroScope.enabled = true;
            camParent.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
            rotfix = new Quaternion(0, 0, 1, 0);
        }
    }

    void Update()
    {
        // if (gyroSuppoted && startY == 0)
        //     ResetGyroRotation();
        transform.localRotation = gyroScope.attitude * rotfix;
    }

    void ResetGyroRotation()
    {
        int x = Screen.width / 2;
        int y = Screen.height / 2;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500))
        {
            Vector3 hitPoint = hit.point;
            float z = Vector3.Distance(Vector3.zero, hitPoint);
            zoomObj.localPosition = new Vector3(0f, zoomObj.localPosition.y, Mathf.Clamp(z, 2f, 10));
        }

        startY = transform.eulerAngles.y;
        worldObj.rotation = Quaternion.Euler(0f, startY, 0f);
    }
}