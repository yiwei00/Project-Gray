using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InventoryMenu : MonoBehaviour
{
    public int inventorySize;
    public GameObject invSlotPrefab;
    public GameObject defaultWeapon;

    List<InventorySlot> inventory;
    InventorySlot weaponSlot;
    InventorySlot toBeSwapped;
    PlayerControls controls; // for getting control inputs
    PlayerInput input;
    private Player _player;
    public Player player
    {
        get => _player;
        set => _player = value;
    }

    public GameObject equippedWeapon
    {
        get => weaponSlot.item.getItem();
    }

    #region Input systems
    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.UI.Enable();
        if (player)
            player.enabled = false;
        if (inventory != null)
            EventSystem.current.SetSelectedGameObject(inventory[0].gameObject);
    }
    public void OnDisable()
    {
        controls.UI.Disable();
    }
    public void OnMenuExit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (player)
                player.enabled = true;
            player.OnInventoryMenuExit();
        }
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        { 
            var selected = EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>();
            setupSwap(selected);
        }
    }

    #endregion

    public void setupSwap(InventorySlot selected)
    {
        if (toBeSwapped == null)
        {
            toBeSwapped = selected;
            toBeSwapped.toggleHighlight();
        }
        else
        {
            toBeSwapped.toggleHighlight();
            swapSlots(toBeSwapped, selected);
            toBeSwapped = null;
        }
    }

    public bool swapSlots(InventorySlot a, InventorySlot b)
    {
        if (!(a == weaponSlot || b == weaponSlot))
        {
            a.swap(b);
            return true;
        }
        else // need to equip new weapon
        {
            a = (a == weaponSlot) ? b : a;
            if (a.item.isWeapon())
            {
                a.swap(weaponSlot);
                player.pc.equipNewWeapon(weaponSlot.item.getItem());
                return true;
            }
        }
        return false;
    }
    public bool addItem(InventoryItem item)
    {
        foreach (var slot in inventory)
        {
            if (slot.isEmpty)
            {
                slot.addItem(item);
                return true;
            }
        }
        return false;
    }

    private void createInventory()
    {
        if (inventorySize == 0) inventorySize = 45;
        inventory = new List<InventorySlot>();
        for (int i = 0; i < inventorySize; ++i)
        {
            var invSlot = Instantiate(invSlotPrefab);
            invSlot.name = "InvSlot" + i;
            var invSlotComp = invSlot.GetComponent<InventorySlot>();
            invSlotComp.inventory = this;
            var invGrid = transform.Find("ItemGrid");
            invSlot.transform.SetParent(invGrid);
            inventory.Add(invSlotComp);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;

        input = GetComponent<PlayerInput>();
        Player.hookInputAction(controls.UI.MenuExit, OnMenuExit);
        Player.hookInputAction(controls.UI.Cancel, OnMenuExit);
        Player.hookInputAction(controls.UI.Submit, OnSubmit);

        player = FindObjectOfType<Player>();
        weaponSlot = transform.Find("WeaponSlot").GetComponent<InventorySlot>();
        weaponSlot.inventory = this;
        weaponSlot.addItem(new WeaponInventoryItem(defaultWeapon));

        if (inventory == null) createInventory();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
