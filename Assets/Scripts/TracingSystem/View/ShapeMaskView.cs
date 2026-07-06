using UnityEngine;

namespace TracingSystem.View
{
    public class ShapeMaskView : MonoBehaviour
    {
        [SerializeField] private SpriteMask shapeMask;
        [SerializeField] private SpriteRenderer shapeRenderer;
    
        public void Init(Sprite shape)
        {
            shapeMask.sprite = shape;
            shapeRenderer.sprite = shape;
        }

        public void Clear()
        {
            shapeMask.sprite = null;
            shapeRenderer.sprite = null;
        }
    }
}