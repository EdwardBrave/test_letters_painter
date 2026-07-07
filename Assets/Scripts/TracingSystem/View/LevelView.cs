using System.Collections.Generic;
using UnityEngine;

namespace TracingSystem.View
{
    public class LevelView : MonoBehaviour
    {
        public ShapeMaskView shapeMaskView;
        public Transform linesContainer;
        public List<LineTracerView> lineTracerViews;
    }
}
