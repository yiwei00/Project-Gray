// defines an item in the inventory

using UnityEngine;

public enum ItemType
{
    EMPTY,
    WEAPON
}
[System.Serializable]
public class SerializedItem
{
    public ItemType type;
    public string jsonItem;
    public SerializedItem(Item item)
    {
        type = item.type;
        jsonItem = item.itemToJson();
    }
}
public class Item
{
    public static Item empty = new Item("", "", null, ItemType.EMPTY);

    private ItemType _type;
    public ItemType type 
    {
        get => _type;
    }

    protected string _title;
    protected string _description;
    protected Sprite _icon;
    protected LootRarity _rarity;
    public string description
    {
        get => _description;
        set => _title = value;
    }
    public string title
    {
        get => _title;
        set => _title = value;
    }
    public Sprite icon
    {
        get => _icon;
        set => _icon = value;
    }

    public LootRarity rarity
    {
        get => _rarity;
        set => _rarity = value;
    }

    public virtual GameObject getItem()
    {
        return null;
    }

    public virtual bool isWeapon()
    {
        return false;
    }

    public virtual void destroy()
    {
        return;
    }

    public virtual string itemToJson()
    {
        return "";
    }

    public string toJson()
    {
        return JsonUtility.ToJson(new SerializedItem(this));
    }

    public static Item fromJson(string json)
    {
        if (json == "") return empty;
        var serializedItem = JsonUtility.FromJson<SerializedItem>(json);
        switch (serializedItem.type)
        {
            case ItemType.WEAPON:
                return new WeaponItem(Weapon.fromJson(serializedItem.jsonItem));
            default:
                return Item.empty;
        }
    }

    protected Item(string title, string description, Sprite icon, ItemType type)
    {
        _type = type;
        _title = title;
        _description = description;
        _icon = icon;
    }

}

public class WeaponItem : Item
{
    GameObject storedWeapon;

    public WeaponItem(GameObject weapon)
        : base("", "", null, ItemType.WEAPON)
    {
        weapon.SetActive(false);
        Weapon weaponComponent = weapon.GetComponent<Weapon>();
        storedWeapon = weapon;
        _title = weaponComponent.weaponName;
        _description = weaponComponent.description;
        _icon = weaponComponent.icon;
        _rarity = weaponComponent.rarity;
    }
    public override GameObject getItem()
    {
        return storedWeapon;
    }

    public override bool isWeapon()
    {
        return storedWeapon != null;
    }

    public override string itemToJson()
    {
        return storedWeapon.GetComponent<Weapon>().toJson();
    }
}
