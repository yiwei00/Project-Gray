using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBehaviour : MonoBehaviour
{
    public GameObject root;

    void Awake()
    {
        root = GameObject.FindGameObjectWithTag("Root");
    }

    // Start is called before the first frame update
    void Start()
    {
        // assign parent to light child object of root
        //transform.SetParent(root.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
