using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    InventoryMenu invMenu;
    InventoryItem item;
    Image icon;

    public bool isEmpty
    {
        get => (item == InventoryItem.empty);
    }

    private void Awake()
    {
        item = InventoryItem.empty;
    }
    // Start is called before the first frame update
    void Start()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
        invMenu = transform.parent.parent.GetComponent<InventoryMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (item == InventoryItem.empty)
        {
            icon.enabled = false;
        }
    }

    public void addItem(InventoryItem newItem)
    {
        if (isEmpty)
        {
            return;
        }
        icon.enabled = true;
        icon.sprite = newItem.icon;
    }

    public InventoryItem popItem()
    {
        var temp = item;
        item = InventoryItem.empty;
        icon.enabled = false;
        return temp;
    }
}
