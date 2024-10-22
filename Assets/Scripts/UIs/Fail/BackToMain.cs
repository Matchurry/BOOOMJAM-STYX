using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMain : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image _image;
    public static UnityEvent BackToMainClick = new UnityEvent();
    void Start()
    {
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = new Color(0,0,0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = new Color(1,1,1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _image.color = new Color(0, 0, 0, 0.5f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        BackToMainClick.Invoke();
        SceneManager.LoadScene(17);
    }
}
