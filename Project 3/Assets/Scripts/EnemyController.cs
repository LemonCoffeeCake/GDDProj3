using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    protected float m_Health;

    [SerializeField]
    protected int m_Damage;

    [SerializeField]
    protected float m_MoveSpeed;

    [SerializeField]
    protected float m_Range;

    [SerializeField]
    protected float m_AttackCooldown;

    [SerializeField]
    protected float m_WindUpTime;

    [SerializeField]
    protected ItemPickup[] m_drops;

    [SerializeField]
    protected float m_dropRate;

    [SerializeField]
    protected float m_dropMinVal;

    [SerializeField]
    protected float m_dropMaxVal;

    [SerializeField]
    protected Color frozenColor;

    [SerializeField]
    protected Color poisonedColor;

    [SerializeField]
    protected AudioClip attackSound;


    [SerializeField]
    protected AudioClip hurtSound;

    [SerializeField]
    protected AudioClip deathSound;

    #endregion

    #region Private Variables
    protected float maxHealth;
    protected Rigidbody2D rb;
    protected bool canAttack;
    protected bool canMove;
    protected float distToPlayer;
    protected Color defaultCol;
    protected NavMeshAgent agent;
    protected Animator anim;
    protected SpriteRenderer spriteR;
    protected bool isFrozen;
    protected bool isPoisoned;
    protected Coroutine frozenIE;
    protected Coroutine poisonIE;
    protected Coroutine updatePoisonIE;
    protected AudioSource audioSource;
    #endregion

    #region Cached Region
    protected GameObject cr_Player;
    protected PlayerController playerController;
    #endregion

    #region Initialization
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultCol = GetComponent<SpriteRenderer>().color;
        canAttack = true;
        canMove = true;
        maxHealth = m_Health;
    }

    private void Start()
    {
        cr_Player = GameObject.FindWithTag("Player");
        playerController = cr_Player.GetComponent<PlayerController>();
        distToPlayer = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = m_MoveSpeed;
        agent.stoppingDistance = m_Range;
        anim = GetComponent<Animator>();
        spriteR = GetComponent<SpriteRenderer>();
        frozenColor = new Color(frozenColor.r, frozenColor.g, frozenColor.b, 255);
        poisonedColor = new Color(poisonedColor.r, poisonedColor.g, poisonedColor.b, 255);
        audioSource = GetComponent<AudioSource>();
    }
    #endregion

    #region Updates
    private void FixedUpdate()
    {
        if (!isFrozen)
        {
            distToPlayer = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
            if (canMove && distToPlayer > m_Range)
            {
                if (cr_Player.transform.position.x - transform.position.x < 0)
                {
                    spriteR.flipX = true;
                }
                else
                {
                    spriteR.flipX = false;
                }
                NavMeshMove();
            }
            else if (canAttack && distToPlayer <= m_Range)
            {
                canAttack = false;
                StartCoroutine(AttackPlayer());
            }
        }
    }
    #endregion

    #region Movement
    protected virtual void NavMeshMove()
    {
        Vector2 playerPos = cr_Player.transform.position;
        agent.SetDestination(playerPos);
    }
    #endregion

    #region Health and Death

    public virtual void TakeDamage(float amount)
    {
        float f = Random.Range(0.0f, 1.0f);
        if (f < 0.5f)
        {
            audioSource.PlayOneShot(hurtSound);
        }
        else
        {
            audioSource.PlayOneShot(deathSound);
        }
        m_Health -= amount;
        if (m_Health <= 0)
        {
            Death();
        }
    }

    protected virtual void Death()
    {
        DropItem();
        Destroy(gameObject);
    }

    private void DropItem()
    {
        // TODO: add RNG
        if (m_drops.Length != 0)
        {
            float isDrop = Random.Range(0f, 100f);
            if (isDrop < m_dropRate)
            {
                int index = Random.Range(0, m_drops.Length);
                ItemPickup drop = Instantiate(m_drops[index], transform.position, Quaternion.identity);
            }
        }
    }
    #endregion

    #region Attacking
    private bool attackPathClear()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, cr_Player.transform.position);
        foreach (RaycastHit2D obj in hits)
        {
            if (obj.collider.gameObject.CompareTag("Obstacle")) {
                return false;
            }
        }
        return true;
    }

    protected virtual IEnumerator AttackPlayer()
    {
        float elapsed = 0;
        canMove = false;
        anim.SetBool("Attacking", true);
        while (elapsed < m_WindUpTime && !isFrozen)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("Attacking", false);
        Vector2 attackVector = new Vector2(m_Range, 0);
        if (cr_Player.transform.position.x - transform.position.x < 0)
        {
            attackVector = -1 * attackVector;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, attackVector, m_Range, LayerMask.GetMask("Player"));
        if (hit == true && hit.collider.CompareTag("Player"))
        {
            cr_Player.GetComponent<PlayerController>().TakeDamage(m_Damage);
        }
        canAttack = false;
        audioSource.PlayOneShot(attackSound);
        StartCoroutine(AttackCooldown());
    }

    protected IEnumerator AttackCooldown()
    {
        float elapsed = 0;
        while (elapsed < m_AttackCooldown && !isFrozen)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        canAttack = true;
        canMove = true;
    }
    #endregion

    #region Collison
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
    #endregion

    public void ApplyPoison(float duration)
    {
        if (poisonIE == null)
        {
            updatePoisonIE = StartCoroutine(UpdatePoisonBool(duration));
            poisonIE = StartCoroutine(Poison(duration));
        }
        else
        {
            StopCoroutine(poisonIE);
            StopCoroutine(updatePoisonIE);
            updatePoisonIE = StartCoroutine(UpdatePoisonBool(duration));
            poisonIE = StartCoroutine(Poison(duration));
        }
    }

    public void ApplyIce(float duration)
    {
        if (frozenIE == null)
        {
            frozenIE = StartCoroutine(Freeze(duration));
        }
        else
        {
            StopCoroutine(frozenIE);
            frozenIE = StartCoroutine(Freeze(duration));
        }
    }

    private IEnumerator Freeze(float duration)
    {
        isFrozen = true;
        spriteR.color = frozenColor;
        yield return new WaitForSeconds(duration * 5f);
        isFrozen = false;
        spriteR.color = defaultCol;
    }

    private IEnumerator Poison(float duration)
    {
        while(isPoisoned)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(1f);
        }
    }

    private IEnumerator UpdatePoisonBool(float duration)
    {
        isPoisoned = true;
        spriteR.color = poisonedColor;
        yield return new WaitForSeconds(duration * 5f);
        isPoisoned = false;
        spriteR.color = defaultCol;
    }
}
