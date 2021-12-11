using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    private static Prefabs _instance;

    public static Prefabs Get
    {
        get => _instance;
    }

    public GameObject invslot;
    public GameObject lootitem;
    public GameObject npc;

    private void Awake()  // singleton class
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
