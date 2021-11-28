using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EffectType
{
    Heal,
    HoT,
    Pure_Damage,
    Pure_DoT,
    Physical_Damage,
    Physical_DoT,
    Magical_Damage,
    Magical_DoT,
    Move_Speedup,
    Move_Slowdown,
    Turn_Speedup,
    Turn_Slowdown,
}

[System.Serializable]
public class Effect: System.IComparable<Effect>
{
    public EffectType effectType;
    public string name = "";

    [Range(0, 120)]
    public float duration;
    public float staticStrength;
    [Range(0, 1)]
    public float percentStrength;

    public int CompareTo(Effect other)
    {
        if (other == null) return 1;
        return this.duration.CompareTo(other.duration);
    }

    public Effect clone()
    {
        var ret = new Effect();
        ret.effectType = this.effectType;
        ret.name = this.name;
        ret.duration = this.duration;
        ret.staticStrength = this.staticStrength;
        ret.percentStrength = this.percentStrength;
        return ret;
    }
}
