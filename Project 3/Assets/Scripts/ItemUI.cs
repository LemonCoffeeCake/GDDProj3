using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image image;

    void Start()
    {


    }

    public void SetSprite(Sprite s)
    {
        image.sprite = s;
    }

    public void SetName(string s)
    {
        nameText.text = s;
    }

    public void DeleteItem()
    {
        Inventory.instance.RemoveAt(transform.GetSiblingIndex());
        Destroy(gameObject);
    }

}
