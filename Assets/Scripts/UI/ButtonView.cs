using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ButtonView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Button button;
        
        public event Action OnClick;

        public void OnButtonClicked()
        {
            OnClick?.Invoke();
        }
        
        public void UpdateImage(Sprite sprite, Color color)
        {
            if (image)
            {
                image.sprite = sprite;
                image.color = color;
            }
        }
    }
}