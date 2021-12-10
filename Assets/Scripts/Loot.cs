using UnityEngine;
using UnityEngine.UI;

public enum LootRarity
{
    COMMON = 0,
    UNCOMMON = 1,
    RARE = 2,
    EPIC = 3,
    LEGENDARY = 4
}
public enum Modifier
{
    Plain,
    Mighty,
    Arcane,
    Peculiar
}
public class Loot : MonoBehaviour
{
    private Item _item = Item.empty;
    
    Text itemName;

    Transform itemCube;
    Vector3 childOffset;

    public static Color rarityToColor(LootRarity rarity)
    {
        Color ret = new Color();
        switch(rarity)
        {
            case LootRarity.COMMON:
                ret = Color.gray;
                break;
            case LootRarity.UNCOMMON:
                ret = Color.green;
                break;
            case LootRarity.RARE:
                ret = Color.blue;
                break;
            case LootRarity.EPIC: // purple
                ret = Color.blue;
                ret.r = .5f;
                break;
            case LootRarity.LEGENDARY: // orange
                ret = Color.red;
                ret.g = .5f;
                break;
        }
        return ret;
    }

    public Item item
    {
        get => _item;
        set
        {
            _item = value;
            if (itemName)
            {
                itemName.text = _item.title;
                itemName.color = rarityToColor(_item.rarity);
            }
        }
    }

    public void Update()
    {
        // move text to item
        transform.position += (itemCube.localPosition - childOffset);
        itemCube.localPosition = childOffset;
    }

    public void Awake()
    {
        itemCube = transform.Find("ItemObj");
        childOffset = itemCube.localPosition;
        itemName = GetComponentInChildren<Text>();
    }

    private void OnDestroy()
    {
        _item.destroy();
    }
}
