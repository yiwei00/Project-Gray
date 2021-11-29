using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public InventoryMenu inventory;
    public GameObject weapon1;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        inventory.addItem(new WeaponInventoryItem(weapon1));
        this.enabled = false;
    }
}
