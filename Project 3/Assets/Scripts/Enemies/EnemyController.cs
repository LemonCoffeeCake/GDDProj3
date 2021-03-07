using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    private float m_Health;

    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private float m_MoveSpeed;
    #endregion

    #region Cached Region
    private GameObject cr_Player;
    #endregion

    #region Initialization
    private void Awake()
    {
      
    }

    private void Start()
    {
        cr_Player = GameObject.FindWithTag("Player");
    }
    #endregion

    #region Updates
    private void FixedUpdate()
    {
        if (m_Health <= 0)
        {
            Death();
        }
        Move();
    }
    #endregion

    #region Movement
    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, cr_Player.transform.position, m_MoveSpeed * Time.deltaTime);
    }
    #endregion

    #region Health and Death
    private void Death()
    {
        Destroy(this);
    }
    #endregion

    #region Collison
    public void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(m_Damage);
        }
    }
    #endregion

}
