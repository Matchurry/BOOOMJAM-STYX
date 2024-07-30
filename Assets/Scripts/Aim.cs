using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    public Player playerscript;
    private Vector3 tarpos;
    void Start(){

    }

    void Update(){
        tarpos = playerscript.AimPosNow();
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
    }
}
