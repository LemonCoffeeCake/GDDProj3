using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string name = "New Item";
    public Sprite icon;
    public enum Type { Gold, HealthPotion, StaminaPotion,
    DamageUp, SpeedUp }
    public Type type;
    public float value;
    public float price;
}
