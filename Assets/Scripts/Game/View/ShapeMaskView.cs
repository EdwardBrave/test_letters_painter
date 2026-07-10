using JetBrains.Annotations;
using UnityEngine;

namespace Game.View
{
    public class ShapeMaskView : MonoBehaviour
    {
        [SerializeField] private SpriteMask shapeMask;
        [SerializeField] private SpriteRenderer shapeRenderer;
    
        public void UpdateSprite([CanBeNull]Sprite shape)
        {
            shapeMask.sprite = shape;
            shapeRenderer.sprite = shape;
        }
    }
}