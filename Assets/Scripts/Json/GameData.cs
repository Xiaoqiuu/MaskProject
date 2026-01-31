using System;
using UnityEngine;

/// <summary>
/// 游戏数据类 - 用于序列化和反序列化
/// </summary>
[Serializable]
public class GameData
{
    public int maxMoney = 0; // 历史最高赚取的货币
    public int maxSushiCount = 0; // 历史最高的寿司数量
    public int coins = 0; // 持有的金币（用于商店购买）
    
    // 商店升级等级
    public int bonusLevel = 0;
    public int rateLevel = 0;
    public int spLevel = 0;
    public int rewardLevel = 0;

    // 可以添加更多需要保存的数据
    public string lastPlayTime = ""; // 最后游玩时间
    public int totalGamesPlayed = 0; // 总游戏次数
    
    public GameData()
    {
        maxMoney = 0;
        maxSushiCount = 0;
        coins = 0;
        lastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        totalGamesPlayed = 0;
        
        bonusLevel = 0;
        rateLevel = 0;
        spLevel = 0;
        rewardLevel = 0;
    }
}
