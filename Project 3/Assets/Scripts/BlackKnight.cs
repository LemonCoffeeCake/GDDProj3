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

    [SerializeField]
    private int m_SpeedBoost;

    [SerializeField]
    private int m_SpeedBoostTime;

    [SerializeField]
    private int m_SpeedBoostCooldown;

    [SerializeField]
    private float m_FirstPhaseDamageReduction;

    [SerializeField]
    private float m_SecondPhaseDamageReduction;

    [SerializeField]
    private float m_SecondPhaseSpeed;

    [SerializeField]
    private int m_SecondPhaseDamage;

    [SerializeField]
    private RectTransform hpBar;
    #endregion

    #region Private Variables
    private bool isCharging;
    private bool canCharge;
    private bool canBoost;
    private bool phaseTwo;
    private Vector3 chargeDirection;
    private Vector3 chargeDestination;
    private float damageReduction;
    private float moveSpeed;
    private float hpInitialWidth;
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
        canBoost = true;
        phaseTwo = false;
        maxHealth = m_Health;
        damageReduction = m_FirstPhaseDamageReduction;
        moveSpeed = m_MoveSpeed;
        hpInitialWidth = hpBar.sizeDelta.x;
    }
    #endregion

    #region Updates
    private void FixedUpdate()
    {
        if (!isFrozen)
        {
            distToPlayer = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (cr_Player.transform.position - transform.position).normalized, 500f, LayerMask.GetMask("Player"));
            if (m_Health <= maxHealth / 2 && !phaseTwo)
            {
                enterPhaseTwo();
            }  
            if (canBoost && !isCharging)
            {
                canBoost = false;
                activateSpeedBoost();
            }
            else if (isCharging)
            {
                Charge();
            }
            else if (canMove && canCharge && hit.collider.gameObject.CompareTag("Player"))
            {
                chargeDirection = (cr_Player.transform.position - transform.position).normalized;
                chargeDestination = cr_Player.transform.position;
                isCharging = true;
                canCharge = false;
                if (cr_Player.transform.position.x - transform.position.x < 0)
                {
                    spriteR.flipX = true;
                }
                else
                {
                    spriteR.flipX = false;
                }
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

    #region Phase Change
    private void enterPhaseTwo()
    {
        damageReduction = m_SecondPhaseDamageReduction;
        moveSpeed = m_SecondPhaseSpeed;
        agent.speed = moveSpeed;
        m_Damage = m_SecondPhaseDamage;
        phaseTwo = true;
        defaultCol = Color.yellow;
    }
    #endregion

    #region Health
    public override void TakeDamage(float amount)
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
        m_Health -= (amount * damageReduction);
        UpdateHealth(m_Health / maxHealth);
        if (m_Health <= 0)
        {
            Death();
        }
    }

    public void UpdateHealth(float percent)
    {
        hpBar.sizeDelta = new Vector2(hpInitialWidth * percent, hpBar.sizeDelta.y);
    }
    #endregion

    #region Movement
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
        transform.position = Vector2.MoveTowards(transform.position, chargeDestination, m_ChargeSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, cr_Player.transform.position) < 0.01f)
        {
            cr_Player.GetComponent<PlayerController>().TakeDamage(m_ChargeDamage);
            isCharging = false;
            StartCoroutine(ChargeCooldown());
            return;
        }
        if (Vector2.Distance(transform.position, chargeDestination) < 0.01f)
        {
            isCharging = false;
            StartCoroutine(ChargeCooldown());
            return;
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

    private void activateSpeedBoost()
    {
        moveSpeed += m_SpeedBoost;
        m_ChargeSpeed += m_SpeedBoost;
        GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(boostCooldown());
    }

    private IEnumerator boostCooldown()
    {
        float elapsed = 0;
        while (elapsed < m_SpeedBoostTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        moveSpeed -= m_SpeedBoost;
        m_ChargeSpeed -= m_SpeedBoost;
        GetComponent<SpriteRenderer>().color = defaultCol;
        elapsed = 0;
        while (elapsed < m_SpeedBoostCooldown)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        canBoost = true;
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
