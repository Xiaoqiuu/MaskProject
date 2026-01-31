using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色帧动画控制器
/// 放手序列：1 1 2 3 4
/// 抬手序列：4 3 2 1 1
/// 支持动画期间打断并切换方向
/// </summary>
[RequireComponent(typeof(Image))]
public class CharacterAnimation : MonoBehaviour
{
    [Header("动画帧设置")]
    [Tooltip("4张动画帧图片")]
    public Sprite[] animationFrames = new Sprite[4];
    
    [Header("播放设置")]
    [Tooltip("每帧持续时间（秒），默认1帧 = 1/60秒")]
    public float frameTime = 1f / 60f;
    
    [Header("调试信息")]
    public bool showDebugLog = false;
    
    // 组件引用
    private Image characterImage;
    
    // 动画状态
    private bool isPlaying = false;
    private Coroutine animationCoroutine;
    
    // 动画方向：true = 放手（1→4），false = 抬手（4→1）
    private bool isGoingDown = true;
    
    // 当前显示的帧（0-3，对应图片1-4）
    private int currentFrame = 0;
    
    // 放手序列：1 1 2 3 4（索引：0 0 1 2 3）
    private readonly int[] downSequence = { 0, 0, 1, 2, 3 };
    
    // 抬手序列：4 3 2 1 1（索引：3 2 1 0 0）
    private readonly int[] upSequence = { 3, 2, 1, 0, 0 };
    
    void Awake()
    {
        characterImage = GetComponent<Image>();
        
        if (characterImage == null)
        {
            Debug.LogError("[CharacterAnimation] 未找到 Image 组件！");
        }
    }
    
    void Start()
    {
        // 设置默认第一帧
        SetFrame(0);
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 角色动画控制器已初始化");
        }
    }
    
    void OnEnable()
    {
        // 订阅输入事件
        InputSystem.OnPlayerInput += OnPlayerInputReceived;
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 已订阅输入事件");
        }
    }
    
    void OnDisable()
    {
        // 取消订阅输入事件
        InputSystem.OnPlayerInput -= OnPlayerInputReceived;
        
        // 停止协程
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 已取消订阅输入事件");
        }
    }
    
    /// <summary>
    /// 当接收到玩家输入时触发
    /// </summary>
    private void OnPlayerInputReceived()
    {
        if (showDebugLog)
        {
            Debug.Log($"[CharacterAnimation] 收到输入 - 当前帧:{currentFrame + 1}, 方向:{(isGoingDown ? "放手" : "抬手")}, 播放中:{isPlaying}");
        }
        
        // 如果正在播放动画
        if (isPlaying)
        {
            // 如果当前是抬手动作，切换为放手动作
            if (!isGoingDown)
            {
                if (showDebugLog)
                {
                    Debug.Log($"[CharacterAnimation] 打断抬手动作，切换为放手，从帧 {currentFrame + 1} 继续");
                }
                
                // 切换方向
                isGoingDown = true;
                
                // 停止当前协程
                if (animationCoroutine != null)
                {
                    StopCoroutine(animationCoroutine);
                }
                
                // 从当前帧继续放手动作
                animationCoroutine = StartCoroutine(PlayFromCurrentFrame());
            }
            else
            {
                // 如果已经是放手动作，忽略输入
                if (showDebugLog)
                {
                    Debug.Log("[CharacterAnimation] 已经在放手，忽略输入");
                }
            }
        }
        else
        {
            // 没有播放动画，开始放手动作
            StartDownAnimation();
        }
    }
    
    /// <summary>
    /// 开始放手动画（完整序列）
    /// </summary>
    private void StartDownAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        isGoingDown = true;
        animationCoroutine = StartCoroutine(PlayDownSequence());
    }
    
    /// <summary>
    /// 从当前帧继续播放
    /// </summary>
    private IEnumerator PlayFromCurrentFrame()
    {
        isPlaying = true;
        
        if (showDebugLog)
        {
            Debug.Log($"[CharacterAnimation] 从帧 {currentFrame + 1} 继续放手");
        }
        
        // 找到当前帧在放手序列中的位置
        int startIndex = -1;
        for (int i = 0; i < downSequence.Length; i++)
        {
            if (downSequence[i] == currentFrame)
            {
                startIndex = i;
                break;
            }
        }
        
        // 如果找不到，从下一帧开始
        if (startIndex == -1)
        {
            startIndex = currentFrame;
        }
        
        // 从当前位置继续播放放手序列
        for (int i = startIndex; i < downSequence.Length; i++)
        {
            currentFrame = downSequence[i];
            SetFrame(currentFrame);
            
            if (showDebugLog)
            {
                Debug.Log($"[CharacterAnimation] 放手 [{i}/{downSequence.Length - 1}] 显示帧 {currentFrame + 1}");
            }
            
            yield return new WaitForSeconds(frameTime);
        }
        
        // 放手完成，开始抬手
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 放手完成，开始抬手");
        }
        
        isGoingDown = false;
        animationCoroutine = StartCoroutine(PlayUpSequence());
    }
    
    /// <summary>
    /// 播放完整的放手序列
    /// </summary>
    private IEnumerator PlayDownSequence()
    {
        isPlaying = true;
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 开始放手动作");
        }
        
        // 播放放手序列：1 1 2 3 4
        for (int i = 0; i < downSequence.Length; i++)
        {
            currentFrame = downSequence[i];
            SetFrame(currentFrame);
            
            if (showDebugLog)
            {
                Debug.Log($"[CharacterAnimation] 放手 [{i}/{downSequence.Length - 1}] 显示帧 {currentFrame + 1}");
            }
            
            yield return new WaitForSeconds(frameTime);
        }
        
        // 放手完成，开始抬手
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 放手完成，开始抬手");
        }
        
        isGoingDown = false;
        animationCoroutine = StartCoroutine(PlayUpSequence());
    }
    
    /// <summary>
    /// 播放抬手序列
    /// </summary>
    private IEnumerator PlayUpSequence()
    {
        isPlaying = true;
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 开始抬手动作");
        }
        
        // 播放抬手序列：4 3 2 1 1
        for (int i = 0; i < upSequence.Length; i++)
        {
            currentFrame = upSequence[i];
            SetFrame(currentFrame);
            
            if (showDebugLog)
            {
                Debug.Log($"[CharacterAnimation] 抬手 [{i}/{upSequence.Length - 1}] 显示帧 {currentFrame + 1}");
            }
            
            yield return new WaitForSeconds(frameTime);
        }
        
        // 抬手完成，回到待机状态
        currentFrame = 0;
        SetFrame(0);
        isPlaying = false;
        animationCoroutine = null;
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 抬手完成，回到待机");
        }
    }
    
    /// <summary>
    /// 设置显示的帧
    /// </summary>
    private void SetFrame(int frameIndex)
    {
        if (characterImage == null || animationFrames == null || frameIndex < 0 || frameIndex >= animationFrames.Length)
        {
            return;
        }
        
        if (animationFrames[frameIndex] != null)
        {
            characterImage.sprite = animationFrames[frameIndex];
        }
        else
        {
            Debug.LogWarning($"[CharacterAnimation] 帧 {frameIndex + 1} 的图片未设置！");
        }
    }
    
    /// <summary>
    /// 验证动画帧是否完整
    /// </summary>
    private bool ValidateFrames()
    {
        if (animationFrames == null || animationFrames.Length != 4)
        {
            Debug.LogError("[CharacterAnimation] 动画帧数组必须包含4张图片！");
            return false;
        }
        
        for (int i = 0; i < animationFrames.Length; i++)
        {
            if (animationFrames[i] == null)
            {
                Debug.LogError($"[CharacterAnimation] 动画帧 {i + 1} 未设置！");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 手动触发动画（用于测试）
    /// </summary>
    public void TriggerAnimation()
    {
        OnPlayerInputReceived();
    }
    
    /// <summary>
    /// 强制停止动画
    /// </summary>
    public void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        
        isPlaying = false;
        currentFrame = 0;
        isGoingDown = true;
        SetFrame(0);
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 动画已强制停止");
        }
    }
    
    /// <summary>
    /// 检查是否正在播放
    /// </summary>
    public bool IsPlaying()
    {
        return isPlaying;
    }
    
    /// <summary>
    /// 获取当前帧索引（0-3）
    /// </summary>
    public int GetCurrentFrameIndex()
    {
        return currentFrame;
    }
    
    /// <summary>
    /// 获取当前动画方向
    /// </summary>
    public bool IsGoingDown()
    {
        return isGoingDown;
    }
}
