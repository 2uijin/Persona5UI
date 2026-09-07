using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] RectTransform activeImg;
    [SerializeField] RectTransform itemImgRect;
    [SerializeField] Image itemImg;

    [SerializeField] RectTransform[] Effects;

    [Header("img animation")]
    [SerializeField] Material darkenMat;
    public float imgAniDuration = 0.15f;
    public float imgMidScale = 1.5f;
    public float imgEndScale = 1.15f;

    [Header("wiggle animation")]
    public float wiggleAniDuration = 0.15f;

    [Header("Event")]
    public UnityEvent onButtonEnter;

    Sequence seq;

    private void Awake()
    {
        activeImg.localScale = new Vector3(imgEndScale, imgEndScale, 1);
    }

    public void SetHighlight(bool isOn)
    {
        if (isOn) OnSelected();
        else OffSelected();
    }

    private void OnSelected()
    {
        ImgBounceAni();
        WiggleBounceAni();

        EffectToggle(true);
        activeImg.gameObject.SetActive(true);
    }

    private void OffSelected()
    {
        ResetImg();
        ResetWiggle();

        EffectToggle(false);
        activeImg.gameObject.SetActive(false);
        itemImg.material = null;
    }

    private void ImgBounceAni()
    {
        seq.Kill();

        itemImgRect.localScale = Vector3.one;

        seq = DOTween.Sequence();
        seq.Append(itemImgRect.DOScale(imgMidScale, imgAniDuration * 0.5f).SetEase(Ease.OutQuad));
        seq.Append(itemImgRect.DOScale(imgEndScale, imgAniDuration * 0.5f).SetEase(Ease.InQuad))
            .OnComplete(() => {
                itemImg.material = darkenMat;
            });
    }

    private void ResetImg()
    {
        seq?.Kill();
        itemImgRect.localScale = Vector3.one;
        //itemImg.localRotation = Quaternion.identity;
    }

    private void WiggleBounceAni()
    {
        foreach (var effect in Effects)
        {
            effect.DOScale(new Vector3(3f, 0.1f, 1f), wiggleAniDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                effect.localScale = Vector3.one;
            });
        }
    }

    private void ResetWiggle()
    {
        foreach (var effect in Effects)
        {
            effect.localScale = Vector3.one;
        }
    }

    private void EffectToggle(bool isOn)
    {
        foreach (var effect in Effects)
        {
            effect.gameObject.SetActive(isOn);  
        }
    }
}
