using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BGMLforIn : MonoBehaviour
{
    private AudioSource aS;
    private float tarVol;

    private void Awake()
    {
        aS = GetComponent<AudioSource>();
        aS.volume = 0;
        tarVol = 0.6f;
    }

    void Start()
    {
        Timebar.playerWin.AddListener(SetToZero);
    }
    
    void Update()
    {
        aS.volume = math.lerp(aS.volume, tarVol, 0.5f * Time.deltaTime);
    }

    private void SetToZero()
    {
        tarVol = 0f;
    }
}
