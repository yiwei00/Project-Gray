using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    /*
     * concept
     * 
     * dungeon: room[]
     *  generate room templates
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
     * room template: room, connects, types
     *
     * an unrealized benefit of this structure is that regardless of changes in elevation, the dungeon will still
     * generate normally as long as the colliders & connections are placed correctly.
     *
     * */

    //public GameObject[] floor_tiles;
    //public GameObject[] wall_tiles;
    //public GameObject[] door_tiles;
    //public GameObject[] connections;
    public List<GameObject> room_templates;
    public GameObject closed_wall;
    public List<GameObject> rooms;
    //public Vector3 template_offset;
    // 5x5 tiles init, small 4-6x4-6, med 6-8x6-8, big 8-10x8-10
    // hall 1x2-4 small, 1-2x4-6 med, 2x6-8 big
    // public enum Type {init,room_s,room_m,room_b,hall_s,hall_m,hall_b,dead_end}; 

    void Awake()
    {
        rooms = new List<GameObject>();
    }

    void Start()
    {
        // generate room templates - INCOMPLETE
        
        /*        
        room_templates.Add(create_rand_room(5,5,0,0)); 
        room_templates.Add(create_rand_room(4,4,2,2));
        room_templates.Add(create_rand_room(6,6,2,2));
        room_templates.Add(create_rand_room(8,8,2,2));
        */

        // create initial room, connection points will do the rest of the work.
        rooms.Add(Instantiate(room_templates[0],transform.position,transform.rotation));

        //summon boss at rooms[-1]

    }
 
    //below is code for random room generation. unfortunately ran out of time before being able to complete this.
/*
    GameObject create_rand_room(int width, int length, int width_range, int length_range)
    {
        int w = width + Random.Range(0,width_range);
        int l = length + Random.Range(0,length_range);

        GameObject temp_room = new GameObject("auto" + w + "x" + l);
        GameObject temp_floor = new GameObject("Floor");
        temp_floor.transform.parent = temp_room.transform;
        GameObject temp_wall = new GameObject("Wall");
        temp_wall.transform.parent = temp_room.transform;
        GameObject temp_door = new GameObject("Connections");
        temp_door.transform.parent = temp_room.transform;

        List<GameObject> temp_floor_tiles = new List<GameObject>();
        List<GameObject> temp_wall_tiles = new List<GameObject>();
        List<GameObject> temp_door_tiles = new List<GameObject>();

        GameObject floor_tile = floor_tiles[Random.Range(0,floor_tiles.Length)];
        GameObject wall_tile = wall_tiles[Random.Range(0,wall_tiles.Length)];
        GameObject door_tile = door_tiles[Random.Range(0,door_tiles.Length)];

        int a,b,c,d;
        a = Random.Range(0,l+1);
        b = Random.Range(0,l+1);
        c = Random.Range(0,w+1);
        d = Random.Range(0,w+1);

        for(int i = 0; i < w; i++)
        {
            for(int j = 0; j < l; j++)
            {
                Quaternion rot = Quaternion.Euler(0,Random.Range(0,4)*90,0);
                temp_floor_tiles.Add((GameObject)Instantiate(floor_tile,template_offset + new Vector3(1+i*2,0,1+j*2),rot));
                if(i == 0)
                {
                    if(j == a)
                    {
                        temp_door_tiles.Add((GameObject)Instantiate(door_tile,template_offset + new Vector3(0,1,1+j*2),Quaternion.Euler(0,270,0)));
                        //temp_door_tiles.Add((GameObject)Instantiate(connections[0],template_offset + new Vector3(0,0,1+j*2),Quaternion.Euler(0,270,0)));
                    }
                    else { temp_wall_tiles.Add((GameObject)Instantiate(wall_tile,template_offset + new Vector3(0,1,1+j*2),Quaternion.Euler(0,270,0))); }
                }
                if(i == w-1)
                {
                    if(j == b)
                    {
                        temp_door_tiles.Add((GameObject)Instantiate(door_tile,template_offset + new Vector3(w*2,1,1+j*2),Quaternion.Euler(0,90,0)));
                        //temp_door_tiles.Add((GameObject)Instantiate(connections[0],template_offset + new Vector3(w*2,0,1+j*2),Quaternion.Euler(0,90,0)));
                    }
                    else { temp_wall_tiles.Add((GameObject)Instantiate(wall_tile,template_offset + new Vector3(w*2,1,1+j*2),Quaternion.Euler(0,90,0))); }
                }
                if(j == 0)
                {
                    if(i == c)
                    {
                        temp_door_tiles.Add((GameObject)Instantiate(door_tile,template_offset + new Vector3(1+i*2,1,0),Quaternion.Euler(0,180,0)));
                        //temp_door_tiles.Add((GameObject)Instantiate(connections[0],template_offset + new Vector3(1+i*2,0,0),Quaternion.Euler(0,180,0)));
                    }
                    else { temp_wall_tiles.Add((GameObject)Instantiate(wall_tile,template_offset + new Vector3(1+i*2,1,0),Quaternion.Euler(0,180,0))); }
                }
                if(j == w-1)
                {
                    if(i == d)
                    {
                        temp_door_tiles.Add((GameObject)Instantiate(door_tile,template_offset + new Vector3(1+i*2,1,l*2),Quaternion.Euler(0,0,0)));
                        //temp_door_tiles.Add((GameObject)Instantiate(connections[0],template_offset + new Vector3(1+i*2,0,l*2),Quaternion.Euler(0,0,0)));
                    } else { temp_wall_tiles.Add((GameObject)Instantiate(wall_tile,template_offset + new Vector3(1+i*2,1,l*2),Quaternion.Euler(0,0,0))); }
                }
            }
        }

        foreach(GameObject tile in temp_floor_tiles)
        {
            tile.transform.parent = temp_floor.transform;
        }
        foreach(GameObject tile in temp_wall_tiles)
        {
            tile.transform.parent = temp_wall.transform;
        }
        
        foreach(GameObject tile in temp_door_tiles)
        {
            tile.transform.parent = temp_door.transform;
        }
        
        return temp_room;
    }
*/
}
