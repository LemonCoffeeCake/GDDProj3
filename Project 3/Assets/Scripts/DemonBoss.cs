using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonBoss : ArcherController
{
    #region Editor Variables
    [SerializeField]
    private AudioClip m_JumpSound;

    [SerializeField]
    private AudioClip m_SlamSound;

    [SerializeField]
    private int m_SlamDamage;

    [SerializeField]
    private int m_SlamRange;

    [SerializeField]
    private int m_SlamCooldown;

    [SerializeField]
    private int m_SlamTime;

    [SerializeField]
    private int m_DamageBoost;

    [SerializeField]
    private int m_DamageBoostTime;

    [SerializeField]
    private int m_DamageBoostCooldown;

    [SerializeField]
    private RectTransform hpBar;
    #endregion

    #region Private Variables
    private bool canSlam;
    private bool canBoost;
    private float hpInitialWidth;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultCol = GetComponent<SpriteRenderer>().color;
        canAttack = true;
        canMove = true;
        canSlam = true;
        canBoost = true;
        maxHealth = m_Health;
        hpInitialWidth = hpBar.sizeDelta.x;
    }

    #region Updates
    private void FixedUpdate()
    {
        if (!isFrozen)
        {
            distToPlayer = Mathf.Abs(Vector2.Distance(cr_Player.transform.position, transform.position));
            if (canBoost)
            {
                canBoost = false;
                activateAttackBoost();
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
        m_Health -= amount;
        UpdateHealth(m_Health / maxHealth);
        if (m_Health <= 0)
        {
            Death();
        }
    }

    public void UpdateHealth(float percent)
    {
        print(percent);
        hpBar.sizeDelta = new Vector2(hpInitialWidth * percent, hpBar.sizeDelta.y);
    }

    #region Attacking
    private IEnumerator Slam()
    {
        isFrozen = true;
        agent.speed = 0;
        canMove = false;
        canAttack = false;
        anim.SetBool("Slamming", true);
        float elapsed = 0;
        audioSource.PlayOneShot(m_JumpSound);
        transform.position = transform.position + new Vector3(0, 2, 0);
        while (elapsed < m_SlamTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = transform.position - new Vector3(0, 2, 0);
        audioSource.PlayOneShot(m_SlamSound);
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
        anim.SetBool("Slamming", false);
        isFrozen = false;
        canMove = true;
        canAttack = true;
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

    private void activateAttackBoost()
    {
        m_Damage += m_DamageBoost;
        m_SlamDamage += m_DamageBoost;
        anim.SetBool("Buffing", true);
        StartCoroutine(boostCooldown());
    }

    private IEnumerator boostCooldown()
    {
        float elapsed = 0;
        while (elapsed < m_DamageBoostTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("Buffing", false);
        elapsed = 0;
        m_Damage -= m_DamageBoost;
        m_SlamDamage -= m_DamageBoost;
        while (elapsed < m_DamageBoostCooldown)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        canBoost = true;
    }
    #endregion
}
