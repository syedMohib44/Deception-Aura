using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowAble : MonoBehaviour
{
    public class Pool
    {
        public GameObject obj;
        public float force = 0.0f;
        public bool powerThrown = false;
    }
    public GameObject sphere;
    private Queue<Pool> objQueue;
    private float force = 0.0f;
    bool powerThrown;
    public GameObject magicButton;
    private int count = 0;

    void Start()
    {
        magicButton.GetComponent<Button>().onClick.AddListener(Shoot);
        objQueue = new Queue<Pool>();
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(sphere);
            Pool pool = new Pool();
            pool.obj = obj;
            pool.powerThrown = false;
            pool.force = 0.0f;
            pool.obj.SetActive(false);
            pool.obj.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, Camera.main.nearClipPlane * 7.5f));
            objQueue.Enqueue(pool);
        }
        gameObject.transform.position = Camera.main.transform.position;
    }
    Pool obj;

    void Shoot()
    {
        if (obj == null)
        {
            obj = objQueue.Dequeue();
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 dir = r.GetPoint(1) - r.GetPoint(0);
            obj.obj.transform.position = r.GetPoint(2);
            obj.obj.transform.rotation = Quaternion.LookRotation(dir);
            obj.force = 20;
            obj.powerThrown = true;

        }
        if (obj != null)
        {
            if (obj.force > 0)
            {

                obj.obj.SetActive(obj.powerThrown);
                obj.obj.transform.position += Camera.main.transform.rotation * new Vector3(0, 0, 1);
                obj.force -= Time.deltaTime * 7;
            }
            else
            {
                obj.powerThrown = !obj.powerThrown;
                obj.obj.SetActive(false);
                obj.obj.transform.position = new Vector3(0, 0, 0);
                objQueue.Enqueue(obj);
                obj = null;
                return;
            }
            objQueue.Enqueue(obj);
        }
    }
    private float speed = 20;
    void Update()
    {
        if (obj != null)
        {
            if (obj.force > 0)
            {

                obj.obj.SetActive(obj.powerThrown);
                obj.obj.transform.position += Camera.main.transform.rotation * new Vector3(0, 0, 1);
                obj.force -= Time.deltaTime * 7;
            }
            else
            {
                obj.powerThrown = !obj.powerThrown;
                obj.obj.SetActive(false);
                obj.obj.transform.position = new Vector3(0, 0, 0);
                objQueue.Enqueue(obj);
                obj = null;
                return;
            }
            objQueue.Enqueue(obj);
        }
    }
    void ThrowBall()
    {

    }
}
