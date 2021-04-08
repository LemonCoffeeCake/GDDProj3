using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    private int numHit;

    public void Hit(List<Vector3> positions)
    {
        StartCoroutine(HitCoroutine(positions));
    }

    private IEnumerator HitCoroutine(List<Vector3> positions)
    {
        if (positions.Count == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Vector3 pos = positions[0];
            while (Vector3.Distance(transform.position, pos) > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * 20f);
                Vector3 vectorToTarget = pos - transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 20f);
                yield return null;
            }
            positions.RemoveAt(0);
            StartCoroutine(HitCoroutine(positions));
        }
    }
}
