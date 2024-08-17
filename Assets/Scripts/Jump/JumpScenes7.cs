using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using Blackboard = NodeCanvas.Framework.Blackboard;

public class JumpScenes7 : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Jump()
    {
        SceneManager.LoadScene(8);
        
    }
}
