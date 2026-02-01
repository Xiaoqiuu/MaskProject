using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Text CoinsCount;
    [SerializeField] private Text SpCount;
    [SerializeField] private Scrollbar SpBar;
    [SerializeField] private Text ComboCount;
    [SerializeField] private Text Combo;
    [SerializeField] private Button PauseButton;
    [SerializeField] private Button StoreButton;

    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject StorePanel;

    [Header("Shop UI")]
    [SerializeField] private Button BuyBonusLevel;
    [SerializeField] private Button BuyRateLevel;
    [SerializeField] private Button BuySpLevel;
    [SerializeField] private Button BuySpecialBonusLevel;
    [SerializeField] private Text BonusPrice;
    [SerializeField] private Text RatePrice;
    [SerializeField] private Text SpPrice;
    [SerializeField] private Text SpecialBonusPrice;
    [SerializeField] private Text BonusLevel;
    [SerializeField] private Text RateLevel;
    [SerializeField] private Text SpLevel;
    [SerializeField] private Text SpecialBonusLevel;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("HUD: GameManager.Instance 为 null！");
            return;
        }

        BonusPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Bonus).ToString();
        RatePrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Rate).ToString();
        SpPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Sp).ToString();
        SpecialBonusPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.SpecialBonus).ToString();

        if (Panel != null)
        {
            GameManager.Instance.pausePanel = Panel;

            Image panelImage = Panel.GetComponent<Image>();
            if (panelImage == null)
            {
                panelImage = Panel.AddComponent<Image>();
            }
            panelImage.color = new Color(1f, 1f, 1f, 0.85f);

            Panel.SetActive(false);
        }
        else
        {
            Debug.LogError("HUD: 请在 Inspector 中将 PausePanel 拖给 HUD 的 Panel 槽位！");
        }

        if (StorePanel != null)
        {
            GameManager.Instance.storePanel = StorePanel;

            ShopUI shopUI = StorePanel.GetComponent<ShopUI>();
            if (shopUI != null)
            {
                shopUI.Init();
            }
        }

        // 订阅事件 - 使用方法引用
        GameManager.Instance.OnComboChanged += OnComboChanged;
        GameManager.Instance.OnSpecialPointChanged += OnSpecialPointChanged;
        GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
        GameManager.Instance.OnBonusLevelChanged += OnBonusLevelChanged;
        GameManager.Instance.OnRateLevelChanged += OnRateLevelChanged;
        GameManager.Instance.OnSpLevelChanged += OnSpLevelChanged;
        GameManager.Instance.OnSpecialBonusLevelChanged += OnSpecialBonusLevelChanged;
        GameManager.Instance.OnPauseStateChanged += OnPauseStateChanged;

        // 按钮事件
        PauseButton.onClick.AddListener(() => GameManager.Instance.PauseGame());
        BuyBonusLevel.onClick.AddListener(() => GameManager.Instance.BuyUpgrade(GameManager.UpgradeType.Bonus));
        BuyRateLevel.onClick.AddListener(() => GameManager.Instance.BuyUpgrade(GameManager.UpgradeType.Rate));
        BuySpLevel.onClick.AddListener(() => GameManager.Instance.BuyUpgrade(GameManager.UpgradeType.Sp));
        BuySpecialBonusLevel.onClick.AddListener(() => GameManager.Instance.BuyUpgrade(GameManager.UpgradeType.SpecialBonus));

        if (StoreButton != null)
        {
            StoreButton.onClick.AddListener(() =>
            {
                Debug.Log("HUD: Store Button Clicked");
                GameManager.Instance.OpenShop();
            });
        }
    }

    private void OnComboChanged(int combo)
    {
        if (ComboCount == null || Combo == null) return;

        if (combo != 0)
        {
            ComboCount.gameObject.SetActive(true);
            Combo.gameObject.SetActive(true);
            ComboCount.text = $"x{combo}";
        }
        else
        {
            ComboCount.gameObject.SetActive(false);
            Combo.gameObject.SetActive(false);
        }
    }

    private void OnSpecialPointChanged(float specialPoint)
    {
        if (SpBar == null || SpCount == null) return;

        SpBar.size = specialPoint / GameBalance.MaxSp;
        SpCount.text = $"{Math.Floor(specialPoint)}/{GameBalance.MaxSp}";
    }

    private void OnMoneyChanged(int money)
    {
        if (CoinsCount == null) return;

        CoinsCount.text = "￥" + money.ToString();
    }

    private void OnBonusLevelChanged(int bonusLevel)
    {
        if (BonusLevel == null || BonusPrice == null || GameManager.Instance == null) return;

        BonusLevel.text = bonusLevel.ToString();
        BonusPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Bonus).ToString();
    }

    private void OnRateLevelChanged(int rateLevel)
    {
        if (RateLevel == null || RatePrice == null || GameManager.Instance == null) return;

        RateLevel.text = rateLevel.ToString();
        RatePrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Rate).ToString();
    }

    private void OnSpLevelChanged(int spLevel)
    {
        if (SpLevel == null || SpPrice == null || GameManager.Instance == null) return;

        SpLevel.text = spLevel.ToString();
        SpPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Sp).ToString();
    }

    private void OnSpecialBonusLevelChanged(int specialBonusLevel)
    {
        if (SpecialBonusLevel == null || SpecialBonusPrice == null || GameManager.Instance == null) return;

        SpecialBonusLevel.text = specialBonusLevel.ToString();
        SpecialBonusPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.SpecialBonus).ToString();
    }

    private void OnPauseStateChanged(bool isPaused)
    {
        if (PauseButton != null)
        {
            PauseButton.gameObject.SetActive(!isPaused);
        }
        if (StoreButton != null)
        {
            StoreButton.gameObject.SetActive(!isPaused);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPauseStateChanged -= OnPauseStateChanged;
            GameManager.Instance.OnComboChanged -= OnComboChanged;
            GameManager.Instance.OnSpecialPointChanged -= OnSpecialPointChanged;
            GameManager.Instance.OnMoneyChanged -= OnMoneyChanged;
            GameManager.Instance.OnBonusLevelChanged -= OnBonusLevelChanged;
            GameManager.Instance.OnRateLevelChanged -= OnRateLevelChanged;
            GameManager.Instance.OnSpLevelChanged -= OnSpLevelChanged;
            GameManager.Instance.OnSpecialBonusLevelChanged -= OnSpecialBonusLevelChanged;
        }
    }

    void Update()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
        {
            if (Camera.main != null)
            {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}
