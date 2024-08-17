using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheld : MonoBehaviour
{
    public Vector3 tarpos;
    private bool _ismoving = true;
    private Player ps;
    void Start()
    {
        ps = Player.instance;
        SheldCube.OnSheldDes.AddListener(Tri);
        SheldCube.sheldCubePutUp.AddListener(PutUp);
        SheldCube.sheldCubePutDown.AddListener(PutDown);
        Cube.OnSheldTri.AddListener(Tri);
    }
    
    void Update()
    { 
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.15f);
    }

    private void PutUp()
    {
        _ismoving = true;
        ps._protectedCol = 999;
        Destroy(gameObject);
    }

    private void PutDown(int x, int z)
    {
        _ismoving = false;
        ps._protectedCol = x;
        Debug.Log(ps._protectedCol);
    }

    private void Tri()
    {
        ps._protectedCol = 999;
        Destroy(gameObject);
    }
    
    private void Tri(int x, int z)
    {
        ps._protectedCol = 999;
        Destroy(gameObject);
    }
}
