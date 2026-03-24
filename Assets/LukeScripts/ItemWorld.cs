using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using System;
using CodeMonkey.Utils;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        ItemAssets itemAssets = ItemAssets.GetInstance();

        if (itemAssets == null)
        {
            Debug.LogError("No ItemAssets found in scene!");
            return null;
        }

        if (itemAssets.pfItemWorld == null)
        {
            Debug.LogError("pfItemWorld is not assigned on ItemAssets!");
            return null;
        }

        Transform spawnedTransform = Instantiate(itemAssets.pfItemWorld, position, Quaternion.identity);

        ItemWorld itemWorld = spawnedTransform.GetComponent<ItemWorld>();
        if (itemWorld == null)
        {
            Debug.LogError("pfItemWorld prefab is missing ItemWorld component!");
            return null;
        }

        itemWorld.SetItem(item);
        return itemWorld;
    }

    private Item item;
    private SpriteRenderer spriteRenderer;
    private Light2D light2D;
    private TextMeshPro textMeshPro;
    private float canBePickedUpTimer;
    private Vector3 defaultScale;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        light2D = GetComponent<Light2D>();
        textMeshPro = transform.Find("Amount").GetComponent<TextMeshPro>();
        defaultScale = transform.localScale;
    }

    private void Update()
    {
        if (canBePickedUpTimer > 0f)
        {
            canBePickedUpTimer -= Time.deltaTime;
        }
    }

    public void SetCanBePickedUpTimer(float time)
    {
        canBePickedUpTimer = time;
    }

    public bool CanBePickedUp()
    {
        return canBePickedUpTimer <= 0f;
    }

    public void SetItem(Item item)
    {
        this.item = item;

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer missing on ItemWorld prefab!", this);
            return;
        }

        spriteRenderer.sprite = item.GetSprite();

        // og prefab size
        transform.localScale = defaultScale;
        if (light2D != null)
        {
            light2D.color = item.GetColor();

            switch (item.itemType)
            {
                case Item.ItemType.Axe:
                case Item.ItemType.Armor:
                    light2D.intensity = 1.4f;
                    light2D.pointLightOuterRadius = 1.2f;
                    break;

                default:
                    light2D.intensity = 1f;
                    light2D.pointLightOuterRadius = 1f;
                    break;
            }
        }

        if (textMeshPro != null)
        {
            if (item.amount > 1)
            {
                textMeshPro.SetText(item.amount.ToString());
            }
            else
            {
                textMeshPro.SetText("");
            }
        }
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    internal static ItemWorld DropItem(Vector3 dropPosition, Item item)
    {
        Vector3 randomDir = UtilsClass.GetRandomDir();

        ItemWorld itemWorld = SpawnItemWorld(dropPosition + randomDir * 1.1f, item);
        itemWorld.SetCanBePickedUpTimer(0.25f);

        Rigidbody2D rb = itemWorld.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.AddForce(randomDir * 3.5f, ForceMode2D.Impulse);
        }

        return itemWorld;
    }
}
