using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextForDebug : MonoBehaviour{
    public TextMeshProUGUI tm;
    public Player ps;
    private RectTransform rt;
    private Vector2 tarpos = new Vector2(-376.09f, -324.9f);
    
    void Start()
    {
        rt = GetComponent<RectTransform>();
        tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, 0.0f);
        if (tm.name == "CubeInHand")
            rt.anchoredPosition = new Vector2(-900f, -324.9f);
        else
            rt.anchoredPosition = new Vector2(12.14f, 0);
        StartCoroutine(wait());
    }

    void Update()
    {
        if (tm.name == "CubeInHand")
            tm.text = ps.CubeInHand.ToString();
        else
        {
            tm.text = " / " + ps.CubeInHandLim.ToString();
            if (ps.CubeInHand >= 10)
                tarpos = new Vector2(33.65f, 0);
            else
                tarpos = new Vector2(12.14f, 0);
        }
            
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, tarpos, Time.deltaTime * 2.5f);
        tm.color = Color.Lerp(tm.color,new Color(tm.color.r, tm.color.g, tm.color.b, 1.0f),Time.deltaTime*1.5f);
    }
    
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.8f);
        if (tm.name == "CubeInHand")
            tarpos = new Vector2(-376.09f, -324.9f);
        else
            tarpos = new Vector2(12.14f, 0);
    }
}
