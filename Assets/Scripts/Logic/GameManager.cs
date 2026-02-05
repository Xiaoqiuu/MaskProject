using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    // 单例模式
    public static GameManager Instance { get; private set; }

    // 游戏时
    public int Money = 1000; // 赚取的货币
    public float moneyScale = 1.0f;
    public int sushiCount = 0; // 寿司的数量
    public int combo = 0;
    public bool isSpecialMode = false; // 是否开心
    public float specialPoint = 80f;

    public float specialPointCD = 0.3f;
    public float curSPCD = 0;

    // 历史统计
    public int maxMoney = 0; // 历史最高赚取的货币
    public int maxSushiCount = 0; // 历史最高的寿司数量

    public SushiSpawner sushiSpawner;

    // 游戏数据
    public GameData gameData;

    private bool isPaused = false;

    [Header("调试选项")]
    [Tooltip("勾选此项，下次运行游戏时会清除存档并重置金币和等级")]
    public bool resetSaveOnStart = true;

    public int bonusLevel = 0;
    public int rateLevel = 0;
    public int spLevel = 0;
    public int specialBonusLevel = 0;

    public List<float> baseBonus = new List<float> { 350f, 250f, 250f, 150f };
    public List<float> coefficient = new List<float> { 23f, 27f, 27f, 31f };
    public List<float> baseRate = new List<float> { 10f, 15f, 30f, 45f };
    public List<float> maxRate = new List<float> { 45f, 30f, 15f, 10f };
    public float baseSp = 1f;
    public float baseSpecialBonus = 1f;

    public enum UpgradeType {
        Bonus,
        Rate,
        Sp,
        SpecialBonus
    }
    private List<float> rates;
    private float total = 0;

    public event Action<int> OnComboChanged;
    public event Action<int> OnMoneyChanged;
    public event Action<float> OnSpecialPointChanged;
    public event Action<bool> OnSpecialModeChanged;
    public event Action<bool> OnPauseStateChanged; // 暂停状态改变事件

    public event Action<int> OnBonusLevelChanged;
    public event Action<int> OnRateLevelChanged;
    public event Action<int> OnSpLevelChanged;
    public event Action<int> OnSpecialBonusLevelChanged;
    public event Action OnTap;

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

    public void gameStart() {
        /// <summary>
        /// 游戏开始
        /// </summary>
        Money = 1000;
        OnMoneyChanged?.Invoke(Money);

        sushiCount = 0;

        specialPoint = 80f;
        OnSpecialPointChanged?.Invoke(specialPoint);
        if (sushiSpawner != null) {
            sushiSpawner.isOn = true;
        }
        // 此处再次唤醒游戏逻辑
    }


    void Awake() {
        // 单例模式实现
        if (Instance == null) {
            Instance = this;
            // 不使用 DontDestroyOnLoad，让场景切换时自动销毁
            // DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        //****Debug, 游戏启动时加载数据
        if (resetSaveOnStart) {
            ResetSaveData();
        }
        else {
            LoadGameData();
        }

        // 强制检查：如果是第一次运行或者想强制重置，可以取消下面这行的注释
        // ResetSaveData(); 
    }

    void Start() {
        OnSpecialPointChanged += (float specialPoint) => {
            if (specialPoint >= GameBalance.MaxSp) {
                isSpecialMode = true;
                OnSpecialModeChanged?.Invoke(isSpecialMode);
            }
        };
        InputSystem.OnPlayerInput += () => {
            OnTap?.Invoke();
        };
    }

    void Update() {
        if (curSPCD >= specialPointCD) {
            curSPCD = 0;
            Debug.Log($"specialPoint = {specialPoint} specialMode = {isSpecialMode}");
            if (isSpecialMode) {
                specialPoint = Math.Max(0, specialPoint - 1);
                OnSpecialPointChanged?.Invoke(specialPoint);

                if (specialPoint == 0) {
                    isSpecialMode = false;
                    OnSpecialModeChanged?.Invoke(isSpecialMode);
                }
            }
        }
        curSPCD += Time.deltaTime;
    }

    /// <summary>
    /// 加载游戏数据
    /// </summary>
    public void LoadGameData() {
        gameData = JsonDataManager.LoadData();
        Money = gameData.coins;
        OnMoneyChanged?.Invoke(Money);
        maxMoney = gameData.maxMoney;
        maxSushiCount = gameData.maxSushiCount;

        bonusLevel = gameData.bonusLevel;
        OnBonusLevelChanged?.Invoke(bonusLevel);

        rateLevel = gameData.rateLevel;
        OnRateLevelChanged?.Invoke(rateLevel);

        spLevel = gameData.spLevel;
        OnSpLevelChanged?.Invoke(spLevel);

        specialBonusLevel = gameData.specialBonusLevel;
        OnSpecialBonusLevelChanged?.Invoke(specialBonusLevel);

        Debug.Log($"加载数据 - 最高金币: {maxMoney}, 持有金币: {Money}, 升级等级: [{bonusLevel},{rateLevel},{spLevel},{specialBonusLevel}]");

        // [测试专用] 如果金币不足，自动补充，方便测试扣费
        if (Money < 1000) {
            Money = GameBalance.InitialCoins; // 默认为 100000
            Debug.Log($"[测试助手] 检测到金币不足，已自动为您补充到 {Money} 以便测试商店功能！");
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
        gameData.coins = Money;
        gameData.maxMoney = maxMoney;
        gameData.maxSushiCount = maxSushiCount;
        gameData.lastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        gameData.totalGamesPlayed++;

        // 保存升级数据
        gameData.bonusLevel = bonusLevel;
        gameData.rateLevel = rateLevel;
        gameData.spLevel = spLevel;
        gameData.specialBonusLevel = specialBonusLevel;

        // 保存到JSON文件
        JsonDataManager.SaveData(gameData);

        Debug.Log($"保存数据 - 最高金币: {maxMoney}, 最高寿司数: {maxSushiCount} bonusLevel = {bonusLevel} rateLevel = {rateLevel} spLevel = {spLevel} specialBonusLevel = {specialBonusLevel}");
    }

    /// <summary>
    /// 游戏结束时调用
    /// </summary>
    public void GameOver() {
        ResumeGame();
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

        // 触发场景过渡动画
        GameSceneTransition transition = FindObjectOfType<GameSceneTransition>();
        if (transition != null) {
            transition.CloseDoorsAndExit();
        }
        else {
            Debug.LogWarning("[GameManager] 找不到 GameSceneTransition 组件");
        }
    }

    /// <summary>
    /// 重置存档（用于测试或清除数据）
    /// </summary>
    [ContextMenu("强制重置存档(ResetSave)")]
    public void ResetSaveData() {
        JsonDataManager.DeleteSaveFile();
        gameData = new GameData();

        // 测试模式：给予初始金币
        Money = GameBalance.InitialCoins;
        gameData.coins = Money;

        maxMoney = 0;
        maxSushiCount = 0;
        bonusLevel = 0;
        rateLevel = 0;
        spLevel = 0;
        specialBonusLevel = 0;

        Debug.Log($"存档已重置，已发放初始测试金币: {GameBalance.InitialCoins}，所有等级重置为 0 (UI显示为Lv1)");

        // 立即保存一次，确保初始金币被写入
        SaveGameData();
    }

    /// <summary>
    /// 获取升级价格
    /// 默认价格增长公式：BasePrice * (1.15 ^ Level)
    /// </summary>
    public int GetUpgradePrice(UpgradeType type) {
        switch (type) {
            case UpgradeType.Bonus:
                return (int)Math.Round(GameBalance.BaseBonusPrice * Math.Pow(1.05, bonusLevel));
            case UpgradeType.Rate:
                return (int)Math.Round(GameBalance.BaseRatePrice * Math.Pow(1.05, rateLevel));
            case UpgradeType.Sp:
                return (int)Math.Round(GameBalance.BaseSpPrice * Math.Pow(1.08, spLevel));
            case UpgradeType.SpecialBonus:
                return (int)Math.Round(GameBalance.BaseSpecialBonusPrice * Math.Pow(1.06, specialBonusLevel));
        }
        return 7000;
    }

    /// <summary>
    /// 尝试购买升级
    /// </summary>
    public bool BuyUpgrade(UpgradeType type) {
        int price = GetUpgradePrice(type);

        if (Money >= price) {
            Money -= price;
            OnMoneyChanged?.Invoke(Money);

            switch (type) {
                case UpgradeType.Bonus:
                    bonusLevel++;
                    OnBonusLevelChanged?.Invoke(bonusLevel);
                    break;
                case UpgradeType.Rate:
                    rateLevel++;
                    OnRateLevelChanged?.Invoke(rateLevel);
                    InitRate();
                    break;
                case UpgradeType.Sp:
                    spLevel++;
                    OnSpLevelChanged?.Invoke(spLevel);
                    break;
                case UpgradeType.SpecialBonus:
                    specialBonusLevel++;
                    OnSpecialBonusLevelChanged?.Invoke(specialBonusLevel);
                    break;
            }

            SaveGameData();
            Debug.Log($"购买成功: {type}, 新等级: {GetLevel(type)}, 剩余金币: {Money}");
            return true;
        }

        Debug.Log($"购买失败: 金币不足。需要 {price}, 拥有 {Money}", this);
        return false;
    }

    public int GetLevel(UpgradeType type) {
        switch (type) {
            case UpgradeType.Bonus: return bonusLevel;
            case UpgradeType.Rate: return rateLevel;
            case UpgradeType.Sp: return spLevel;
            case UpgradeType.SpecialBonus: return specialBonusLevel;
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

    public float GetSpecialBonus() {
        return (float)(GameBalance.SpecialBonus * Math.Pow(1.03, specialBonusLevel));
    }

    public void InitRate() {
        rates = new List<float>();
        total = 0;
        for (int i = 0; i < 4; ++i) {
            float rate = GetRate(i);
            total += rate;
            rates.Add(total);
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

    public void Miss() {
        combo = 0;
        OnComboChanged?.Invoke(combo);

        if (!isSpecialMode) {
            Money = Math.Max(0, Money - 500);
            OnMoneyChanged?.Invoke(Money);

            specialPoint = Math.Min(GameBalance.MaxSp, Math.Max(0, specialPoint - 10));
            Debug.Log($"SpecialPoint = {specialPoint}");
            OnSpecialPointChanged?.Invoke(specialPoint);
        }


        if (Money == 0) {
            GameOver();
        }
    }

    public void Success(float bonus) {
        ++sushiCount;

        ++combo;
        OnComboChanged?.Invoke(combo);
        if (combo % 10 == 0) {
            moneyScale = Math.Min(1.5f, moneyScale + 0.02f);
        }

        Money += (int)Math.Round((isSpecialMode ? GetSpecialBonus() : bonus) * moneyScale);
        OnMoneyChanged?.Invoke(Money);

        if (!isSpecialMode) {
            specialPoint = Math.Min(GameBalance.MaxSp, Math.Max(0, specialPoint + GetSp()));
            Debug.Log($"SpecialPoint = {specialPoint}");
            OnSpecialPointChanged?.Invoke(specialPoint);
        }

    }

    /// <summary>
    /// 切换暂停状态（绑定到暂停按钮）
    /// </summary>
    public void TogglePause() {
        if (isPaused) {
            ResumeGame();
        }
        else {
            PauseGame();
        }
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame() {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("游戏暂停");
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame() {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("游戏恢复");
    }

    /// <summary>
    /// 获取当前是否暂停
    /// </summary>
    public bool IsPaused() {
        return isPaused;
    }

    public void Tap() {
        OnTap?.Invoke();
    }
}
