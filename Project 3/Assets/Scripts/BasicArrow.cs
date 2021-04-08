using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArrow : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    private int m_Damage;

    [SerializeField]
    private float m_Speed;

    [SerializeField]
    private float m_Range;

    [SerializeField]
    private float m_DespawnTime;

    public float getRange()
    {
        return m_Range;
    }
    public float getSpeed()
    {
        return m_Speed;
    }
    #endregion

    #region Private Variables
    private Vector3 direction;
    private GameObject player;
    #endregion

    #region Initialization
    private void Awake()
    {
        StartCoroutine(Countdown());
        player = GameObject.FindWithTag("Player");
    }
    #endregion

    #region Updates
    private void FixedUpdate()
    {
        transform.position += direction * m_Speed * Time.deltaTime;
    }
    #endregion

    #region Despawning
    private IEnumerator Countdown()
    {
        float elapsed = 0;
        while (elapsed < m_DespawnTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
    #endregion

    #region Attacking
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
            other.GetComponent<PlayerController>().TakeDamage(m_Damage);
            Destroy(gameObject);
        }
    }
    public void setup(Vector3 dir)
    {
        direction = dir;
    }
    #endregion
}
