using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeEnemyController : EnemyController
{
    #region Editor Variables
    [SerializeField]
    private GameObject attackCenter;

    [SerializeField]
    private float m_AttackArc;
    #endregion

    #region Attacking
    protected override IEnumerator AttackPlayer()
    {
        //if (cr_Player.transform.position.x - transform.position.x < 0)
        //{
        //    spriteR.flipX = true;
        //}
        //else
        //{
        //    spriteR.flipX = false;
        //}
        float elapsed = 0;
        canMove = false;
        anim.SetBool("Attacking", true);
        while (elapsed < m_WindUpTime && !isFrozen)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("Attacking", false);
        Rigidbody2D rbPlayer = cr_Player.GetComponent<Rigidbody2D>();
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter.transform.position, m_Range);
        foreach (Collider2D enemy in hits)
        {
           if (enemy.gameObject.CompareTag("Player"))
            {
                float attackArc = m_AttackArc;
                Vector2 toPlayerAngle = cr_Player.transform.position - transform.position;
                Vector2 toPlayerVec = cr_Player.transform.position - transform.position;
                if (spriteR.flipX == true)
                {
                    toPlayerAngle = new Vector2(-toPlayerAngle.x, toPlayerAngle.y);
                }
                float angle = Mathf.Atan2(toPlayerAngle.y, toPlayerAngle.x) * Mathf.Rad2Deg;
                if (-attackArc <= angle && angle <= attackArc && ((toPlayerVec.x <= 0 && spriteR.flipX == true) || (toPlayerVec.x >= 0 && spriteR.flipX == false)))
                {
                    enemy.gameObject.GetComponent<PlayerController>().TakeDamage(m_Damage);
                }
            }
        }
        canAttack = false;
        audioSource.PlayOneShot(attackSound);
        StartCoroutine(AttackCooldown());
    }
    #endregion
}
