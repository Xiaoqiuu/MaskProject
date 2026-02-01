using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 特殊模式视觉控制器
/// 监听 GameManager 的特殊模式状态，切换背景和角色
/// </summary>
public class SpecialModeVisualController : MonoBehaviour
{
    [Header("背景设置")]
    [Tooltip("普通背景 Image")]
    public Image normalBackground;
    
    [Tooltip("特殊背景 Image")]
    public Image specialBackground;
    
    [Header("角色设置")]
    [Tooltip("普通角色 GameObject")]
    public GameObject normalCharacter;
    
    [Tooltip("特殊角色 GameObject")]
    public GameObject specialCharacter;
    
    [Header("渐变设置")]
    [Tooltip("背景渐变时长（秒）")]
    public float transitionDuration = 1.0f;
    
    [Header("背景音乐设置")]
    [Tooltip("普通模式开头音乐（播放一次）")]
    public AudioClip normalIntroMusic;
    
    [Tooltip("普通模式循环音乐")]
    public AudioClip normalLoopMusic;
    
    [Tooltip("特殊模式音乐")]
    public AudioClip specialMusic;
    
    [Tooltip("音乐音量")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    
    [Header("调试")]
    [Tooltip("显示调试日志")]
    public bool showDebugLog = false;
    
    // 当前是否在特殊模式
    private bool isInSpecialMode = false;
    
    // 渐变协程
    private Coroutine transitionCoroutine;
    
    // 音频源
    private AudioSource audioSource;
    
    // 是否已播放过开头音乐
    private bool hasPlayedIntro = false;
    
    void Start()
    {
        // 设置音频源
        SetupAudioSource();
        
        // 初始化状态
        InitializeVisuals();
        
        // 播放普通模式音乐
        PlayNormalMusic();
        
        if (showDebugLog)
        {
            Debug.Log("[SpecialModeVisualController] 初始化完成");
        }
    }
    
    void OnEnable()
    {
        // 订阅特殊模式变化事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSpecialModeChanged += OnSpecialModeChanged;
            
            if (showDebugLog)
            {
                Debug.Log("[SpecialModeVisualController] 已订阅特殊模式事件");
            }
        }
        else
        {
            Debug.LogWarning("[SpecialModeVisualController] GameManager.Instance 为 null");
        }
    }
    
    void OnDisable()
    {
        // 取消订阅
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSpecialModeChanged -= OnSpecialModeChanged;
            
            if (showDebugLog)
            {
                Debug.Log("[SpecialModeVisualController] 已取消订阅特殊模式事件");
            }
        }
        
        // 停止协程
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
    }
    
    /// <summary>
    /// 设置音频源
    /// </summary>
    private void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = false;
        
        if (showDebugLog)
        {
            Debug.Log("[SpecialModeVisualController] 音频源设置完成");
        }
    }
    
    /// <summary>
    /// 播放普通模式音乐
    /// </summary>
    private void PlayNormalMusic()
    {
        if (audioSource == null) return;
        
        // 如果还没播放过开头音乐，先播放开头
        if (!hasPlayedIntro && normalIntroMusic != null)
        {
            audioSource.clip = normalIntroMusic;
            audioSource.loop = false;
            audioSource.Play();
            
            // 开头音乐播放完后，播放循环音乐
            StartCoroutine(WaitForIntroToFinish());
            
            if (showDebugLog)
            {
                Debug.Log("[SpecialModeVisualController] 播放普通模式开头音乐");
            }
        }
        else if (normalLoopMusic != null)
        {
            // 直接播放循环音乐
            audioSource.clip = normalLoopMusic;
            audioSource.loop = true;
            audioSource.Play();
            
            if (showDebugLog)
            {
                Debug.Log("[SpecialModeVisualController] 播放普通模式循环音乐");
            }
        }
    }
    
    /// <summary>
    /// 等待开头音乐播放完毕
    /// </summary>
    private IEnumerator WaitForIntroToFinish()
    {
        hasPlayedIntro = true;
        
        // 等待开头音乐播放完
        yield return new WaitWhile(() => audioSource.isPlaying);
        
        // 播放循环音乐
        if (normalLoopMusic != null && !isInSpecialMode)
        {
            audioSource.clip = normalLoopMusic;
            audioSource.loop = true;
            audioSource.Play();
            
            if (showDebugLog)
            {
                Debug.Log("[SpecialModeVisualController] 开头音乐结束，开始循环音乐");
            }
        }
    }
    
    /// <summary>
    /// 播放特殊模式音乐
    /// </summary>
    private void PlaySpecialMusic()
    {
        if (audioSource == null || specialMusic == null) return;
        
        audioSource.Stop();
        audioSource.clip = specialMusic;
        audioSource.loop = true;
        audioSource.Play();
        
        if (showDebugLog)
        {
            Debug.Log("[SpecialModeVisualController] 播放特殊模式音乐");
        }
    }
    
    /// <summary>
    /// 初始化视觉元素
    /// </summary>
    private void InitializeVisuals()
    {
        // 检查背景
        if (normalBackground == null || specialBackground == null)
        {
            Debug.LogError("[SpecialModeVisualController] 背景 Image 未设置！");
            return;
        }
        
        // 检查角色
        if (normalCharacter == null || specialCharacter == null)
        {
            Debug.LogError("[SpecialModeVisualController] 角色 GameObject 未设置！");
            return;
        }
        
        // 设置初始状态（普通模式）
        normalBackground.gameObject.SetActive(true);
        specialBackground.gameObject.SetActive(true);
        
        // 特殊背景初始完全透明
        Color specialColor = specialBackground.color;
        specialColor.a = 0f;
        specialBackground.color = specialColor;
        
        // 角色初始状态
        normalCharacter.SetActive(true);
        specialCharacter.SetActive(false);
        
        isInSpecialMode = false;
        
        if (showDebugLog)
        {
            Debug.Log("[SpecialModeVisualController] 初始化为普通模式");
        }
    }
    
    /// <summary>
    /// 特殊模式状态改变
    /// </summary>
    private void OnSpecialModeChanged(bool isSpecial)
    {
        if (showDebugLog)
        {
            Debug.Log($"[SpecialModeVisualController] 特殊模式变化: {isSpecial}");
        }
        
        // 如果状态没有变化，忽略
        if (isInSpecialMode == isSpecial)
        {
            return;
        }
        
        isInSpecialMode = isSpecial;
        
        // 停止当前渐变
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        
        // 开始新的渐变
        if (isSpecial)
        {
            // 进入特殊模式
            transitionCoroutine = StartCoroutine(TransitionToSpecialMode());
        }
        else
        {
            // 退出特殊模式
            transitionCoroutine = StartCoroutine(TransitionToNormalMode());
        }
    }
    
    /// <summary>
    /// 渐变到特殊模式
    /// </summary>
    private IEnumerator TransitionToSpecialMode()
    {
        if (showDebugLog)
        {
            Debug.Log("[SpecialModeVisualController] 开始渐变到特殊模式");
        }
        
        // 播放特殊模式音乐
        PlaySpecialMusic();
        
        // 立即切换角色
        normalCharacter.SetActive(false);
        specialCharacter.SetActive(true);
        
        // 渐变背景：特殊背景从左到右显现（通过改变 fillAmount）
        // 使用 Image 的 Fill 功能
        if (specialBackground.type != Image.Type.Filled)
        {
            specialBackground.type = Image.Type.Filled;
            specialBackground.fillMethod = Image.FillMethod.Horizontal;
            specialBackground.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        
        float elapsedTime = 0f;
        Color specialColor = specialBackground.color;
        specialColor.a = 1f;
        specialBackground.color = specialColor;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            
            // 从左到右填充
            specialBackground.fillAmount = Mathf.Lerp(0f, 1f, t);
            
            yield return null;
        }
        
        // 确保完全显示
        specialBackground.fillAmount = 1f;
        
        if (showDebugLog)
        {
            Debug.Log("[SpecialModeVisualController] 特殊模式渐变完成");
        }
        
        transitionCoroutine = null;
    }
    
    /// <summary>
    /// 渐变回普通模式
    /// </summary>
    private IEnumerator TransitionToNormalMode()
    {
        if (showDebugLog)
        {
            Debug.Log("[SpecialModeVisualController] 开始渐变回普通模式");
        }
        
        // 恢复播放普通模式循环音乐
        if (audioSource != null && normalLoopMusic != null)
        {
            audioSource.Stop();
            audioSource.clip = normalLoopMusic;
            audioSource.loop = true;
            audioSource.Play();
            
            if (showDebugLog)
            {
                Debug.Log("[SpecialModeVisualController] 恢复普通模式循环音乐");
            }
        }
        
        // 立即切换角色
        specialCharacter.SetActive(false);
        normalCharacter.SetActive(true);
        
        // 等待一帧，确保所有组件的OnEnable被调用
        yield return null;
        
        // 重新初始化角色动画状态
        if (normalCharacter != null)
        {
            // 获取CharacterAnimation组件并重置
            CharacterAnimation charAnim = normalCharacter.GetComponent<CharacterAnimation>();
            if (charAnim != null)
            {
                charAnim.StopAnimation();
                
                if (showDebugLog)
                {
                    Debug.Log("[SpecialModeVisualController] 已重置角色动画状态");
                }
            }
            
            // 确保CharacterOutlineHover组件所在的GameObject是激活的
            CharacterOutlineHover[] hoverComponents = normalCharacter.GetComponentsInChildren<CharacterOutlineHover>(true);
            foreach (var hover in hoverComponents)
            {
                if (!hover.gameObject.activeSelf)
                {
                    hover.gameObject.SetActive(true);
                    
                    if (showDebugLog)
                    {
                        Debug.Log($"[SpecialModeVisualController] 激活 CharacterOutlineHover: {hover.gameObject.name}");
                    }
                }
            }
        }
        
        // 渐变背景：特殊背景从左到右消失
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            
            // 从右到左清空（反向）
            specialBackground.fillAmount = Mathf.Lerp(1f, 0f, t);
            
            yield return null;
        }
        
        // 确保完全隐藏
        specialBackground.fillAmount = 0f;
        
        if (showDebugLog)
        {
            Debug.Log("[SpecialModeVisualController] 普通模式渐变完成");
        }
        
        transitionCoroutine = null;
    }
    
    /// <summary>
    /// 手动切换到特殊模式（用于测试）
    /// </summary>
    public void TestSwitchToSpecial()
    {
        OnSpecialModeChanged(true);
    }
    
    /// <summary>
    /// 手动切换到普通模式（用于测试）
    /// </summary>
    public void TestSwitchToNormal()
    {
        OnSpecialModeChanged(false);
    }
}
