using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class lazerSign : MonoBehaviour
{
    public int tarcol = 999;
    private Player ps;
    private Vector3 tarpos;
    public GameObject Lazer;
    public static UnityEvent lazerAttack = new UnityEvent();
    void Start()
    {
        ps = Player.instance;
        while(tarcol==999){;}
        tarpos = new Vector3(tarcol, 2f, ps.pos[1] - 512);
        transform.position = new Vector3(tarcol, 5f+5f,ps.pos[1] - 512);
        StartCoroutine(RunAttack());
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.15f);
    }

    IEnumerator RunAttack()
    {
        yield return new WaitForSeconds(1f);
        GameObject laz = Instantiate(Lazer);
        Lazer sc = laz.GetComponent<Lazer>();
        sc.tarcol = tarcol;
        yield return new WaitForSeconds(0.1f);
        ps.lazerAttcking = tarcol;
        lazerAttack.Invoke();
        //Debug.Log(tarcol.ToString() + ' ' + ps.pos[0].ToString());
        yield return new WaitForSeconds(0.5f);
        ps.lazerAttcking = 999;
        Destroy(gameObject);
    }
}
