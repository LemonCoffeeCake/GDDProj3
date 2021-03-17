using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public PlayerController player;
    public static Inventory instance;
    public List<Item> items = new List<Item>();
    public int maxCapacity;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public bool Add(Item item)
    {
        if (items.Count < maxCapacity)
        {
            items.Add(item);
            if (item.type == Item.Type.DamageUp)
            {
                player.damage.AddModifier(item.value);
            }
            else if (item.type == Item.Type.SpeedUp)
            {
                player.speed.AddModifier(item.value);
            }
            return true;
        }
        return false;
    }

    public void Remove(Item item)
    {
        if (item.type == Item.Type.DamageUp)
        {
            player.damage.RemoveModifier(item.value);
        }
        else if (item.type == Item.Type.SpeedUp)
        {
            player.speed.RemoveModifier(item.value);
        }
        items.Remove(item);
    }
}
