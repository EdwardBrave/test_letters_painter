using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Game.View
{
    public class LinePathView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer dotPrefab;
        [SerializeField] private SpriteRenderer starPrefab;
        
        [SerializeField] private Transform pointer;

        [SerializeField] private float dotDistance = 0.5f;

        private List<SpriteRenderer> dots = new ();
        
        private Sequence sequence;
        
        public void DrawPath(Spline spline, float layer)
        {
            Clear();
            
            transform.position = new Vector3(transform.position.x, transform.position.y, -layer);
            
            float splineLength = spline.GetLength();
            float segmentsCount = math.round(splineLength / dotDistance);
            float segmentLength = splineLength / segmentsCount;
            
            for(int i = 0; i < segmentsCount; i++)
            {
                
                CreatePointAtDistance(spline, dotPrefab, i * segmentLength);
            }
            
            CreatePointAtDistance(spline, starPrefab, splineLength);
        }

        public void UpdatePointer(Spline spline, float progress)
        {
            pointer.localScale = Vector3.one;
            pointer.localPosition = spline.EvaluatePosition(progress);
        }

        private void CreatePointAtDistance(Spline spline, SpriteRenderer prefab, float distance)
        {
            float t = spline.ConvertIndexUnit(distance, PathIndexUnit.Distance, PathIndexUnit.Normalized);
            var dot = Instantiate(prefab, spline.EvaluatePosition(t), Quaternion.identity, transform);
            dot.transform.localPosition = new Vector3(dot.transform.localPosition.x, dot.transform.localPosition.y, 0);
            
            dots.Add(dot);
        }
        
        public void Clear()
        {
            pointer.localScale = Vector3.zero;
            
            if (dots == null || dots.Count == 0)
            {
                return;
            }
            
            foreach (var dot in dots)
            {
                if (dot == null)
                {
                    continue;
                }
                
                if (Application.isPlaying)
                {
                    Destroy(dot.gameObject);
                }
                else
                {
                    DestroyImmediate(dot.gameObject);
                }
            }
            dots.Clear();
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }
    }
}