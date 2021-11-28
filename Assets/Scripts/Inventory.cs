using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    protected string _title;
    protected string _description;
    protected Texture2D _icon;
    public string description
    {
        get => _description;
    }
    public string title
    {
        get => _title;
    }
    public Texture2D icon
    {
        get => _icon;
    }

    public virtual Weapon getItem(Weapon type)
    {
        return null;
    }

    protected InventoryItem(string title, string description, Texture2D icon)
    {
        _title = title;
        _description = description;
        _icon = icon;
    }

}

public class EmptyInventoryItem : InventoryItem
{
    public EmptyInventoryItem() : base("", "", null) {}
    
}

public class WeaponInventoryItem : InventoryItem
{
    Weapon storedWeapon;
    public WeaponInventoryItem(
        string title, 
        string description, 
        Texture2D icon,
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
