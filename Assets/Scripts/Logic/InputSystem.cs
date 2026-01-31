using System;
using UnityEngine;

/// <summary>
/// 输入模式枚举
/// </summary>
public enum InputMode
{
    Touch,      // 触摸/点击模式
    Keyboard,   // 键盘模式
    Gamepad     // 手柄模式
}

/// <summary>
/// 输入系统 - 统一管理所有输入并广播事件
/// </summary>
public class InputSystem : MonoBehaviour
{
    // 单例模式
    public static InputSystem Instance { get; private set; }
    
    [Header("输入模式设置")]
    public InputMode currentInputMode = InputMode.Touch;
    
    [Header("键盘设置")]
    public KeyCode keyboardKey = KeyCode.Space; // 默认空格键
    
    [Header("手柄设置")]
    [Range(0.1f, 1f)]
    public float joystickDeadZone = 0.3f; // 摇杆死区
    
    [Header("调试信息")]
    public bool showDebugLog = true;
    
    // 输入事件 - 其他脚本可以订阅这个事件
    public static event Action OnPlayerInput;
    
    // 上一帧的输入状态（用于检测按下瞬间）
    private bool lastFrameInput = false;
    
    void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        LoadInputSettings();
    }
    
    void Update()
    {
        bool currentInput = false;
        
        // 根据当前输入模式检测输入
        switch (currentInputMode)
        {
            case InputMode.Touch:
                currentInput = DetectTouchInput();
                break;
                
            case InputMode.Keyboard:
                currentInput = DetectKeyboardInput();
                break;
                
            case InputMode.Gamepad:
                currentInput = DetectGamepadInput();
                break;
        }
        
        // 检测到输入的瞬间（从无输入到有输入）
        if (currentInput && !lastFrameInput)
        {
            TriggerPlayerInput();
        }
        
        lastFrameInput = currentInput;
    }
    
    /// <summary>
    /// 检测触摸/点击输入
    /// </summary>
    private bool DetectTouchInput()
    {
        // 鼠标左键点击（PC端测试）
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        
        // 触摸输入（移动端）
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 检测键盘输入
    /// </summary>
    private bool DetectKeyboardInput()
    {
        return Input.GetKeyDown(keyboardKey);
    }
    
    /// <summary>
    /// 检测手柄输入
    /// </summary>
    private bool DetectGamepadInput()
    {
        // 检测左摇杆
        float leftStickX = Input.GetAxis("Horizontal");
        float leftStickY = Input.GetAxis("Vertical");
        float leftStickMagnitude = Mathf.Sqrt(leftStickX * leftStickX + leftStickY * leftStickY);
        
        if (leftStickMagnitude > joystickDeadZone)
        {
            return true;
        }
        
        // 检测右摇杆（如果有配置）
        try
        {
            float rightStickX = Input.GetAxis("RightStickHorizontal");
            float rightStickY = Input.GetAxis("RightStickVertical");
            float rightStickMagnitude = Mathf.Sqrt(rightStickX * rightStickX + rightStickY * rightStickY);
            
            if (rightStickMagnitude > joystickDeadZone)
            {
                return true;
            }
        }
        catch
        {
            // 如果没有配置右摇杆轴，忽略错误
        }
        
        // 检测手柄按钮（A键/Cross键等）
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1"))
        {
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 触发玩家输入事件
    /// </summary>
    private void TriggerPlayerInput()
    {
        if (showDebugLog)
        {
            Debug.Log($"[InputSystem] 玩家输入 - 模式: {currentInputMode}");
        }
        
        // 广播事件给所有订阅者
        OnPlayerInput?.Invoke();
    }
    
    /// <summary>
    /// 切换输入模式
    /// </summary>
    public void SwitchInputMode(InputMode newMode)
    {
        currentInputMode = newMode;
        SaveInputSettings();
        
        if (showDebugLog)
        {
            Debug.Log($"[InputSystem] 切换输入模式: {newMode}");
        }
    }
    
    /// <summary>
    /// 设置键盘按键
    /// </summary>
    public void SetKeyboardKey(KeyCode newKey)
    {
        keyboardKey = newKey;
        SaveInputSettings();
        
        if (showDebugLog)
        {
            Debug.Log($"[InputSystem] 设置键盘按键: {newKey}");
        }
    }
    
    /// <summary>
    /// 获取当前输入模式
    /// </summary>
    public InputMode GetCurrentInputMode()
    {
        return currentInputMode;
    }
    
    /// <summary>
    /// 获取当前键盘按键
    /// </summary>
    public KeyCode GetKeyboardKey()
    {
        return keyboardKey;
    }
    
    /// <summary>
    /// 保存输入设置
    /// </summary>
    private void SaveInputSettings()
    {
        PlayerPrefs.SetInt("InputMode", (int)currentInputMode);
        PlayerPrefs.SetInt("KeyboardKey", (int)keyboardKey);
        PlayerPrefs.SetFloat("JoystickDeadZone", joystickDeadZone);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 加载输入设置
    /// </summary>
    private void LoadInputSettings()
    {
        if (PlayerPrefs.HasKey("InputMode"))
        {
            currentInputMode = (InputMode)PlayerPrefs.GetInt("InputMode");
        }
        
        if (PlayerPrefs.HasKey("KeyboardKey"))
        {
            keyboardKey = (KeyCode)PlayerPrefs.GetInt("KeyboardKey");
        }
        
        if (PlayerPrefs.HasKey("JoystickDeadZone"))
        {
            joystickDeadZone = PlayerPrefs.GetFloat("JoystickDeadZone");
        }
        
        if (showDebugLog)
        {
            Debug.Log($"[InputSystem] 加载设置 - 模式: {currentInputMode}, 按键: {keyboardKey}");
        }
    }
    
    /// <summary>
    /// 重置输入设置为默认值
    /// </summary>
    public void ResetInputSettings()
    {
        currentInputMode = InputMode.Touch;
        keyboardKey = KeyCode.Space;
        joystickDeadZone = 0.3f;
        SaveInputSettings();
        
        if (showDebugLog)
        {
            Debug.Log("[InputSystem] 输入设置已重置");
        }
    }
}
