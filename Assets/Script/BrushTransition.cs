using UnityEngine;
using DG.Tweening;

public class BrushTransition : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] RectTransform transitionImg;
    [SerializeField] Material brushMat;
    [SerializeField] float transDuration = 0.8f;

    RenderTexture rt;

    public void PlayTranstion()
    {
        int width = cam.pixelWidth;
        int height = cam.pixelHeight;

        if (rt == null || rt.width != width || rt.height != height)
        {
            if (rt != null)
                rt.Release();

            rt = new RenderTexture(width, height, 0);
        }

        RenderTexture prevRT = cam.targetTexture;
        cam.targetTexture = rt;
        cam.Render();
        cam.targetTexture = prevRT;
        brushMat.SetTexture("_BaseMap", rt);

        brushMat.DOFloat(1f, "_Progress", transDuration)
            .SetEase(Ease.InOutSine)
            .OnStart(() => transitionImg.gameObject.SetActive(true))
            .OnComplete(() => {
                transitionImg.gameObject.SetActive(false);
                brushMat.SetFloat("_Progress", 0f);
            });

    }

    private void OnDestroy()
    {
        if (rt != null)
            rt.Release();
    }
}
