using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    [Header("GamePlay UI")]
    [SerializeField] private Text CoinsCount;
    [SerializeField] private Text SpCount;
    [SerializeField] private Scrollbar SpBar;
    [SerializeField] private Text ComboCount;
    [SerializeField] private Text Combo;
    
    [SerializeField] private Text Tip;
    [SerializeField] private Button TapButton;

    [Header("Pause UI")]
    [SerializeField] private Button PauseButton;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private Button ResumeButton;

    [Header("Shop UI")]
    [SerializeField] private Button ShopButton;
    [SerializeField] private GameObject ShopPanel;
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
    [SerializeField] private Button ShopExit;

    private bool IsMobileWebGL() {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Input.touchSupported && !Input.mousePresent)
                return true;
#endif
        return Application.isMobilePlatform;
    }

    void Start() {
        if (GameManager.Instance == null) {
            Debug.LogError("HUD: GameManager.Instance 为 null！");
            return;
        }

        BonusPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Bonus).ToString();
        RatePrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Rate).ToString();
        SpPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Sp).ToString();
        SpecialBonusPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.SpecialBonus).ToString();
        Tip.text = IsMobileWebGL() ? "▲TAP!" : "▲SPACE!";

        // 订阅事件 - 使用方法引用
        GameManager.Instance.OnComboChanged += OnComboChanged;
        GameManager.Instance.OnSpecialPointChanged += OnSpecialPointChanged;
        GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
        GameManager.Instance.OnBonusLevelChanged += OnBonusLevelChanged;
        GameManager.Instance.OnRateLevelChanged += OnRateLevelChanged;
        GameManager.Instance.OnSpLevelChanged += OnSpLevelChanged;
        GameManager.Instance.OnSpecialBonusLevelChanged += OnSpecialBonusLevelChanged;

        // 按钮事件
        
        BuyBonusLevel.onClick.AddListener(() => GameManager.Instance.BuyUpgrade(GameManager.UpgradeType.Bonus));
        BuyRateLevel.onClick.AddListener(() => GameManager.Instance.BuyUpgrade(GameManager.UpgradeType.Rate));
        BuySpLevel.onClick.AddListener(() => GameManager.Instance.BuyUpgrade(GameManager.UpgradeType.Sp));
        BuySpecialBonusLevel.onClick.AddListener(() => GameManager.Instance.BuyUpgrade(GameManager.UpgradeType.SpecialBonus));

        ShopButton.onClick.AddListener(() => {
            Debug.Log("HUD: Store Button Clicked");
            GameManager.Instance.PauseGame();
            ShopPanel.SetActive(true);
            PausePanel.SetActive(false);
        });

        TapButton.onClick.AddListener(() => {
            GameManager.Instance.Tap();
        });

        ShopExit.onClick.AddListener(() => {
            GameManager.Instance.ResumeGame();
            ShopPanel.SetActive(false);
            PausePanel.SetActive(false);
        });

        PauseButton.onClick.AddListener(() => {
            GameManager.Instance.PauseGame();
            ShopPanel.SetActive(false);
            PausePanel.SetActive(true);
        });

        ResumeButton.onClick.AddListener(() => {
            GameManager.Instance.ResumeGame();
            PausePanel.SetActive(false);
            ShopPanel.SetActive(false);
        });
    }

    private void OnComboChanged(int combo) {
        if (ComboCount == null || Combo == null) return;

        if (combo != 0) {
            ComboCount.gameObject.SetActive(true);
            Combo.gameObject.SetActive(true);
            ComboCount.text = $"x{combo}";
        }
        else {
            ComboCount.gameObject.SetActive(false);
            Combo.gameObject.SetActive(false);
        }
    }

    private void OnSpecialPointChanged(float specialPoint) {
        if (SpBar == null || SpCount == null) return;

        SpBar.size = specialPoint / GameBalance.MaxSp;
        SpCount.text = $"{Math.Floor(specialPoint)}/{GameBalance.MaxSp}";
    }

    private void OnMoneyChanged(int money) {
        if (CoinsCount == null) return;

        CoinsCount.text = "￥" + money.ToString();
    }

    private void OnBonusLevelChanged(int bonusLevel) {
        if (BonusLevel == null || BonusPrice == null || GameManager.Instance == null) return;

        BonusLevel.text = bonusLevel.ToString();
        BonusPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Bonus).ToString();
    }

    private void OnRateLevelChanged(int rateLevel) {
        if (RateLevel == null || RatePrice == null || GameManager.Instance == null) return;

        RateLevel.text = rateLevel.ToString();
        RatePrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Rate).ToString();
    }

    private void OnSpLevelChanged(int spLevel) {
        if (SpLevel == null || SpPrice == null || GameManager.Instance == null) return;

        SpLevel.text = spLevel.ToString();
        SpPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.Sp).ToString();
    }

    private void OnSpecialBonusLevelChanged(int specialBonusLevel) {
        if (SpecialBonusLevel == null || SpecialBonusPrice == null || GameManager.Instance == null) return;

        SpecialBonusLevel.text = specialBonusLevel.ToString();
        SpecialBonusPrice.text = GameManager.Instance.GetUpgradePrice(GameManager.UpgradeType.SpecialBonus).ToString();
    }

    private void OnDestroy() {
        if (GameManager.Instance != null) {
            GameManager.Instance.OnComboChanged -= OnComboChanged;
            GameManager.Instance.OnSpecialPointChanged -= OnSpecialPointChanged;
            GameManager.Instance.OnMoneyChanged -= OnMoneyChanged;
            GameManager.Instance.OnBonusLevelChanged -= OnBonusLevelChanged;
            GameManager.Instance.OnRateLevelChanged -= OnRateLevelChanged;
            GameManager.Instance.OnSpLevelChanged -= OnSpLevelChanged;
            GameManager.Instance.OnSpecialBonusLevelChanged -= OnSpecialBonusLevelChanged;
        }
    }

    void Update() {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null) {
            if (Camera.main != null) {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}
