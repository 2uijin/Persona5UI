using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
[ExecuteAlways]
public class SkillPageMaskUVSetter : MonoBehaviour
{
    Graphic graphic;
    RectTransform rt;

    void OnEnable()
    {
        graphic = GetComponent<Graphic>();
        rt = GetComponent<RectTransform>();
        UpdateRect();
    }

#if UNITY_EDITOR
    void Update() => UpdateRect(); // 에디터에서 크기 바뀔 때 갱신용
#endif

    void UpdateRect()
    {
        if (graphic == null || graphic.material == null) return;
        Rect r = rt.rect;
        graphic.material.SetVector("_LocalRect", new Vector4(r.xMin, r.yMin, r.xMax, r.yMax));
    }
}