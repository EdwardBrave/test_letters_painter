using System.Collections.Generic;
using UnityEngine;

namespace TracingSystem.View
{
    public class LineTracerView : MonoBehaviour 
    {
        private static readonly int FillPercent = Shader.PropertyToID("_FillPercent");
        [SerializeField] private LineRenderer lineRenderer;

        public void Init(List<Vector2> points, float width, Color color, float progress = 0f)
        {
            lineRenderer.startWidth = lineRenderer.endWidth = width;
            ApplyColor(color);
            ApplyProgressChange(progress);
            
            lineRenderer.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(points[i].x, points[i].y, 0f));
            }
        }
        
        public void ApplyColor(Color color)
        {
            lineRenderer.startColor = lineRenderer.endColor = color;
        }
        
        public void ApplyProgressChange(float progress)
        {
#if UNITY_EDITOR
            if (Application.isPlaying) // prevents material editing in edit mode
#endif
                lineRenderer.material.SetFloat(FillPercent, progress);
        }

        public void Clear()
        {
            lineRenderer.positionCount = 0;
#if UNITY_EDITOR
            if (Application.isPlaying) // prevents material editing in edit mode
#endif
                lineRenderer.material.SetFloat(FillPercent, 0f);
        }
        
#if UNITY_EDITOR
        public void CollectState(out List<Vector2> points, out float width)
        {
            points = new List<Vector2>();
            width = 0f;
            
            if (lineRenderer == null || lineRenderer.positionCount == 0)
            {
                return;
            }

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                var pos = lineRenderer.GetPosition(i);
                points.Add(new Vector2(pos.x, pos.y));
            }

            width = lineRenderer.startWidth;
        }
    }
#endif
}