using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fail : MonoBehaviour
{
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0);
        BackToMain.BackToMainClick.AddListener(HandleBackToMainClick);
        Restart.RestartClick.AddListener(HandleRestartClick);
    }
    
    void Update()
    {
        
    }

    private void HandleBackToMainClick()
    {
        
    }

    private void HandleRestartClick()
    {
        
    }
}
