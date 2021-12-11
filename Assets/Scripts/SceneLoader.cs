using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            WorldManager.Instance.SaveState();
            SceneManager.LoadScene(1);
            SceneManager.UnloadSceneAsync(0);
        }
        
    }
}
