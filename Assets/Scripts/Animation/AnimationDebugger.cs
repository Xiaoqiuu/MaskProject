using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 动画调试器 - 帮助诊断 Timeline 和动画问题
/// </summary>
public class AnimationDebugger : MonoBehaviour
{
    [Header("监控对象")]
    public GameObject targetObject;
    public PlayableDirector playableDirector;
    
    [Header("调试设置")]
    public bool logEveryFrame = false;
    public KeyCode debugKey = KeyCode.F12;
    
    private bool lastActiveState = false;
    private int frameCount = 0;
    
    void Update()
    {
        if (targetObject == null) return;
        
        bool currentActiveState = targetObject.activeSelf;
        
        // 检测状态变化
        if (currentActiveState != lastActiveState)
        {
            Debug.Log($"[AnimationDebugger] 帧 {Time.frameCount}: GameObject 状态变化 {lastActiveState} → {currentActiveState}");
            lastActiveState = currentActiveState;
        }
        
        // 每帧日志
        if (logEveryFrame)
        {
            frameCount++;
            if (frameCount % 30 == 0)
            {
                Debug.Log($"[AnimationDebugger] 帧 {Time.frameCount}: Active={currentActiveState}, TimeScale={Time.timeScale}");
                
                if (playableDirector != null)
                {
                    Debug.Log($"[AnimationDebugger] Timeline State={playableDirector.state}, Time={playableDirector.time}");
                }
            }
        }
        
        // 手动调试
        if (Input.GetKeyDown(debugKey))
        {
            PrintDebugInfo();
        }
    }
    
    void PrintDebugInfo()
    {
        Debug.Log("========== 动画调试信息 ==========");
        Debug.Log($"当前帧: {Time.frameCount}");
        Debug.Log($"Time.timeScale: {Time.timeScale}");
        Debug.Log($"GameObject Active: {targetObject != null && targetObject.activeSelf}");
        
        if (playableDirector != null)
        {
            Debug.Log($"PlayableDirector State: {playableDirector.state}");
            Debug.Log($"PlayableDirector Time: {playableDirector.time}");
            Debug.Log($"PlayableDirector Duration: {playableDirector.duration}");
        }
        
        var characterAnim = FindObjectOfType<CharacterAnimation>();
        if (characterAnim != null)
        {
            Debug.Log($"CharacterAnimation IsActive: {characterAnim.IsActive()}");
        }
        
        Debug.Log("==================================");
    }
}
