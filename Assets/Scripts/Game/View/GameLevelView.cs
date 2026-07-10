using System.Collections.Generic;
using UnityEngine;

namespace Game.View
{
    public class GameLevelView : MonoBehaviour
    {
        public ShapeMaskView shapeMaskView;
        public LinePathView linePathView;
        public Transform linesContainer;
        public List<LineTracerView> lineTracerViews;

        public void Dispose()
        {
            shapeMaskView.UpdateSprite(null);
            linePathView.Clear();
            lineTracerViews.Clear();
            Destroy(gameObject);
        }
    }
}
