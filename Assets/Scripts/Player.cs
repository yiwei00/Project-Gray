using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, PlayerControls.IGameplayActions
{
    PlayerControls controls;

    Vector3 MovementDirection;
    float velocity;

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
        velocity = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += velocity * MovementDirection * Time.deltaTime;
    }
}
