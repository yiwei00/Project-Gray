using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public bool valid;

    // initializations
    void Awake()
    {
        valid = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
        valid = false;
    }
}
