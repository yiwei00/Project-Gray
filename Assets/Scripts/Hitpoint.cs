using UnityEngine;
using UnityEngine.UI;

public enum DmgType: int
{
    Pure,
    Physical,
    Magical
}
public class Hitpoint : MonoBehaviour
{
    public int level = -1;
    public int maxHP;
    private float _curHP;
    public int curHP
    {
        get => Mathf.RoundToInt(_curHP);
    }

    private float[] _resistance;
    public Resistances resist;
    public Slider healthbar;

    [System.Serializable]
    public class Resistances
    {
        public float pure_resistance;
        public float physical_resistance;
        public float magical_resistance;
    }

    public float pure_resistance
    {
        get => _resistance[0];
        set
        {
            resist.pure_resistance = value;
            _resistance[0] = value;
        }
    }

    public float physical_resistance
    {
        get => _resistance[1];
        set
        {
            resist.physical_resistance = value;
            _resistance[1] = value;
        }
    }

    public float magical_resistance
    {
        get => _resistance[2];
        set
        {
            resist.magical_resistance = value;
            _resistance[2] = value;
        }
    }

    public void heal(float amount)
    {
        _curHP = Mathf.Clamp(_curHP + amount, 0f, maxHP);
    }

    public float damage(float[] dmg)
    {
        float net_dmg = 0;
        foreach(int dmgType in (DmgType[])System.Enum.GetValues(typeof(DmgType)))
        {
            if (dmgType == (int) DmgType.Pure)
            {
                net_dmg += dmg[dmgType] * Mathf.Max(0, 1 - _resistance[dmgType]);
            }
            else // pure resists everything
            {
                net_dmg += dmg[dmgType] * Mathf.Max(0, 1 - _resistance[dmgType] - _resistance[(int) DmgType.Pure]);
            }
        }
        _curHP = Mathf.Clamp(_curHP - net_dmg, 0, maxHP);
        return net_dmg;
    }

    public static bool isHitpointEffect(Effect effect)
    {
        switch(effect.effectType)
        {
            case EffectType.Heal:
            case EffectType.HoT:
            case EffectType.Pure_Damage:
            case EffectType.Physical_Damage:
            case EffectType.Magical_Damage:
            case EffectType.Pure_DoT:
            case EffectType.Physical_DoT:
            case EffectType.Magical_DoT:
                return true;
            default:
                return false;
        }
    }

    public void applyEffect(Effect effect, float duration)
    {
        float[] dmg = new float[3];
        float heal = 0.0f;
        float strengthPerSecond = effect.staticStrength + (maxHP) * effect.percentStrength;
        switch (effect.effectType)
        {
            case EffectType.Heal:
                heal += effect.staticStrength;
                heal += maxHP * effect.percentStrength;
                break;
            case EffectType.HoT:
                heal += strengthPerSecond * duration;
                break;
            case EffectType.Pure_Damage:
                dmg[(int) DmgType.Pure] += effect.staticStrength;
                dmg[(int) DmgType.Pure] += maxHP * effect.percentStrength;
                break;
            case EffectType.Physical_Damage:
                dmg[(int)DmgType.Physical] += effect.staticStrength;
                dmg[(int)DmgType.Physical] += maxHP * effect.percentStrength;
                break;
            case EffectType.Magical_Damage:
                dmg[(int)DmgType.Magical] += effect.staticStrength;
                dmg[(int)DmgType.Magical] += maxHP * effect.percentStrength;
                break;
            case EffectType.Pure_DoT:
                dmg[(int)DmgType.Pure] += strengthPerSecond * duration;
                break;
            case EffectType.Physical_DoT:
                dmg[(int)DmgType.Physical] += strengthPerSecond * duration;
                break;
            case EffectType.Magical_DoT:
                dmg[(int)DmgType.Magical] += strengthPerSecond * duration;
                break;
        }
        this.heal(heal);
        damage(dmg);
    }

    private void Update()
    {
        // my spaghetti approach to being able to set resistances in the unity editor
        pure_resistance = resist.pure_resistance;
        physical_resistance = resist.physical_resistance;
        magical_resistance = resist.magical_resistance;

        if (healthbar)
        {
            float hpPercent = _curHP / (1.0f * maxHP);
            healthbar.value = hpPercent;
        }
    }

    public void adjustToLevel(int newLevel)
    {
        level = newLevel;
        maxHP = Mathf.FloorToInt(25 * Mathf.Log(level + 1) * Mathf.Pow(1 + level, 1 / 16f));
        if (level < 50) physical_resistance = (level / 100f) - .3f;
        else physical_resistance = .2f;
        if (level < 30) magical_resistance = -.3f;
        else if (level < 80) magical_resistance = (level / 100f) - .8f;
        else magical_resistance = .2f;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _resistance = new float[3] { 0, 0, 0 };
        if (level > 0)
        {
            adjustToLevel(level);
        }
        else
        {

            pure_resistance = resist.pure_resistance;
            physical_resistance = resist.physical_resistance;
            magical_resistance = resist.magical_resistance;
        }
        _curHP = maxHP;
    }
}
