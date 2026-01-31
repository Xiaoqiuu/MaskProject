using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreButtoninfo : MonoBehaviour
{
    public int ItemID;
    public Text PriceTxt;
    public Text QuantityTxt;
    public GameObject ShopManager;

    void Update()
    {
        // 实时更新价格和等级显示
        if (GameManager.Instance != null)
        {
            GameManager.UpgradeType type = GetUpgradeType(ItemID);
            
            // 显示价格
            if (PriceTxt != null)
            {
                PriceTxt.text = "Price: $" + GameManager.Instance.GetUpgradePrice(type).ToString();
            }
            
            // 显示当前等级或数量 (从 Lv1 开始显示)
            if (QuantityTxt != null)
            {
                QuantityTxt.text = "Lv: " + (GameManager.Instance.GetLevel(type) + 1).ToString();
            }
        }
    }

    /// <summary>
    /// 将 ItemID 映射到 GameManager.UpgradeType
    /// </summary>
    private GameManager.UpgradeType GetUpgradeType(int id)
    {
        switch (id)
        {
            case 1: return GameManager.UpgradeType.Bonus;
            case 2: return GameManager.UpgradeType.Rate;
            case 3: return GameManager.UpgradeType.Sp;
            case 4: return GameManager.UpgradeType.Reward;
            default: return GameManager.UpgradeType.Bonus;
        }
    }

    /// <summary>
    /// 购买按钮点击事件
    /// </summary>
    public void Buy()
    {
        if (ItemID == 0)
        {
            Debug.LogError($"[StoreButtoninfo] 错误: 按钮 {gameObject.name} 的 ItemID 未设置！请在 Inspector 中将其设置为 1, 2, 3 或 4。");
            return;
        }

        if (ShopManager != null)
        {
            var manager = ShopManager.GetComponent<ShopManagerScript>();
            if (manager != null)
            {
                // 调用 ShopManagerScript 的购买逻辑
                manager.Buy(ItemID);
            }
        }
        else
        {
            // 如果没绑定 ShopManager，也可以尝试直接调用 GameManager
            // 但为了保持与 ShopManagerScript 的一致性，建议绑定
            GameManager.UpgradeType type = GetUpgradeType(ItemID);
            GameManager.Instance.BuyUpgrade(type);
        }
    }
}
