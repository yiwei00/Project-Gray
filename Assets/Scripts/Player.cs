// File manages user input
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private static Player _instance;

    public static Player Instance
    {
        get => _instance;
    }

    // camera
    public Camera cam;
    float camOffset = 4f;
    float camHeight = 7f;
    float camAngle = 60f;

    // other components
    PlayerControls controls; // for getting control inputs
    PlayerInput input;
    GrayCharacterController _pc; // player character controller
    public InventoryMenu invMenu;
    public PauseMenu pauseMenu;

    // state keeping to ensure smooth controls
    float INPUT_DELAY = .3f;
    Vector3 displacement;
    bool isSprintHeld;
    bool isSprintToggled;
    float lastMove;
    float lastAttack;
    float lastRoll;

    // to help reset
    Vector3 _initPos;
    Quaternion _initRot;

    private int _totalExp;

    Hitpoint _hp;

    public Hitpoint hp
    {
        get => _hp;
    }

    public static int xpToLvl(int xp)
    {
        return Mathf.FloorToInt(Mathf.Pow((.5f*xp), 1/ 2.35f));
    }

    public static int lvlToXp(int lvl)
    {
        return Mathf.FloorToInt(2*Mathf.Pow(lvl, 2.35f));
    }
    public int totalExp
    {
        get => _totalExp;
    }
    public int level
    {
        get {
            int lvl = xpToLvl(_totalExp);
            return _totalExp;
        }
    }

    public void gainExp(int n)
    {
        _totalExp += n;
    }
    
    public int xpToNextLvl()
    {
        return Mathf.CeilToInt(xpToLvl(level + 1) - _totalExp);
    }

    public Vector3 initPos
    {
        get => _initPos;
        set => _initPos = value;
    }

    public Quaternion initRot
    {
        get => _initRot;
        set => _initRot = value;
    }

    public GrayCharacterController pc
    {
        get => _pc;
        private set => _pc = value;
    }

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

    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            input.SwitchCurrentActionMap("UI");
            Time.timeScale = 0;
            pauseMenu.pauseText = "Paused";
            pauseMenu.gameObject.SetActive(true);
        }
    }

    public void OnInventoryMenuExit()
    {
        if (pc.dead && pauseMenu.gameObject.activeInHierarchy) return;
        input.SwitchCurrentActionMap("gameplay");
        Time.timeScale = 1;
        if (invMenu.gameObject.activeInHierarchy)
            invMenu.gameObject.SetActive(false);
        if (pauseMenu.gameObject.activeInHierarchy)
            pauseMenu.gameObject.SetActive(false);
    }

    public void OnPickup(InputAction.CallbackContext context)
    { 
        if (context.phase == InputActionPhase.Started)
        {
            pickUpLoot();
        }
    }

    #endregion

    public void pickUpLoot()
    {
        var colliders = Physics.OverlapSphere(transform.position, 3);
        float minDist = Mathf.Infinity;
        Collider closestLoot = null;
        foreach (var collider in colliders)
        {
            if (collider.gameObject.GetComponent<Loot>())
            {
                float dist = (collider.transform.position - transform.position).magnitude;
                if (dist < minDist)
                {
                    closestLoot = collider;
                    minDist = dist;
                }
            }
        }
        if (minDist < Mathf.Infinity)
        {
            var loot = closestLoot.gameObject.GetComponent<Loot>();
            if (loot)
            {
                invMenu.addItem(loot.item);
                Destroy(closestLoot.gameObject);
            }
        }
    }
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

    public void resetPlayer()
    {
        displacement = Vector3.zero;
        isSprintHeld = false;
        isSprintToggled = false;
        lastMove = 0f;
        lastAttack = 0f;
        lastRoll = 0f;
        pc.cc.enabled = false;
        transform.position = _initPos;
        pc.cc.enabled = true;
        transform.rotation = _initRot;
        pc.resetCharacter();
    }
    private void Awake()  // singleton class
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else
        {
            _instance = this;

            // get components
            input = GetComponent<PlayerInput>();
            pc = GetComponent<GrayCharacterController>();
            _hp = GetComponent<Hitpoint>();

            lastMove = -INPUT_DELAY - 1;
            lastAttack = -INPUT_DELAY - 1;
            lastRoll = -INPUT_DELAY - 1;
        }
    }
    
    void Start()
    {
        hookInputAction(controls.gameplay.Movement, OnMovement);
        hookInputAction(controls.gameplay.SprintToggle, OnSprintToggle);
        hookInputAction(controls.gameplay.SprintHold, OnSprintHold);
        hookInputAction(controls.gameplay.Roll, OnRoll);
        hookInputAction(controls.gameplay.Attack, OnAttack);
        hookInputAction(controls.gameplay.InventoryMenu, OnInventoryMenu);
        hookInputAction(controls.gameplay.Pickup, OnPickup);
        hookInputAction(controls.gameplay.PauseMenu, OnPauseMenu);

        invMenu.gameObject.SetActive(false);
        invMenu.player = this;

        _initPos = transform.position;
        _initRot = transform.rotation;

        pc.equipNewWeapon(invMenu.equippedWeapon);
    }
    void Update()
    {
        if (pc.dead)
        {
            input.SwitchCurrentActionMap("UI");
            Time.timeScale = 0;
            pauseMenu.pauseText = "Game Over";
            pauseMenu.gameObject.SetActive(true);
            return;
        }
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
