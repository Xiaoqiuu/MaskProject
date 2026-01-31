using UnityEngine;
using UnityEditor;

/// <summary>
/// Sorting Layer 编辑器工具
/// 提供快捷菜单来设置 Sorting Layer
/// </summary>
public class SortingLayerTools : Editor
{
    [MenuItem("GameObject/Sorting Layer/Set to Background", false, 0)]
    static void SetToBackground()
    {
        SetSortingLayer("Background", 0);
    }
    
    [MenuItem("GameObject/Sorting Layer/Set to Conveyor", false, 1)]
    static void SetToConveyor()
    {
        SetSortingLayer("Conveyor", 0);
    }
    
    [MenuItem("GameObject/Sorting Layer/Set to GameObjects", false, 2)]
    static void SetToGameObjects()
    {
        SetSortingLayer("GameObjects", 0);
    }
    
    [MenuItem("GameObject/Sorting Layer/Set to Character", false, 3)]
    static void SetToCharacter()
    {
        SetSortingLayer("Character", 0);
    }
    
    [MenuItem("GameObject/Sorting Layer/Set to Effects", false, 4)]
    static void SetToEffects()
    {
        SetSortingLayer("Effects", 0);
    }
    
    [MenuItem("GameObject/Sorting Layer/Set to UI", false, 5)]
    static void SetToUI()
    {
        SetSortingLayer("UI", 0);
    }
    
    [MenuItem("GameObject/Sorting Layer/Print Current Layer", false, 20)]
    static void PrintCurrentLayer()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("请先选中一个对象");
            return;
        }
        
        SpriteRenderer spriteRenderer = selected.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Debug.Log($"{selected.name}: Sorting Layer = {spriteRenderer.sortingLayerName}, Order = {spriteRenderer.sortingOrder}");
            return;
        }
        
        Canvas canvas = selected.GetComponent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"{selected.name}: Canvas Sorting Layer = {canvas.sortingLayerName}, Order = {canvas.sortingOrder}");
            return;
        }
        
        Debug.LogWarning($"{selected.name} 没有 SpriteRenderer 或 Canvas 组件");
    }
    
    static void SetSortingLayer(string layerName, int order)
    {
        GameObject[] selected = Selection.gameObjects;
        
        if (selected.Length == 0)
        {
            Debug.LogWarning("请先选中要设置的对象");
            return;
        }
        
        int count = 0;
        foreach (GameObject obj in selected)
        {
            // 尝试设置 SpriteRenderer
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Undo.RecordObject(spriteRenderer, "Set Sorting Layer");
                spriteRenderer.sortingLayerName = layerName;
                spriteRenderer.sortingOrder = order;
                count++;
                continue;
            }
            
            // 尝试设置 Canvas
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                Undo.RecordObject(canvas, "Set Sorting Layer");
                canvas.sortingLayerName = layerName;
                canvas.sortingOrder = order;
                count++;
                continue;
            }
            
            Debug.LogWarning($"{obj.name} 没有 SpriteRenderer 或 Canvas 组件");
        }
        
        Debug.Log($"已设置 {count} 个对象的 Sorting Layer 为 {layerName} ({order})");
    }
}
