using UnityEngine;

public class TriggerCast : MonoBehaviour
{
    public Animator anim;
    public AudioSource audioSource;
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 1 << 9))
        {
            anim.SetTrigger("Scream");
            if (!audioSource.isPlaying)
                audioSource.Play();
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
}
