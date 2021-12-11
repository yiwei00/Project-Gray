using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    Hitpoint hp;

    bool givenGift;
    private void Awake()
    {
        hp = GetComponent<Hitpoint>();
        givenGift = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Weapon>() && other.name.Contains("Banana"))
        {
            if (givenGift) return;
            givenGift = true;
            float[] damage = { (float)hp.maxHP, (float)hp.maxHP, (float)hp.maxHP };
            hp.damage(damage);
            var newWep = Instantiate(WorldManager.Instance.weaponDict[2]);
            newWep.SetActive(false);

            Weapon wep = newWep.GetComponent<Weapon>();

            var lootObj = GameObject.Instantiate(Prefabs.Get.lootitem);
            Loot loot = lootObj.GetComponent<Loot>();
            loot.item = new WeaponItem(newWep);
            lootObj.transform.position = transform.position;
        }
        else
        {
            hp.heal(1000000);
        }
    }
}
