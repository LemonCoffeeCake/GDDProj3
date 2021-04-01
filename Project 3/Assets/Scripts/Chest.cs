using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField]
    private ItemPickup[] m_drops;

    [SerializeField]
    private float m_dropRate;

    [SerializeField]
    private float m_dropMinVal;

    [SerializeField]
    private float m_dropMaxVal;

    private Animator anim;
    private bool hasOpened;
    // Start is called before the first frame update

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public override void Interact()
    {
        gameObject.tag = "Untagged";
        if (!hasOpened)
        {
            anim.SetTrigger("ChestOpenTrigger");
            hasOpened = true;
            StartCoroutine(Drop());
        }
    }

    private IEnumerator Drop()
    {
        yield return new WaitForSeconds(0.7f);
        DropItem();
    }
    private void DropItem()
    {
        // TODO: add RNG 
        if (m_drops.Length != 0)
        {
            float isDrop = Random.Range(0f, 100f);
            if (isDrop < m_dropRate)
            {
                int index = Random.Range(0, m_drops.Length);
                ItemPickup drop = Instantiate(m_drops[index], transform.position - new Vector3(0f, 1f, 0f), Quaternion.identity);
            }
        }
    }
}
