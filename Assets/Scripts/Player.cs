// File manages user input
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, PlayerControls.IGameplayActions
{

    // camera
    public Camera cam;
    float camOffset = 5f;
    float camHeight = 10f;
    float camAngle = 60f;

    PlayerControls controls; // for getting control inputs
    GrayCharacterController pc; // player character controller

    float INPUT_DELAY = .3f;
    Vector3 displacement;
    bool isSprintHeld;
    float lastMove;
    float lastAttack;
    float lastRoll;

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
        displacement = new Vector3(direction.x, 0.0f, direction.y);
        pc.Move(displacement);
    }

    public void OnSprintToggle(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            pc.sprint = !pc.sprint;
        }
    }

    public void OnSprintHold(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isSprintHeld = true;
            pc.sprint = true;
        }
        else if( context.phase == InputActionPhase.Canceled)
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
    #endregion

    public float lastInput
    {
        get
        {
            if (isSprintHeld) return Time.time;
            return Mathf.Max(Mathf.Max(lastMove, lastAttack), Mathf.Max(lastRoll, 0));
        }
    }

    void Start()
    {
        // get components
        pc = GetComponent<GrayCharacterController>();

        lastMove= -INPUT_DELAY - 1;
        lastAttack= -INPUT_DELAY - 1;
        lastRoll= -INPUT_DELAY - 1;
    }
    void Update()
    {
        // auto untoggle sprint
        if ((Time.time - lastMove) > .05f && !isSprintHeld)
        {
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
