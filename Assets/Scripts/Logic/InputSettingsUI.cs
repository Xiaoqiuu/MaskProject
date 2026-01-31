using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 输入设置UI管理器
/// 用于在游戏设置界面中切换输入模式和更改按键
/// </summary>
public class InputSettingsUI : MonoBehaviour
{
    [Header("UI引用")]
    public Dropdown inputModeDropdown; // 输入模式下拉菜单
    public Text currentKeyText; // 显示当前按键的文本
    public Button changeKeyButton; // 更改按键的按钮
    public Text statusText; // 状态提示文本
    
    [Header("按键更改设置")]
    private bool isWaitingForKey = false; // 是否正在等待玩家按键
    
    void Start()
    {
        InitializeUI();
    }
    
    void Update()
    {
        // 如果正在等待玩家按键
        if (isWaitingForKey)
        {
            DetectKeyPress();
        }
    }
    
    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        if (InputSystem.Instance == null)
        {
            Debug.LogError("[InputSettingsUI] InputSystem 实例不存在！");
            return;
        }
        
        // 设置下拉菜单
        if (inputModeDropdown != null)
        {
            inputModeDropdown.ClearOptions();
            inputModeDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "触摸/点击模式",
                "键盘模式",
                "手柄模式"
            });
            
            inputModeDropdown.value = (int)InputSystem.Instance.GetCurrentInputMode();
            inputModeDropdown.onValueChanged.AddListener(OnInputModeChanged);
        }
        
        // 设置按键显示
        UpdateKeyDisplay();
        
        // 设置按钮事件
        if (changeKeyButton != null)
        {
            changeKeyButton.onClick.AddListener(OnChangeKeyButtonClicked);
        }
    }
    
    /// <summary>
    /// 当输入模式改变时
    /// </summary>
    private void OnInputModeChanged(int index)
    {
        if (InputSystem.Instance != null)
        {
            InputMode newMode = (InputMode)index;
            InputSystem.Instance.SwitchInputMode(newMode);
            
            UpdateStatusText($"已切换到: {GetInputModeName(newMode)}");
            
            // 只有键盘模式才显示按键设置
            if (changeKeyButton != null)
            {
                changeKeyButton.gameObject.SetActive(newMode == InputMode.Keyboard);
            }
            if (currentKeyText != null)
            {
                currentKeyText.gameObject.SetActive(newMode == InputMode.Keyboard);
            }
        }
    }
    
    /// <summary>
    /// 当点击更改按键按钮时
    /// </summary>
    private void OnChangeKeyButtonClicked()
    {
        isWaitingForKey = true;
        UpdateStatusText("请按下你想要设置的按键...");
        
        if (changeKeyButton != null)
        {
            changeKeyButton.interactable = false;
        }
    }
    
    /// <summary>
    /// 检测按键输入
    /// </summary>
    private void DetectKeyPress()
    {
        // 遍历所有可能的按键
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                // 排除一些不适合的按键
                if (IsValidKey(keyCode))
                {
                    SetNewKey(keyCode);
                    return;
                }
            }
        }
    }
    
    /// <summary>
    /// 检查按键是否有效
    /// </summary>
    private bool IsValidKey(KeyCode keyCode)
    {
        // 排除鼠标按键和一些特殊按键
        if (keyCode >= KeyCode.Mouse0 && keyCode <= KeyCode.Mouse6)
            return false;
        
        if (keyCode == KeyCode.None || keyCode == KeyCode.Escape)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// 设置新按键
    /// </summary>
    private void SetNewKey(KeyCode newKey)
    {
        if (InputSystem.Instance != null)
        {
            InputSystem.Instance.SetKeyboardKey(newKey);
            UpdateKeyDisplay();
            UpdateStatusText($"按键已设置为: {newKey}");
        }
        
        isWaitingForKey = false;
        
        if (changeKeyButton != null)
        {
            changeKeyButton.interactable = true;
        }
    }
    
    /// <summary>
    /// 更新按键显示
    /// </summary>
    private void UpdateKeyDisplay()
    {
        if (currentKeyText != null && InputSystem.Instance != null)
        {
            KeyCode currentKey = InputSystem.Instance.GetKeyboardKey();
            currentKeyText.text = $"当前按键: {currentKey}";
        }
    }
    
    /// <summary>
    /// 更新状态文本
    /// </summary>
    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            
            // 2秒后清除状态文本
            CancelInvoke("ClearStatusText");
            Invoke("ClearStatusText", 2f);
        }
    }
    
    /// <summary>
    /// 清除状态文本
    /// </summary>
    private void ClearStatusText()
    {
        if (statusText != null)
        {
            statusText.text = "";
        }
    }
    
    /// <summary>
    /// 获取输入模式的中文名称
    /// </summary>
    private string GetInputModeName(InputMode mode)
    {
        switch (mode)
        {
            case InputMode.Touch:
                return "触摸/点击模式";
            case InputMode.Keyboard:
                return "键盘模式";
            case InputMode.Gamepad:
                return "手柄模式";
            default:
                return "未知模式";
        }
    }
    
    /// <summary>
    /// 重置输入设置
    /// </summary>
    public void ResetInputSettings()
    {
        if (InputSystem.Instance != null)
        {
            InputSystem.Instance.ResetInputSettings();
            
            if (inputModeDropdown != null)
            {
                inputModeDropdown.value = (int)InputSystem.Instance.GetCurrentInputMode();
            }
            
            UpdateKeyDisplay();
            UpdateStatusText("输入设置已重置");
        }
    }
}
