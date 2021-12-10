using UnityEngine;

public class AI_2hsword : MonoBehaviour
{
    float combatDistance;
    float lastRoll;
    float rollCD;

    float lastAttack;
    float atkCD;

    public int powerLevel;

    bool spawnedLoot;

    Player player;
    GrayCharacterController character;
    Hitpoint hitpoint;
    void Start()
    {
        player = Player.Instance;

        spawnedLoot = false;

        character = GetComponent<GrayCharacterController>();
        hitpoint = GetComponent<Hitpoint>();

        hitpoint.adjustToLevel(powerLevel);

        lastRoll = -Mathf.Infinity;
        lastAttack = -Mathf.Infinity;

        combatDistance = 1.5f;
        rollCD = 5;

        atkCD = 3;
    }

    // AI script, aka big ole if else blocks
    void Update()
    {
        if (character.dead && !spawnedLoot)
        {
            spawnedLoot = true;
            LootTable.spawnLoots(powerLevel, transform.position);
        }
        if (character.weapon)
            character.weapon.powerAmp = 5f*Mathf.Log(1+powerLevel)*Mathf.Pow(1+powerLevel, 1/16f);
        // dont do anything if attacking
        if (character.isAttacking()) return;
        // zero out movement first
        character.ZeroMovement();
        // get player dist
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // get player dir
        var playerDir = (player.transform.position - transform.position).normalized;
        character.TurnTo(playerDir);
        // dormant if player is too far
        if (distToPlayer > 15) 
            return;
        else if (distToPlayer > combatDistance) // don't move to player if they're too close
            character.Move(playerDir);
        if (distToPlayer > 7) // run towards player
        {
            character.sprint = true;
        }
        else
        {
            character.sprint = false;
        }
        // within combat range
        if (distToPlayer <= combatDistance)
        {
            if ((player.pc.attackState == 2) && ((Time.time - lastRoll) > rollCD)) // avoid attack!
            {
                lastRoll = Time.time;
                character.Roll();
                return;
            }
            // technically cosine angle but what's the difference really?
            float angleToPlayer = Vector3.Dot(playerDir, transform.forward);
            if ((angleToPlayer > .95) && ((Time.time - lastAttack) > atkCD))
            {
                lastAttack = Time.time;
                character.Attack();
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Enemy Poofed");
    }
}
