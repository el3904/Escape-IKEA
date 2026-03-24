using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType
    {
        Sword,
        HealthPill,
        SpeedPill,
        Coin,
        Medkit,
        Armor,
        Axe,
    }

    public ItemType itemType;
    public int amount;

    public Item Clone()
    {
        return new Item
        {
            itemType = this.itemType,
            amount = this.amount
        };
    }

    public Sprite GetSprite()
    {
        return ItemAssets.Instance.GetSprite(itemType);
    }

    public float GetUIScale()
    {
        switch (itemType)
        {
            case ItemType.Sword:
                return 1f;

            case ItemType.Axe:
                return 0.74f;

            case ItemType.Armor:
                return 0.8f;

            case ItemType.HealthPill:
                return 0.75f;

            case ItemType.SpeedPill:
                return 0.75f;

            case ItemType.Coin:
                return 0.65f;

            case ItemType.Medkit:
                return 0.8f;

            default:
                return 1f;
        }
    }

    public Color GetColor()
    {
        switch (itemType)
        {
            case ItemType.Sword:
                return new Color(1f, 1f, 1f);

            case ItemType.Axe:
            case ItemType.Armor:
                return new Color(0.85f, 0.9f, 1f);

            case ItemType.HealthPill:
                return new Color(1f, 0f, 0f);

            case ItemType.SpeedPill:
                return new Color(0f, 0f, 1f);

            case ItemType.Coin:
                return new Color(1f, 1f, 0f);

            case ItemType.Medkit:
                return new Color(1f, 0f, 0.75f);

            default:
                return Color.white;
        }
    }

    public bool IsStackable()
    {
        switch (itemType)
        {
            case ItemType.Coin:
            case ItemType.HealthPill:
            case ItemType.SpeedPill:
                return true;

            case ItemType.Sword:
            case ItemType.Axe:
            case ItemType.Armor:
            case ItemType.Medkit:
                return false;

            default:
                return false;
        }
    }
}