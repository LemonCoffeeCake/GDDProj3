using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    private float m_Health;

    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private float m_MoveSpeed;

    [SerializeField]
    private float m_Range;

    [SerializeField]
    private float m_AttackCooldown;

    [SerializeField]
    private float m_WindUpTime;
    #endregion

    #region Private Variables
    private Rigidbody2D rb;
    private float prevAttackTime;
    private bool canAttack;
    private bool canMove;
    private float distToPlayer;
    private Color defaultCol;
    #endregion

    #region Cached Region
    private GameObject cr_Player;
    #endregion

    #region Initialization
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultCol = GetComponent<SpriteRenderer>().color;
        canAttack = true;
        canMove = true;
    }

    private void Start()
    {
        cr_Player = GameObject.FindWithTag("Player");
        distToPlayer = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
    }
    #endregion

    #region Updates
    private void FixedUpdate()
    {
        distToPlayer = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
        if (canMove && distToPlayer > m_Range)
        {
            Move();
        }
        else if (canAttack && distToPlayer <= m_Range)
        {
            canAttack = false;
            StartCoroutine(AttackPlayer());
        }
    }
    #endregion

    #region Movement
    private void Move()
    {
        // transform.position = Vector2.MoveTowards(cr_Player.transform.position, transform.position, m_MoveSpeed * Time.deltaTime);
        Vector2 dir = cr_Player.transform.position - transform.position;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rb.rotation = ang;
        dir.Normalize();
        Movement(dir);

    }

    private void Movement(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * m_MoveSpeed * Time.deltaTime));
    }
    #endregion

    #region Health and Death

    public void TakeDamage(float amount)
    {
        m_Health -= amount;
        if (m_Health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Attacking
    private IEnumerator AttackPlayer()
    {
        Color startingCol = defaultCol;
        float elapsed = 0;
        canMove = false;
        while (GetComponent<SpriteRenderer>().color != Color.black)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(startingCol, Color.black, elapsed/m_WindUpTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Vector2 toPlayer = cr_Player.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer, m_Range, LayerMask.GetMask("Player"));
        if (hit == true && hit.collider.CompareTag("Player"))
        {
            cr_Player.GetComponent<PlayerController>().TakeDamage(m_Damage);
        }
        canAttack = false;
        // GetComponent<SpriteRenderer>().color = Color.black;
        StartCoroutine(AttackCooldown(m_AttackCooldown));
    }

    private IEnumerator AttackCooldown(float time)
    {
        Color col = GetComponent<SpriteRenderer>().color;
        float elapsed = 0;
        while (GetComponent<SpriteRenderer>().color != defaultCol)
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(col, defaultCol, elapsed/time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canAttack = true;
        canMove = true;
    }
    #endregion

    #region Collison
    public void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player"))
        {
            
        }
    }
    #endregion

}
