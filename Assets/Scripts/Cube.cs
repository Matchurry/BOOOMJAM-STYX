using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int[] pos = new int[2];
    public Player ps;
    private const float CubeYValue = 0.505f;
    private bool is_moving = false;
    private Vector3 tarpos;
    void Start(){
        tarpos = new Vector3(pos[0]-512,CubeYValue,pos[1]-512);
        ps = Player.instance;
        ps.OnCubePutOn.AddListener(HandleOnCubePutOn);
        ps.OnCubePutDown.AddListener(HandleOnCubePutDown);
    }

    void Update()
    {
        if(ps.pos[0]==pos[0] && ps.pos[1]==pos[1] && !is_moving){
            transform.position = new Vector3(transform.position.x,CubeYValue-0.2f,transform.position.z);
        }
        else{
            if(is_moving)
                tarpos = ps.transform.position + Vector3.up*1.25f;
            transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
        }
    }

    void HandleOnCubePutOn(int x, int z){
        if(x==pos[0] && z==pos[1]){
            //被抬起
            tarpos = ps.transform.position + Vector3.up*1.25f;
            is_moving=true;
        }
    }

    void HandleOnCubePutDown(int x, int z){
        if(is_moving){
            //被放下
            pos[0]=x;
            pos[1]=z;
            tarpos = new Vector3(pos[0]-512,CubeYValue,pos[1]-512);
            //StartCoroutine(Wait(1.1f));
            is_moving=false;
        }
    }

    IEnumerator Wait(float t)
    {
        yield return new WaitForSeconds(t);
    }
}
