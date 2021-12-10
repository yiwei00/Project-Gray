using System.Collections.Generic;
using UnityEngine;

public class GrayCharacterController : MonoBehaviour
{
    // #defines
    float ROT_MULTIPLIER = 180 / Mathf.PI;

    // serialized members:
    public float rotationSpeed = 9.4f;
    public float baseSpeed = 5.0f;
    public Transform hand = null;
    public GameObject defaultWeapon;

    float sprintMult = 2.0f;
    float curSpeed = 0.0f;

    // private members
    bool isSprinting;
    Vector3 displacement; // set by player controls, normalized by default
    Vector3 rotDir; // like displacement but for rotation only

    // state machine
    bool sprintToggled;
    bool rollTriggered;
    bool rollStarted;
    bool attackTriggered;
    bool attackStarted;
    bool newWeaponFlag;
    bool _isDead;
    GameObject newWeaponObj;

    public bool dead
    {
        get => _isDead;
    }

    public bool alive
    {
        get => !_isDead;
    }

    // get attack state for ai
    public int attackState
    {
        get
        {
            if (attackTriggered) return 1;
            if (attackStarted) return 2;
            if (inAttackAnim()) return 3;
            return 0;
        }
    }

    // animation related
    bool hasAnimator;
    Animator animator;
    CharacterController characterController;
    int move_state_hash;
    int roll_trigger_hash;
    int attack1_trigger_hash;
    int death_trigger_hash;

    // hitpoints
    Hitpoint hitpoint;

    // effects
    [SerializeField]
    List<Effect> activeEffects; // TODO: implement custom priority queue to speed this up
    float mSpeedFromEffects;
    float rSpeedFromEffects;

    // weapon n equpiment
    Weapon equippedWeapon;

    public Weapon weapon
    {
        get => equippedWeapon;
    }

    #region Endpoints

    public void Move(Vector3 displacement)
    {
        this.displacement = displacement;
    }

    public void TurnTo(Vector3 rotDir)
    {
        this.rotDir = rotDir;
    }

    // zero out movement
    public void ZeroMovement()
    {
        displacement = Vector3.zero;
        rotDir = Vector3.zero;
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
        // get character controller
        characterController = GetComponent<CharacterController>();
        // animation setup
        animator = GetComponent<Animator>();
        hasAnimator = animator != null;
        if (hasAnimator)
        {
            move_state_hash = Animator.StringToHash("move_state");
            roll_trigger_hash = Animator.StringToHash("roll_trigger");
            attack1_trigger_hash = Animator.StringToHash("attack1_trigger");
            death_trigger_hash = Animator.StringToHash("death_trigger");
        }
        // got hands?
        if (!hand) hand = transform;
        // hitpoints
        hitpoint = GetComponent<Hitpoint>();
        // active effects
        activeEffects = new List<Effect>();
    }

    // Update is called once per frame
    void Update()
    {
        //handle health and death at start
        handleHitpoint();
        if (dead)
        {
            if (hasAnimator && animator.GetCurrentAnimatorStateInfo(0).IsName("Sink"))
            {
                die();
            }
            return;
        }

        if (hasAnimator && !equippedWeapon) // equip weapon if has animation
        {
            equippedWeapon = GetComponentInChildren<Weapon>();
            if (!equippedWeapon) equippedWeapon = hand.gameObject.GetComponentInChildren<Weapon>();
            if (!equippedWeapon && defaultWeapon)
            {
                equipNewWeapon(defaultWeapon);
            }
        }
        handleWeaponEquip();
        handleEffects();
        handleTransform();
        handleAttack();
        if (hasAnimator)
            handleAnim();
        Debug.Log(string.Format("{0}: HP: {1}, Active Effect Count: {2}",gameObject.name,  hitpoint.curHP, activeEffects.Count));
    }

    public bool equipNewWeapon(GameObject newWeapon)
    {
        var weapon = newWeapon.GetComponent<Weapon>();
        if (!weapon) return false;

        newWeaponFlag = true;
        newWeaponObj = newWeapon;
        return true;
    }

    // private methods:
    private void handleHitpoint()
    {
        if (alive && (hitpoint.curHP <= 0))
        {
            _isDead = true;
            if (hasAnimator)
            {
                animator.SetTrigger(death_trigger_hash);
            }
            else
            {
                // poof :(
                die();
            }
        }
    }
    private void handleTransform()
    {
        Vector3 displacement = Vector3.zero;
        Quaternion rot = transform.rotation;
        Quaternion moveRot = transform.rotation;
        float mspeed = mSpeedFromEffects + this.baseSpeed;
        float rspeed = rSpeedFromEffects + this.rotationSpeed;
        // input is given
        if (this.displacement != Vector3.zero)
        {
            moveRot = Quaternion.LookRotation(this.displacement, Vector3.up);
        }
        else if (rotDir != Vector3.zero)
        {
            moveRot = Quaternion.LookRotation(rotDir, Vector3.up);
        }
        // allow movement
        if (canMove())
        {
            if (rollTriggered && hasAnimator)
            {
                animator.SetTrigger(roll_trigger_hash);
                rollStarted = true;
                rollTriggered = false;
            }
            // rotation
            if (rspeed < 0)
            {
                rot = Quaternion.RotateTowards(transform.rotation, moveRot, 360f);
            }
            else
            {
                rot = Quaternion.RotateTowards(transform.rotation, moveRot, ROT_MULTIPLIER * rspeed * Time.deltaTime);
            }
            float rotAngle = Quaternion.Angle(rot, moveRot);
            // calculate rotation and displacement
            if (rotAngle < 10) // when rotational angle is small, simply move along
            {
                displacement = mspeed * this.displacement;
            }
            else // when rotational angle is big, have player move slower as they'll be rotating
            {
                System.Func<float, float> angMap = (x) => (1 + (-x / 180));
                System.Func<float, float> expCurve = (x) => (Mathf.Pow(10, x) - 1) / (10 - 1);
                float slowDown = expCurve(angMap(rotAngle));
                displacement = slowDown * mspeed * this.displacement;
            }


            // handle sprint
            isSprinting = sprintToggled && displacement.magnitude >= (mspeed * .5f);
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
            displacement = transform.forward * mspeed * .8f;
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
                if (hasAnimator)
                    animator.SetTrigger(attack1_trigger_hash);
                attackStarted = true;
                if (equippedWeapon)
                    equippedWeapon.newAtkCycle();
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

    private void handleEffects()
    {
        float mSpeedStaticDiff = 0.0f;
        float rSpeedStaticDiff = 0.0f;
        float mSpeedPercentDiff = 1.0f;
        float rSpeedPercentDiff = 1.0f;
        List<Effect> to_remove = new List<Effect>();
        foreach (Effect effect in activeEffects)
        {
            float duration = Mathf.Min(Time.deltaTime, effect.duration);
            effect.duration = effect.duration - duration;
            if (effect.duration <= 0)
            {
                to_remove.Add(effect);
            }
            if (Hitpoint.isHitpointEffect(effect))
            {
                hitpoint.applyEffect(effect, duration);
            }
            else
            {
                switch (effect.effectType)
                {
                    case EffectType.Move_Slowdown:
                        mSpeedStaticDiff -= effect.staticStrength;
                        mSpeedPercentDiff *= Mathf.Max(1 - effect.percentStrength, 0);
                        break;
                    case EffectType.Move_Speedup:
                        mSpeedStaticDiff += effect.staticStrength;
                        mSpeedPercentDiff *= effect.percentStrength;
                        break;
                    case EffectType.Turn_Slowdown:
                        rSpeedStaticDiff -= effect.staticStrength;
                        rSpeedPercentDiff *= Mathf.Max(1 - effect.percentStrength, 0);
                        break;
                    case EffectType.Turn_Speedup:
                        rSpeedStaticDiff += effect.staticStrength;
                        rSpeedPercentDiff *= effect.percentStrength;
                        break;
                }
            }
        }
        while ((activeEffects.Count > 0) && activeEffects[activeEffects.Count - 1].duration <= 0)
            activeEffects.RemoveAt(activeEffects.Count - 1);
        mSpeedFromEffects = mSpeedStaticDiff + (mSpeedPercentDiff - 1) * baseSpeed;
        rSpeedFromEffects = rSpeedStaticDiff + (rSpeedPercentDiff - 1) * rotationSpeed;
        mSpeedFromEffects = Mathf.Max(mSpeedFromEffects, -baseSpeed);
        rSpeedFromEffects = Mathf.Max(rSpeedFromEffects, -rotationSpeed);
    }

    private void die()
    {
        // TODO: lootdrop here.

        if (gameObject != Player.Instance.gameObject)
            Destroy(this.gameObject);
    }

    public void handleWeaponEquip()
    {
        if (newWeaponFlag && canMove())
        {
            // get new weapon
            var newWeapon = Instantiate(newWeaponObj);
            newWeaponFlag = false;
            newWeaponObj = null;
            // remove current weapon
            if (equippedWeapon)
            {
                var curWeapon = equippedWeapon.gameObject;
                Destroy(curWeapon);
            }

            equippedWeapon = newWeapon.GetComponent<Weapon>();
            newWeapon.transform.parent = transform;
            equippedWeapon.enabled = true;
        }

    }

    private void applySingleEffect(Effect e)
    {
        switch (e.effectType)
        {
            case EffectType.Heal:
            case EffectType.Pure_Damage:
            case EffectType.Physical_Damage:
            case EffectType.Magical_Damage:
                e.duration = 0;
                break;
        }
        foreach (var activeEffect in activeEffects)
        {
            if (activeEffect.name == e.name && activeEffect.effectType == e.effectType)
            {
                activeEffect.duration = Mathf.Max(activeEffect.duration, e.duration);
                activeEffect.staticStrength = Mathf.Max(activeEffect.staticStrength, e.staticStrength);
                activeEffect.percentStrength = Mathf.Max(activeEffect.percentStrength, e.percentStrength);
                return;
            }
        }
        activeEffects.Add(e.clone());
    }

    public void applyEffect(List<Effect> effects, float powerAmp = 1)
    {
        var toApply = effects.ConvertAll<Effect>(e => e.clone());
        foreach (var e in toApply)
        {
            e.staticStrength *= powerAmp;
            e.percentStrength *= powerAmp;
            applySingleEffect(e);
        }
        // sort desc
        activeEffects.Sort((a, b) => b.CompareTo(a));
    }


    public bool isRolling()
    {
        if (!hasAnimator) return false;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            rollStarted = false;
            return true;
        }
        if (rollStarted)
            return true;
        return false;
    }

    public bool inAttackAnim()
    {
        if (!hasAnimator) return false;
        return (
               animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("SprintAttack")
        );
    }
    public bool isAttacking()
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

    private bool canMove()
    {
        return !isRolling() && !isAttacking();
    }

    private bool canAttack()
    {
        return !isSprinting && !isRolling();
    }
}
