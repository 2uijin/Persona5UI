using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasRenderer))]
public class NavbarTrapezoidShape : MaskableGraphic
{
    [Header("vertex")]
    public Vector2[] svgPoints = new Vector2[4]
    {
        new Vector2(10f, 0f),    // 왼쪽 위
        new Vector2(98f, 0f),    // 오른쪽 위
        new Vector2(108.1f, 47f),// 오른쪽 아래
        new Vector2(0f, 47f),    // 왼쪽 아래
    };

    public Vector2 svgViewBoxSize = new Vector2(108.1f, 47f);

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (svgPoints == null || svgPoints.Length != 4)
        {
            Debug.LogWarning($"[{name}] NavbarTrapezoidShape는 정확히 4개 점이 필요합니다. 현재: {svgPoints?.Length ?? 0}개");
            return;
        }

        Rect rect = GetPixelAdjustedRect();

        for (int i = 0; i < 4; i++)
        {
            Vector2 local = SvgToLocal(svgPoints[i], svgViewBoxSize, rect);

            UIVertex v = UIVertex.simpleVert;
            v.color = color;
            v.position = local;
            v.uv0 = new Vector2(
                svgPoints[i].x / svgViewBoxSize.x,
                1f - (svgPoints[i].y / svgViewBoxSize.y));

            vh.AddVert(v);
        }

        // 사다리꼴(볼록 4점) → 팬 삼각분할 2개면 충분
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);
    }

    Vector2 SvgToLocal(Vector2 svgPoint, Vector2 viewBoxSize, Rect rect)
    {
        float u = svgPoint.x / viewBoxSize.x;
        float v = svgPoint.y / viewBoxSize.y;
        float x = rect.xMin + u * rect.width;
        float y = rect.yMax - v * rect.height; // SVG는 y가 아래로 증가 → 반전
        return new Vector2(x, y);
    }

    public void SetPoints(Vector2[] points)
    {
        if (points == null || points.Length != 4) return;
        svgPoints = points;
        SetVerticesDirty(); // 다음 프레임에 OnPopulateMesh 다시 호출됨
    }

#if UNITY_EDITOR
    // Inspector에서 좌표를 바꿀 때마다 즉시 다시 그리기
    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
    }
#endif
}