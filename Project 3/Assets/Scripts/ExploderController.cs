using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderController : EnemyController
{
    private void FixedUpdate(){
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
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerController>().TakeDamage(m_Damage);
            TakeDamage(m_Health);
            audioSource.PlayOneShot(attackSound);
        }
    }
}
