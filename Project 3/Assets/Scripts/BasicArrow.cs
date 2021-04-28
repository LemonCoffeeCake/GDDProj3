using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicArrow : MonoBehaviour
{
    #region Editor Variables
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
    private int m_Damage;
    #endregion

    #region Initialization
    private void Awake()
    {
        StartCoroutine(Countdown());
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(m_Damage);
            Destroy(gameObject);
        }
    }
    public void setup(Vector3 dir, int damage)
    {
        direction = dir;
        transform.Rotate(0.0f, 0.0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        m_Damage = damage;
    }
    #endregion
}
