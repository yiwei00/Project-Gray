using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private GameObject[] room_templates;
    //private GameObject[] hall_templates;
    private GameObject root;
    private List<GameObject> rooms;
    private GameObject closed_wall;
    private GameObject temp_room;
    private int index, attempt, timeout;
    private List<int> available;
    private bool success;
    private bool spawned;
    private float ttl;

    void Start()
    {
        ttl = 4f;
        spawned = false;
        success = false;
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
            available = Enumerable.Range(0,room_templates.Length).ToList();
            if(room_templates.Length == 0) { Debug.Log("no room templates found!"); }
            // try all random rooms 
            while(available.Count > 0)
            {
                index = Random.Range(0,available.Count);
                attempt = available[index];
                available.RemoveAt(index);
                temp_room = (GameObject)Instantiate(room_templates[index],transform.position,transform.rotation);
                if(test_collision(temp_room)) { Destroy(temp_room); continue; Debug.Log("intersected");}
                else { success = true; break; Debug.Log("success");}
            }
            // no room fits, put closed wall
            if(!success) { temp_room = (GameObject)Instantiate(closed_wall,transform.position,transform.rotation); }
            else { rooms.Add(temp_room); }
            spawned = true;
        } 
    }

    bool test_collision(GameObject room)
    {
        foreach(GameObject other_room in rooms)
        {
            if(temp_room.GetComponent<Collider>().bounds.Intersects(other_room.GetComponent<Collider>().bounds))
            {
                Debug.Log("collision detected");
                return true;
            }
        }
        return false;
    }
}
