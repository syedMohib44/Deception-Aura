using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterMovement : MonoBehaviour
{
    private bool reached = false;
    private Vector3 pos1, pos2, p1, p2;
    RaycastHit hit;
    CapsuleCollider cc;
    private float randomAttackTime, attackTimer, randomMoveTime, moveTimer;
    private int attackSeconds = 0, moveSeconds = 0;
    public GameObject sphere, hands;
    public PlayerMovement playerMovement;
    private Vector3[] randomPos;
    int randomPosIndex;
    public Collider sword;
    private AudioSource magicEffect;
    public Slider healthBar;
    public bool IsDead { get; set; }
    private Animator enemyAnim;
    public TextMeshProUGUI GameOverText;
    public Image img;
    void Start()
    {
        enemyAnim = gameObject.transform.Find("peasant_girl").GetComponent<Animator>();
        randomPosIndex = Random.Range(0, 3);
        randomPos = new Vector3[] { new Vector3(0.2f, -3.0f, 7.5f), new Vector3(5, -3.0f, 9.0f), new Vector3(-3.0f, -3.0f, 6.0f) };
        pos1 = transform.position;
        pos2 = new Vector3(5, 0, 0);
        randomAttackTime = Random.Range(0.0f, 5.0f);
        randomMoveTime = Random.Range(20.0f, 30.0f);
        magicEffect = gameObject.GetComponent<AudioSource>();
        cc = gameObject.GetComponent<CapsuleCollider>();
    }
    GameObject obj;
    void Update()
    {
        attackTimer += Time.deltaTime;
        moveTimer += Time.deltaTime;
        // turn float seonds to int
        moveSeconds = (int)(attackTimer % 60);
        attackSeconds = (int)(attackTimer % 60);
        if (randomAttackTime < attackSeconds)
        {
            attackTimer = 0;
            gameObject.transform.Find("peasant_girl").GetComponent<Animator>().SetTrigger("Attack");
            if (magicEffect)
                magicEffect.Play();
            randomAttackTime = Random.Range(5.0f, 10.0f);
        }
        if (enemyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f &&
            enemyAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack_State"))
        {
            if (obj == null)
                obj = Instantiate(sphere, hands.transform.position, Quaternion.identity);

            Destroy(obj, 2);
        }

        if (obj != null)
            obj.transform.position -= gameObject.transform.rotation * new Vector3(0, 0, 1); //Vector3.MoveTowards(obj.transform.position, new Vector3(tempPos.x, tempPos.y, Camera.main.transform.position.z), 1);//;

        Vector3 direction = Camera.main.transform.position + this.transform.position;
        direction.y = 0;
        direction.x = -direction.x;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);

        p1 = transform.position + cc.center + Vector3.forward * -cc.height * 0.5F;
        p2 = p1 + Vector3.up * cc.height;
        if (Physics.CapsuleCast(p1, p2, cc.radius, Vector3.forward, out hit, 1.5f))
        {
            if (hit.collider.tag == "Magic")
            {
                healthBar.value -= 0.1f;
                gameObject.transform.Find("peasant_girl").GetComponent<Animator>().SetTrigger("HitByMagic");
            }
        }
        if (cc.bounds.Intersects(playerMovement.sword.GetComponent<BoxCollider>().bounds) && playerMovement.enemyHitted)
        {
            IsDead = true;
            enemyAnim.SetTrigger("HitByMagic");
            healthBar.value -= 0.05f;
        }

        if (healthBar.value <= 0.0f)
        {
            IsDead = true;
            enemyAnim.SetBool("Dead", true);
            GameOverText.SetText("You Win!");
            if (enemyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                img.color += new Color(0, 0, 0, 0.8f) * Time.deltaTime;
                GameOverText.alpha += 0.3f * Time.deltaTime;
                if (img.color.a >= 1.0f && GameOverText.color.a >= 1.0f)
                    SceneManager.LoadScene(0);
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
