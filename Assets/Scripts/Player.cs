// File manages user input
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // camera
    public Camera cam;
    float camOffset = 5f;
    float camHeight = 10f;
    float camAngle = 60f;

    // other components
    PlayerControls controls; // for getting control inputs
    PlayerInput input;
    GrayCharacterController pc; // player character controller
    public InventoryMenu invMenu;

    // state keeping to ensure smooth controls
    float INPUT_DELAY = .3f;
    Vector3 displacement;
    bool isSprintHeld;
    bool isSprintToggled;
    float lastMove;
    float lastAttack;
    float lastRoll;

    // player inventory
    List<InventoryItem> inventory;
    Weapon equipedWeapon;

    #region Input systems
    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
        }
        controls.gameplay.Enable();
    }
    public void OnDisable()
    {
        controls.gameplay.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        lastMove = Time.time;
        var direction = context.ReadValue<Vector2>();
        displacement = new Vector3(direction.x, 0.0f, direction.y);
        pc.Move(displacement);
    }

    public void OnSprintToggle(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isSprintToggled = !isSprintToggled;
            pc.sprint = isSprintToggled;
        }
    }

    public void OnSprintHold(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isSprintHeld = true;
            pc.sprint = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isSprintHeld = false;
            pc.sprint = false;
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && (Time.time - lastRoll) > INPUT_DELAY)
        {
            pc.Roll();
            lastRoll = Time.time;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && (Time.time - lastAttack) > INPUT_DELAY)
        {
            pc.Attack();
            lastAttack = Time.time;
        }
    }

    public void OnInventoryMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            input.SwitchCurrentActionMap("UI");
            Time.timeScale = 0;
            invMenu.gameObject.SetActive(true);
        }
    }

    public void OnInventoryMenuExit()
    {
        input.SwitchCurrentActionMap("gameplay");
        Time.timeScale = 1;
        invMenu.gameObject.SetActive(false);
    }
    #endregion

    public float lastInput
    {
        get
        {
            if (isSprintHeld) return Time.time;
            return Mathf.Max(Mathf.Max(lastMove, lastAttack), Mathf.Max(lastRoll, 0));
        }
    }
    public static void hookInputAction(InputAction action, System.Action<InputAction.CallbackContext> callback)
    {
        if (!action.enabled)
            action.Enable();
        action.started += callback;
        action.performed += callback;
        action.canceled += callback;
    }


    void Start()
    {
        // get components
        input = GetComponent<PlayerInput>();
        pc = GetComponent<GrayCharacterController>();

        hookInputAction(controls.gameplay.Movement, OnMovement);
        hookInputAction(controls.gameplay.SprintToggle, OnSprintToggle);
        hookInputAction(controls.gameplay.SprintHold, OnSprintHold);
        hookInputAction(controls.gameplay.Roll, OnRoll);
        hookInputAction(controls.gameplay.Attack, OnAttack);
        hookInputAction(controls.gameplay.InventoryMenu, OnInventoryMenu);


        lastMove = -INPUT_DELAY - 1;
        lastAttack= -INPUT_DELAY - 1;
        lastRoll= -INPUT_DELAY - 1;
        inventory = new List<InventoryItem>();

        invMenu.gameObject.SetActive(false);
        invMenu.player = this;
    }
    void Update()
    {
        // auto untoggle sprint
        if (isSprintToggled && (Time.time - lastMove) > .05f && !isSprintHeld)
        {
            isSprintToggled = false;
            pc.sprint = false;
        }
        // set last move
        if (displacement.magnitude > 0)
        {
            lastMove = Time.time;
        }
        camTransform();
    }

    private void camTransform()
    {
        cam.transform.position = transform.position + new Vector3(0f, camHeight, -camOffset);
        cam.transform.rotation = Quaternion.AngleAxis(camAngle, Vector3.right);
    }
}
