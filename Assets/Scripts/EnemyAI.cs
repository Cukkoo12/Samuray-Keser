using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stoppingDistance = 1.2f;
    public float attackRate = 1.5f;  
    private float nextAttackTime = 0f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stoppingDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopAndAttack();
        }
    }

    void MoveTowardsPlayer()
    {
        float direction = player.position.x - transform.position.x;
        rb.linearVelocity = new Vector2(Mathf.Sign(direction) * moveSpeed, rb.linearVelocity.y);
        transform.localScale = new Vector3(Mathf.Sign(direction) > 0 ? 1 : -1, 1, 1);
        anim.SetFloat("Speed", 1f);
    }

    void StopAndAttack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        anim.SetFloat("Speed", 0f);

        if (Time.time >= nextAttackTime)
        {
            anim.SetTrigger("Attack");
            nextAttackTime = Time.time + attackRate;
        }
    }
    public void EnemyHitEvent()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log("Saldırı anında mesafe: " + distance);
        if (distance <= stoppingDistance + 1.0f)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(4);
        }
    }
}