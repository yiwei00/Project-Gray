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
                if (i < 2) continue;
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

    [System.Serializable]
    public class GameState
    {
        public string invString;
        public int playerExp;
        public GameState(string invString, int playerExp)
        {
            this.invString = invString;
            this.playerExp = playerExp;
        }
    }


    public void SaveState()
    {
        string invString = inventory.toJson();
        int playerExp = player.totalExp;
        string gameStateStr = JsonUtility.ToJson(new GameState(invString, playerExp));
        string filename = Path.Combine(Application.dataPath, "saves", "save.json");
        Directory.CreateDirectory(Path.GetDirectoryName(filename));
        File.WriteAllText(filename, gameStateStr);
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

        string gameStateStr = File.ReadAllText(filename);
        GameState gameState = JsonUtility.FromJson<GameState>(gameStateStr);
        inventory.fromJson(gameState.invString);
        player.totalExp = gameState.playerExp;
        player.resetPlayer();
    }

    public void resetToHub()
    {
        WorldManager.Instance.SaveState();
        SceneManager.LoadScene(0);
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        player = Player.Instance;
        inventory = player.invMenu;
        LoadState();
    }


    private void Start()
    {
        player = Player.Instance;
        inventory = player.invMenu;
    }

}
