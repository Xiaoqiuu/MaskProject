using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// GameScene 推拉门动画控制器
/// 场景开始时门拉开，游戏结束时门关闭
/// </summary>
public class GameSceneTransition : MonoBehaviour
{
    [Header("推拉门图片")]
    [Tooltip("左侧门图片")]
    public Image leftDoor;
    
    [Tooltip("右侧门图片")]
    public Image rightDoor;
    
    [Header("动画设置")]
    [Tooltip("门打开动画时长（秒）")]
    public float openDuration = 1.0f;
    
    [Tooltip("门关闭动画时长（秒）")]
    public float closeDuration = 1.0f;
    
    [Header("场景切换")]
    [Tooltip("游戏结束后跳转的场景名称")]
    public string targetSceneName = "HomeScene";
    
    [Header("音效")]
    [Tooltip("门动画音效")]
    public AudioClip doorSound;
    
    [Tooltip("音效音量")]
    [Range(0f, 1f)]
    public float soundVolume = 1.0f;
    
    [Header("调试")]
    [Tooltip("显示调试日志")]
    public bool showDebugLog = false;
    
    // 音频源
    private AudioSource audioSource;
    
    // 门的位置
    private Vector2 leftDoorOpenPos;   // 左门打开位置（左侧外）
    private Vector2 leftDoorClosedPos; // 左门关闭位置（中心）
    private Vector2 rightDoorOpenPos;  // 右门打开位置（右侧外）
    private Vector2 rightDoorClosedPos; // 右门关闭位置（中心）
    
    // Canvas
    private Canvas canvas;
    
    // 是否正在执行动画
    private bool isAnimating = false;

    void Awake()
    {
        // 设置音频源
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        
        // 获取Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[GameSceneTransition] 找不到父级Canvas！");
        }
    }

    void Start()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("[GameSceneTransition] 左右门图片未设置！");
            return;
        }
        
        // 初始化门的大小和位置
        InitializeDoors();
        
        // 门初始状态：关闭（在中心）
        leftDoor.rectTransform.anchoredPosition = leftDoorClosedPos;
        rightDoor.rectTransform.anchoredPosition = rightDoorClosedPos;
        
        // 场景开始时播放拉开动画
        StartCoroutine(OpenDoorsOnStart());
        
        if (showDebugLog)
        {
            Debug.Log("[GameSceneTransition] 初始化完成");
        }
    }

    void OnEnable()
    {
        // 订阅游戏结束事件
        // 注意：需要等待GameManager初始化
        StartCoroutine(SubscribeToGameManager());
    }

    void OnDisable()
    {
        // 取消订阅
        if (GameManager.Instance != null)
        {
            // GameManager没有GameOver事件，我们需要手动调用
            // 这里暂时不做处理，由外部调用CloseDoorsAndExit
        }
    }

    /// <summary>
    /// 等待并订阅GameManager事件
    /// </summary>
    private IEnumerator SubscribeToGameManager()
    {
        // 等待GameManager初始化
        while (GameManager.Instance == null)
        {
            yield return null;
        }
        
        if (showDebugLog)
        {
            Debug.Log("[GameSceneTransition] 已连接到GameManager");
        }
    }

    /// <summary>
    /// 初始化门的位置（不改变尺寸）
    /// </summary>
    private void InitializeDoors()
    {
        float canvasWidth;
        
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasWidth = canvasRect.rect.width;
        }
        else
        {
            canvasWidth = Screen.width;
        }
        
        // 获取门的当前宽度（保持原始尺寸）
        float leftDoorWidth = leftDoor.rectTransform.rect.width;
        float rightDoorWidth = rightDoor.rectTransform.rect.width;
        
        // 设置锚点为屏幕中心
        leftDoor.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        leftDoor.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rightDoor.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rightDoor.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        
        // 左门：pivot在右边缘中点（1, 0.5）
        leftDoor.rectTransform.pivot = new Vector2(1f, 0.5f);
        
        // 右门：pivot在左边缘中点（0, 0.5）
        rightDoor.rectTransform.pivot = new Vector2(0f, 0.5f);
        
        // 计算位置
        // 左门：打开位置在左侧外（-门宽度），关闭位置在中心（0）
        leftDoorOpenPos = new Vector2(-leftDoorWidth, 0);
        leftDoorClosedPos = new Vector2(0, 0);
        
        // 右门：打开位置在右侧外（门宽度），关闭位置在中心（0）
        rightDoorOpenPos = new Vector2(rightDoorWidth, 0);
        rightDoorClosedPos = new Vector2(0, 0);
        
        if (showDebugLog)
        {
            Debug.Log($"[GameSceneTransition] Canvas宽度: {canvasWidth}");
            Debug.Log($"[GameSceneTransition] 左门宽度: {leftDoorWidth}, 右门宽度: {rightDoorWidth}");
            Debug.Log($"[GameSceneTransition] 左门 - 打开: {leftDoorOpenPos}, 关闭: {leftDoorClosedPos}");
            Debug.Log($"[GameSceneTransition] 右门 - 打开: {rightDoorOpenPos}, 关闭: {rightDoorClosedPos}");
        }
    }

    /// <summary>
    /// 场景开始时打开门
    /// </summary>
    private IEnumerator OpenDoorsOnStart()
    {
        if (isAnimating) yield break;
        isAnimating = true;
        
        // 播放音效
        PlaySound();
        
        if (showDebugLog)
        {
            Debug.Log("[GameSceneTransition] 开始拉开门动画");
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / openDuration;
            
            // 使用平滑曲线
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            // 从关闭位置移动到打开位置
            leftDoor.rectTransform.anchoredPosition = Vector2.Lerp(leftDoorClosedPos, leftDoorOpenPos, smoothT);
            rightDoor.rectTransform.anchoredPosition = Vector2.Lerp(rightDoorClosedPos, rightDoorOpenPos, smoothT);
            
            yield return null;
        }
        
        // 确保到达最终位置
        leftDoor.rectTransform.anchoredPosition = leftDoorOpenPos;
        rightDoor.rectTransform.anchoredPosition = rightDoorOpenPos;
        
        if (showDebugLog)
        {
            Debug.Log("[GameSceneTransition] 门已拉开");
        }
        
        isAnimating = false;
    }

    /// <summary>
    /// 游戏结束时关闭门并切换场景（公共方法，供GameManager调用）
    /// </summary>
    public void CloseDoorsAndExit()
    {
        if (!isAnimating)
        {
            StartCoroutine(CloseDoorsCoroutine());
        }
    }

    /// <summary>
    /// 关闭门并切换场景
    /// </summary>
    private IEnumerator CloseDoorsCoroutine()
    {
        if (isAnimating) yield break;
        isAnimating = true;
        
        // 播放音效
        PlaySound();
        
        if (showDebugLog)
        {
            Debug.Log("[GameSceneTransition] 开始关闭门动画");
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < closeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // 使用unscaledDeltaTime以防游戏暂停
            float t = elapsedTime / closeDuration;
            
            // 使用平滑曲线
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            // 从打开位置移动到关闭位置
            leftDoor.rectTransform.anchoredPosition = Vector2.Lerp(leftDoorOpenPos, leftDoorClosedPos, smoothT);
            rightDoor.rectTransform.anchoredPosition = Vector2.Lerp(rightDoorOpenPos, rightDoorClosedPos, smoothT);
            
            yield return null;
        }
        
        // 确保到达最终位置
        leftDoor.rectTransform.anchoredPosition = leftDoorClosedPos;
        rightDoor.rectTransform.anchoredPosition = rightDoorClosedPos;
        
        if (showDebugLog)
        {
            Debug.Log("[GameSceneTransition] 门已关闭");
        }
        
        // 等待一小段时间
        yield return new WaitForSecondsRealtime(0.5f);
        
        // 切换场景
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            if (showDebugLog)
            {
                Debug.Log($"[GameSceneTransition] 切换到场景: {targetSceneName}");
            }
            SceneManager.LoadScene(targetSceneName);
        }
        
        isAnimating = false;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    private void PlaySound()
    {
        if (doorSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(doorSound, soundVolume);
        }
    }
}
