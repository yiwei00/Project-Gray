using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Weapon : MonoBehaviour
{
    public static Weapon BasicWeapon;
    public List<Effect> weaponEffects;

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
        if (alreadyHit.Find((x) => x == other)) return;
        alreadyHit.Add(other);
        Debug.Log(string.Format("Hit {0}", other.gameObject.name));
        var character = other.GetComponent<GrayCharacterController>();
        character.applyEffect(weaponEffects);
    }

    // fires projectile for projectile weapons
    public void ProjectileFire()
    {

    }

    void Start()
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
    }
}
