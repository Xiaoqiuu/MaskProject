# Timeline 动画卡住问题排查指南

## 🐛 问题描述

在 Timeline 窗口打开时动画正常播放，但关闭 Timeline 窗口后动画有时会卡住不播放。

## 🔍 常见原因

### 1. Timeline 和脚本控制权冲突

**原因：**
- Timeline 窗口打开时，Unity 会强制刷新 Timeline 状态
- 关闭后，Timeline 可能仍然"持有"对象的控制权
- 脚本的 `SetActive()` 调用被 Timeline 覆盖

**解决方法：**
已在 `CharacterAnimation.cs` 中添加了 Timeline 兼容处理：
- 自动检测 PlayableDirector 状态
- 在激活前停止 Timeline 播放
- 使用 `LateUpdate()` 监控控制权变化

### 2. Time.timeScale 问题

**原因：**
- Timeline 编辑器可能会修改 `Time.timeScale`
- 如果 `Time.timeScale = 0`，协程会暂停

**解决方法：**
已在协程中添加 `Time.timeScale` 检查，只在 `timeScale > 0` 时计数帧数。

### 3. GameObject 被其他系统控制

**原因：**
- Animator 组件可能在控制对象
- 其他脚本也在修改 `SetActive` 状态

**解决方法：**
检查目标对象上的所有组件，确保没有冲突。

## 🛠️ 修复步骤

### 步骤 1：配置 CharacterAnimation

1. 选中挂载了 `CharacterAnimation.cs` 的 GameObject
2. 在 Inspector 中找到 **Timeline 兼容** 部分
3. 如果目标对象有 PlayableDirector，拖入到 **Playable Director** 字段
4. 勾选 **Show Debug Log** 以查看详细日志

### 步骤 2：检查 Timeline 设置

1. 选中目标 GameObject
2. 如果有 PlayableDirector 组件，检查：
   - **Update Method**: 建议设置为 **Game Time**
   - **Play On Awake**: 建议取消勾选（让脚本控制）
   - **Wrap Mode**: 建议设置为 **None** 或 **Hold**

### 步骤 3：使用调试工具

1. 创建一个空的 GameObject，命名为 "AnimationDebugger"
2. 添加 `AnimationDebugger.cs` 脚本
3. 配置：
   - **Target Object**: 拖入你的动画对象
   - **Playable Director**: 拖入 PlayableDirector（如果有）
   - 勾选 **Log Every Frame** 查看实时状态
4. 运行游戏，按 **F12** 查看详细调试信息

### 步骤 4：测试

1. 运行游戏
2. 打开 Timeline 窗口，测试动画
3. 关闭 Timeline 窗口，再次测试
4. 查看 Console 日志，寻找异常信息

## 📋 检查清单

### 基础检查
- [ ] 目标对象是否正确设置？
- [ ] Active Frames 是否大于 0？
- [ ] InputSystem 是否正常工作？
- [ ] Show Debug Log 是否勾选？

### Timeline 相关
- [ ] PlayableDirector 是否已设置？
- [ ] Timeline 的 Update Method 是否为 Game Time？
- [ ] Play On Awake 是否已取消勾选？
- [ ] 是否有多个 Timeline 在控制同一个对象？

### 性能相关
- [ ] Time.timeScale 是否为 1？
- [ ] 游戏是否暂停？
- [ ] 帧率是否正常（> 30 fps）？

## 🔧 高级解决方案

### 方案 1：完全禁用 Timeline 控制

如果不需要 Timeline，可以删除或禁用 PlayableDirector 组件：

```csharp
// 在 Start() 中
if (playableDirector != null)
{
    playableDirector.enabled = false;
}
```

### 方案 2：使用 Animator 代替 Timeline

如果动画简单，建议使用 Animator Controller：

1. 创建 Animator Controller
2. 添加动画状态
3. 使用 Trigger 参数控制
4. 删除 PlayableDirector 组件

### 方案 3：强制重置状态

在 `CharacterAnimation.cs` 中添加强制重置方法：

```csharp
void OnApplicationFocus(bool hasFocus)
{
    if (hasFocus && !isActive && targetObject != null)
    {
        // 确保对象状态正确
        targetObject.SetActive(false);
        
        if (playableDirector != null)
        {
            playableDirector.Stop();
        }
    }
}
```

### 方案 4：使用 WaitForSecondsRealtime

如果 Time.timeScale 经常变化，使用不受影响的等待方式：

```csharp
// 修改协程
yield return new WaitForSecondsRealtime(activeFrames / 60f);
```

## 📊 调试日志示例

### 正常情况：
```
[CharacterAnimation] 收到输入事件，当前状态: isActive=False
[CharacterAnimation] ✅ 激活目标对象，将在 30 帧后关闭
[CharacterAnimation] 已等待 10/30 帧
[CharacterAnimation] 已等待 20/30 帧
[CharacterAnimation] ✅ 关闭目标对象，可以接收新输入
```

### 异常情况（Timeline 冲突）：
```
[CharacterAnimation] 收到输入事件，当前状态: isActive=False
[CharacterAnimation] 停止 Timeline 播放
[CharacterAnimation] ✅ 激活目标对象，将在 30 帧后关闭
[CharacterAnimation] Timeline 开始控制对象  ← 异常！
[CharacterAnimation] 已等待 10/30 帧
```

### 异常情况（Time.timeScale = 0）：
```
[CharacterAnimation] ✅ 激活目标对象，将在 30 帧后关闭
[CharacterAnimation] 已等待 0/30 帧  ← 卡住！
[CharacterAnimation] 已等待 0/30 帧  ← 卡住！
```

## 💡 最佳实践

1. **分离控制权**：不要让 Timeline 和脚本同时控制同一个对象
2. **使用事件**：Timeline 播放完成后发送事件，通知脚本恢复控制
3. **简化动画**：如果只是简单的显示/隐藏，不需要使用 Timeline
4. **测试环境**：在 Build 版本中测试，编辑器行为可能不同
5. **日志记录**：保持 Debug Log 开启，直到问题完全解决

## 🎯 推荐配置

### 简单动画（推荐）
```
GameObject
├── CharacterAnimation.cs
│   ├── Target Object: AnimationObject
│   ├── Active Frames: 30
│   └── Show Debug Log: ✓
└── AnimationObject
    └── Animator (不使用 Timeline)
```

### 复杂动画（使用 Timeline）
```
GameObject
├── CharacterAnimation.cs
│   ├── Target Object: AnimationObject
│   ├── Active Frames: 30
│   ├── Playable Director: AnimationObject/PlayableDirector
│   ├── Play Timeline On Activate: ✓
│   └── Show Debug Log: ✓
└── AnimationObject
    ├── PlayableDirector
    │   ├── Update Method: Game Time
    │   ├── Play On Awake: ✗
    │   └── Wrap Mode: None
    └── Timeline Asset
```

## 🆘 仍然无法解决？

1. 导出完整的日志文件
2. 检查 Unity 版本（某些版本有 Timeline bug）
3. 尝试在新场景中重现问题
4. 检查是否有第三方插件冲突
5. 考虑使用 Animator 代替 Timeline
