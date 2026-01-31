using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 游戏时
    public int Money = 0; // 赚取的货币
    public int sushiCount = 0; // 寿司的数量
    
    // 历史统计
    public int maxMoney = 0; // 历史最高赚取的货币
    public int maxSushiCount = 0; // 历史最高的寿司数量
    
    public SushiSpawner sushiSpawner;
    
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
    


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
