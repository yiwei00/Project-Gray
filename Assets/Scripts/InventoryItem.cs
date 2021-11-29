using UnityEngine;

public class InventoryItem
{
    public static InventoryItem empty = new InventoryItem("", "", null);

    protected string _title;
    protected string _description;
    protected Sprite _icon;
    public string description
    {
        get => _description;
    }
    public string title
    {
        get => _title;
    }
    public Sprite icon
    {
        get => _icon;
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
