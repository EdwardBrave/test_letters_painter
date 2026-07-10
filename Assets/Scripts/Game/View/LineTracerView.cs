using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.View
{
    public class LineTracerView : MonoBehaviour
    {
        private static readonly int FillPercent = Shader.PropertyToID("_FillPercent");
        [SerializeField] private LineRenderer lineRenderer;
        
        public LineRenderer LineRenderer => lineRenderer;

        public void Init(IReadOnlyList<Vector2> points, float width, Color color, float progress = 0f)
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
        
        public class Factory : PlaceholderFactory<LineTracerView>
        {
        }
    }
}