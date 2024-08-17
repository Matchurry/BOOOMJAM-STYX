using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    private RectTransform rt;
    private Vector2 tarpos = new Vector2(-640, -358);
    void Start()
    {
        rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-640, -700);
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, tarpos, Time.deltaTime * 2.5f);
    }
}
