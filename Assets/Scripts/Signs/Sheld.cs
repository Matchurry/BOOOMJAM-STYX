using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheld : MonoBehaviour
{
    public Vector3 tarpos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.15f);
    }
}
