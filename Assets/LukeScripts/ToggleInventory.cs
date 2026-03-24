using UnityEngine;

public class ToggleInventory : MonoBehaviour
{
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private GameObject utilityQuickSlot;

    private bool isOpen = false;

    private void Start()
    {
        uiRoot.SetActive(false);

        if (utilityQuickSlot != null)
        {
            utilityQuickSlot.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            uiRoot.SetActive(isOpen);

            if (utilityQuickSlot != null)
            {
                utilityQuickSlot.SetActive(!isOpen);
            }
        }
    }
}