using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour {
    [Header("UI Components")]
    public Text nameText;
    public Text descText;
    public Text levelText;
    public Text priceText;
    public Button buyButton;
    public Image iconImage;
    public Image bgImage; // 背景图，用于区分不同商品或美化

    private GameManager.UpgradeType type;
    private ShopUI shopUI;

    public void Init(ShopUI ui, GameManager.UpgradeType upgradeType, string name, string desc, Sprite icon) {
        this.shopUI = ui;
        this.type = upgradeType;
        
        if (nameText) nameText.text = name;
        if (descText) descText.text = desc;
        if (iconImage && icon) iconImage.sprite = icon;
        
        // 移除旧监听，防止重复
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClicked);
        
        Refresh();
    }

    public void Refresh() {
        int currentLevel = GameManager.Instance.GetLevel(type);
        int price = GameManager.Instance.GetUpgradePrice(type);
        
        if (levelText) levelText.text = $"Lv.{currentLevel}";
        if (priceText) priceText.text = $"¥{price}";
        
        // 检查钱够不够
        if (GameManager.Instance.gameData != null) {
            bool canAfford = GameManager.Instance.gameData.coins >= price;
            buyButton.interactable = canAfford;
            
            // 可选：钱不够时文字变红
            if (priceText) {
                priceText.color = canAfford ? Color.white : Color.red;
            }
        }
    }

    void OnBuyClicked() {
        if (GameManager.Instance.BuyUpgrade(type)) {
            // 播放简单的缩放动画反馈
            StopAllCoroutines();
            StartCoroutine(AnimateButton());
            
            shopUI.RefreshAll();
        }
    }
    
    // 简单的点击反馈动画
    System.Collections.IEnumerator AnimateButton() {
        transform.localScale = Vector3.one * 0.95f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = Vector3.one;
    }
}
