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
    public GameData gameData;

    [Header("调试选项")]
    [Tooltip("勾选此项，下次运行游戏时会清除存档并重置金币和等级")]
    public bool resetSaveOnStart = true;

    public int bonusLevel = 0;
    public int rateLevel = 0;
    public int spLevel = 0;
    public int rewardLevel = 0;

    public enum UpgradeType {
        Bonus,
        Rate,
        Sp,
        Reward
    }
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

        //****Debug, 游戏启动时加载数据
        if (resetSaveOnStart)
        {
            ResetSaveData();
        }
        else
        {
            LoadGameData();
        }
        
        // 强制检查：如果是第一次运行或者想强制重置，可以取消下面这行的注释
        // ResetSaveData(); 
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
        
        bonusLevel = gameData.bonusLevel;
        rateLevel = gameData.rateLevel;
        spLevel = gameData.spLevel;
        rewardLevel = gameData.rewardLevel;

        Debug.Log($"加载数据 - 最高金币: {maxMoney}, 持有金币: {gameData.coins}, 升级等级: [{bonusLevel},{rateLevel},{spLevel},{rewardLevel}]");

        // [测试专用] 如果金币不足，自动补充，方便测试扣费
        if (gameData.coins < 1000)
        {
            gameData.coins = GameBalance.InitialCoins; // 默认为 100000
            Debug.Log($"[测试助手] 检测到金币不足，已自动为您补充到 {gameData.coins} 以便测试商店功能！");
            SaveGameData(); // 保存修改
        }
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
        
        // 保存升级数据
        gameData.bonusLevel = bonusLevel;
        gameData.rateLevel = rateLevel;
        gameData.spLevel = spLevel;
        gameData.rewardLevel = rewardLevel;

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

        // 结算金币到钱包
        if (gameData != null) {
            gameData.coins += Money;
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
    [ContextMenu("强制重置存档(ResetSave)")]
    public void ResetSaveData() {
        JsonDataManager.DeleteSaveFile();
        gameData = new GameData();
        
        // 测试模式：给予初始金币
        gameData.coins = GameBalance.InitialCoins;
        
        maxMoney = 0;
        maxSushiCount = 0;
        bonusLevel = 0;
        rateLevel = 0;
        spLevel = 0;
        rewardLevel = 0;

        Debug.Log($"存档已重置，已发放初始测试金币: {GameBalance.InitialCoins}，所有等级重置为 0 (UI显示为Lv1)");
        
        // 立即保存一次，确保初始金币被写入
        SaveGameData();
    }

    /// <summary>
    /// 获取升级价格
    /// 默认价格增长公式：BasePrice * (1.15 ^ Level)
    /// </summary>
    public int GetUpgradePrice(UpgradeType type) {
        float basePrice = 0;
        int currentLevel = 0;

        switch (type) {
            case UpgradeType.Bonus:
                basePrice = GameBalance.BaseBonusPrice;
                currentLevel = bonusLevel;
                break;
            case UpgradeType.Rate:
                basePrice = GameBalance.BaseRatePrice;
                currentLevel = rateLevel;
                break;
            case UpgradeType.Sp:
                basePrice = GameBalance.BaseSpPrice;
                currentLevel = spLevel;
                break;
            case UpgradeType.Reward:
                basePrice = GameBalance.BaseRewardPrice;
                currentLevel = rewardLevel;
                break;
        }

        return (int)basePrice;
    }

    /// <summary>
    /// 尝试购买升级
    /// </summary>
    public bool BuyUpgrade(UpgradeType type) {
        int price = GetUpgradePrice(type);
        
        if (gameData.coins >= price) {
            gameData.coins -= price;
            
            switch (type) {
                case UpgradeType.Bonus: bonusLevel++; break;
                case UpgradeType.Rate: rateLevel++; break;
                case UpgradeType.Sp: spLevel++; break;
                case UpgradeType.Reward: rewardLevel++; break;
            }
            
            // 同步回 gameData 并保存
            gameData.bonusLevel = bonusLevel;
            gameData.rateLevel = rateLevel;
            gameData.spLevel = spLevel;
            gameData.rewardLevel = rewardLevel;
            
            SaveGameData();
            Debug.Log($"购买成功: {type}, 新等级: {GetLevel(type)}, 剩余金币: {gameData.coins}");
            return true;
        }
        
        Debug.Log($"购买失败: 金币不足。需要 {price}, 拥有 {gameData.coins}", this);
        return false;
    }
    
    public int GetLevel(UpgradeType type) {
        switch (type) {
            case UpgradeType.Bonus: return bonusLevel;
            case UpgradeType.Rate: return rateLevel;
            case UpgradeType.Sp: return spLevel;
            case UpgradeType.Reward: return rewardLevel;
            default: return 0;
        }
    }

    public float GetBonus(int type) {
        return (float)(GameBalance.BaseBonus[type] + GameBalance.Coefficient[type] * Math.Pow(bonusLevel, 0.7));
    }

    public float GetRate(int type) {
        return (float)(GameBalance.BaseRate[type] + (GameBalance.MaxRate[type] - GameBalance.BaseRate[type]) * (1 - 1 / (1 + 0.25 * rateLevel)));
    }

    public float GetSp() {
        return (float)(GameBalance.BaseSp + 0.8 * Math.Pow(spLevel, 0.5));
    }

    public float GetReward() {
        return (float)(GameBalance.BaseReward * Math.Pow(1.03, rewardLevel));
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
