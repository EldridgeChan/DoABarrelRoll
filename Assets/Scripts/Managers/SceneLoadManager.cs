using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }
}
