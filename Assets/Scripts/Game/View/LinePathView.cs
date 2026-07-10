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
        [SerializeField] private Transform helper;

        [SerializeField] private float dotDistance = 0.5f;
        
        [SerializeField] private float enteringTime = 1f;
        [SerializeField] private float pointerEnteringTime = 0.25f;
        [SerializeField] private float helperEnteringTime = 0.2f;
        [SerializeField] private float helperPulseScale = 1.12f;
        [SerializeField] private float helperPulseTime = 0.35f;
        // Speed in distance that needs to be converted to progress for every spline.
        [SerializeField] private float helperSpeed = 0.5f;

        private readonly List<SpriteRenderer> dots = new();
        private readonly List<Vector3> dotScales = new();
        
        private Sequence enteringSequence;
        private Tween pointerTween;
        private Sequence helperSequence;
        
        public void DrawPath(Spline spline, float layer)
        {
            Clear();
            
            transform.position = new Vector3(transform.position.x, transform.position.y, -layer);
            
            float splineLength = spline.GetLength();
            int segmentsCount = math.max(1, (int)math.round(splineLength / math.max(0.01f, dotDistance)));
            float segmentLength = splineLength / segmentsCount;
            
            for (int i = 0; i < segmentsCount; i++)
            {
                CreatePointAtDistance(spline, dotPrefab, i * segmentLength);
            }
            
            CreatePointAtDistance(spline, starPrefab, splineLength);
            UpdatePointerImmediate(spline, 0f);
            PlayEnteringSequence();
        }

        public void UpdatePointer(Spline spline, float progress)
        {
            EndHelperSequence();
            UpdatePointerImmediate(spline, progress);
            
            if (pointer != null && pointer.localScale == Vector3.zero)
            {
                pointer.localScale = Vector3.one;
            }
        }
        
        public void StartHelperSequence(Spline spline, float progress)
        {
            if (helper == null)
            {
                return;
            }
            
            helperSequence?.Kill();
            
            float startProgress = math.clamp(progress, 0f, 0.95f);
            float splineLength = spline.GetLength();
            float duration = math.max(0.1f, splineLength * (1f - startProgress) / math.max(0.01f, helperSpeed));
            float helperProgress = startProgress;
            
            helper.localPosition = spline.EvaluatePosition(helperProgress);
            helper.localScale = Vector3.zero;
            
            helperSequence = DOTween.Sequence();
            helperSequence.Append(helper.DOScale(Vector3.one, helperEnteringTime).SetEase(Ease.OutBack));
            helperSequence.Append(helper.DOScale(Vector3.one * helperPulseScale, helperPulseTime).SetEase(Ease.InOutSine));
            helperSequence.Append(helper.DOScale(Vector3.one, helperPulseTime).SetEase(Ease.InOutSine));
            helperSequence.Append(DOTween.To(
                    () => helperProgress,
                    value =>
                    {
                        helperProgress = value;
                        helper.localPosition = spline.EvaluatePosition(helperProgress);
                    },
                    1f,
                    duration)
                .SetEase(Ease.InOutSine));
            helperSequence.Append(helper.DOScale(Vector3.zero, helperEnteringTime).SetEase(Ease.InBack));
            helperSequence.SetLoops(-1, LoopType.Restart);
        }
        
        public void EndHelperSequence()
        {
            helperSequence?.Kill();
            helperSequence = null;
            
            if (helper != null)
            {
                helper.localScale = Vector3.zero;
            }
        }

        private void CreatePointAtDistance(Spline spline, SpriteRenderer prefab, float distance)
        {
            float t = spline.ConvertIndexUnit(distance, PathIndexUnit.Distance, PathIndexUnit.Normalized);
            var dot = Instantiate(prefab, spline.EvaluatePosition(t), Quaternion.identity, transform);
            dot.transform.localPosition = new Vector3(dot.transform.localPosition.x, dot.transform.localPosition.y, 0f);
            
            dotScales.Add(dot.transform.localScale);
            dot.transform.localScale = Vector3.zero;
            SetAlpha(dot, 0f);
            
            dots.Add(dot);
        }
        
        public void Clear()
        {
            enteringSequence?.Kill();
            enteringSequence = null;
            pointerTween?.Kill();
            pointerTween = null;
            EndHelperSequence();
            
            if (pointer != null)
            {
                pointer.localScale = Vector3.zero;
            }
            
            if (dots.Count == 0)
            {
                dotScales.Clear();
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
            dotScales.Clear();
        }

        private void PlayEnteringSequence()
        {
            enteringSequence?.Kill();
            enteringSequence = DOTween.Sequence();
            
            float itemDuration = math.max(0.05f, enteringTime / math.max(1, dots.Count));
            for (int i = 0; i < dots.Count; i++)
            {
                SpriteRenderer dot = dots[i];
                if (dot == null)
                {
                    continue;
                }
                
                Vector3 targetScale = i < dotScales.Count ? dotScales[i] : Vector3.one;
                enteringSequence.Insert(i * itemDuration, dot.DOFade(1f, itemDuration * 0.8f));
                enteringSequence.Insert(i * itemDuration, dot.transform.DOScale(targetScale, itemDuration).SetEase(Ease.OutBack));
            }
            
            if (pointer != null)
            {
                pointerTween?.Kill();
                pointer.localScale = Vector3.zero;
                pointerTween = pointer.DOScale(Vector3.one, pointerEnteringTime).SetEase(Ease.OutBack);
                enteringSequence.Append(pointerTween);
            }
        }

        private void UpdatePointerImmediate(Spline spline, float progress)
        {
            if (pointer == null)
            {
                return;
            }
            
            progress = math.clamp(progress, 0f, 1f);
            pointer.localPosition = spline.EvaluatePosition(progress);
        }

        private static void SetAlpha(SpriteRenderer spriteRenderer, float alpha)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        private void OnDestroy()
        {
            enteringSequence?.Kill();
            pointerTween?.Kill();
            helperSequence?.Kill();
        }
    }
}
