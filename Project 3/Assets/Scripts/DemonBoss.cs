using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBoss : EnemyController
{
    #region Editor Variables
    [SerializeField]
    private int m_SlamDamage;

    [SerializeField]
    private int m_SlamRange;

    [SerializeField]
    private int m_SlamCooldown;

    [SerializeField]
    private int m_SlamTime;
    #endregion

    #region Private Variables
    private bool canSlam;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultCol = GetComponent<SpriteRenderer>().color;
        canAttack = true;
        canMove = true;
        canSlam = true;
    }

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
            } else if (distToPlayer <= m_SlamRange && canSlam)
            {
                canSlam = false;
                StartCoroutine(Slam());
            } else if (canAttack && distToPlayer <= m_Range)
            {
                canAttack = false;
                StartCoroutine(AttackPlayer());
            }
        }
    }
    #endregion

    #region Attacking
    private IEnumerator Slam()
    {
        isFrozen = true;
        agent.speed = 0;
        float elapsed = 0;
        canMove = false;
        while (elapsed < m_SlamRange && !isFrozen)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = transform.position + new Vector3(0, 2, 0);
        elapsed = 0;
        while (elapsed < m_SlamTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = transform.position - new Vector3(0, 2, 0);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, m_SlamRange);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.CompareTag("Player"))
            {
                cr_Player.GetComponent<PlayerController>().TakeDamage(m_SlamDamage);
            }
        }
        elapsed = 0;
        while (elapsed < 2.0f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        isFrozen = false;
        agent.speed = m_MoveSpeed;
        StartCoroutine(SlamCooldown());
    }

    private IEnumerator SlamCooldown()
    {
        float elapsed = 0;
        while (elapsed < m_SlamCooldown)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        canSlam = true;
    }
    #endregion
}
