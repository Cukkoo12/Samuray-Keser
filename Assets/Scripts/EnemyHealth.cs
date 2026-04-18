using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator anim;
    private bool isDead = false;
    public float knockbackForce = 6f;
    private Rigidbody2D rb;

    public static event System.Action OnAnyEnemyDied;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 attackerPos)
    {
        if (isDead) return;

        currentHealth -= damage;

        Vector2 direction = (Vector2)transform.position - attackerPos;
        rb.linearVelocity = new Vector2(direction.normalized.x * knockbackForce, 2f);

        if (currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
            if (SoundManager.Instance != null) SoundManager.Instance.PlayEnemyHit();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Kritik: Die() fonksiyonu çalıştı!");

        anim.Play("Enemy_Death");

        if (SoundManager.Instance != null) SoundManager.Instance.PlayDeath();

        GetComponent<Collider2D>().enabled = false;
        GetComponent<EnemyAI>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        OnAnyEnemyDied?.Invoke();

        Destroy(gameObject, 3f);
    }
}