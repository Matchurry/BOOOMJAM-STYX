using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPUI : MonoBehaviour
{
    private RectTransform rt;
    private Vector2 tarpos = new Vector2(-1119, -422);
    void Start()
    {
        rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-1119, -422);
        StartCoroutine(wait());
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, tarpos, Time.deltaTime * 2.5f);
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.8f);
        tarpos = new Vector2(-531, -422);
    }
}