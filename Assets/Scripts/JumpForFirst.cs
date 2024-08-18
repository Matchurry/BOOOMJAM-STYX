using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpForFirst : MonoBehaviour
{
    private static bool firstLaunch = true;

    private void Awake()
    {
        if (firstLaunch)
        {
            firstLaunch = false;
            SceneManager.LoadScene(17);
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
