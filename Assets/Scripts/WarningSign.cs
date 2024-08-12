using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class WarningSign : MonoBehaviour
{
    private Material mt;
    private Vector3 tarpos;
    private float startTime;
    void Start()
    {
        startTime = Time.time;
        tarpos = new Vector3(transform.position.x,1.01f,transform.position.z);
        mt = GetComponent<Renderer>().material;
        StartCoroutine(Show());
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
        if(Time.time - startTime > 0.5f)
            mt.color = new Color(
                mt.color.r,
                mt.color.b,
                mt.color.g,
                math.lerp(mt.color.a, 0f, 2.5f * Time.deltaTime));
    }

    IEnumerator Show()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
