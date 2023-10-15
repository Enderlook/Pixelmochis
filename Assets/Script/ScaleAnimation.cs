using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixelmochis
{
    public class ScaleAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float duration = 2.0f;
        [SerializeField] private float initialScaleAnimation = 0f;

        private Vector3 initialScale;
        private float timer = 0f;

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        private void OnEnable()
        {
            PlayScaleAnimation();
        }

        private void Update()
        {
            if (timer < duration)
            {
                float t = timer / duration;
                float scaleValue = scaleCurve.Evaluate(t);
                transform.localScale = initialScale * scaleValue;
                timer += Time.deltaTime;
            }
        }

        private void PlayScaleAnimation()
        {
            timer = 0f;
            transform.localScale = new Vector3(initialScaleAnimation, initialScaleAnimation, initialScaleAnimation);
        }
    }
}