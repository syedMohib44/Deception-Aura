using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private const float SPEED = 30.0f;
    private Vector3 tempPos, p1, p2;
    float slowdownAmount = 1.0f;
    float power = 0.2f;
    float duration = 0;
    float initialDuration = 0;
    RaycastHit hit;
    CapsuleCollider cc;
    public Slider healthBar;
    public GameObject sword;
    public bool IsDead { get; private set; }
    public GameObject switchTOn, switchTOff, slashButton;
    private bool switchTouch = false, swordSwing = false;
    public bool enemyHitted { get; private set; }
    private Vector3 dir;
    private float deathTimer;
    private int deathSeconds;
    public TextMeshProUGUI GameOverText;
    public Image img;


    void Start()
    {
        gameObject.transform.Find("Player").transform.Find("AirParticles").gameObject.SetActive(false);
        dir = Vector3.zero;
        switchTOn.GetComponent<Button>().onClick.AddListener(SwitchOff);
        switchTOff.GetComponent<Button>().onClick.AddListener(SwitchOn);
        slashButton.GetComponent<Button>().onClick.AddListener(Slash);
        healthBar.value = 1;
        cc = gameObject.transform.Find("Player").GetComponent<CapsuleCollider>();
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
    void Slash()
    {
        dir.z = 1.0f;
        gameObject.transform.Find("Player").transform.Find("AirParticles").gameObject.SetActive(true);
        enemyHitted = true;
        swordSwing = true;
    }
    void FixedUpdate()
    {
        if (swordSwing)
        {
            sword.transform.Rotate(250 * Time.deltaTime, 200 * Time.deltaTime, 100 * Time.deltaTime, Space.Self);
            if (sword.transform.rotation.eulerAngles.z >= 280)
                swordSwing = false;
        }
        else
        {
            Vector3 rotAngle = new Vector3(Camera.main.transform.eulerAngles.x - 160.0f, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
            Quaternion quaternionAngle = Quaternion.Euler(rotAngle);
            sword.transform.rotation = Quaternion.Slerp(sword.transform.rotation, quaternionAngle, 3 * Time.deltaTime);
        }

        if (transform.parent.position.z >= 8)
        {
            dir.z = -1.0f;
            enemyHitted = false;
        }
        else if (transform.parent.position.z <= 0 && !enemyHitted)
        {
            gameObject.transform.Find("Player").transform.Find("AirParticles").gameObject.SetActive(false);
            dir.z = 0;
        }

        sword.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.8f, 0.4f, Camera.main.nearClipPlane * 3.5f));
        transform.parent.Translate(new Vector3(0, -dir.z, 0) * SPEED * Time.deltaTime);

        p1 = transform.position + cc.center + Vector3.forward * -cc.height * 0.5f;
        p2 = p1 + Vector3.up * cc.height;
        if (Physics.CapsuleCast(p1, p2, cc.radius, Vector3.forward, out hit, 1.5f))
        {
            if (hit.collider.tag == "EnemyAttack")
            {
                duration = 1.0f;
                tempPos = gameObject.transform.localPosition;
                healthBar.value -= 0.1f;
            }
        }
        if (duration > 0)
        {
            gameObject.transform.localPosition = gameObject.transform.localPosition + Random.insideUnitSphere * power;
            duration -= Time.deltaTime * slowdownAmount;
        }
        else
        {
            initialDuration = 0;
            gameObject.transform.localPosition = tempPos;
        }
        if (healthBar.value <= 0.0f)
        {
            switchTOff.transform.parent.gameObject.SetActive(false);
            IsDead = true;
            GameOverText.SetText("You Lose!");
            sword.SetActive(false);
            gameObject.transform.parent.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0, 1, 1, -1), Time.deltaTime);
            if (IsDead)
            {
                deathTimer += Time.deltaTime;
                deathSeconds = (int)(deathTimer % 60);
                if (deathSeconds > 2)
                {
                    img.color += new Color(0, 0, 0, 0.8f) * Time.deltaTime;
                    GameOverText.alpha += 0.3f * Time.deltaTime;
                    if (img.color.a >= 1.0f && GameOverText.color.a >= 1.0f)
                        SceneManager.LoadScene(0);
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(p1, 1.5f);
        Gizmos.DrawSphere(p2, 1.5f);
    }
}

