using UnityEngine;

public class InventoryItem
{
    public static InventoryItem empty = new InventoryItem("", "", null);

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

    protected InventoryItem(string title, string description, Sprite icon)
    {
        _title = title;
        _description = description;
        _icon = icon;
    }

}

public class WeaponInventoryItem : InventoryItem
{
    GameObject storedWeapon;

    public WeaponInventoryItem(GameObject weapon)
        : base("", "", null)
    {
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
}
