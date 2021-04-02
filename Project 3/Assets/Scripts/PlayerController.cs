using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float maxRollSpeed = 20f;
    public float rollSpeedDecreaseRate = 5f;
    public float minRollSpeed = 5f;
    public float attackRange = 1f;
    // When attacking, we spawn a circle here to detect enemies
    public Transform attackCenter;
    public LayerMask enemyLayer;
    // The center around which to rotate the weapon
    public Transform weaponCenter;
    // The angle the weapon will raise before swinging
    public float weaponRaiseAngle;
    // The angle the weapon will rotate when swung
    public float weaponSwingAngle;
    // How long it takes the player to raise the weapon
    public float attackFirstHalfDuration = 0.05f;
    // How long it takes the player to swing the weapon
    public float attackSecondHalfDuration = 0.15f;
    // No attack spamming
    public float attackCoolDown = 0.1f;
    private float rollSpeed;
    private Vector3 lookDir;
    private Vector3 movementVector;
    private Vector3 rollVector;
    private Rigidbody2D rb;
    private bool isRolling;
    private bool isAttacking;
    private Quaternion weaponInitialRotation;
    private float initialXScale;

    public int maxHealth = 10;
    private int health;
    public float maxStamina = 100;
    private float stamina;
    private float staminaRecoveryTimer;
    private float timeSinceLastAction;
    public float timeUntilStaminaRecovers = 1f;
    public float staminaRecoveryPerSecond = 20f;
    public float rollStaminaCost = 20f;

    public Stat damage;
    public Stat speed;

    [SerializeField]
    private HudController m_HUD;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponInitialRotation = weaponCenter.rotation;
        health = maxHealth;
        m_HUD.UpdateHealth(health);
        stamina = maxStamina;
        m_HUD.UpdateStamina(1f);
        initialXScale = transform.localScale.x;
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
            if (!isAttacking)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                lookDir = (mousePos - transform.position).normalized;
                if (lookDir.x < 0f)
                {
                    transform.localScale = new Vector3(-initialXScale, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(initialXScale, transform.localScale.y, transform.localScale.z);
                }
                RotateWeaponToMousePosition(false);
            }
            // TODO: link MOVEMENTVECTOR, ISROLLING with animations
            movementVector = new Vector3(x, y, 0f).normalized;
            if (Input.GetKeyDown(KeyCode.Space) && stamina >= rollStaminaCost)
            {
                Roll();
            }
            if (Input.GetMouseButtonDown(0) && !isAttacking)
            {
                Attack();
            }
            staminaRecoveryTimer += Time.deltaTime;
            if (staminaRecoveryTimer >= timeUntilStaminaRecovers)
            {
                stamina = Mathf.Min(maxStamina, stamina += staminaRecoveryPerSecond * Time.deltaTime);
                m_HUD.UpdateStamina(stamina / maxStamina);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject.CompareTag("Interactable"))
                    {
                        Interactable interactable = colliders[i].gameObject.GetComponent<Interactable>();
                        interactable.Interact();
                        break;
                    }
                }
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
            rb.velocity = movementVector * speed.GetValue();
        }
        else
        {
            rb.velocity = rollVector * rollSpeed;
        }
    }

    private void RotateWeaponToMousePosition(bool lerp)
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(weaponCenter.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        Quaternion desRot = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        if (!lerp)
        {
            weaponCenter.rotation = desRot;
        }
        else
        {
            weaponCenter.rotation = Quaternion.Lerp(weaponCenter.rotation, desRot, Time.deltaTime * 20f);
        }
    }

    private void Roll()
    {
        // The player starts a roll with initial speed MAXROLLSPEED in the direction
        // of ROLLVECTOR.
        rollVector = movementVector;
        isRolling = true;
        rollSpeed = maxRollSpeed;
        stamina -= rollStaminaCost;
        staminaRecoveryTimer = 0f;
        m_HUD.UpdateStamina(stamina / maxStamina);
    }

    private void Attack()
    {
        // The attack is split into 3 sections: the weapon going down, the weapon going up, and the cooldown
        weaponInitialRotation = weaponCenter.rotation;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.GetComponent<SpriteRenderer>())
            {
                enemy.GetComponent<SpriteRenderer>().color = new Color(
      Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
            enemy.GetComponent<EnemyController>().TakeDamage(damage.GetValue());
        }
        isAttacking = true;
        StartCoroutine(AttackFirstHalf(attackFirstHalfDuration, attackSecondHalfDuration, attackCoolDown));
    }

    private IEnumerator AttackFirstHalf(float firstTime, float secondTime, float attackCoolDown)
    {
        while (firstTime > 0f)
        {
            firstTime -= Time.deltaTime;
            weaponCenter.Rotate(Vector3.forward * weaponRaiseAngle / attackFirstHalfDuration * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(AttackSecondHalf(secondTime, attackCoolDown));
    }

    private IEnumerator AttackSecondHalf(float secondTime, float attackCoolDown)
    {
        while (secondTime > 0f)
        {
            secondTime -= Time.deltaTime;
            weaponCenter.Rotate(-Vector3.forward * weaponSwingAngle / attackSecondHalfDuration * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(AttackCoolDown(attackCoolDown));
    }

    private IEnumerator AttackCoolDown(float coolDownTime)
    {
        while (coolDownTime > 0f)
        {
            coolDownTime -= Time.deltaTime;
            RotateWeaponToMousePosition(true);
            yield return null;
        }
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackCenter.position, attackRange);
    }

    public void TakeDamage(int amount)
    {
        if (!isRolling)
        {
            health = Mathf.Clamp(health - amount, 0, maxHealth);
            print(amount + " damage taken, health is now " + health);
            m_HUD.UpdateHealth(health);
            if (health <= 0)
            {
                Death();
            }
        }
    }

    public void RecoverStamina()
    {
        stamina = maxStamina;
        m_HUD.UpdateStamina(1f);
    }

    private void Death()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void ExportStats(GameManager manage){
        manage.currHealth = health;
        manage.currDamage = damage;
        manage.currSpeed = speed;
    }

    public void ImportStats(GameManager manage){
        speed = manage.currSpeed;
        damage = manage.currDamage;
        health = manage.currHealth;
    }
}
