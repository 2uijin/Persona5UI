using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CharacterItem : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] RectTransform activeImg;
    [SerializeField] RectTransform itemImgRect;
    [SerializeField] Image itemImg;

    [SerializeField] RectTransform[] Effects;

    [Header("img animation")]
    [SerializeField] Material darkenMat;
    [SerializeField] float imgAniDuration = 0.2f;
    [SerializeField] float scaleMag = 1.3f;

    public void SetHighlight(bool isOn)
    {
        if (isOn)
        {
            DOTween.Kill(itemImgRect.transform, complete: true);

            itemImgRect.DOScale(scaleMag, imgAniDuration).SetEase(Ease.OutBounce)
                .OnComplete(() => { 
                    foreach (var effect in Effects) effect.gameObject.SetActive(true); 
                    activeImg.gameObject.SetActive(true);
                    itemImg.material = darkenMat;
                });               
        }
        else
        {
            DOTween.Kill(itemImgRect.transform, complete: true);

            foreach (var effect in Effects) effect.gameObject.SetActive(false);
            activeImg.gameObject.SetActive(false);
            itemImg.material = null;

            itemImgRect.DOScale(1f, imgAniDuration).SetEase(Ease.OutBounce).OnComplete(() => {
            });
        }
    }
}
