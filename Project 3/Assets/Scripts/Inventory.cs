﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public PlayerController player;
    public static Inventory instance;
    public List<Item> items = new List<Item>();
    public int maxCapacity;
    public int gold;
    private bool isOpen;
    public GameObject inventoryUI;
    public TextMeshProUGUI goldText;
    public GameObject ItemPrefab;
    public GameObject Grid;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                inventoryUI.SetActive(true);
                UpdateUI();
            }
            else
            {
                inventoryUI.SetActive(false);
            }
        }
    }

    public void Add(Item item)
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
            if (isOpen)
            {
                UpdateUI();
            }
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

    public void SellAt(int index)
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
        Shop.instance.Add(item);
        AddGold(Mathf.RoundToInt(item.value));
        if (isOpen)
        {
            UpdateUI();
        }
    }

    public bool IsFull()
    {
        return items.Count >= maxCapacity;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        goldText.text = gold.ToString();
    }

    public void UpdateUI()
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
            if (Shop.instance != null && Shop.instance.isOpen)
            {
                itemUI.SetDrop(false);
            }
            else
            {
                itemUI.SetDrop(true);
            }
        }
    }

    public Inventory Instance(){return instance;}

    public void ExportStats(GameManager manage){
        manage.currItems = items;
        manage.currGold = gold;
    }

    public void ImportStats(GameManager manage){
        gold = manage.currGold;
        items = manage.currItems;
    }
}
