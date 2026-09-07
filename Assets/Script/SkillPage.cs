using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SkillPage : MonoBehaviour
{
    [Header("background")]
    [SerializeField] Image background;
    [SerializeField] Sprite backgroundSprite;

    [Header("Mask Move")]
    [SerializeField] RectTransform maskImg;
    [SerializeField] Material maskMat;
    [SerializeField] Vector3 maskStartPos;
    [SerializeField] Vector3 maskEndPos;
    [SerializeField] float maskStartRot;
    [SerializeField] float maskEndRot;
    [SerializeField] float maskMoveDuration = 0.3f;

    [Header("inner mask")]
    [SerializeField] RectTransform innerMaskImg;
    [SerializeField] Material innerMaskMat;
    [SerializeField] RectTransform selectInfoImg;

    [Header("Animation")]
    [SerializeField] JokerCam jokerCam;
    [SerializeField] RectTransform jokerRawImg;
    [SerializeField] Animator jokerAnimator;
    [SerializeField] float animationDuration = 0.3f;
    [SerializeField] RectTransform jokerImg;

    [Header("Command Img")]
    [SerializeField] Image cmdImg;
    [SerializeField] float cmdFadeDuration = 0.5f;
    [SerializeField] Image infoImg;

    [Header("List")]
    [SerializeField] RectTransform btnList;
    [SerializeField] Vector3 listStartPos;
    [SerializeField] Vector3 listEndPos;
    [SerializeField] float listMoveDuration = 0.5f;

    [Header("Transition")]
    [SerializeField] BrushTransition transition;
    [SerializeField] SettingPage settingPage;


    public bool isOpen { private set; get; } = false;
    DG.Tweening.Sequence sequence;

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        background.sprite = backgroundSprite;
        
        innerMaskMat.SetFloat("_Progress", 0.035f);
        innerMaskImg.gameObject.SetActive(true);
        
        cmdImg.color = new Color(1f,1f,1f,0f);
        cmdImg.gameObject.SetActive(true);
        infoImg.color = new Color(1f, 1f, 1f, 0f);
        infoImg.gameObject.SetActive(true);

        btnList.anchoredPosition = listStartPos;
        btnList.gameObject.SetActive(true);

        sequence?.Kill();
        sequence = DOTween.Sequence().SetUpdate(true);

        float phaseSpeed = 0.4f;

        // 1. mask이동
        //sequence.Append(maskImg.DOAnchorPos(maskEndPos, maskMoveDuration).SetEase(Ease.OutCubic));
        sequence.Append(maskImg.DORotate(new Vector3(0, 0, maskEndRot), maskMoveDuration))
                .SetEase(Ease.OutCirc);

        // 1단계: progress 내려가면서 뚫림 (회전과 동시에)
        sequence.Join(
            DOTween.To(() => maskMat.GetFloat("_Progress"),
                       x => maskMat.SetFloat("_Progress", x),
                       0.95f, maskMoveDuration * 0.3f)
                   .SetEase(Ease.InOutSine)
        );
        sequence.Join(
            DOTween.To(() => maskMat.GetFloat("_CurvePhase"),
                       x => maskMat.SetFloat("_CurvePhase", x),
                       phaseSpeed * Mathf.Deg2Rad, maskMoveDuration * 0.5f)
                   .SetEase(Ease.Linear)
        );

        // 2단계: 1단계가 끝난 뒤에 순서대로 실행
        sequence.Append(
            DOTween.To(() => maskMat.GetFloat("_Progress"),
                       x => maskMat.SetFloat("_Progress", x),
                       1.5f, maskMoveDuration * 0.7f)
                   .SetEase(Ease.InOutSine)
        );
        sequence.Join(
            DOTween.To(() => maskMat.GetFloat("_CurvePhase"),
                       x => maskMat.SetFloat("_CurvePhase", x),
                       phaseSpeed * 2f * Mathf.Deg2Rad, maskMoveDuration * 0.7f)
                   .SetEase(Ease.Linear)
        );

        // 4. info창 마스크
        sequence.Join(
            DOTween.To(() => innerMaskMat.GetFloat("_Progress"),
                       x => innerMaskMat.SetFloat("_Progress", x),
                       1.5f, maskMoveDuration * 0.7f)
                   .SetEase(Ease.InOutSine)
        );

        // command타이틀 fade in
        sequence.JoinCallback(() => {
            cmdImg.DOFade(1f, cmdFadeDuration).SetEase(Ease.OutQuad);
            infoImg.DOFade(1f, cmdFadeDuration).SetEase(Ease.OutQuad);
            }
        );

        // 매뉴 list
        sequence.Join(
            btnList.DOAnchorPos(listEndPos, listMoveDuration).SetEase(Ease.OutQuad)
        );

        // 2. 애니메이션 재생
        sequence.JoinCallback(() => {
            jokerCam.ChangePos(false);
            jokerRawImg.gameObject.SetActive(true);
            jokerAnimator.Play("SkillMenu", -1, 0f);
        });
        sequence.InsertCallback(maskMoveDuration + 0.55f, () => {
            jokerRawImg.gameObject.SetActive(false);
            jokerImg.gameObject.SetActive(true);
            selectInfoImg.gameObject.SetActive(true);
        });
    }

    public void Close()
    {
        isOpen = false;

        // transition 하기
        transition.PlayTranstion();

        innerMaskImg.gameObject.SetActive(false);
        jokerImg.gameObject.SetActive(false);
        cmdImg.gameObject.SetActive(false);
        infoImg.gameObject.SetActive(false);
        selectInfoImg.gameObject.SetActive(false);
        btnList.gameObject.SetActive(false);
        settingPage.ChangeToSetting();
    }

    

    private void OnDestroy() => sequence?.Kill();
}
