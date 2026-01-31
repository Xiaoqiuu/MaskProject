using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 单例模式
    public static GameManager Instance { get; private set; }
    
    // 游戏时
    public int Money = 0; // 赚取的货币
    public int sushiCount = 0; // 寿司的数量
    
    // 历史统计
    public int maxMoney = 0; // 历史最高赚取的货币
    public int maxSushiCount = 0; // 历史最高的寿司数量
    
    public SushiSpawner sushiSpawner;
    
    // 游戏数据
    private GameData gameData;
    
    // 获取当前时间的方法
    public float GetCurrentTime()
    {
        if (sushiSpawner != null)
        {
            return sushiSpawner.curTime;
        }
        return 0f;
    }
    
    // 获取目标时间的方法
    public float GetTargetTime()
    {
        if (sushiSpawner != null)
        {
            return sushiSpawner.targetTime;
        }
        return 0f;
    }

    void gameStart() {
        /// <summary>
        /// 游戏开始
        /// </summary>
        Money = 0;
        sushiCount = 0;
        if (sushiSpawner != null) {
            sushiSpawner.isOn = true;
        }
        // 此处再次唤醒游戏逻辑
    }
    

    void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换时不销毁
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 游戏启动时加载数据
        LoadGameData();
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
    
    /// <summary>
    /// 加载游戏数据
    /// </summary>
    public void LoadGameData()
    {
        gameData = JsonDataManager.LoadData();
        maxMoney = gameData.maxMoney;
        maxSushiCount = gameData.maxSushiCount;
        
        Debug.Log($"加载数据 - 最高金币: {maxMoney}, 最高寿司数: {maxSushiCount}");
    }
    
    /// <summary>
    /// 保存游戏数据
    /// </summary>
    public void SaveGameData()
    {
        // 更新历史最高记录
        if (Money > maxMoney)
        {
            maxMoney = Money;
        }
        
        if (sushiCount > maxSushiCount)
        {
            maxSushiCount = sushiCount;
        }
        
        // 更新游戏数据对象
        gameData.maxMoney = maxMoney;
        gameData.maxSushiCount = maxSushiCount;
        gameData.lastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        gameData.totalGamesPlayed++;
        
        // 保存到JSON文件
        JsonDataManager.SaveData(gameData);
        
        Debug.Log($"保存数据 - 最高金币: {maxMoney}, 最高寿司数: {maxSushiCount}");
    }
    
    /// <summary>
    /// 游戏结束时调用
    /// </summary>
    public void GameOver()
    {
        // 停止生成寿司
        if (sushiSpawner != null)
        {
            sushiSpawner.isOn = false;
        }
        
        // 保存游戏数据
        SaveGameData();
        
        Debug.Log($"游戏结束 - 本局金币: {Money}, 本局寿司数: {sushiCount}");
    }
    
    /// <summary>
    /// 重置存档（用于测试或清除数据）
    /// </summary>
    public void ResetSaveData()
    {
        JsonDataManager.DeleteSaveFile();
        gameData = new GameData();
        maxMoney = 0;
        maxSushiCount = 0;
        
        Debug.Log("存档已重置");
    }
}
