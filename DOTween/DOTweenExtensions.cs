#if USING_DOTWEEN

using DG.Tweening;
using UnityEngine;
using Utils.Common;

namespace Utils.DOTween
{
    public static class DOTweenExtensions
    {
        private const int CacheSize = 1000;
        private static readonly LRUCache<Transform, Vector3> _initialScales = new(CacheSize);

        public static Tweener DoSafeShakeScale(this Transform transform, float duration, Vector3 strength, int vibrato = 10,
            float randomness = 90)
        {
            if (!_initialScales.Contains(transform))
            {
                _initialScales.Add(transform, transform.localScale);
            }

            return transform.DOShakeScale(duration, strength, vibrato, randomness)
                .OnKill(() => RestoreScale(transform))
                .OnComplete(() => RestoreScale(transform));
        }

        private static void RestoreScale(Transform transform)
        {
            if (_initialScales.TryGet(transform, out Vector3 originalScale))
            {
                transform.localScale = originalScale;
            }
        }
    }    
}
#endif