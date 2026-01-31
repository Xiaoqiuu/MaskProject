using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    // 单例模式
    public static GameManager Instance { get; private set; }

    // 游戏时
    public int Money = 0; // 赚取的货币
    public int sushiCount = 0; // 寿司的数量
    public bool isHappy = false; // 是否开心

    // 历史统计
    public int maxMoney = 0; // 历史最高赚取的货币
    public int maxSushiCount = 0; // 历史最高的寿司数量

    public SushiSpawner sushiSpawner;

    // 游戏数据
    private GameData gameData;

    public float bonusLevel = 0;
    public float rateLevel = 0;
    public float spLevel = 0;
    public float rewardLevel = 0;

    public List<float> baseBonus = new List<float> { 350f, 250f, 250f, 150f };
    public List<float> coefficient = new List<float> { 23f, 27f, 27f, 31f };
    public List<float> baseRate = new List<float> { 10f, 15f, 30f, 45f };
    public List<float> maxRate = new List<float> { 45f, 30f, 15f, 10f };
    public float baseSp = 1f;
    public float baseReward = 1f;

    public float baseBonusPrice = 1800;
    public float baseRatePrice = 2000;
    public float baseSpPrice = 2200;
    public float baseRewardPrice = 2000;

    private List<float> rates;
    private float total = 0;

    // 获取当前时间的方法
    public float GetCurrentTime() {
        if (sushiSpawner != null) {
            return sushiSpawner.curTime;
        }
        return 0f;
    }

    // 获取目标时间的方法
    public float GetTargetTime() {
        if (sushiSpawner != null) {
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


    void Awake() {
        // 单例模式实现
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换时不销毁
        }
        else {
            Destroy(gameObject);
            return;
        }

        // 游戏启动时加载数据
        LoadGameData();
    }

    void Start() {

    }

    void Update() {

    }

    /// <summary>
    /// 加载游戏数据
    /// </summary>
    public void LoadGameData() {
        gameData = JsonDataManager.LoadData();
        maxMoney = gameData.maxMoney;
        maxSushiCount = gameData.maxSushiCount;

        Debug.Log($"加载数据 - 最高金币: {maxMoney}, 最高寿司数: {maxSushiCount}");
    }

    /// <summary>
    /// 保存游戏数据
    /// </summary>
    public void SaveGameData() {
        // 更新历史最高记录
        if (Money > maxMoney) {
            maxMoney = Money;
        }

        if (sushiCount > maxSushiCount) {
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
    public void GameOver() {
        // 停止生成寿司
        if (sushiSpawner != null) {
            sushiSpawner.isOn = false;
        }

        // 保存游戏数据
        SaveGameData();

        rates = null;
        total = 0;
        Debug.Log($"游戏结束 - 本局金币: {Money}, 本局寿司数: {sushiCount}");
    }

    /// <summary>
    /// 重置存档（用于测试或清除数据）
    /// </summary>
    public void ResetSaveData() {
        JsonDataManager.DeleteSaveFile();
        gameData = new GameData();
        maxMoney = 0;
        maxSushiCount = 0;

        Debug.Log("存档已重置");
    }

    public float GetBonus(int type) {
        return (float)(baseBonus[type] + coefficient[type] * Math.Pow(bonusLevel, 0.7));
    }

    public float GetRate(int type) {
        return (float)(baseRate[type] + (maxRate[type] - baseRate[type]) * (1 - 1 / (1 + 0.25 * rateLevel)));
    }

    public float GetSp() {
        return (float)(baseSp + 0.8 * Math.Pow(spLevel, 0.5));
    }

    public float GetReward() {
        return (float)(baseReward * Math.Pow(1.03, rewardLevel));
    }

    public void InitRate() {
        rates = new List<float>();
        total = 0;
        for (int i = 0; i < 4; ++i) {
            float rate = GetRate(i);
            rates.Add(rate);
            total += rate;
        }
    }

    public int GetSuShiType() {
        if (rates == null || rates.Count == 0) InitRate();
        float r = UnityEngine.Random.Range(0, 1f) * total;
        for (int i = 0; i < rates.Count; ++i) {
            if (r < rates[i]) {
                return i;
            }
        }
        return 0;
    }
}
