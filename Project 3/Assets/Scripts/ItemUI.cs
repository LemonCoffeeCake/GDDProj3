using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image image;
    public TextMeshProUGUI dropText;
    public Image deleteButtonBG;
    public Color dropColor = Color.red;
    public Color sellColor = Color.yellow;

    public void SetSprite(Sprite s)
    {
        image.sprite = s;
    }

    public void SetName(string s)
    {
        nameText.text = s;
    }

    public void SetDrop(bool isDrop)
    {
        if (isDrop)
        {
            dropText.text = "X";
            deleteButtonBG.color = dropColor;
        }
        else
        {
            dropText.text = "$";
            deleteButtonBG.color = sellColor;
        }
    }

    public void DeleteItem()
    {
        if (transform.parent.parent.parent.name == "ShopBG")
        {
            if (Shop.instance != null && Shop.instance.isOpen)
            {
                Shop.instance.BuyAt(transform.GetSiblingIndex());
            }
        }
        else
        {
            if (Shop.instance != null && Shop.instance.isOpen)
            {
                Inventory.instance.SellAt(transform.GetSiblingIndex());
            }
            else
            {
                Inventory.instance.RemoveAt(transform.GetSiblingIndex());
            }
        }
    }
}
