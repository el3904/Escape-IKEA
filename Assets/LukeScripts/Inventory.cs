using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class Inventory
{
    public event EventHandler OnItemListChanged;
    private List<Item> itemList;
    private Action<Item> useItemAction;

    public Inventory(Action<Item> useItemAction)
    {
        this.useItemAction = useItemAction;
        itemList = new List<Item>();

        //AddItem(new Item { itemType = Item.ItemType.SpeedPill, amount = 1 });
        //Add Default Items Here
        this.useItemAction = useItemAction;
    }

    public Item FindFirstItemByType(Item.ItemType itemType)
    {
        foreach (Item item in itemList)
        {
            if (item.itemType == itemType)
            {
                return item;
            }
        }

        return null;
    }

    public void AddItem(Item item)
    {
        
        if (item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in itemList)
            {
                if(inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }
            if (!itemAlreadyInInventory)
            {
                itemList.Add(item);
            }
        }
        else
        {
            itemList.Add(item);
        }
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetItemIndex(Item item)
    {
        return itemList.IndexOf(item);
    }

    public void InsertItemAt(Item item, int index)
    {
        if (item == null) return;

        if (item.IsStackable())
        {
            bool itemAlreadyInInventory = false;

            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                    break;
                }
            }

            if (!itemAlreadyInInventory)
            {
                index = Mathf.Clamp(index, 0, itemList.Count);
                itemList.Insert(index, item);
            }
        }
        else
        {
            index = Mathf.Clamp(index, 0, itemList.Count);
            itemList.Insert(index, item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public void UseItem(Item item)
    {
        useItemAction(item);
    }

    public void RemoveItem(Item item)
    {
        if (item.IsStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryItem in itemList)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount -= item.amount;
                    itemInInventory = inventoryItem;
                }
            }
            if (itemInInventory != null && itemInInventory.amount <= 0)
            {
                itemList.Remove(itemInInventory);
            }
        }
        else
        {
            itemList.Remove(item);
        }
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveOneItem(Item item)
    {
        if (item.IsStackable())
        {
            item.amount--;

            if (item.amount <= 0)
            {
                itemList.Remove(item);
            }
        }
        else
        {
            itemList.Remove(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }
}
