using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingDestroy : MonoBehaviour
{
    private void Start()
    {
        if (!GameManager.instance.OnTestFeatures)
        {
            Destroy(gameObject);
        }
    }
}
