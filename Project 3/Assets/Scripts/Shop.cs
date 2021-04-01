using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : Interactable
{
    public PlayerController player;
    public static Shop instance;
    public List<Item> items = new List<Item>();
    public bool isOpen;
    public GameObject shopUI;
    public GameObject ItemPrefab;
    public GameObject Grid;
    private bool prevIsOpen;


    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public override void Interact()
    {
        isOpen = true;
        shopUI.SetActive(true);
        Inventory.instance.UpdateUI();
        UpdateUI();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > 3f)
        {
            isOpen = false;
            shopUI.SetActive(false);
        }
        if (prevIsOpen != isOpen)
        {
            Inventory.instance.UpdateUI();
            prevIsOpen = isOpen;
        }
    }

    public void Add(Item item)
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
        if (isOpen)
        {
            UpdateUI();
        }
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
        if (isOpen)
        {
            UpdateUI();
        }
    }

    public void RemoveAt(int index)
    {
        Item item = items[index];
        if (item.type == Item.Type.DamageUp)
        {
            player.damage.RemoveModifier(item.value);
        }
        else if (item.type == Item.Type.SpeedUp)
        {
            player.speed.RemoveModifier(item.value);
        }
        items.Remove(item);
        if (isOpen)
        {
            UpdateUI();
        }
    }

    public void BuyAt(int index)
    {
        Item item = items[index];
        if (Inventory.instance.gold >= item.value)
        {
            if (item.type == Item.Type.HealthPotion)
            {
                player.TakeDamage(
                    Mathf.RoundToInt(-item.value));
            }
            else if (item.type == Item.Type.StaminaPotion)
            {
                player.RecoverStamina();
            }
            else if (item.type == Item.Type.DamageUp)
            {
                player.damage.RemoveModifier(item.value);
            }
            else if (item.type == Item.Type.SpeedUp)
            {
                player.speed.RemoveModifier(item.value);
            }
            items.Remove(item);
            if (item.type != Item.Type.HealthPotion && item.type != Item.Type.StaminaPotion)
            {
                Inventory.instance.Add(item);
            }
            Inventory.instance.AddGold(Mathf.RoundToInt(-item.value));
            if (isOpen)
            {
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        foreach (Transform child in Grid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < items.Count; i++)
        {
            GameObject prefabInstance = Instantiate(ItemPrefab);
            prefabInstance.transform.SetParent(Grid.transform);
            ItemUI itemUI = prefabInstance.GetComponent<ItemUI>();
            Item item = items[i];
            itemUI.SetName(item.name);
            itemUI.SetSprite(item.icon);
        }
    }
}
