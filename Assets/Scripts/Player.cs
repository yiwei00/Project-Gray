using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, PlayerControls.IGameplayActions
{
    // public members
    public float rotationSpeed = 5;
    // use the method getMovementSpeed to get actual speed (need to factor in controller rates)
    public float baseSpeed = 10;

    // private members
    PlayerControls controls; // for getting control inputs
    Vector3 MovementDirection; // set by player controls, normalized by default

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
    }

    // Update is called once per frame
    void Update()
    {
        // updates movement and rotation
        updateTransform();
    }

    // private methods:
    private void updateTransform()
    {
        if (MovementDirection != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(MovementDirection, Vector3.up);
            // when rotating, have player move slower
            if (transform.rotation == rot)
            {
                transform.position += baseSpeed * MovementDirection * Time.deltaTime;
            }
            else
            {
                transform.position += .3f * baseSpeed * MovementDirection * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 100 * rotationSpeed * Time.deltaTime);
            }
        }
    }

    /* * * * * * * * * * *
     * 
     * Public Methods Below:
     * 
     * * * * * * * * * * */
    // Get actual movement speed of character, factoring in controller tilt
    public float getMovementSpeed()
    {
        float controllerSpeed = MovementDirection.magnitude;
        return controllerSpeed * baseSpeed;
    }
}
