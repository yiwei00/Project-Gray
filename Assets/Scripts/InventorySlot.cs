using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Color highlightFrameColor;
    Color defaultFrameColor;
    InventoryItem _item;
    Image icon;
    Image frame;
    Image background;
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
        background = GetComponent<Image>();

        defaultFrameColor = frame.color;

        resetColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (item == InventoryItem.empty)
        {
            icon.enabled = false;
        }
    }
    
    private void resetColor()
    {
        Color transWhite = Color.white;
        transWhite.a = .3f;
        background.color = transWhite;
    }
    public void addItem(InventoryItem newItem)
    {
        if (!isEmpty || newItem == InventoryItem.empty)
        {
            return;
        }
        item = newItem;
        icon.enabled = true;
        icon.sprite = newItem.icon;
        // set color
        Color rarityColor = LootItem.rarityToColor(newItem.rarity);
        rarityColor.a = .5f;
        background.color = rarityColor;
    }

    public InventoryItem popItem()
    {
        var temp = item;
        item = InventoryItem.empty;
        icon.enabled = false;
        // remove color
        resetColor();
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
