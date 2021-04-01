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
            Inventory.instance.AddGold(Mathf.RoundToInt(item.value));
            Destroy(gameObject);
        }
        else
        {
            if (!Inventory.instance.IsFull())
            {
                Item itemInstance = ScriptableObject.CreateInstance<Item>();
                itemInstance.name = item.name;
                itemInstance.icon = item.icon;
                itemInstance.type = item.type;
                itemInstance.value = item.value;
                itemInstance.price = item.price;
                Inventory.instance.Add(itemInstance);
                Destroy(gameObject);
            }
        }
    }
}