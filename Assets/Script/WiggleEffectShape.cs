using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WiggleEffectShape : Graphic
{
    [Header("vertex")]
    public Vector2[] points = new Vector2[4] 
    {
        new Vector2(0f, 0f),       // 왼쪽 위
        new Vector2(200f, 0f),       // 오른쪽 위
        new Vector2(200f, 200f),   // 오른쪽 아래
        new Vector2(0f, 200f),       // 왼쪽 아래
    };


public Vector2 viewBoxSize = new Vector2(200f, 200f);

    private Vector2[] originalPoints;

    public Vector2[] GetOriginalPoint() { return originalPoints; }

    protected override void Awake()
    {
        originalPoints = points.ToArray();
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points == null || points.Length != 4)
        {
            Debug.LogWarning($"vertex가 4개가 아님");
            return;
        }

        Rect rect = GetPixelAdjustedRect();

        for (int i = 0; i < 4; i++)
        {
            Vector2 local = SvgToLocal(points[i], viewBoxSize, rect);

            UIVertex v = UIVertex.simpleVert;
            v.color = color;
            v.position = local;
            v.uv0 = new Vector2(
                points[i].x / viewBoxSize.x,
                1f - (points[i].y / viewBoxSize.y));

            vh.AddVert(v);
        }

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(0, 2, 3);
    }

    Vector2 SvgToLocal(Vector2 svgPoint, Vector2 viewBoxSize, Rect rect)
    {
        if (viewBoxSize.x <= 0f || viewBoxSize.y <= 0f) // 0나누기 방지.
            return Vector2.zero;

        float u = svgPoint.x / viewBoxSize.x;
        float v = svgPoint.y / viewBoxSize.y;
        float x = rect.xMin + u * rect.width;
        float y = rect.yMax - v * rect.height;
        return new Vector2(x, y);
    }

    public void SetPoints(Vector2[] points)
    {
        if (points == null || points.Length != 4) return;
        this.points = points;
        SetVerticesDirty();
    }




#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();

        SetBoxSize();
        rectTransform.sizeDelta = viewBoxSize;
    }
#endif

    private void SetBoxSize()
    {
        float x1 = Mathf.Abs(points[0].x - points[1].x);
        float x2 = Mathf.Abs(points[2].x - points[3].x);
        float y1 = Mathf.Abs(points[0].y - points[3].y);
        float y2 = Mathf.Abs(points[1].y - points[2].y);

        float w = Mathf.Max(x1, x2);
        float h = Mathf.Max(y1, y2);

        // 0이면 이전 값 유지
        if (w > 0.0001f) viewBoxSize.x = w;
        if (h > 0.0001f) viewBoxSize.y = h;
    }
}
