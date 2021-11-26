using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, PlayerControls.IGameplayActions
{
    // #defines
    int ROT_MULTIPLIER = 100;

    // camera
    public Camera cam;

    float camOffset = 5f;
    float camHeight = 10f;
    float camAngle = 60f;

    // player settings
    public float rotationSpeed = 5.0f;
    public float baseSpeed = 10.0f;

    float sprintMult = 2.0f;
    float rollMult = 4.0f;
    float curSpeed = 0.0f;

    // private members
    bool isSprinting;
    float lastMoved;
    Vector3 MovementDirection; // set by player controls, normalized by default

    // control related
    bool sprintToggled;
    bool sprintHeld;
    bool rollTriggered;
    PlayerControls controls; // for getting control inputs

    // animation related
    Animator animator;
    CharacterController characterController;
    int move_state_hash;
    int roll_trigger_hash;

    #region Input systems
    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            controls.gameplay.SetCallbacks(this);
        }
        controls.gameplay.Enable();
    }
    public void OnDisable()
    {
        controls.gameplay.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();
        MovementDirection = new Vector3(direction.x, 0.0f, direction.y);
    }

    public void OnSprintToggle(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            sprintToggled = !sprintToggled;
        }
    }

    public void OnSprintHold(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            sprintHeld = true;
        }
        else if( context.phase == InputActionPhase.Canceled)
        {
            sprintHeld = false;
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && canMove())
        {
            rollTriggered = true;
        }
    }
    #endregion

    void Start()
    {
        // default values
        sprintToggled = false;
        sprintHeld = false;
        MovementDirection = new Vector3();

        // animation setup
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        move_state_hash = Animator.StringToHash("move_state");
        roll_trigger_hash = Animator.StringToHash("roll_trigger");
    }

    // Update is called once per frame
    void Update()
    {
        handleTransform();
        handleCam();
        handleAnim();
    }

    // private methods:
    private void handleTransform()
    {
        Vector3 displacement = Vector3.zero;
        Quaternion rot = transform.rotation;
        if (rollTriggered)
        {
            animator.SetTrigger(roll_trigger_hash);
            rollTriggered = false;
        }
        if ((MovementDirection != Vector3.zero) && !isRolling())
        {
            Quaternion moveRot = Quaternion.LookRotation(MovementDirection, Vector3.up);
            float rotAngle = Quaternion.Angle(transform.rotation, moveRot);
            // when rotational angle is big, have player move slower
            if (rotAngle < 10)
            {
                displacement = baseSpeed * MovementDirection;
            }
            else
            {
                float slowDown = -rotAngle / 180 + 1;
                displacement = slowDown * baseSpeed * MovementDirection;
            }
            rot = Quaternion.RotateTowards(transform.rotation, moveRot, ROT_MULTIPLIER * rotationSpeed * Time.deltaTime);

            // handle sprint
            isSprinting = (sprintToggled || sprintHeld) && displacement.magnitude >= (baseSpeed * .5f);
            if (isSprinting)
            {
                displacement *= sprintMult;
            }
            lastMoved = Time.time;
        }
        else // no input is detected
        {
            if ((Time.time - lastMoved) > .05f)
                sprintToggled = false;
            if (isRolling())
            {
                displacement = transform.forward * rollMult;
            }
        }
        transform.rotation = rot;
        characterController.SimpleMove(displacement);
        curSpeed = displacement.magnitude;
        // Debug.Log(string.Format("Speed: {0}", curSpeed));
    }

    private void handleCam()
    {
        cam.transform.position = transform.position + new Vector3(0f, camHeight, -camOffset);
        cam.transform.rotation = Quaternion.AngleAxis(camAngle, Vector3.right);
    }

    private void handleAnim()
    {
        int move_state = 0;
        if (isSprinting)
        {
            move_state = 3;
        }
        else if (curSpeed >= baseSpeed * .5f)
        {
            move_state = 2;
        }
        else if (curSpeed > 0)
        {
            move_state = 1;
        }
        animator.SetInteger(move_state_hash, move_state);
    }

    private bool isRolling()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Roll");
    }

    private bool canMove()
    {
        return !isRolling();
    }

    /* * * * * * * * * * *
     * 
     * Public Methods Below:
     * 
     * * * * * * * * * * */
    // Get actual movement speed of character, factoring in controller tilt
    public float getMovementSpeed()
    {
        return curSpeed;
    }
}
