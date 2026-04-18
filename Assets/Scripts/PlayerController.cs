using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private float moveInput;

    [Header("Collider Ayarları")]
    public BoxCollider2D bodyCollider;
    public Vector2 standingSize = new Vector2(0.5f, 1.8f);
    public Vector2 standingOffset = new Vector2(0f, 0.9f);
    public Vector2 proneSize = new Vector2(1.8f, 0.4f);
    public Vector2 proneOffset = new Vector2(0f, 0.2f);

    [Header("Vampir Ayarları")]
    public int lifestealAmount = 10;

    [Header("Enerji Sistemi")]
    public float maxEnergy = 100f;
    public float currentEnergy = 0f;
    public float energyPerHit = 25f;
    public Slider energySlider;

    [Header("Savaş Ayarları")]
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attack1Damage = 40;
    public int attack2Damage = 100;

    [Header("Shockwave (İtme) Ayarları")]
    public float shockwaveRadius = 3f;
    public float shockwaveForce = 12f;
    public float shockwaveCooldown = 3f;
    public Slider shockwaveCooldownSlider;
    public ParticleSystem shockwaveEffect;
    private float nextShockwaveTime = 0f;

    private bool isAttacking = false;
    public float attackAnimDuration = 0.4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (bodyCollider == null) bodyCollider = GetComponent<BoxCollider2D>();
        currentEnergy = 0f;
        UpdateEnergyBar();
    }

    void Update()
    {
        UpdateShockwaveUI();

        if (GetComponent<PlayerHealth>().isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextShockwaveTime)
        {
            DoShockwave();
            nextShockwaveTime = Time.time + shockwaveCooldown;
        }

        if (isAttacking) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        if (moveInput > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(1, 1, 1);

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack1();
                nextAttackTime = Time.time + attackCooldown;
            }
            else if (Input.GetMouseButtonDown(1) && currentEnergy >= maxEnergy)
            {
                Attack2();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack1()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Attack1");
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySwordSwing();
        Invoke("ResetAttack", attackAnimDuration);
    }

    void Attack2()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Attack2");
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySwordSwing();
        currentEnergy = 0f;
        UpdateEnergyBar();
        Invoke("ResetAttack", attackAnimDuration);
    }

    void DoShockwave()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, shockwaveRadius, enemyLayers);

        foreach (Collider2D enemy in enemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb == null) continue;

            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyRb.linearVelocity = new Vector2(direction.x * shockwaveForce, shockwaveForce * 0.5f);
        }

        if (shockwaveEffect != null) shockwaveEffect.Play();
        if (SoundManager.Instance != null) SoundManager.Instance.PlayShockwave();

        Debug.Log("Shockwave! İtilen düşman sayısı: " + enemies.Length);
    }

    void UpdateShockwaveUI()
    {
        if (shockwaveCooldownSlider == null) return;

        float remaining = nextShockwaveTime - Time.time;
        if (remaining <= 0f)
        {
            shockwaveCooldownSlider.value = 1f;
        }
        else
        {
            shockwaveCooldownSlider.value = 1f - (remaining / shockwaveCooldown);
        }
    }

    public void HitEvent()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        if (hitEnemies.Length > 0)
        {
            currentEnergy += energyPerHit;
            if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
            UpdateEnergyBar();
            Debug.Log("İsabetli vuruş! Enerji kazanıldı: " + currentEnergy);
        }

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(attack1Damage, transform.position);
        }
    }

    public void SpecialHitEvent()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(attack2Damage, transform.position);
            GetComponent<PlayerHealth>().Heal(lifestealAmount);
        }
    }

    public void SetColliderProne()
    {
        bodyCollider.size = proneSize;
        bodyCollider.offset = proneOffset;
    }

    public void SetColliderStanding()
    {
        bodyCollider.size = standingSize;
        bodyCollider.offset = standingOffset;
    }

    void UseHeal()
    {
        GetComponent<PlayerHealth>().Heal(10);
        currentEnergy = 0f;
        UpdateEnergyBar();
    }

    void ResetAttack() => isAttacking = false;

    void UpdateEnergyBar()
    {
        if (energySlider != null) energySlider.value = currentEnergy / maxEnergy;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
    }
}