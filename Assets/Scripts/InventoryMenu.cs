using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InventoryMenu : MonoBehaviour
{
    public int inventorySize;
    public GameObject invSlotPrefab;

    List<InventorySlot> inventory;
    PlayerControls controls; // for getting control inputs
    PlayerInput input;
    private Player _player;
    public Player player
    {
        get => _player;
        set => _player = value;
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
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        Player.hookInputAction(controls.UI.MenuExit, OnMenuExit);

        if (inventorySize == 0) inventorySize = 45;
        inventory = new List<InventorySlot>();
        for (int i = 0; i < inventorySize; ++i)
        {
            var invSlot = Instantiate(invSlotPrefab);
            invSlot.name = "InvSlot" + i;
            var invGrid = transform.Find("ItemGrid");
            invSlot.transform.SetParent(invGrid);
            inventory.Add(invSlot.GetComponent<InventorySlot>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("MENU!");
    }
}
