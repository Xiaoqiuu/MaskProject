using System.IO;
using UnityEngine;

/// <summary>
/// JSON数据管理器 - 负责数据的读取和写入
/// </summary>
public class JsonDataManager
{
    private static string saveFilePath = Application.persistentDataPath + "/gamedata.json";
    
    /// <summary>
    /// 保存游戏数据到JSON文件
    /// </summary>
    public static void SaveData(GameData data)
    {
        try
        {
            // 将数据转换为JSON字符串
            string jsonString = JsonUtility.ToJson(data, true);
            
            // 写入文件
            File.WriteAllText(saveFilePath, jsonString);
            
            Debug.Log($"数据保存成功: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存数据失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 从JSON文件加载游戏数据
    /// </summary>
    public static GameData LoadData()
    {
        try
        {
            // 检查文件是否存在
            if (File.Exists(saveFilePath))
            {
                // 读取文件内容
                string jsonString = File.ReadAllText(saveFilePath);
                
                // 将JSON字符串转换为对象
                GameData data = JsonUtility.FromJson<GameData>(jsonString);
                
                Debug.Log($"数据加载成功: {saveFilePath}");
                return data;
            }
            else
            {
                Debug.Log("存档文件不存在，创建新数据");
                return new GameData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载数据失败: {e.Message}");
            return new GameData();
        }
    }
    
    /// <summary>
    /// 删除存档文件
    /// </summary>
    public static void DeleteSaveFile()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("存档文件已删除");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"删除存档失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 检查存档文件是否存在
    /// </summary>
    public static bool SaveFileExists()
    {
        return File.Exists(saveFilePath);
    }
    
    /// <summary>
    /// 获取存档文件路径
    /// </summary>
    public static string GetSaveFilePath()
    {
        return saveFilePath;
    }
}
