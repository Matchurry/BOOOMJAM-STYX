using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    private Player ps;
    void Start()
    {
        ps = Player.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(!ps.is_resumed)
            transform.position -= Vector3.forward * (0.05f * ps.gameSpeed);
        if(transform.position.z<=-50f)
            Destroy(gameObject);
    }
}
