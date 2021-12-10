using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable
{
    Player player = Player.Instance;

    #region rng
    // not my code, taken from here: https://answers.unity.com/questions/421968/normal-distribution-random.html
    public static float NextGaussianFloat(float mu, float sig)
    {
        float u, v, S;

        do
        {
            u = 2f * Random.value - 1f;
            v = 2f * Random.value - 1f;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        float fac = Mathf.Sqrt(-2f * Mathf.Log(S) / S);
        return (u * fac) * sig + mu;
    }

    public static float NormalizedRandom(float minValue, float maxValue)
    {

        float mean = (minValue + maxValue) / 2;
        float sigma = (maxValue - mean) / 3;
        return NextGaussianFloat(mean, sigma);
    }
    #endregion

    public static Modifier getRandomModifier()
    {
        return (Modifier)Random.Range(0, System.Enum.GetValues(typeof(Modifier)).Length);
    }
    public static Item getRandomLeggo()
    {
        int playerLvl = Player.Instance.level;
        // get random leggo weapon:
        var leggoWeapon = WorldManager.Instance.leggos[Random.Range(0, WorldManager.Instance.leggos.Count)];
        var weaponObj = GameObject.Instantiate(leggoWeapon);
        weaponObj.SetActive(false);
        Weapon weapon = weaponObj.GetComponent<Weapon>();

        Modifier m = getRandomModifier();
        float weaponScale = 1 + Mathf.Log(playerLvl + 1) * Mathf.Pow(playerLvl + 1, 1 / 16);

        for (int i = 0; i < weapon.weaponEffects.Count; ++i)
        {
            weapon.weaponEffects[i] = modifyEffect(weapon.weaponEffects[i], m, LootRarity.COMMON, weaponScale);
        }
        weapon.rarity = LootRarity.LEGENDARY;
        weapon.weaponName = string.Format("{0} {1}", m.ToString(), weapon.weaponName);
        return new WeaponItem(weaponObj);
    }

        public static Item getRandomBasic(int enemyLevel)
    {
        int playerLvl = Player.Instance.level;
        // get random base weapon:
        var basicWeapon = WorldManager.Instance.basic[Random.Range(0, WorldManager.Instance.basic.Count)];
        var weaponObj = GameObject.Instantiate(basicWeapon);
        weaponObj.SetActive(false);
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        // rarity first
        int lvlDiff = Mathf.Clamp(enemyLevel - playerLvl, -5, 5);
        float scaled = lvlDiff * 2f / 5f;
        float min = Mathf.Max(0f, 0f + scaled);
        float max = Mathf.Min(4f, 4f + scaled);
        int normInt = Mathf.RoundToInt(NormalizedRandom(min, max));
        LootRarity rarity = (LootRarity)normInt;
        // tee hee
        if (rarity == LootRarity.LEGENDARY) rarity = LootRarity.UNCOMMON;

        Modifier m = getRandomModifier();

        float weaponScale = 1 + Mathf.Log(playerLvl + 1) * Mathf.Pow(playerLvl + 1, 1 / 16);

        for (int i = 0; i < weapon.weaponEffects.Count; ++i)
        {
            weapon.weaponEffects[i] = modifyEffect(weapon.weaponEffects[i], m, rarity, weaponScale);
        }
        weapon.rarity = rarity;
        weapon.weaponName = string.Format("{0} {1}", m.ToString(), weapon.weaponName);
        return new WeaponItem(weaponObj);
    }

    public static Effect modifyEffect(Effect e, Modifier m, LootRarity rarity, float scale)
    {
        System.Func<float, float> strengthMap = (x) => (x + (int)rarity * 2) * scale;
        System.Func<float, float> percentMap = (x) => (x + (int)rarity/100f) * (scale);
        #region apply modifiers here 
        if (Effect.isDmgType(e.effectType))
        {
            switch (m)
            {
                case Modifier.Mighty:
                    if (e.effectType == EffectType.Physical_Damage)
                    {
                        e.percentStrength += .1f;
                    }
                    else
                    {
                        e.effectType = EffectType.Physical_Damage;
                    }
                    break;
                case Modifier.Arcane:
                    if (e.effectType == EffectType.Magical_Damage)
                    {
                        e.percentStrength += .1f;
                    }
                    else
                    {
                        e.effectType = EffectType.Magical_Damage;
                    }
                    break;
                case Modifier.Peculiar:
                    if (e.effectType == EffectType.Pure_Damage)
                    {
                        e.percentStrength += .1f;
                    }
                    else
                    {
                        e.effectType = EffectType.Pure_Damage;
                    }
                    break;
            }
        }
        if (Effect.isDotType(e.effectType))
        {
            switch (m)
            {
                case Modifier.Mighty:
                    if (e.effectType == EffectType.Physical_DoT)
                    {
                        e.staticStrength += 2;
                    }
                    else
                    {
                        e.effectType = EffectType.Physical_DoT;
                    }
                    break;
                case Modifier.Arcane:
                    if (e.effectType == EffectType.Magical_DoT)
                    {
                        e.staticStrength += 2;
                    }
                    else
                    {
                        e.effectType = EffectType.Magical_DoT;
                    }
                    break;
                case Modifier.Peculiar:
                    if (e.effectType == EffectType.Pure_DoT)
                    {
                        e.staticStrength += 2;
                    }
                    else
                    {
                        e.effectType = EffectType.Pure_DoT;
                    }
                    break;
            }
        }
        #endregion

        // get new strength:
        e.staticStrength = strengthMap(e.staticStrength);
        e.percentStrength = percentMap(e.percentStrength);

        return e;
    }

    public static void spawnLoots(int level, Vector3 at)
    {
        int lootCount = Mathf.RoundToInt(LootTable.NormalizedRandom(1, 10));
        for (int i = 0; i < lootCount; ++i)
        {
            var lootObj = GameObject.Instantiate(Prefabs.Get.lootitem);
            Loot loot = lootObj.GetComponent<Loot>();
            loot.item = LootTable.getRandomBasic(level);

            var pos = 3 * Random.insideUnitCircle;
            lootObj.transform.position = at + new Vector3(pos.x, 1, pos.y);
            Rigidbody rb = loot.GetComponentInChildren<Rigidbody>();
            rb.AddForce(Random.Range(0f, 100f)*(lootObj.transform.position - at));
        }
    }
}
