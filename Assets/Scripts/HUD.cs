using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Text levelCounter;

    // Update is called once per frame
    void Start()
    {
        levelCounter.text = "Level: " + Player.Instance.level;
    }
}
