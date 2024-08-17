using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    private Player _ps;
    private Vector3 _birthPos;
    private bool _veryFirst = true;
    private int[] _bgInter = new int[3];
    void Start()
    {
        _ps = Player.instance;
        _birthPos = transform.position;
        _bgInter[0] = 16;
        _bgInter[1] = 16;
        _bgInter[2] = 16;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_ps.is_resumed)
            transform.position -= Vector3.forward * (0.05f * _ps.gameSpeed);
        if(transform.position.z<=-50f)
            Destroy(gameObject);
        if (_veryFirst && _birthPos.z - transform.position.z >= _bgInter[_ps.level-1])
        {
            _ps.next_bg = true;
            _veryFirst = false;
        }
            
    }
}
