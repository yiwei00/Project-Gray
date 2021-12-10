// defines the inventory and it's associated menu
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InventoryMenu : MonoBehaviour
{
    public int inventorySize;
    public GameObject invSlotPrefab;
    public int defaultWeapon;

    List<InventorySlot> inventory;
    InventorySlot weaponSlot;
    InventorySlot toBeSwapped;

    WorldManager world;

    PlayerControls controls; // for getting control inputs
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
    public void OnInvExit(InputAction.CallbackContext context)
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
            setupSwap(EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>());
        }
    }
    public void OnDropItem(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            dropItem(EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>());
        }
    }    

    #endregion

    public void dropItem(InventorySlot selected)
    {
        if (selected.isEmpty || selected == weaponSlot) return;
        var newLootItem = Instantiate(Prefabs.Get.lootitem);
        newLootItem.GetComponent<Loot>().item = selected.popItem();
        newLootItem.transform.parent = null;
        newLootItem.transform.position = player.transform.position;

        world.SaveState();
    }

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
            world.SaveState();
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
    public bool addItem(Item item)
    {
        foreach (var slot in inventory)
        {
            if (slot.isEmpty)
            {
                slot.addItem(item);
                world.SaveState();
                return true;
            }
        }
        return false;
    }

    private void createInventory()
    {
        if (inventory != null)
        {
            for (int i = 0; i < inventory.Count; ++i)
            {
                Destroy(inventory[i].gameObject);
                inventory[i] = null;
            }
        }
        inventory = null;
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
        world = WorldManager.Instance;

        Player.hookInputAction(controls.UI.InvExit, OnInvExit);
        Player.hookInputAction(controls.UI.Cancel, OnInvExit);
        Player.hookInputAction(controls.UI.DropItem, OnDropItem);
        Player.hookInputAction(controls.UI.Submit, OnSubmit);

        weaponSlot = transform.Find("WeaponSlot").GetComponent<InventorySlot>();
        weaponSlot.inventory = this;
        weaponSlot.addItem(new WeaponItem(Instantiate(world.weaponDict[defaultWeapon])));

        if (inventory == null) createInventory();

        gameObject.SetActive(false);
    }

    [System.Serializable]
    public class SerializedInv
    {
        public string weapSlotJson;
        public List<string> inventory;
        public SerializedInv(InventoryMenu inv)
        {
            weapSlotJson = inv.weaponSlot.item.toJson();
            inventory = inv.inventory.ConvertAll(slot => slot.item.toJson());
        }
    }

    public string toJson()
    {
        return JsonUtility.ToJson(new SerializedInv(this));
    }


    // reload inventory based on json
    public void fromJson(string json)
    {
        // reset swapf
        toBeSwapped = null;

        var invMenu = JsonUtility.FromJson<SerializedInv>(json);
        weaponSlot.popItem().destroy();
        weaponSlot.addItem(Item.fromJson(invMenu.weapSlotJson));
        inventorySize = invMenu.inventory.Count;
        createInventory();
        for (int i = 0; i < inventorySize; ++i)
        {
            inventory[i].addItem(Item.fromJson(invMenu.inventory[i]));
        }
    }
}
