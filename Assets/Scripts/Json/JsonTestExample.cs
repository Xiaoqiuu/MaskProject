using UnityEngine;

/// <summary>
/// JSON系统测试示例 - 可以挂载到任意GameObject上进行测试
/// </summary>
public class JsonTestExample : MonoBehaviour
{
    void Update()
    {
        // 按 S 键保存数据
        if (Input.GetKeyDown(KeyCode.S))
        {
            TestSave();
        }
        
        // 按 L 键加载数据
        if (Input.GetKeyDown(KeyCode.L))
        {
            TestLoad();
        }
        
        // 按 R 键重置存档
        if (Input.GetKeyDown(KeyCode.R))
        {
            TestReset();
        }
        
        // 按 G 键模拟游戏结束
        if (Input.GetKeyDown(KeyCode.G))
        {
            TestGameOver();
        }
        
        // 按 M 键增加金币
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddMoney();
        }
    }
    
    void TestSave()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGameData();
            Debug.Log("手动保存数据");
        }
    }
    
    void TestLoad()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameData();
            Debug.Log($"加载数据 - 最高金币: {GameManager.Instance.maxMoney}, 最高寿司: {GameManager.Instance.maxSushiCount}");
        }
    }
    
    void TestReset()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetSaveData();
            Debug.Log("存档已重置");
        }
    }
    
    void TestGameOver()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
            Debug.Log("游戏结束，数据已保存");
        }
    }
    
    void AddMoney()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Money += 100;
            GameManager.Instance.sushiCount += 10;
            Debug.Log($"当前金币: {GameManager.Instance.Money}, 当前寿司: {GameManager.Instance.sushiCount}");
        }
    }
}
