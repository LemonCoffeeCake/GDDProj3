using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeEnemyController : EnemyController
{
    #region Editor Variables
    [SerializeField]
    private GameObject attackCenter;
    #endregion

    #region Attacking
    protected override IEnumerator AttackPlayer()
    {
        if (cr_Player.transform.position.x - transform.position.x < 0)
        {
            spriteR.flipX = true;
        }
        else
        {
            spriteR.flipX = false;
        }
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
                enemy.gameObject.GetComponent<PlayerController>().TakeDamage(m_Damage);
            }
        }
        canAttack = false;
        StartCoroutine(AttackCooldown());
    }
    #endregion
}
