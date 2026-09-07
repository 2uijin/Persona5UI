using UnityEngine;

[RequireComponent(typeof(NavbarTrapezoidShape))]
public class NavbarJellyVertexAnimator : MonoBehaviour
{
    public enum WobbleDirection { Horizontal, Vertical }

    [Header("Vertex")]
    public Vector2[] basePoints = new Vector2[4]
    {
        new Vector2(10f, 0f),
        new Vector2(98f, 0f),
        new Vector2(108.1f, 47f),
        new Vector2(0f, 47f),
    };

    [Header("Animation")]
    [Tooltip("Direction")]
    public WobbleDirection direction = WobbleDirection.Horizontal;
    [Tooltip("주축 방향으로 벌어지는 배율 (기존 scaleX=1.25 고정값이었던 부분)")]
    public float stretchAmount = 1.25f;
    [Tooltip("보조축 스케일이 오갈 폭 (0.1 = 100%~110%)")]
    public float squishAmount = 0.1f;
    [Tooltip("스큐 각도 진폭 (기존 20 / -15에 대응)")]
    public float skewAmountPos = 20f;
    public float skewAmountNeg = -15f;
    [Tooltip("보조 이동 거리 (기존 translateY 3 / -2에 대응)")]
    public float moveAmountPos = 3f;
    public float moveAmountNeg = -2f;

    [Header("Animation")]
    public float duration = 0.6f;
    public bool playOnEnable = true;

    struct Keyframe
    {
        public float scaleX, scaleY, skewDeg, moveAmt;
        public Keyframe(float sx, float sy, float skew, float move)
        { scaleX = sx; scaleY = sy; skewDeg = skew; moveAmt = move; }
    }

    Keyframe KF0, KF1, KF2;

    NavbarTrapezoidShape shape;
    float timer;
    bool playing;

    void Awake()
    {
        shape = GetComponent<NavbarTrapezoidShape>();
        BuildKeyframes();
    }

    void OnValidate() => BuildKeyframes();

    void BuildKeyframes()
    {
        float mainScale = stretchAmount;
        float squishLow = 1f;
        float squishHigh = 1f + squishAmount;

        if (direction == WobbleDirection.Horizontal)
        {
            // 주축(X)은 고정 확장, 보조축(Y)이 오가며 스큐/이동은 X축 기준
            KF0 = new Keyframe(mainScale, squishLow, skewAmountPos, moveAmountPos);
            KF1 = new Keyframe(mainScale, squishHigh, skewAmountNeg, moveAmountNeg);
        }
        else
        {
            // 주축(Y)은 고정 확장, 보조축(X)이 오가며 스큐/이동은 Y축 기준
            KF0 = new Keyframe(squishLow, mainScale, skewAmountPos, moveAmountPos);
            KF1 = new Keyframe(squishHigh, mainScale, skewAmountNeg, moveAmountNeg);
        }
        KF2 = KF0;
    }

    void OnEnable()
    {
        timer = 0f;
        playing = playOnEnable;
    }

    public void Play() { timer = 0f; playing = true; }

    public void Stop()
    {
        playing = false;
        shape.SetPoints((Vector2[])basePoints.Clone());
    }

    void Update()
    {
        if (!playing) return;
        timer += Time.deltaTime;
        // PingPong 대신 Repeat: 0→1로 흘러가다 100%(=0%)에서 다시 0으로 이어붙음 → 끊김 없음
        float t = Mathf.Repeat(timer / duration, 1f);
        Keyframe kf;
        if (t < 0.5f)
        {
            float u = Mathf.SmoothStep(0f, 1f, t / 0.5f);
            kf = Lerp(KF0, KF1, u);
        }
        else
        {
            float u = Mathf.SmoothStep(0f, 1f, (t - 0.5f) / 0.5f);
            kf = Lerp(KF1, KF2, u);
        }
        ApplyKeyframe(kf);
    }

    Keyframe Lerp(Keyframe a, Keyframe b, float u)
    {
        return new Keyframe(
            Mathf.Lerp(a.scaleX, b.scaleX, u),
            Mathf.Lerp(a.scaleY, b.scaleY, u),
            Mathf.Lerp(a.skewDeg, b.skewDeg, u),
            Mathf.Lerp(a.moveAmt, b.moveAmt, u)
        );
    }

    void ApplyKeyframe(Keyframe kf)
    {
        float centerX = 0f, centerY = 0f;
        for (int i = 0; i < 4; i++) { centerX += basePoints[i].x; centerY += basePoints[i].y; }
        centerX *= 0.25f; centerY *= 0.25f;

        float skewRad = kf.skewDeg * Mathf.Deg2Rad;
        Vector2[] animated = new Vector2[4];

        for (int i = 0; i < 4; i++)
        {
            Vector2 p = basePoints[i];
            float x = centerX + (p.x - centerX) * kf.scaleX;
            float y = centerY + (p.y - centerY) * kf.scaleY;

            if (direction == WobbleDirection.Horizontal)
            {
                // skewX: y가 클수록 x가 더 밀림
                x += (p.y - centerY) * Mathf.Tan(skewRad);
                y += kf.moveAmt; // 보조 상하 이동
            }
            else
            {
                // skewY: x가 클수록 y가 더 밀림
                y += (p.x - centerX) * Mathf.Tan(skewRad);
                x += kf.moveAmt; // 보조 좌우 이동
            }

            animated[i] = new Vector2(x, y);
        }
        shape.SetPoints(animated);
    }
}