using UnityEngine;

/// <summary>
/// Sorting Layer 设置辅助工具
/// 用于快速设置对象的渲染层级
/// </summary>
public class SortingLayerSetter : MonoBehaviour
{
    [Header("Sorting Layer 设置")]
    [Tooltip("Sorting Layer 名称")]
    public string sortingLayerName = "Default";
    
    [Tooltip("Order in Layer（数值越大越在前面）")]
    public int orderInLayer = 0;
    
    [Header("自动应用")]
    [Tooltip("在 Start 时自动应用设置")]
    public bool applyOnStart = true;
    
    [Tooltip("应用到所有子对象")]
    public bool applyToChildren = false;
    
    void Start()
    {
        if (applyOnStart)
        {
            ApplySortingLayer();
        }
    }
    
    /// <summary>
    /// 应用 Sorting Layer 设置
    /// </summary>
    public void ApplySortingLayer()
    {
        // 应用到当前对象
        SetSortingLayer(gameObject, sortingLayerName, orderInLayer);
        
        // 应用到子对象
        if (applyToChildren)
        {
            SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in childRenderers)
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = orderInLayer;
            }
            
            Debug.Log($"[SortingLayerSetter] 已应用到 {childRenderers.Length} 个子对象");
        }
    }
    
    /// <summary>
    /// 设置指定对象的 Sorting Layer
    /// </summary>
    private void SetSortingLayer(GameObject obj, string layerName, int order)
    {
        // 尝试获取 SpriteRenderer
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = layerName;
            spriteRenderer.sortingOrder = order;
            Debug.Log($"[SortingLayerSetter] {obj.name}: SpriteRenderer 设置为 {layerName} ({order})");
            return;
        }
        
        // 尝试获取 Canvas
        Canvas canvas = obj.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingLayerName = layerName;
            canvas.sortingOrder = order;
            Debug.Log($"[SortingLayerSetter] {obj.name}: Canvas 设置为 {layerName} ({order})");
            return;
        }
        
        // 尝试获取 Renderer（通用）
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sortingLayerName = layerName;
            renderer.sortingOrder = order;
            Debug.Log($"[SortingLayerSetter] {obj.name}: Renderer 设置为 {layerName} ({order})");
            return;
        }
        
        Debug.LogWarning($"[SortingLayerSetter] {obj.name} 没有找到可设置的 Renderer 组件");
    }
    
    /// <summary>
    /// 在编辑器中预览当前设置
    /// </summary>
    void OnValidate()
    {
        // 验证 Sorting Layer 是否存在
        if (!string.IsNullOrEmpty(sortingLayerName))
        {
            if (SortingLayer.NameToID(sortingLayerName) == 0 && sortingLayerName != "Default")
            {
                Debug.LogWarning($"[SortingLayerSetter] Sorting Layer '{sortingLayerName}' 不存在！请在 Tags & Layers 中创建。");
            }
        }
    }
}
