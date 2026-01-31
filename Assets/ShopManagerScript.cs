using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopManagerScript : MonoBehaviour
{
    [Header("UI引用")]
    public Text CoinsTXT; // 显示金币的文本组件

    // 为了兼容用户提供的参考代码结构，我们保留这个数组定义，
    // 但实际数据逻辑会通过 GameManager 代理，确保数据持久化。
    // 索引定义: [Type, ItemID] -> 1=ID, 2=Price, 3=Quantity
    // public int[,] shopItems = new int[5, 5]; 

    void Start()
    {
        UpdateCoinDisplay();
    }

    void Update()
    {
        // 实时刷新金币显示
        UpdateCoinDisplay();
    }

    private void UpdateCoinDisplay()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameData != null && CoinsTXT != null)
        {
            CoinsTXT.text = "Coins: " + GameManager.Instance.gameData.coins.ToString();
        }
    }

    /// <summary>
    /// 自动购买方法 (无参数)
    /// 会尝试自动获取当前点击按钮上的 StoreButtoninfo 组件中的 ID
    /// </summary>
    public void Buy()
    {
        // 获取当前被点击的按钮对象
        GameObject btnObj = EventSystem.current.currentSelectedGameObject;
        
        if (btnObj != null)
        {
            // 尝试获取按钮上的 StoreButtoninfo 组件
            var info = btnObj.GetComponent<StoreButtoninfo>();
            if (info != null)
            {
                // 获取成功，调用带参数的购买方法
                Buy(info.ItemID);
            }
            else
            {
                Debug.LogError($"[ShopManagerScript] 自动购买失败: 当前点击的对象 {btnObj.name} 上没有找到 StoreButtoninfo 组件。");
            }
        }
        else
        {
            Debug.LogError("[ShopManagerScript] 自动购买失败: 无法获取当前点击的按钮 (EventSystem.current.currentSelectedGameObject 为空)。");
        }
    }

    /// <summary>
    /// 购买物品方法
    /// 供 StoreButtoninfo.cs 调用
    /// </summary>
    /// <param name="itemID">商品ID (1-4)</param>
    public void Buy(int itemID)
    {
        GameManager.UpgradeType type;

        // 映射 ID 到升级类型
        switch (itemID)
        {
            case 1: type = GameManager.UpgradeType.Bonus; break;
            case 2: type = GameManager.UpgradeType.Rate; break;
            case 3: type = GameManager.UpgradeType.Sp; break;
            case 4: type = GameManager.UpgradeType.Reward; break;
            default:
                Debug.LogError($"[ShopManagerScript] 购买失败: 接收到的商品ID为 {itemID}。\n" +
                               "原因可能是:\n" +
                               "1. 按钮的 OnClick 事件绑定了 ShopManagerScript.Buy(int) 但没有在下方输入框填写数字 (1-4)。\n" +
                               "2. 或者 StoreButtoninfo 的 ItemID 未设置。\n" +
                               "建议: 将按钮绑定改为 StoreButtoninfo.Buy() (无参数版本)，它会自动使用 Inspector 中设置的 ID。");
                return;
        }

        // 检查 GameManager 是否存在
        if (GameManager.Instance == null)
        {
            Debug.LogError("[ShopManagerScript] 严重错误: 场景中找不到 GameManager 实例！\n" +
                           "请确保：\n" +
                           "1. 场景中有一个挂载了 'GameManager' 脚本的游戏对象。\n" +
                           "2. 如果是从商店场景直接运行，请先运行主菜单/游戏场景，或者在商店场景也临时放一个 GameManager。");
            return;
        }

        // 调用 GameManager 的购买接口
        // BuyUpgrade 会自动处理金币扣除、等级提升和存档保存
        if (GameManager.Instance.BuyUpgrade(type))
        {
            Debug.Log($"购买成功: ID {itemID}");
            
            // 购买成功后，更新金币显示
            UpdateCoinDisplay();
            
            // 注意：StoreButtoninfo 的 Update 方法会自动检测新的等级和价格并刷新 UI
        }
        else
        {
            Debug.Log($"购买失败: ID {itemID} (可能是金币不足)");
        }
    }
}
