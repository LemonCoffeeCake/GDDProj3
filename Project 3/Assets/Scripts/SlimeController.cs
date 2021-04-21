using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : EnemyController
{
    public int size;

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
        if (other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerController>().TakeDamage(m_Damage);
        }
    }

    protected override void Death(){
        if(size > 1) {
            GameObject child1 = Instantiate(gameObject, gameObject.transform.position + Vector3.left * 1.0f, Quaternion.identity);
            GameObject child2 = Instantiate(gameObject, gameObject.transform.position + Vector3.right * 1.0f, Quaternion.identity);
            child1.GetComponent<SlimeController>().size -= 1;
            child2.GetComponent<SlimeController>().size -= 1;
        }
        Destroy(gameObject);
    }
}
