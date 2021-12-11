using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDescription : MonoBehaviour
{
    public Text title;
    public Text description;
    public Text stats;

    // Update is called once per frame
    void Update()
    {
        InventorySlot invSlot = EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>();
        if (invSlot && !invSlot.isEmpty)
        {
            title.text = invSlot.item.title;
            title.color = Loot.rarityToColor(invSlot.item.rarity);

            description.text = invSlot.item.description;

            if (invSlot.item.type == ItemType.WEAPON)
            {
                string text = "";
                Weapon wep = invSlot.item.getItem().GetComponent<Weapon>();
                foreach (Effect e in wep.weaponEffects)
                {
                    text += string.Format("{0:+#;-#;0} and {1:P2} {2} ", e.staticStrength, e.percentStrength, e.effectType.ToString());
                    if (e.duration > 0)
                    {
                        text += string.Format("for {0} seconds", e.duration);
                    }
                    text += '\n';
                }
                stats.text = text;
            }
        }
        else
        {
            title.text = "";
            description.text = "";
            stats.text = "Select an item with your D-Pad or Pointer to learn more about it!";
        }
    }
}
