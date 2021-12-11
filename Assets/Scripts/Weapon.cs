using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedWeapon
{
    public int weaponID;
    public string weaponName;
    public List<string> weaponEffectsJson;
    public LootRarity rarity;
    public float powerAmp;

    public SerializedWeapon(Weapon w)
    {
        weaponID = w.weaponID;
        weaponName = w.weaponName;
        weaponEffectsJson = new List<string>();
        weaponEffectsJson.AddRange(w.weaponEffects.ConvertAll(e => e.toJson()));
        rarity = w.rarity;
        powerAmp = w.powerAmp;
    }
    public GameObject toWeaponObj()
    {
        var newObj = GameObject.Instantiate(WorldManager.Instance.weaponDict[weaponID]);
        newObj.SetActive(false);
        Weapon w = newObj.GetComponent<Weapon>();
        w.weaponName = weaponName;
        w.weaponEffects = weaponEffectsJson.ConvertAll(s => Effect.fromJson(s));
        w.rarity = rarity;
        w.powerAmp = powerAmp;

        return newObj;
    }
}

public class Weapon : MonoBehaviour
{
    WorldManager world = WorldManager.Instance;
    public int weaponID;
    public string weaponName;
    public List<Effect> weaponEffects;
    public LootRarity rarity;
    public float powerAmp
    {
        get => _powerAmp * 100;
        set => _powerAmp = value / 100;
    }

    public Sprite icon;
    public string description;

    // collidable weapons are like swords
    // non-collidable weapons are like wands
    public bool isCollidable;
    // gimmick to make sure to not hit the same collider twice
    List<Collider> alreadyHit;

    GameObject attachedTo;
    GrayCharacterController attachedCharacter;
    // positional
    public Vector3 attachPos;
    public Vector3 attachRot;

    private float _powerAmp = 0;

    public GameObject holder
    {
        get => attachedTo;
        set => attachedTo = value;
    }
   

    public void newAtkCycle()
    {
        alreadyHit = new List<Collider>();
    }
    public void OnTriggerEnter(Collider other)
    {
        // not ready yet
        if (!other) return;
        if (!isCollidable || !attachedCharacter.inAttackAnim()) return;
        if (other.gameObject == attachedTo) return;
        var targetChar = other.gameObject.GetComponent<GrayCharacterController>();
        if (!targetChar) return;
        if (targetChar.isRolling()) return;
        if (alreadyHit.Find((x) => x == other)) return;
        alreadyHit.Add(other);
        Debug.Log(string.Format("Hit {0}", other.gameObject.name));
        var character = other.GetComponent<GrayCharacterController>();
        character.applyEffect(weaponEffects, _powerAmp + 1);
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
            if (!transform.parent)
            {
                gameObject.SetActive(false);
                return;
            }
            attachedTo = transform.parent.gameObject;
        attachedCharacter = attachedTo.GetComponent<GrayCharacterController>();
        if (!attachedCharacter)
            throw new UnityException("Weapon not attached to viable character");
        transform.parent = attachedCharacter.hand;
        transform.localPosition = attachPos;
        transform.localRotation = Quaternion.Euler(attachRot);
    }

    // json-fy:
    public string toJson()
    {
        return JsonUtility.ToJson(new SerializedWeapon(this));
    }

    public static GameObject fromJson(string json)
    {
        var sWeapon = JsonUtility.FromJson<SerializedWeapon>(json);
        return sWeapon.toWeaponObj();
    }
}
