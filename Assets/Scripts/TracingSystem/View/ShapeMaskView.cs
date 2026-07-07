using UnityEngine;

namespace TracingSystem.View
{
    public class ShapeMaskView : MonoBehaviour
    {
        [SerializeField] private SpriteMask shapeMask;
        [SerializeField] private SpriteRenderer shapeRenderer;
    
        public void UpdateSprite(Sprite shape = null)
        {
            shapeMask.sprite = shape;
            shapeRenderer.sprite = shape;
        }
    }
}