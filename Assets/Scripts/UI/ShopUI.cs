using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour {
    [Header("General")]
    public GameObject panelRoot; // 整个商店界面的根节点
    public Text coinText;
    public Button closeButton;
    public Button openButton; // 用于打开商店的按钮（如果在主界面）

    [Header("Item Template")]
    public ShopItemUI itemTemplate; // 商品项预制体
    public Transform contentRoot;   // Grid Layout Group 所在的父节点

    [System.Serializable]
    public struct ShopItemConfig {
        public GameManager.UpgradeType type;
        public string name;
        public string desc;
        public Sprite icon;
    }

    [Header("Configuration")]
    public List<ShopItemConfig> shopConfigs;

    private List<ShopItemUI> spawnedItems = new List<ShopItemUI>();

    void Start() {
        if (closeButton) closeButton.onClick.AddListener(CloseShop);
        if (openButton) openButton.onClick.AddListener(OpenShop);
        
        // 如果没有配置，自动添加默认配置
        if (shopConfigs == null || shopConfigs.Count == 0) {
            InitDefaultConfigs();
        }

        // 生成所有商品
        GenerateItems();
        
        // 初始刷新
        RefreshAll();
        
        // 默认关闭
        if (panelRoot) panelRoot.SetActive(false);
    }
    
    void InitDefaultConfigs() {
        shopConfigs = new List<ShopItemConfig>();
        
        shopConfigs.Add(new ShopItemConfig {
            type = GameManager.UpgradeType.Bonus,
            name = "寿司升值",
            desc = "提高寿司的基础售价"
        });
        
        shopConfigs.Add(new ShopItemConfig {
            type = GameManager.UpgradeType.Rate,
            name = "高级进货",
            desc = "增加高级寿司出现的概率"
        });
        
        shopConfigs.Add(new ShopItemConfig {
            type = GameManager.UpgradeType.Sp,
            name = "SP充能",
            desc = "加快SP技能槽的积攒速度"
        });
        
        shopConfigs.Add(new ShopItemConfig {
            type = GameManager.UpgradeType.Reward,
            name = "连击奖励",
            desc = "提升连击带来的额外倍率"
        });
    }

    void GenerateItems() {
        if (itemTemplate == null || contentRoot == null) return;

        // 清理旧的（除了模板本身）
        foreach(Transform child in contentRoot) {
            if (child.gameObject != itemTemplate.gameObject) Destroy(child.gameObject);
        }
        itemTemplate.gameObject.SetActive(false);

        foreach (var config in shopConfigs) {
            ShopItemUI item = Instantiate(itemTemplate, contentRoot);
            item.gameObject.SetActive(true);
            // 确保缩放正确
            item.transform.localScale = Vector3.one;
            
            item.Init(this, config.type, config.name, config.desc, config.icon);
            spawnedItems.Add(item);
        }
    }

    public void RefreshAll() {
        if (GameManager.Instance && GameManager.Instance.gameData != null) {
            if (coinText) coinText.text = $"¥ {GameManager.Instance.gameData.coins:N0}";
        }
        
        foreach (var item in spawnedItems) {
            item.Refresh();
        }
    }
    
    public void OpenShop() {
        if (panelRoot) {
            panelRoot.SetActive(true);
            // 简单的弹出动画
            panelRoot.transform.localScale = Vector3.one * 0.8f;
            StartCoroutine(PopUpAnimation());
        }
        RefreshAll();
    }
    
    System.Collections.IEnumerator PopUpAnimation() {
        float t = 0;
        while (t < 1) {
            t += Time.deltaTime * 5;
            panelRoot.transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);
            yield return null;
        }
        panelRoot.transform.localScale = Vector3.one;
    }
    
    public void CloseShop() {
        if (panelRoot) panelRoot.SetActive(false);
    }
}
