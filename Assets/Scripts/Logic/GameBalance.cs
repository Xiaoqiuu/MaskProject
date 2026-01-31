using System.Collections.Generic;

/// <summary>
/// 游戏数值配置类
/// 用于存放所有的基础数值、价格配置和成长系数
/// 可以直接修改此类中的数值来调整游戏平衡
/// </summary>
public static class GameBalance
{
    // ================= 基础数值 =================
    
    // 寿司基础分数 (对应4种寿司)
    public static List<float> BaseBonus = new List<float> { 350f, 250f, 250f, 150f };
    
    // 分数成长系数
    public static List<float> Coefficient = new List<float> { 23f, 27f, 27f, 31f };
    
    // 寿司出现基础概率 (对应4种寿司)
    public static List<float> BaseRate = new List<float> { 10f, 15f, 30f, 45f };
    
    // 寿司出现最大概率
    public static List<float> MaxRate = new List<float> { 45f, 30f, 15f, 10f };

    public static float MaxSp = 200f;
    
    // SP积攒速度基础值
    public static float BaseSp = 1f;
    
    // 连击奖励基础倍率
    public static float BaseReward = 1f;

    // ================= 商店价格配置 =================
    
    // 基础价格----> 默认画布价格
    public static float BaseBonusPrice = 1800;  // 寿司升值
    public static float BaseRatePrice = 2000;   // 高级进货
    public static float BaseSpPrice = 2200;     // SP充能
    public static float BaseRewardPrice = 2000; // 连击奖励
    
    // 价格增长指数 (价格 = 基础价格 * (PriceGrowthFactor ^ 等级))
    public static double PriceGrowthFactor = 1.15;

    // ================= 测试/调试配置  debug=================
    public static int InitialCoins = 100000; 
}
