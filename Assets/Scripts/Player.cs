using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, PlayerControls.IGameplayActions
{
    // #defines
    int SPRINT_SPEED = 15;
    int RUN_SPEED = 5;
    int ROT_MULTIPLIER = 100;

    // camera
    public Camera cam;
    public float camOffset = 5f;
    public float camHeight = 10f;
    public float camAngle = 60f;

    // player settings
    public float rotationSpeed = 5;
    public float baseSpeed = 10;
    private float curSpeed = 0;

    // private members
    PlayerControls controls; // for getting control inputs
    Vector3 MovementDirection; // set by player controls, normalized by default

    // animation related
    Animator anim;
    int move_state_hash;

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
    #endregion

    void Start()
    {
        anim = GetComponent<Animator>();
        move_state_hash = Animator.StringToHash("move_state");
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
        if (MovementDirection != Vector3.zero)
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, moveRot, ROT_MULTIPLIER * rotationSpeed * Time.deltaTime);
        }
        transform.position += displacement * Time.deltaTime;
        curSpeed = displacement.magnitude;
    }

    private void handleCam()
    {
        cam.transform.position = transform.position + new Vector3(0f, camHeight, -camOffset);
        cam.transform.rotation = Quaternion.AngleAxis(camAngle, Vector3.right);
    }

    private void handleAnim()
    {
        int move_state = 0;
        if (curSpeed >= SPRINT_SPEED)
        {
            move_state = 3;
        }
        else if (curSpeed >= RUN_SPEED)
        {
            move_state = 2;
        }
        else if (curSpeed > 0)
        {
            move_state = 1;
        }
        anim.SetInteger(move_state_hash, move_state);
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
