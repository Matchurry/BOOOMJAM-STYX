using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    public int tarcol;
    private Vector3 tarpos;
    void Start()
    {
        transform.position = new Vector3(0, 1, 90);
        while(tarcol==999){;}
        transform.position = new Vector3(tarcol, 1f, transform.position.z);
        tarpos = new Vector3(tarcol, 1f, -100f);
        StartCoroutine(RunAttack());
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.05f);
    }

    IEnumerator RunAttack()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
