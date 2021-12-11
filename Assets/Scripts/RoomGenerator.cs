using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public List<int> types_accepted;
    private GameObject root;
    private GameObject[] room_templates;
    private List<GameObject> rooms;
    private GameObject closed_wall;
    private GameObject temp_room;
    private int index, attempt, timeout;
    private List<int> available;
    private bool success;
    private bool spawned;
    private float ttl;

    void Awake()
    {
        // failsafe time to live: destroy spawnpoint to ensure no infinite gen loop
        ttl = 4f;
        spawned = false;
        success = false;
    }

    void Start()
    {
        root = GameObject.FindGameObjectWithTag("Root");
        room_templates = root.GetComponent<DungeonGenerator>().room_templates;
        rooms = root.GetComponent<DungeonGenerator>().rooms;
        closed_wall = root.GetComponent<DungeonGenerator>().closed_wall;

        Destroy(gameObject,ttl);
        Invoke("generate",0.1f);
    }

    void generate()
    {
        if(spawned == false)
        {
            //available = Enumerable.Range(1,room_templates.Count).ToList();
            available = new List<int>(types_accepted);
            
            if(room_templates.Length == 0) { Debug.Log("no room templates found!"); }
            // try all random rooms 
            while(available.Count > 0)
            {
                index = Random.Range(0,available.Count);
                attempt = available[index];
                available.RemoveAt(index);
                temp_room = (GameObject)Instantiate(room_templates[attempt],transform.position,transform.rotation);
                //Debug.Log("pos: " + transform.position + "; rst: " + transform.eulerAngles);
                // collision detected: destroy room
                if(test_collision(temp_room)) { Destroy(temp_room); continue; }
                else { success = true; break; }
            }
            // no room fits, put closed wall
            if(!success) { temp_room = (GameObject)Instantiate(closed_wall,transform.position,transform.rotation); }
            rooms.Add(temp_room);
            spawned = true;
        } 
    }

    bool test_collision(GameObject room)
    {
        foreach(GameObject other_room in rooms)
        {
            // get arrays of all colliders to test collision
            Collider[] self_colliders = temp_room.GetComponents<Collider>();
            Collider[] other_colliders = other_room.GetComponents<Collider>();

            foreach(Collider sc in self_colliders)
            {
                foreach(Collider oc in other_colliders)
                {
                    // .bounds found from unity docs. originally, attempt was to attach roombehaviour to every room and
                    // use OnCollisionEnter to set a bool flag but that was too slow, must do it like this so we can
                    // wait for collision detect to finish before moving on to next generation.
                    if(sc.bounds.Intersects(oc.bounds))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
