using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    /*
     * concept
     * 
     * dungeon: room[]
     *  create entry room
     *  add to room[]
     *  add boss to last room
     *
     * room: connection[]
     *  add npc, props
     *  add self to room[]
     *
     * connection:
     *  make random valid room
     *  
     * */

    public GameObject[] room_templates;
    public GameObject initial_room;
    public List<GameObject> rooms;

    void Awake()
    {
        // create initial room, connection points will do the rest of the work.
        initial_room = Instantiate(room_templates[0],transform.position,transform.rotation);
        rooms.Add(initial_room);
    }

    void Update()
    {    

    }
}
