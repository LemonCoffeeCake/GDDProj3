using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxRollSpeed = 20f;
    public float rollSpeedDecreaseRate = 5f;
    public float minRollSpeed = 5f;
    public float attackRange = 1f;
    // When attacking, we spawn a circle here to detect enemies
    public Transform attackCenter;
    public LayerMask enemyLayer;
    // The center around which to rotate the weapon
    public Transform weaponCenter;
    // The angle the weapon will rotate when swung
    public float weaponRotateAngle = 100f;
    // How long it takes the player to swing the weapon down
    public float attackFirstHalfDuration = 0.05f;
    // How long it takes the player to recover from swinging the weapon
    public float attackSecondHalfDuration = 0.15f;
    // No attack spamming
    public float attackCoolDown = 0.1f;
    private float rollSpeed;
    private Vector3 movementVector;
    private Vector3 rollVector;
    private Rigidbody2D rb;
    private bool isRolling;
    private bool isAttacking;
    private Quaternion weaponInitialRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponInitialRotation = weaponCenter.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //WASD to move, SPACE to roll, LEFT CLICK to attack.
        if (!isRolling)
        {
            int x = 0;
            int y = 0;
            if (Input.GetKey(KeyCode.W))
            {
                y += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                y -= 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                x -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                x += 1;
            }
            // TODO: link MOVEMENTVECTOR, ISROLLING with animations
            movementVector = new Vector3(x, y, 0f).normalized;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Roll();
            }
            if (Input.GetMouseButtonDown(0) && !isAttacking)
            {
                Attack();
            }
        }
        else
        {
            // The player can't change direction or attack mid-roll. The roll speed drops at
            // a rate of ROLLSPEEDDECREASERATE until it reaches MINROLLSPEED, at which
            // point the player exits the roll and is able to control their direction again.
            rollSpeed -= rollSpeed * rollSpeedDecreaseRate * Time.deltaTime;
            if (rollSpeed < minRollSpeed)
            {
                isRolling = false;

            }
        }
    }

    void FixedUpdate()
    {
        if (!isRolling)
        {
            rb.velocity = movementVector * moveSpeed;
        }
        else
        {
            rb.velocity = rollVector * rollSpeed;
        }
    }

    private void Roll()
    {
        // The player starts a roll with initial speed MAXROLLSPEED in the direction
        // of ROLLVECTOR.
        rollVector = movementVector;
        isRolling = true;
        rollSpeed = maxRollSpeed;
    }

    private void Attack()
    {
        // TODO: connect with the enemy script, make enemy take damage
        // The attack is split into 3 sections: the weapon going down, the weapon going up, and the cooldown
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            print(enemy.gameObject.name + " hit");
            if (enemy.GetComponent<SpriteRenderer>())
            {
                // Add something here
                enemy.GetComponent<SpriteRenderer>().color = new Color(
      Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
        }
        isAttacking = true;
        StartCoroutine(AttackFirstHalf(attackFirstHalfDuration, attackSecondHalfDuration));
    }

    private IEnumerator AttackFirstHalf(float firstTime, float secondTime)
    {
        while (firstTime > 0f)
        {
            firstTime -= Time.deltaTime;
            weaponCenter.Rotate(-Vector3.forward * weaponRotateAngle / attackFirstHalfDuration * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(AttackSecondHalf(secondTime));
    }

    private IEnumerator AttackSecondHalf(float secondTime)
    {
        while (secondTime > 0f)
        {
            secondTime -= Time.deltaTime;
            weaponCenter.Rotate(Vector3.forward * weaponRotateAngle / attackSecondHalfDuration * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(AttackCoolDown(attackCoolDown));
    }

    private IEnumerator AttackCoolDown(float coolDownTime)
    {
        weaponCenter.rotation = weaponInitialRotation;
        while (coolDownTime > 0f)
        {
            coolDownTime -= Time.deltaTime;
            yield return null;
        }
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackCenter.position, attackRange);
    }
}
