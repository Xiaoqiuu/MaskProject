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
    
    // 可以添加更多需要保存的数据
    public string lastPlayTime = ""; // 最后游玩时间
    public int totalGamesPlayed = 0; // 总游戏次数
    
    public GameData()
    {
        maxMoney = 0;
        maxSushiCount = 0;
        lastPlayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        totalGamesPlayed = 0;
    }
}
