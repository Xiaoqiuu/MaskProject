using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 角色动画控制器
/// 接收输入事件后激活目标 GameObject，等待 30 帧后自动关闭
/// 激活期间忽略所有新的输入
/// 兼容 Timeline 和 Animator
/// </summary>
public class CharacterAnimation : MonoBehaviour
{
    [Header("目标对象")]
    [Tooltip("需要激活/关闭的 GameObject")]
    public GameObject targetObject;
    
    [Header("激活设置")]
    [Tooltip("激活持续的帧数")]
    public int activeFrames = 30;
    
    [Header("Timeline 兼容")]
    [Tooltip("如果使用 Timeline，拖入 PlayableDirector 组件")]
    public PlayableDirector playableDirector;
    
    [Tooltip("是否在激活时播放 Timeline")]
    public bool playTimelineOnActivate = false;
    
    [Header("调试信息")]
    public bool showDebugLog = false;

    [Header("bug修复")]
    public GameObject g1;
    public GameObject g2;
    public GameObject g3;
    public GameObject g4;
    
    // 状态标记
    private bool isActive = false;
    private Coroutine activeCoroutine;
    private bool wasTimelineControlled = false;
    
    void Start()
    {
        // 自动查找 PlayableDirector
        if (playableDirector == null && targetObject != null)
        {
            playableDirector = targetObject.GetComponent<PlayableDirector>();
        }
        
        // 确保目标对象初始状态为关闭
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[CharacterAnimation] 未设置目标对象！请在 Inspector 中设置 Target Object");
        }
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 角色动画控制器已初始化");
            if (playableDirector != null)
            {
                Debug.Log("[CharacterAnimation] 检测到 PlayableDirector，已启用 Timeline 兼容模式");
            }
        }
    }
    
    void LateUpdate()
    {
        // 检测 Timeline 是否在控制对象
        if (playableDirector != null && targetObject != null)
        {
            bool isTimelineControlling = playableDirector.state == PlayState.Playing;
            
            // 如果 Timeline 开始控制，记录状态
            if (isTimelineControlling && !wasTimelineControlled)
            {
                wasTimelineControlled = true;
                if (showDebugLog)
                {
                    Debug.Log("[CharacterAnimation] Timeline 开始控制对象");
                }
            }
            // 如果 Timeline 停止控制，恢复脚本控制
            else if (!isTimelineControlling && wasTimelineControlled)
            {
                wasTimelineControlled = false;
                if (showDebugLog)
                {
                    Debug.Log("[CharacterAnimation] Timeline 停止控制，恢复脚本控制");
                }
                
                // 确保对象状态正确
                if (!isActive)
                {
                    targetObject.SetActive(false);
                }
            }
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
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
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
            Debug.Log($"[CharacterAnimation] 收到输入事件，当前状态: isActive={isActive}");
        }
        
        // 如果正在激活状态，忽略新的输入
        if (isActive)
        {
            if (showDebugLog)
            {
                Debug.LogWarning("[CharacterAnimation] ❌ 正在激活状态，忽略输入");
            }
            return;
        }
        
        // 激活目标对象
        ActivateTarget();
    }
    
    /// <summary>
    /// 激活目标对象
    /// </summary>
    public void ActivateTarget()
    {
        if (targetObject == null)
        {
            Debug.LogError("[CharacterAnimation] 目标对象未设置！");
            return;
        }
        
        // 再次检查状态，防止重复调用
        if (isActive)
        {
            if (showDebugLog)
            {
                Debug.LogWarning("[CharacterAnimation] ❌ 已经在激活状态，取消本次调用");
            }
            return;
        }
        
        // 立即设置为激活状态，防止重复触发
        isActive = true;
        
        // 如果已经有协程在运行，先停止
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        
        // 开始激活协程
        activeCoroutine = StartCoroutine(ActivateCoroutine());
    }
    
    /// <summary>
    /// 激活协程：激活对象 → 等待指定帧数 → 关闭对象
    /// </summary>
    private IEnumerator ActivateCoroutine()
    {
        // 如果 Timeline 正在播放，先停止
        if (playableDirector != null && playableDirector.state == PlayState.Playing)
        {
            playableDirector.Stop();
            if (showDebugLog)
            {
                Debug.Log("[CharacterAnimation] 停止 Timeline 播放");
            }
        }
        
        // 激活目标对象
        targetObject.SetActive(true);
        
        if (showDebugLog)
        {
            Debug.Log($"[CharacterAnimation] ✅ 激活目标对象，将在 {activeFrames} 帧后关闭");
        }
        
        // 如果需要播放 Timeline
        if (playTimelineOnActivate && playableDirector != null)
        {
            playableDirector.time = 0;
            playableDirector.Play();
            if (showDebugLog)
            {
                Debug.Log("[CharacterAnimation] 播放 Timeline");
            }
        }
        
        // 等待指定帧数
        int waitedFrames = 0;
        while (waitedFrames < activeFrames)
        {
            // 检查 Time.timeScale，如果为 0 则不计数（暂停状态）
            if (Time.timeScale > 0)
            {
                waitedFrames++;
                
                if (showDebugLog && waitedFrames % 10 == 0)
                {
                    Debug.Log($"[CharacterAnimation] 已等待 {waitedFrames}/{activeFrames} 帧");
                }
            }
            
            yield return null; // 等待一帧
        }
        
        // 停止 Timeline（如果在播放）
        if (playableDirector != null && playableDirector.state == PlayState.Playing)
        {
            playableDirector.Stop();
        }
        
        // 关闭目标对象
        targetObject.SetActive(false);
        g1.SetActive(true);
        g2.SetActive(false);
        g3.SetActive(false);
        g4.SetActive(false);
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] ✅ 关闭目标对象，可以接收新输入");
        }
        
        // 重置状态
        isActive = false;
        activeCoroutine = null;
    }
    
    /// <summary>
    /// 手动触发激活（用于测试）
    /// </summary>
    public void TriggerActivation()
    {
        if (!isActive)
        {
            ActivateTarget();
        }
    }
    
    /// <summary>
    /// 强制停止并关闭目标对象
    /// </summary>
    public void ForceStop()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
        
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
        
        isActive = false;
        
        if (showDebugLog)
        {
            Debug.Log("[CharacterAnimation] 强制停止");
        }
    }
    
    /// <summary>
    /// 检查是否正在激活状态
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }
}
