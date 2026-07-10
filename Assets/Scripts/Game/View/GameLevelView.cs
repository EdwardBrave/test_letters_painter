using System.Collections.Generic;
using UnityEngine;

namespace TracingSystem.View
{
    public class GameLevelView : MonoBehaviour
    {
        public ShapeMaskView shapeMaskView;
        public Transform linesContainer;
        public List<LineTracerView> lineTracerViews;
    }
}
