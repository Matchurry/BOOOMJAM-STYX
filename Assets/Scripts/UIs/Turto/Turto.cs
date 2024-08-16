using System.Collections;
using System.Collections.Generic;
using UIs.Turto;
using UnityEngine;

public class Turto : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 tarpos;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 540 + 540);
        Confirm.TurClick.AddListener(HandleClick);
        tarpos = new Vector2(0, 0);
    }

    
    void Update()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, tarpos, 0.15f);
    }

    private void HandleClick()
    {
        tarpos = new Vector2(0, 540 + 540);
        StartCoroutine(Des());
    }

    IEnumerator Des()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
