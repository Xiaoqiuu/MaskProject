using UnityEngine;

/// <summary>
/// 输入监听器示例 - 展示如何订阅和响应输入事件
/// 你的同事可以参考这个脚本来编写判定逻辑
/// </summary>
public class InputListener : MonoBehaviour
{
    [Header("调试设置")]
    public bool showDebugLog = true;
    
    void OnEnable()
    {
        // 订阅输入事件
        InputSystem.OnPlayerInput += OnPlayerInputReceived;
        
        if (showDebugLog)
        {
            Debug.Log("[InputListener] 已订阅输入事件");
        }
    }
    
    void OnDisable()
    {
        // 取消订阅输入事件（重要！防止内存泄漏）
        InputSystem.OnPlayerInput -= OnPlayerInputReceived;
        
        if (showDebugLog)
        {
            Debug.Log("[InputListener] 已取消订阅输入事件");
        }
    }
    
    /// <summary>
    /// 当接收到玩家输入时调用
    /// 在这里编写你的判定逻辑
    /// </summary>
    private void OnPlayerInputReceived()
    {
        if (showDebugLog)
        {
            Debug.Log("[InputListener] 收到玩家输入！执行判定逻辑...");
        }
        
        // ===== 在这里添加你的判定逻辑 =====
        // 例如：
        // - 检查寿司是否在正确位置
        // - 计算得分
        // - 播放音效
        // - 触发动画
        // 等等...
        
        PerformJudgement();
    }
    
    /// <summary>
    /// 执行判定逻辑（示例）
    /// </summary>
    private void PerformJudgement()
    {
        // 这里是判定逻辑的示例
        // 你的同事可以在这里编写具体的判定代码
        
        if (showDebugLog)
        {
            Debug.Log("[InputListener] 执行判定：Perfect!");
        }
    }
}
