using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MenuButton : Button
{
    [SerializeField] GameObject wiggleEffect;
    [SerializeField] Transform img;

    [Header("Punch Effect")]
    [SerializeField] private float punchScale = 1.2f;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Wiggle Stretch")]
    [SerializeField] private Image[] wiggleImages; // wiggle_red, wiggle_cyan 할당

    private static readonly int StretchStartTimeID = Shader.PropertyToID("_StretchStartTime");
    private Material[] wiggleMaterials;
    private Canvas canvas;
    private Vector3 defaultScale;
    private Coroutine punchCorutine;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        defaultScale = img.localScale;
        
        // 인스턴스화 
        wiggleMaterials = new Material[wiggleImages.Length];
        for (int i = 0; i < wiggleImages.Length; i++)
        {
            wiggleMaterials[i] = Instantiate(wiggleImages[i].material);
            wiggleImages[i].material = wiggleMaterials[i];
        }
    }

    public void SetHighlight(bool isOn)
    {
        if (isOn)
        {
            canvas.sortingOrder = 2;
            wiggleEffect.SetActive(true);
            PlaySelectPunch();
            PlayStretchEffect();
        }
        else
        {
            canvas.sortingOrder = 1;
            wiggleEffect.SetActive(false);
        }
    }
    
    public void PlaySelectPunch()
    {
        if (punchCorutine != null)
            StopCoroutine(punchCorutine);
        punchCorutine = StartCoroutine(PunchRoutine());
    }
    private void PlayStretchEffect()
    {
        float now = Time.time;
        foreach (var mat in wiggleMaterials)
        {
            mat.SetFloat(StretchStartTimeID, now);
        }
    }
    private IEnumerator PunchRoutine()
    {
        Vector3 targetScale = defaultScale * punchScale;
        yield return ScaleTo(targetScale, duration);
        yield return ScaleTo(defaultScale, duration);
        punchCorutine = null;
    }
    private IEnumerator ScaleTo(Vector3 target, float time)
    {
        Vector3 startScale = img.localScale;
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsed / time);
            img.localScale = Vector3.LerpUnclamped(startScale, target, t);
            yield return null;
        }
        img.localScale = target;
    }

    internal void OnSelect()
    {
        throw new NotImplementedException();
    }
}