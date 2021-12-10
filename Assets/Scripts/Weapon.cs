using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public static Weapon BasicWeapon;
    public List<Effect> weaponEffects;
    public Sprite icon;
    public string description;
    public LootRarity rarity;

    private float _powerAmp;

    public float powerAmp
    {
        get => (_powerAmp-1) * 100;
        set => _powerAmp = (1 + value/100);
    }

    // collidable weapons are like swords
    // non-collidable weapons are like wands
    public bool isCollidable;
    List<Collider> alreadyHit;

    GameObject attachedTo;
    GrayCharacterController attachedCharacter;

    public GameObject holder
    {
        get => attachedTo;
        set => attachedTo = value;
    }
   
    // positional
    public Vector3 attachPos;
    public Vector3 attachRot;

    public void newAtkCycle()
    {
        alreadyHit = new List<Collider>();
    }
    public void OnTriggerEnter(Collider other)
    {
        // not ready yet
        if (!isCollidable || !attachedCharacter.inAttackAnim()) return;
        if (other.gameObject == attachedTo) return;
        var targetChar = other.gameObject.GetComponent<GrayCharacterController>();
        if (!targetChar) return;
        if (targetChar.isRolling()) return;
        if (alreadyHit.Find((x) => x == other)) return;
        alreadyHit.Add(other);
        Debug.Log(string.Format("Hit {0}", other.gameObject.name));
        var character = other.GetComponent<GrayCharacterController>();
        character.applyEffect(weaponEffects, _powerAmp);
    }

    // fires projectile for projectile weapons
    public void ProjectileFire()
    {

    }

    void OnEnable()
    {
        if (isCollidable)
        {
            var collider = GetComponent<Collider>();
            if (!collider)
            {
                throw new UnityException("Missing collider component for collidable weapon");
            }
            if (!collider.isTrigger)
            {
                throw new UnityException("Weapon collider need to be trigger");
            }
        }
        if (!attachedTo)
            attachedTo = transform.parent.gameObject;
        attachedCharacter = attachedTo.GetComponent<GrayCharacterController>();
        if (!attachedCharacter)
            throw new UnityException("Weapon not attached to viable character");
        transform.parent = attachedCharacter.hand;
        transform.localPosition = attachPos;
        transform.localRotation = Quaternion.Euler(attachRot);

        powerAmp = 0;
    }
}
