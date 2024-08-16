using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIs.Turto
{
    public class Confirm : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Image _image;
        public static UnityEvent TurClick = new UnityEvent();
    
        void Start()
        {
            _image = GetComponent<Image>();
        }
    
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
            TurClick.Invoke();
        }
    }
}
