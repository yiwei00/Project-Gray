using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    // automatically attached to every room. put props + npc spawning here
    // status: props, incomplete (intended to be completed together with create_random_room in DungeonGenerator)
    
    // the way the work was divided, NPC in some of yi's branches so stuff is abstract here
    private GameObject[] NPC_prefab;
    private GameObject BOSS_prefab;
    private GameObject root;

    void Awake()
    {
        // get NPC prefabs, boss prefab
        // NPC_prefab = getnpcprefab
        // BOSS_prefab = getbossprefab
        
        root = GameObject.FindGameObjectWithTag("Root"); 


    }

    void Start()
    {
        int npcCount = Random.Range(0, 10);
        for (int i = 0; i < npcCount; ++i)
        {
            var newNpc = Instantiate(Prefabs.Get.npc);
            newNpc.transform.position = transform.position;
            newNpc.SetActive(true);
        }

        if (!(root.GetComponent<DungeonGenerator>().boss) && this.CompareTag("Boss"))
        {
            root.GetComponent<DungeonGenerator>().boss = true;
            Debug.Log("boss spawned by: " + this);
        }
    }
}
