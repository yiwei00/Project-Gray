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
    public GameObject closed_wall;
    public GameObject initial_room;
    public List<GameObject> rooms;

    void Awake()
    {
        // create initial room, connection points will do the rest of the work.
        rooms.Add(Instantiate(initial_room,transform.position,transform.rotation));
    }
}
