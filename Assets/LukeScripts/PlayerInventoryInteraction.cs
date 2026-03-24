using System.Collections;
using UnityEngine;
using static EquipmentEnum;

public class PlayerInventoryInteraction : MonoBehaviour
{
    private Inventory inventory;

    [SerializeField] private UI_Inventory uiInventory;
    [SerializeField] private EquipmentUI equipmentUI;
    [SerializeField] private EquipmentData equipmentData;
    [SerializeField] private Dialogue playerDialogue;

    private SpriteRenderer playerSpriteRenderer;
    private PlayerMovement playerMovement;

    private Coroutine flashCoroutine;
    private Color baseColor;

    private bool firstItemFound;

    private void Awake()
    {
        inventory = new Inventory(UseItem);
        playerMovement = GetComponent<PlayerMovement>();

        playerSpriteRenderer = transform.Find("PlayerSprite").GetComponent<SpriteRenderer>();
        baseColor = playerSpriteRenderer.color;

        firstItemFound = false;
    }

    private void Start()
    {
        uiInventory.SetPlayer(this);
        uiInventory.SetInventory(inventory);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UseEquippedUtility();
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void TryAutoEquipPickedUpItem(Item item)
    {
        if (item == null) return;
        if (!ItemEquipClassifier.IsEquipable(item)) return;
        if (equipmentData == null) return;

        EquipTag equipTag = ItemEquipClassifier.GetEquipTag(item);
        Item equippedItem = equipmentData.GetEquippedItem(equipTag);

        // if empty, equip stuff
        if (equippedItem == null)
        {
            AutoEquipItem(item);
            return;
        }

        // stack on the slot if the item is stackable
        if (equipTag == EquipTag.Utility &&
            equippedItem.itemType == item.itemType &&
            equippedItem.IsStackable())
        {
            equippedItem.amount += item.amount;
            inventory.RemoveItem(item);
            equipmentUI.RefreshSlot(EquipTag.Utility);
        }
    }

    private void TryAutoEquipReplacementUtility(Item.ItemType itemType)
    {
        Item replacementItem = inventory.FindFirstItemByType(itemType);

        if (replacementItem == null) return;
        if (!ItemEquipClassifier.IsEquipable(replacementItem)) return;

        EquipTag equipTag = ItemEquipClassifier.GetEquipTag(replacementItem);
        if (equipTag != EquipTag.Utility) return;

        Item itemToEquip = replacementItem.Clone();
        equipmentUI.EquipItem(itemToEquip);

        if (replacementItem.IsStackable())
        {
            inventory.RemoveItem(replacementItem);
        }
        else
        {
            inventory.RemoveOneItem(replacementItem);
        }
    }

    private void AutoEquipItem(Item item)
    {
        Item itemToEquip = item.Clone();
        Item oldItem = equipmentUI.EquipItem(itemToEquip);

        if (item.IsStackable())
        {
            inventory.RemoveItem(item);
        }
        else
        {
            inventory.RemoveOneItem(item);
        }

        if (oldItem != null)
        {
            inventory.AddItem(oldItem);
        }
    }

    private void UseEquippedUtility()
    {
        Item utilityItem = equipmentData.GetEquippedItem(EquipTag.Utility);

        if (utilityItem == null) return;

        Item.ItemType usedItemType = utilityItem.itemType;
        bool wasStackable = utilityItem.IsStackable();

        switch (utilityItem.itemType)
        {
            case Item.ItemType.SpeedPill:
                playerMovement.BoostSpeedFor10Seconds();
                FlashBlue();
                ConsumeUtilityItem(utilityItem, usedItemType, wasStackable);
                break;

            case Item.ItemType.Medkit:
                FlashPink();
                ConsumeUtilityItem(utilityItem, usedItemType, wasStackable);
                break;
        }
    }

    private void ConsumeUtilityItem(Item utilityItem, Item.ItemType usedItemType, bool wasStackable)
    {
        utilityItem.amount--;

        if (utilityItem.amount <= 0)
        {
            equipmentUI.ClearSlot(EquipTag.Utility);

            if (!wasStackable)
            {
                TryAutoEquipReplacementUtility(usedItemType);
            }
        }
        else
        {
            equipmentUI.RefreshSlot(EquipTag.Utility);
        }
    }

    internal void UseItem(Item item)
    {
        if (ItemEquipClassifier.IsEquipable(item))
        {
            int originalIndex = inventory.GetItemIndex(item);

            Item itemToEquip = item.Clone();
            Item oldItem = equipmentUI.EquipItem(itemToEquip);

            if (item.IsStackable())
            {
                inventory.RemoveItem(item);
            }
            else
            {
                inventory.RemoveOneItem(item);
            }

            if (oldItem != null)
            {
                inventory.InsertItemAt(oldItem, originalIndex);
            }

            return;
        }

        switch (item.itemType)
        {
            case Item.ItemType.HealthPill:
                FlashGreen();
                inventory.RemoveOneItem(item);
                break;

            case Item.ItemType.Medkit:
                FlashPink();
                inventory.RemoveOneItem(item);
                break;
        }
    }

    public void UnequipItem(EquipTag equipTag)
    {
        Item removedItem = equipmentUI.UnequipItem(equipTag);

        if (removedItem != null)
        {
            inventory.AddItem(removedItem);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();

        if (itemWorld != null && itemWorld.CanBePickedUp())
        {
            if (!firstItemFound)
            {
                firstItemFound = true;
                playerDialogue.ShowDialogue();
            }

            Item pickedUpItem = itemWorld.GetItem();

            inventory.AddItem(pickedUpItem);
            TryAutoEquipPickedUpItem(pickedUpItem);

            itemWorld.DestroySelf();
        }
    }

    private void StartFlash(Color targetColor)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            playerSpriteRenderer.color = baseColor;
        }

        flashCoroutine = StartCoroutine(FlashSmooth(targetColor));
    }

    private IEnumerator FlashSmooth(Color flashColor)
    {
        float flashInTime = 0.10f;
        float flashOutTime = 0.45f;

        float timer = 0f;

        while (timer < flashInTime)
        {
            timer += Time.deltaTime;
            float t = timer / flashInTime;
            t = 1f - Mathf.Pow(1f - t, 3f);
            playerSpriteRenderer.color = Color.Lerp(baseColor, flashColor, t);
            yield return null;
        }

        playerSpriteRenderer.color = flashColor;

        timer = 0f;

        while (timer < flashOutTime)
        {
            timer += Time.deltaTime;
            float t = timer / flashOutTime;
            t = t * t;
            playerSpriteRenderer.color = Color.Lerp(flashColor, baseColor, t);
            yield return null;
        }

        playerSpriteRenderer.color = baseColor;
        flashCoroutine = null;
    }

    public void FlashGreen()
    {
        StartFlash(new Color(0.4f, 1f, 0.4f));
    }

    public void FlashBlue()
    {
        StartFlash(new Color(0.4f, 0.4f, 1f));
    }

    public void FlashPink()
    {
        StartFlash(new Color(1f, 0.4f, 0.8f));
    }
}