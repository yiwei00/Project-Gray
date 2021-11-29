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

    public virtual Weapon getItem(Weapon type)
    {
        return null;
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
    Weapon storedWeapon;
    public WeaponInventoryItem(
        string title, 
        string description, 
        Sprite icon,
        Weapon weapon
     ) : base(title, description, icon)
    {
        storedWeapon = weapon;
    }
    public override Weapon getItem(Weapon type)
    {
        return storedWeapon;
    }
}
