using DG.Tweening;

using EasyButtons;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Pixelmotchis
{
    public class HapinessBarController : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private List<Hapiness> hapinessStates;
        [SerializeField] private Light2D light2D;

        private Hapiness currentHapinessState;

        [Serializable]
        private struct Hapiness
        {
            public HapinessState state;
            public float fillValue;
            public float lightFalloff;
        }

        public enum HapinessState
        {
            VeryHappy,
            Happy,
            Normal,
            Sad,
            VerySad
        }

        [Button]
        public void SetHapinessState(HapinessState state)
        {
            var sta = hapinessStates.Find(x => x.state == state);
            currentHapinessState = sta;

            //DO This but with DOTween
            DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, currentHapinessState.fillValue, 0.5f);
            DOTween.To(() => light2D.shapeLightFalloffSize, x => light2D.shapeLightFalloffSize = x, currentHapinessState.lightFalloff, 0.5f);
        }
    }

}