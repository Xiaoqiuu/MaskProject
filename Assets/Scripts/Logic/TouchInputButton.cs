using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 触摸输入按钮
/// 这是一个透明的全屏按钮，用于触摸/点击模式
/// 挂载到Canvas下的一个全屏Image上
/// </summary>
[RequireComponent(typeof(Image))]
public class TouchInputButton : MonoBehaviour, IPointerDownHandler
{
    private Image buttonImage;
    
    [Header("设置")]
    public bool onlyActiveInTouchMode = true; // 是否只在触摸模式下激活
    
    void Awake()
    {
        buttonImage = GetComponent<Image>();
        
        // 设置为透明
        Color color = buttonImage.color;
        color.a = 0f; // 完全透明
        buttonImage.color = color;
        
        // 确保可以接收射线检测
        buttonImage.raycastTarget = true;
    }
    
    void Update()
    {
        // 根据当前输入模式决定是否激活
        if (onlyActiveInTouchMode && InputSystem.Instance != null)
        {
            bool shouldBeActive = InputSystem.Instance.GetCurrentInputMode() == InputMode.Touch;
            
            if (gameObject.activeSelf != shouldBeActive)
            {
                gameObject.SetActive(shouldBeActive);
            }
        }
    }
    
    /// <summary>
    /// 当按钮被点击时
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // 触发输入事件
        // 注意：InputSystem 会自动检测点击，所以这里不需要手动触发
        // 这个方法主要用于提供额外的反馈或特殊处理
        
        Debug.Log("[TouchInputButton] 按钮被点击");
    }
}
