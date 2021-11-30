using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject weapon1;
    public GameObject lootPrefab;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var lootItem = Instantiate(lootPrefab);
        lootItem.GetComponent<LootItem>().item = new WeaponInventoryItem(weapon1);
        this.enabled = false;
    }
}
