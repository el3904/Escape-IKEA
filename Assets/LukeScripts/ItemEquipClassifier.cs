using static EquipmentEnum;

public static class ItemEquipClassifier
{
    public static EquipTag GetEquipTag(Item item)
    {
        if (item == null) return EquipTag.None;

        switch (item.itemType)
        {
            case Item.ItemType.Sword:
            case Item.ItemType.Axe:
                return EquipTag.Weapon;

            case Item.ItemType.Armor:
                return EquipTag.Armor;

            case Item.ItemType.SpeedPill:
            case Item.ItemType.Medkit:
                return EquipTag.Utility;

            default:
                return EquipTag.None;
        }
    }

    public static bool IsEquipable(Item item)
    {
        return GetEquipTag(item) != EquipTag.None;
    }
}