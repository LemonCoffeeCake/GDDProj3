using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;
    private PlayerController player;

    private void Start()
    {
        player = Inventory.instance.player;
    }
    public override void Interact()
    {
        PickUp();
    }

    private void PickUp()
    {
        if (item.type == Item.Type.HealthPotion)
        {
            player.TakeDamage(
                Mathf.RoundToInt(-item.value));
            Destroy(gameObject);
        }
        else if (item.type == Item.Type.StaminaPotion)
        {
            player.RecoverStamina();
            Destroy(gameObject);
        }
        else if (item.type == Item.Type.Gold)
        {
            player.AddGold(Mathf.RoundToInt(item.value));
            Destroy(gameObject);
        }
        else
        {
            if (Inventory.instance.Add(item))
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetValue(float newValue)
    {
        item.value = newValue;
    }
}