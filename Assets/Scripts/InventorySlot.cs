using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Color highlightFrameColor;
    Color defaultFrameColor;
    InventoryItem _item;
    Image icon;
    Image frame;
    Button button;
    InventoryMenu invMenu;

    public InventoryMenu inventory
    {
        get => invMenu;
        set => invMenu = value;
    }

    public InventoryItem item
    {
        get => _item;
        private set => _item = value;
    }

    public void toggleHighlight()
    {
        frame.color = (frame.color == defaultFrameColor) ?
            highlightFrameColor : defaultFrameColor;
    }

    public bool isEmpty
    {
        get => (item == InventoryItem.empty);
    }

    private void Awake()
    {
        item = InventoryItem.empty;
        button = GetComponent<Button>();
        button.onClick.AddListener(() => invMenu.setupSwap(this));
        icon = transform.Find("Icon").GetComponent<Image>();
        frame = transform.Find("Frame").GetComponent<Image>();
        defaultFrameColor = frame.color;
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
        if (!isEmpty)
        {
            return;
        }
        item = newItem;
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

    public void swap(InventorySlot other)
    {
        if (other.isEmpty)
        {
            other.addItem(popItem());
        } 
        else if (isEmpty)
        {
            addItem(other.popItem());
        }
        else
        {
            var myItem = popItem();
            addItem(other.popItem());
            other.addItem(myItem);
        }
    }
}
