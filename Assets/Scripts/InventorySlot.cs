// defines a single slot in an inventory

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour
{
    public Color highlightFrameColor;

    Color defaultFrameColor;
    Item _item;
    Image icon;
    Image frame;
    Image background;
    InventoryMenu invMenu;

    public InventoryMenu inventory
    {
        get => invMenu;
        set => invMenu = value;
    }

    public Item item
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
        get => (item == Item.empty);
    }

    private void Awake()
    {
        item = Item.empty;
        icon = transform.Find("Icon").GetComponent<Image>();
        frame = transform.Find("Frame").GetComponent<Image>();
        background = GetComponent<Image>();

        defaultFrameColor = frame.color;

        resetColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (item == Item.empty)
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
    public void addItem(Item newItem)
    {
        if (!isEmpty || newItem == Item.empty)
        {
            return;
        }
        item = newItem;
        icon.enabled = true;
        icon.sprite = newItem.icon;
        // set color
        Color rarityColor = Loot.rarityToColor(newItem.rarity);
        rarityColor.a = .5f;
        background.color = rarityColor;
    }

    public Item popItem()
    {
        var temp = item;
        item = Item.empty;
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

    private void OnDestroy()
    {
        item.destroy();
    }
}
