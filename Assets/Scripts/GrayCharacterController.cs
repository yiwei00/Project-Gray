using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrayCharacterController : MonoBehaviour
{
    // #defines
    float ROT_MULTIPLIER = 180 / Mathf.PI;

    // player settings
    public float rotationSpeed = 360f;
    public float baseSpeed = 5.0f;

    float sprintMult = 2.0f;
    float curSpeed = 0.0f;

    // private members
    bool isSprinting;
    float lastMovementInputTime;
    Vector3 displacement; // set by player controls, normalized by default

    // state machine
    bool sprintToggled;
    bool rollTriggered;
    bool rollStarted;
    bool attackTriggered;
    bool attackStarted;

    // animation related
    Animator animator;
    CharacterController characterController;
    int move_state_hash;
    int roll_trigger_hash;
    int attack1_trigger_hash;

    #region Endpoints

    public void Move(Vector3 displacement)
    {
        this.displacement = displacement;
    }

    public bool sprint
    {
        get => sprintToggled;
        set => sprintToggled = value;
    }


    public void Roll()
    {
        rollTriggered = true;
    }

    public void Attack()
    {
        attackTriggered = true;
    }
    
    // Get actual movement speed of character, factoring in controller tilt
    public float moveSpeed
    {
        get => curSpeed;
    }
    #endregion

    void Start()
    {
        // default values
        sprintToggled = false;
        displacement = new Vector3();

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
        handleAnim();
    }

    // private methods:
    private void handleTransform()
    {
        Vector3 displacement = Vector3.zero;
        Quaternion rot = transform.rotation;
        Quaternion moveRot = transform.rotation;
        // input is given
        if ((this.displacement != Vector3.zero))
        {
            moveRot = Quaternion.LookRotation(this.displacement, Vector3.up);
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
            Debug.Log(rotationSpeed);
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
                displacement = baseSpeed * this.displacement;
            }
            else // when rotational angle is big, have player move slower as they'll be rotating
            {
                Func<float, float> angMap = (x) => (1 + (-x / 180));
                Func<float, float> expCurve = (x) => (Mathf.Pow(10, x) - 1) / (10 - 1);
                float slowDown = expCurve(angMap(rotAngle));
                displacement = slowDown * baseSpeed * this.displacement;
            }


            // handle sprint
            isSprinting = sprintToggled && displacement.magnitude >= (baseSpeed * .5f);
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
}