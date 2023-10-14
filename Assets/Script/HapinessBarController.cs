using EasyButtons;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HapinessBarController : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private List<Hapiness> hapinessStates;

    private Hapiness currentHapinessState;

    [Serializable]
    private struct Hapiness
    {
        public HapinessState state;
        public float fillValue;
    }

    public enum HapinessState { 
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
    }
}

