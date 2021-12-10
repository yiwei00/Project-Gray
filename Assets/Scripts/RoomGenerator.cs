using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private GameObject[] room_templates;
    //private GameObject[] hall_templates;
    private GameObject closed_wall;
    private GameObject temp_room;
    private int index, attempt;
    private List<int> available;

    // initializations
    void Awake()
    {
        room_templates = GameObject.FindGameObjectWithTag("Root").GetComponent<DungeonGenerator>().room_templates;
        available = Enumerable.Range(0,room_templates.Length).ToList();
        if(room_templates.Length == 0) { Debug.Log("no room templates found!"); }
    }

    void Start()
    {
        // try all random rooms 
        while(available.Count > 0)
        {
            index = Random.Range(0,available.Count);
            attempt = available[index];
            available.RemoveAt(index);
            temp_room = (GameObject)Instantiate(room_templates[index],transform.position,transform.rotation);
            if(temp_room.GetComponent<RoomBehaviour>().valid) { Destroy(gameObject); return; }
            else { Destroy(temp_room); }
        }
        // no room fits, put closed wall
        temp_room = (GameObject)Instantiate(closed_wall,transform.position,transform.rotation);
    }
}
