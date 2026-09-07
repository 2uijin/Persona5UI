using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SettingPage : MonoBehaviour
{
    [Header("background Fade")]
    [SerializeField] Image background;
    [SerializeField] Sprite backgroundSprite;
    [SerializeField] float bgAlphaTarget = 0.7f;
    [SerializeField] float bgFadeDuration = 0.2f;

    [Header("Mask Move")]
    [SerializeField] RectTransform maskImg;
    [SerializeField] Vector3 maskStartPos;
    [SerializeField] Vector3 maskEndPos;
    [SerializeField] float maskStartRot;
    [SerializeField] float maskEndRot;
    [SerializeField] float maskMoveDuration = 0.3f;

    [Header("Animation")]
    [SerializeField] JokerCam jokerCam;
    [SerializeField] RectTransform jokerRawImg;
    [SerializeField] Animator jokerAnimator;
    [SerializeField] float animationDuration = 0.3f;
    [SerializeField] RectTransform jokerImg;

    [Header("Screen Shake")]
    [SerializeField] float shakeDuration = 0.3f;
    [SerializeField] float shakeStrength = 20f;
    [SerializeField] int shakeVibrato = 10;

    [Header("UI")]
    [SerializeField] RectTransform commandTitle;
    [SerializeField] RectTransform buttons;
    [SerializeField] Image yenImg;

    [Header("Transition")]
    [SerializeField] BrushTransition transition;

    [Header("Change Page")]
    [SerializeField] SkillPage skillPage;
    [SerializeField] GameObject menuList;
    [SerializeField] Image cmdtitleImg;

    public bool isOpen { private set; get; } = false;
    Sequence sequence;

    public void Toggle()
    {
        if (!isOpen) Open();
        else Close();
    }

    private void Open()
    {
        if (isOpen) return;
        isOpen = true;

        background.color = SetAlpha(background.color, 0f);
        maskImg.anchoredPosition = maskStartPos;
        maskImg.localEulerAngles = new Vector3(0, 0, maskStartRot);
        jokerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        sequence?.Kill();
        sequence = DOTween.Sequence().SetUpdate(true);

        sequence.Append(background.DOFade(bgAlphaTarget, bgFadeDuration));

        sequence.Append(maskImg.DOAnchorPos(maskEndPos, maskMoveDuration).SetEase(Ease.OutCubic));
        sequence.Join(maskImg.DORotate(new Vector3(0, 0, maskEndRot), maskMoveDuration)).SetEase(Ease.OutCirc);

        Vector2 jokerStartPos = jokerImg.anchoredPosition;
        sequence.AppendCallback(()=>jokerRawImg.gameObject.SetActive(true));
        sequence.AppendCallback(() => jokerAnimator.Play("MainMenu",-1,0f));
        sequence.AppendInterval(animationDuration);
        sequence.AppendCallback(() => jokerRawImg.gameObject.SetActive(false));
        sequence.AppendCallback(() => jokerImg.gameObject.SetActive(true));
        sequence.Append(
        jokerImg.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato)
        .SetEase(Ease.OutCubic)
        .OnComplete(() => {
            jokerImg.anchoredPosition = jokerStartPos;
        })
        );
        sequence.InsertCallback(sequence.Duration() - shakeDuration, () => // 방금 추가한 흔들림의 시작 시점으로 되돌아가서 삽입
        {
            commandTitle.gameObject.SetActive(true);
            yenImg.gameObject.SetActive(true);
            buttons.gameObject.SetActive(true);
        });

    }

    private void Close()
    {
        isOpen = false;

        transition.PlayTranstion();

        background.color = SetAlpha(background.color, 0f);
        maskImg.anchoredPosition = maskStartPos;
        maskImg.localRotation = Quaternion.identity;
        jokerImg.gameObject.SetActive(false);
        commandTitle.gameObject.SetActive(false);
        buttons.gameObject.SetActive(false);
        yenImg.gameObject.SetActive(false);
    }

    public void ChangeToSkillPage()
    {
        sequence?.Kill();
        sequence = DOTween.Sequence().SetUpdate(true);

        sequence.Append(cmdtitleImg.DOFade(0, 0.3f));
        sequence.Join(yenImg.DOFade(0, 0.3f));
        sequence.AppendCallback(() => {
            jokerImg.gameObject.SetActive(false);
            buttons.gameObject.SetActive(false);
        });
        

        sequence.AppendCallback(() => skillPage.Open());
    }

    public void ChangeToSetting()
    {
        jokerImg.gameObject.SetActive(true);
        buttons.gameObject.SetActive(true);
        cmdtitleImg.color = new Color(1, 1, 1, 1);
        yenImg.color = new Color(1, 1, 1, 1);
        maskImg.anchoredPosition = maskEndPos;
        maskImg.localRotation = Quaternion.Euler(new Vector3(0,0,maskEndRot));
        background.sprite = backgroundSprite;
    }

    private Color SetAlpha(Color c, float a) => new Color(c.r, c.g, c.b, a);

    private void OnDestroy() => sequence?.Kill();

}
