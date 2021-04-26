using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : EnemyController
{
    #region Editor Variables

    [SerializeField]
    private GameObject arrow;

    [SerializeField]
    private GameObject arrowSpawnPoint;
    #endregion

    #region Attacking
    protected override IEnumerator AttackPlayer()
    {
        if (cr_Player.transform.position.x - transform.position.x < 0)
        {
            spriteR.flipX = true;
        } else
        {
            spriteR.flipX = false;
        }
        float elapsed = 0;
        canMove = false;
        anim.SetBool("Attacking", true);
        while (elapsed < m_WindUpTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        anim.SetBool("Attacking", false);

        Rigidbody2D rbPlayer = cr_Player.GetComponent<Rigidbody2D>();

        float playerX = cr_Player.transform.position.x;
        float playerY = cr_Player.transform.position.y;
        float playerVeloX = rbPlayer.velocity.x;
        float playerVeloY = rbPlayer.velocity.y;

        float a = Mathf.Pow(playerVeloX, 2) + Mathf.Pow(playerVeloY, 2) - Mathf.Pow(arrow.GetComponent<BasicArrow>().getSpeed(), 2);
        float b = (2 * playerX * playerVeloX) - (2 * playerX * playerVeloX) +
            (2 * playerY * playerVeloY) - (2 * playerY * playerVeloY);
        float c = Mathf.Pow(playerX, 2) - (2 * playerX * transform.position.x) + Mathf.Pow(transform.position.x, 2) +
            Mathf.Pow(playerY, 2) - (2 * playerY * transform.position.y) + Mathf.Pow(transform.position.y, 2);
        float t1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - (4 * a * c))) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - (4 * a * c))) / (2 * a);
        float tActual = Mathf.Max(t1, t2);
        float X = tActual * rbPlayer.velocity.x + cr_Player.transform.position.x;
        float Y = tActual * rbPlayer.velocity.y + cr_Player.transform.position.y;

        if (float.IsNaN(X) || float.IsNaN(Y))
        {
            X = cr_Player.transform.position.x;
            Y = cr_Player.transform.position.y;
        }

        GameObject projectile = Instantiate(arrow, arrowSpawnPoint.transform.position, Quaternion.identity);
        projectile.GetComponent<BasicArrow>().setup((new Vector3(X, Y, 0) - transform.position).normalized);
        
        // Destroy(projectile, 2.0f);
        
        canAttack = false;
        audioSource.PlayOneShot(attackSound);
        StartCoroutine(AttackCooldown());
    }
    #endregion
}
