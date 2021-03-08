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
    private float m_AttackTimer;
    #endregion

    #region Private Variables
    private Rigidbody2D rb;
    private float prevAttackTime;
    private bool canAttack;
    #endregion

    #region Cached Region
    private GameObject cr_Player;
    #endregion

    #region Initialization
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        canAttack = true;
    }

    private void Start()
    {
        cr_Player = GameObject.FindWithTag("Player");
    }
    #endregion

    #region Updates
    private void FixedUpdate()
    {
        float dist = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
        if (dist > m_Range)
        {
            Move();
        } else if (dist <= m_Range && canAttack)
        {
            AttackPlayer();
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

    public void TakeDamage(int amount)
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
    private void AttackPlayer()
    {
        GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        cr_Player.GetComponent<PlayerController>().TakeDamage(m_Damage);
        prevAttackTime = m_AttackTimer;
        canAttack = false;
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        while (prevAttackTime > 0)
        {
            prevAttackTime -= Time.deltaTime;
            yield return null;
        }
        canAttack = true;
    }
    #endregion

    #region Collison
    public void OnCollisionStay2D(Collision2D collision)
    {

    }
    #endregion

}
