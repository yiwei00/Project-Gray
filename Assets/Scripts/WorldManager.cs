using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;

    public static WorldManager Instance
    {
        get => _instance;
    }
    private void Awake()  // singleton class
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            _leggos = new List<GameObject>();
            _basic = new List<GameObject>();
            // properly set weaponid;
            for (int i = 0; i < weaponDict.Count; ++i)
            {
                Weapon weapon = weaponDict[i].GetComponent<Weapon>();
                weapon.weaponID = i;
                if (i == 0) continue;
                if (weapon.rarity == LootRarity.LEGENDARY)
                {
                    _leggos.Add(weapon.gameObject);
                }
                else
                {
                    _basic.Add(weapon.gameObject);
                }
            }
        }
    }

    // i want this to be in editor
    public List<GameObject> weaponDict;
    List<GameObject> _leggos;
    List<GameObject> _basic;
    public List<GameObject> leggos
    {
        get => _leggos;
    }
    public List<GameObject> basic
    {
        get => _basic;
    }
    Player player;
    InventoryMenu inventory;

    public void SaveState()
    {
        string invString = inventory.toJson();
        string filename = Path.Combine(Application.dataPath, "saves", "save.json");
        Directory.CreateDirectory(Path.GetDirectoryName(filename));
        File.WriteAllText(filename, invString);
    }

    public void LoadState()
    {
        string filename = Path.Combine(Application.dataPath, "saves", "save.json");
        Directory.CreateDirectory(Path.GetDirectoryName(filename));
        if (!File.Exists(filename)) return;
        // clean up
        foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.GetComponentInChildren<Loot>())
            {
                Destroy(obj);
            }
            if (obj.GetComponent<Weapon>())
            {
                Destroy(obj);
            }
        }

        string invString = File.ReadAllText(filename);
        inventory.fromJson(invString);
        player.resetPlayer();
        
    }

    private void Start()
    {
        player = Player.Instance;
        inventory = player.invMenu;
        LoadState();
    }

}
