using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// 角色白边高亮效果
/// 鼠标悬停时显示白边，移出时隐藏
/// 键盘输入invoke时立即隐藏白边，1秒后根据鼠标位置决定是否重新显示
/// </summary>
[RequireComponent(typeof(Image))]
public class CharacterOutlineHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("白边图片")]
    [Tooltip("白边Image组件，鼠标悬停时显示")]
    public Image outlineImage;
    
    [Header("输入invoke设置")]
    [Tooltip("输入invoke后延迟检测时间（秒）")]
    public float inputInvokeDelay = 1f;
    
    [Header("调试")]
    [Tooltip("显示调试日志")]
    public bool showDebugLog = false;
    
    // 鼠标是否在图片上
    private bool isMouseOver = false;
    
    // 是否正在处理输入invoke
    private bool isHandlingInputInvoke = false;
    
    // 延迟检测协程
    private Coroutine delayCheckCoroutine = null;
    
    void Start()
    {
        if (outlineImage != null)
        {
            // 初始隐藏白边
            outlineImage.gameObject.SetActive(false);
            
            if (showDebugLog)
            {
                Debug.Log($"[CharacterOutlineHover] 初始化 {gameObject.name}，白边已隐藏");
            }
        }
        else
        {
            Debug.LogError($"[CharacterOutlineHover] {gameObject.name} 未设置 outlineImage！");
        }
        
        // 检查Raycast Target
        Image img = GetComponent<Image>();
        if (img != null && !img.raycastTarget && showDebugLog)
        {
            Debug.LogWarning($"[CharacterOutlineHover] {gameObject.name} 的 Raycast Target 未勾选！");
        }
        
        // 订阅输入事件
        InputSystem.OnPlayerInput += OnInputInvoke;
    }
    
    void OnEnable()
    {
        // GameObject被重新激活时，重置状态
        isMouseOver = false;
        isHandlingInputInvoke = false;
        
        // 停止所有协程
        if (delayCheckCoroutine != null)
        {
            StopCoroutine(delayCheckCoroutine);
            delayCheckCoroutine = null;
        }
        
        // 确保白边隐藏
        if (outlineImage != null)
        {
            outlineImage.gameObject.SetActive(false);
        }
        
        if (showDebugLog)
        {
            Debug.Log($"[CharacterOutlineHover] {gameObject.name} 已重新激活，状态已重置");
        }
    }
    
    void OnDisable()
    {
        // GameObject被停用时，停止所有协程
        if (delayCheckCoroutine != null)
        {
            StopCoroutine(delayCheckCoroutine);
            delayCheckCoroutine = null;
        }
        
        isHandlingInputInvoke = false;
        
        if (showDebugLog)
        {
            Debug.Log($"[CharacterOutlineHover] {gameObject.name} 已停用");
        }
    }
    
    void OnDestroy()
    {
        // 取消订阅输入事件
        InputSystem.OnPlayerInput -= OnInputInvoke;
    }
    
    /// <summary>
    /// 输入invoke处理 - 最高优先级
    /// </summary>
    private void OnInputInvoke()
    {
        if (outlineImage == null) return;
        
        // 检查当前GameObject是否激活
        if (!gameObject.activeInHierarchy)
        {
            if (showDebugLog)
            {
                Debug.LogWarning($"[CharacterOutlineHover] GameObject {gameObject.name} 未激活，无法启动协程");
            }
            return;
        }
        
        // 立即隐藏白边（输入invoke权限最大）
        outlineImage.gameObject.SetActive(false);
        isHandlingInputInvoke = true;
        
        if (showDebugLog)
        {
            Debug.Log($"[CharacterOutlineHover] 收到输入invoke - 立即隐藏白边");
        }
        
        // 停止之前的延迟检测协程
        if (delayCheckCoroutine != null)
        {
            StopCoroutine(delayCheckCoroutine);
        }
        
        // 启动延迟检测协程
        delayCheckCoroutine = StartCoroutine(DelayCheckMousePosition());
    }
    
    /// <summary>
    /// 延迟检测鼠标位置
    /// </summary>
    private IEnumerator DelayCheckMousePosition()
    {
        // 等待指定时间
        yield return new WaitForSeconds(inputInvokeDelay);
        
        // 检测鼠标是否在图片上
        if (isMouseOver)
        {
            // 鼠标在图片上，重新显示白边
            if (outlineImage != null)
            {
                outlineImage.gameObject.SetActive(true);
                
                if (showDebugLog)
                {
                    Debug.Log($"[CharacterOutlineHover] 延迟检测 - 鼠标在图片上，重新显示白边");
                }
            }
        }
        else
        {
            if (showDebugLog)
            {
                Debug.Log($"[CharacterOutlineHover] 延迟检测 - 鼠标不在图片上，保持隐藏");
            }
        }
        
        isHandlingInputInvoke = false;
        delayCheckCoroutine = null;
    }
    
    /// <summary>
    /// 鼠标进入
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        
        // 如果正在处理输入invoke，不立即显示白边
        if (isHandlingInputInvoke)
        {
            if (showDebugLog)
            {
                Debug.Log($"[CharacterOutlineHover] 鼠标进入 - 正在处理输入invoke，暂不显示白边");
            }
            return;
        }
        
        if (outlineImage != null)
        {
            outlineImage.gameObject.SetActive(true);
            
            if (showDebugLog)
            {
                Debug.Log($"[CharacterOutlineHover] 鼠标进入 - 显示白边");
            }
        }
    }
    
    /// <summary>
    /// 鼠标离开
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        
        if (outlineImage != null)
        {
            outlineImage.gameObject.SetActive(false);
            
            if (showDebugLog)
            {
                Debug.Log($"[CharacterOutlineHover] 鼠标离开 - 隐藏白边");
            }
        }
    }
}
