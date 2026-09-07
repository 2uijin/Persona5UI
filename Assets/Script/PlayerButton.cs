using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerButton : Button
{
    [SerializeField] GameObject wiggleEffect;
    [SerializeField] RectTransform img;

    [SerializeField] float scaleAniDuration = 0.5f;
    [SerializeField] float scaleMag = 1.3f;

    public void SetHighlight(bool isOn)
    {
        if (isOn)
        {
            //wiggleEffect.SetActive(true);
            img.DOScale(scaleMag, scaleAniDuration).SetEase(Ease.OutBounce);
        }
        else
        {
            img.DOScale(1f, scaleAniDuration).SetEase(Ease.OutBounce);
            //wiggleEffect.SetActive(false);
        }
    }

    internal void OnSelect()
    {
        throw new NotImplementedException();
    }
}
