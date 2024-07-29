using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    public Player playerscript;
    void Start(){
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.SetFloat("_Alpha", 0.5f); // 设置 Alpha 值为 0.5
    }

    void Update(){
        transform.position = playerscript.AimPosNow();
    }
}
