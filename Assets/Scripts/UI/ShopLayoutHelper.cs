using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店布局辅助脚本
/// 挂载到商品按钮的父物体上 (Content)，它会自动帮你：
/// 1. 固定所有子物体的位置（禁止拖动）
/// 2. 自动整齐排列（水平居中）
/// 3. 自动适配间距
/// </summary>
[RequireComponent(typeof(HorizontalLayoutGroup))]
[RequireComponent(typeof(ContentSizeFitter))]
public class ShopLayoutHelper : MonoBehaviour
{
    [Header("布局设置")]
    public float spacing = 20f; // 商品之间的间距
    public TextAnchor alignment = TextAnchor.MiddleCenter; // 居中对齐
    public bool disableScrolling = true; // 是否强制禁止滚动

    void Start()
    {
        SetupLayout();
    }

    void OnValidate()
    {
        // 在编辑器里修改数值时实时生效
        SetupLayout();
    }

    [ContextMenu("强制刷新布局")]
    public void SetupLayout()
    {
        // 1. 设置水平布局组
        var group = GetComponent<HorizontalLayoutGroup>();
        if (group != null)
        {
            group.spacing = spacing;
            group.childAlignment = alignment;
            group.childControlWidth = false; // 不强制拉伸宽度，保持商品原有大小
            group.childControlHeight = false; // 不强制拉伸高度
            group.childForceExpandWidth = false;
            group.childForceExpandHeight = false;
        }

        // 2. 设置内容适配器
        var fitter = GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // 3. 如果父级或更上级有 ScrollRect，尝试禁用它
        if (disableScrolling)
        {
            var scrollRect = GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                // 如果商品数量少，直接禁用滚动
                scrollRect.horizontal = false;
                scrollRect.vertical = false;
                scrollRect.movementType = ScrollRect.MovementType.Clamped; // 防止回弹
            }
        }
    }
}
