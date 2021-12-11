using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollowObj : MonoBehaviour
{
    Canvas ui;
    // Start is called before the first frame update
    void Awake()
    {
        ui = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;

        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
