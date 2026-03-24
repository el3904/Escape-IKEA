using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSpriteEntry
{
    public Item.ItemType itemType;
    public Sprite sprite;
}

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    public static ItemAssets GetInstance()
    {
        if (Instance == null)
        {
            Instance = Object.FindFirstObjectByType<ItemAssets>(FindObjectsInactive.Include);
        }
        return Instance;
    }

    public Transform pfItemWorld;
    [SerializeField] private List<ItemSpriteEntry> itemSpriteList;

    private Dictionary<Item.ItemType, Sprite> spriteDict;

    private void Awake()
    {
        Instance = this;

        spriteDict = new Dictionary<Item.ItemType, Sprite>();

        foreach (ItemSpriteEntry entry in itemSpriteList)
        {
            if (!spriteDict.ContainsKey(entry.itemType))
            {
                spriteDict.Add(entry.itemType, entry.sprite);
            }
            else
            {
                Debug.LogWarning("Duplicate itemType in ItemAssets: " + entry.itemType);
            }
        }
    }

    public Transform GetPfItemWorld()
    {
        return pfItemWorld;
    }

    public Sprite GetSprite(Item.ItemType itemType)
    {
        if (spriteDict.TryGetValue(itemType, out Sprite sprite))
        {
            return sprite;
        }

        Debug.LogWarning("No sprite found for itemType: " + itemType);
        return null;
    }
}