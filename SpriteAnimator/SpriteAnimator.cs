using UnityEngine;
using VoizehSpriteAnimator;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteAnimation CurrentAnimation => currentAnimation;

    public int CurrentFrame => currentFrame;
    public float FramePassedTime => framePassedTime;

    public SpriteAnimatorController Controller
    {
        get => controller;
        set
        {
            controller = value;

            if (value != null && value.AnimationsCount > 0)
                PlayAnimationByIndex(0);
            else
                framesCount = 0;
        }
    }

    public CullingMode CullMode
    {
        get => cullingMode;
        set
        {
            cullingMode = value;

            if (value == CullingMode.AlwaysAnimate)
                Culled = false;
        }
    }

    public PositionType TypeOfPosition
    {
        get => positionType;
        set => positionType = value;
    }

    public bool Culled
    {
        get => culled;
        set
        {
            bool wasCulled = culled;

            culled = value;

            if (wasCulled && value == false)
                UpdateSpriteRendererFrame();
        }
    }

    public Rect Bounds
    {
        get => bounds;
        set => bounds = value;
    }
 
    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0f, float.MaxValue);
    }

    public Vector2 Position
    {
        get
        {
            if (isStatic)
                return position;

            if (positionType == PositionType.GlobalPosition)
                return myTransform.position;
            else
                return myTransform.localPosition;
        }
    }

    public bool IsStatic
    {
        get => isStatic;
        set
        {
            isStatic = value;
            if (value)
                position = transform.position;
        }
    }


    [SerializeField] private SpriteAnimatorController controller;
    [Space]
    [SerializeField] private bool playOnAwake = true;
    [Space]
    [Range(0f, 10f)]
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool isStatic = false;
    [SerializeField] private PositionType positionType = PositionType.GlobalPosition;
    [Header("Cull Type And Bounds")]
    [SerializeField] private CullingMode cullingMode = CullingMode.CullAutomatic;
    [SerializeField] private Rect bounds = new Rect(0f, 0f, 1f, 1f);


    public event System.Action AnimationEnd;


    private SpriteAnimation currentAnimation;

    private int framesCount;
    private float frameLenght = 0.1f;
    private float framePassedTime;

    private bool culled = false;

    private int currentFrame = 0;

    private SpriteRenderer spriteRenderer;
    private Transform myTransform;
    private Vector2 position;

    private bool initialized = false;


    public enum CullingMode { AlwaysAnimate, CullAutomatic }
    public enum PositionType { GlobalPosition, LocalPosition }


    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if (SpriteAnimatorUpdater.HasInstance == false)
            SpriteAnimatorUpdater.CreateInstance();

        SpriteAnimatorUpdater.Instance.RegisterAnimator(this);
    }

    private void OnDisable()
    {
        if (SpriteAnimatorUpdater.HasInstance)
            SpriteAnimatorUpdater.Instance.AnimatorDeactivated(this);
    }

    private void OnDestroy()
    {
        if (SpriteAnimatorUpdater.HasInstance)
            SpriteAnimatorUpdater.Instance.AnimatorDestroyed(this);

        AnimationEnd = null;
    }

    private void Initialize()
    {
        if (initialized)
            return;

        initialized = true;

        spriteRenderer = GetComponent<SpriteRenderer>();

        myTransform = GetComponent<Transform>();
        position = myTransform.position;

        if (controller != null && controller.AnimationsCount > 0 && playOnAwake)
        {
            PlayAnimationByIndex(0);
        }
        else
        {
            framesCount = 0;
        }
    }

    public void UpdateAnimation(float deltaTime)
    {
        if (framesCount <= 0)
            return;

        framePassedTime += deltaTime * speed;

        if (framePassedTime >= frameLenght)
        {
            framePassedTime -= frameLenght;

            currentFrame += 1;
            if (currentFrame >= framesCount)
            {
                if (currentFrame == framesCount)
                {
                    AnimationEnd?.Invoke();
                }
                if (currentAnimation.Loop)
                {
                    currentFrame = 0;
                    framePassedTime = 0f;
                    frameLenght = currentAnimation.GetFrameLenght(currentFrame);
                }
            }
            else
            {
                frameLenght = currentAnimation.GetFrameLenght(currentFrame);
            }

            if (culled == false)
            {
                UpdateSpriteRendererFrame();
            }
        }
    }

    public void PlayAnimationByIndex(int index)
    {
        currentAnimation = controller.GetAnimationByIndex(index);
        UpdateAnimationValues();
    }

    public void PlayAnimation(string name)
    {
        currentAnimation = controller.GetAnimation(name);
        UpdateAnimationValues();
    }

    public void PlayAnimation(SpriteAnimation animation)
    {
        currentAnimation = animation;
        UpdateAnimationValues();
    }

    public void PlayAnimation(int hash)
    {
        currentAnimation = controller.GetAnimation(hash);
        UpdateAnimationValues();
    }

    private void UpdateAnimationValues()
    {
        Initialize();

        currentFrame = 0;
        framePassedTime = 0f;

        InitializeAnimationValues(currentAnimation);

        spriteRenderer.sprite = framesCount > 0 ? currentAnimation.FirstFrame : null;
    }

    private void InitializeAnimationValues(SpriteAnimation animation)
    {
        framesCount = animation.FramesCount;
        frameLenght = animation.GetFrameLenght(0);
    }

    private void UpdateSpriteRendererFrame()
    {
        int frameIndex = Mathf.Clamp(currentFrame, 0, framesCount - 1);
        spriteRenderer.sprite = currentAnimation[frameIndex];
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color oldColor = Gizmos.color;

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(transform.position + (Vector3)bounds.position, bounds.size);

        Gizmos.color = oldColor;
    }

    private void OnValidate()
    {
        if (initialized == false)
            return;

        TypeOfPosition = positionType;

        if (cullingMode == CullingMode.AlwaysAnimate)
            culled = false;

        IsStatic = isStatic;
    }
#endif
}
