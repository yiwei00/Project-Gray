using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, PlayerControls.IGameplayActions
{
    // #defines
    float ROT_MULTIPLIER = 360/Mathf.PI;
    float INPUT_DELAY = .3f;

    // camera
    public Camera cam;

    float camOffset = 5f;
    float camHeight = 10f;
    float camAngle = 60f;

    // player settings
    public float rotationSpeed = 360f;
    public float baseSpeed = 5.0f;

    float sprintMult = 2.0f;
    float curSpeed = 0.0f;

    // private members
    bool isSprinting;
    float lastMovementInputTime;
    Vector3 MovementDirection; // set by player controls, normalized by default

    // control related
    bool sprintToggled;
    bool sprintHeld;
    bool rollTriggered;
    bool rollStarted;
    bool attackTriggered;
    bool attackStarted;
    PlayerControls controls; // for getting control inputs

    // animation related
    Animator animator;
    CharacterController characterController;
    int move_state_hash;
    int roll_trigger_hash;
    int attack1_trigger_hash;

    #region Input systems
    float lastAttack;
    float lastRoll;
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
        if (context.phase == InputActionPhase.Started)
        {
            sprintToggled = !sprintToggled;
        }
    }

    public void OnSprintHold(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
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
        if (context.phase == InputActionPhase.Started && (Time.time - lastRoll) > INPUT_DELAY)
        {
            rollTriggered = true;
            lastRoll = Time.time;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && (Time.time - lastAttack) > INPUT_DELAY)
        {
            attackTriggered = true;
            lastAttack = Time.time;
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
        attack1_trigger_hash = Animator.StringToHash("attack1_trigger");
    }

    // Update is called once per frame
    void Update()
    {
        handleTransform();
        handleAttack();
        handleCam();
        handleAnim();
    }

    // private methods:
    private void handleTransform()
    {
        Vector3 displacement = Vector3.zero;
        Quaternion rot = transform.rotation;
        Quaternion moveRot = transform.rotation;
        // input is given
        if ((MovementDirection != Vector3.zero))
        {
            moveRot = Quaternion.LookRotation(MovementDirection, Vector3.up);
            lastMovementInputTime = Time.time;
        }
        else // no input, auto un-sprint
        {
            if ((Time.time - lastMovementInputTime) > .05f)
                sprintToggled = false;
        }
        // allow movement
        if (canMove())
        {
            if (rollTriggered)
            {
                animator.SetTrigger(roll_trigger_hash);
                rollStarted = true;
                rollTriggered = false;
            }
            // rotation
            if (rotationSpeed < 0)
            {
                rot = Quaternion.RotateTowards(transform.rotation, moveRot, 360f);
            }
            else
            {
                rot = Quaternion.RotateTowards(transform.rotation, moveRot, ROT_MULTIPLIER * rotationSpeed * Time.deltaTime);
            }
            float rotAngle = Quaternion.Angle(rot, moveRot);
            // calculate rotation and displacement
            if (rotAngle < 10) // when rotational angle is small, simply move along
            {
                displacement = baseSpeed * MovementDirection;
            }
            else // when rotational angle is big, have player move slower as they'll be rotating
            {
                Func<float, float> angMap = (x) => (1 + (-x / 180));
                Func<float, float> expCurve = (x) => (Mathf.Pow(10, x) - 1) / (10 - 1);
                float slowDown = expCurve(angMap(rotAngle));
                displacement = slowDown * baseSpeed * MovementDirection;
            }
            

            // handle sprint
            isSprinting = (sprintToggled || sprintHeld) && displacement.magnitude >= (baseSpeed * .5f);
            if (isSprinting)
            {
                displacement *= sprintMult;
            }
        }
        // handle rotation first
        transform.rotation = rot;
        // roll
        if (isRolling()) // displacement for rolling
        {
            displacement = transform.forward * baseSpeed * .8f;
        }
        characterController.SimpleMove(displacement);
        curSpeed = displacement.magnitude;
        // Debug.Log(string.Format("Speed: {0}", curSpeed));
    }

    private void handleAttack()
    {
        if (attackTriggered)
        {
            if (canAttack())
            {
                animator.SetTrigger(attack1_trigger_hash);
                attackStarted = true;
            }
            attackTriggered = false;
        }
    }
    private void handleCam()
    {
        cam.transform.position = transform.position + new Vector3(0f, camHeight, -camOffset);
        cam.transform.rotation = Quaternion.AngleAxis(camAngle, Vector3.right);
    }

    private void handleAnim()
    {
        if (isAttacking())
            return;
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            rollStarted = false;
            return true;
        }
        if (rollStarted)
            return true;
        return false;
    }

    private bool inAttackAnim()
    {
        return (
               animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("SprintAttack")
        );
    }
    private bool isAttacking()
    {
        if (inAttackAnim())
        {
            attackStarted = false;
            return true;
        }
        if (attackStarted)
            return true;
        return false;
    }

    private bool isInvincible()
    {
        return isRolling();
    }

    private bool canMove()
    {
        return !isRolling() && !isAttacking();
    }

    private bool canAttack()
    {
        return !isSprinting && !isRolling();
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
