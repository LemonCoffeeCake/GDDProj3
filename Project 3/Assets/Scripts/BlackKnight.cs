using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackKnight : EnemyController
{
    #region Editor Variables
    [SerializeField]
    private float m_ChargeSpeed;

    [SerializeField]
    private float m_ChargeCooldown;

    [SerializeField]
    private int m_ChargeDamage;
    #endregion

    #region Private Variables
    private bool isCharging;
    private bool canCharge;
    private Vector3 chargeDirection;
    private Vector3 chargeDestination;
    #endregion

    #region Awake
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultCol = GetComponent<SpriteRenderer>().color;
        canAttack = true;
        canMove = true;
        isCharging = false;
        canCharge = true;
    }
    #endregion

    #region Updates
    private void FixedUpdate()
    {
        if (!isFrozen)
        {
            distToPlayer = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (cr_Player.transform.position - transform.position).normalized, 500f, LayerMask.GetMask("Player"));
            if (canMove && canCharge && hit.collider.gameObject.CompareTag("Player"))
            {
                chargeDirection = (cr_Player.transform.position - transform.position).normalized;
                chargeDestination = cr_Player.transform.position;
                isCharging = true;
                canCharge = false;
                agent.speed = m_ChargeSpeed;
                Charge();
            } else if (isCharging)
            {
                Charge();
            } else if (canMove && distToPlayer > m_Range)
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

    #region Charge
    private void Charge()
    {
        //transform.position += chargeDirection * m_ChargeSpeed * Time.deltaTime;
        //transform.position = Vector2.MoveTowards(transform.position, chargeDestination, m_ChargeSpeed * Time.deltaTime);
        //distToPlayer = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
        //if (distToPlayer < m_Range)
        //{
        //    isCharging = false;
        //    StartCoroutine(ChargeCooldown());
        //}
        agent.SetDestination(chargeDestination);
        if (Vector2.Distance(transform.position, cr_Player.transform.position) < 0.001f)
        {
            cr_Player.GetComponent<PlayerController>().TakeDamage(m_ChargeDamage);
        }
        if (Vector2.Distance(transform.position, chargeDestination) < 0.001f)
        {
            isCharging = false;
            agent.speed = m_MoveSpeed;
            StartCoroutine(ChargeCooldown());
        }
    }

    private IEnumerator ChargeCooldown()
    {
        float elasped = 0;
        while (elasped < m_ChargeCooldown)
        {
            elasped += Time.deltaTime;
            yield return null;
        }
        canCharge = true;
    }
    #endregion

    #region Collision
    /** Currently not in use */
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {   
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
    #endregion
}
