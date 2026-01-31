# 角色动画系统使用说明（GameObject 激活版）

## 📖 概述

`CharacterAnimation.cs` 控制目标 GameObject 的激活状态。当接收到玩家输入时，激活目标对象，等待 30 帧后自动关闭。激活期间忽略所有新的输入。

## 🎬 工作原理

1. 监听 `InputSystem.OnPlayerInput` 事件
2. 接收到输入 → 激活目标 GameObject (`SetActive(true)`)
3. 等待 30 帧
4. 自动关闭目标 GameObject (`SetActive(false)`)
5. 激活期间忽略所有新的输入

## 🛠️ 设置步骤

### 1. 创建控制器 GameObject

1. 在场景中创建一个空的 GameObject
2. 命名为 "CharacterAnimationController"
3. 将 `CharacterAnimation.cs` 脚本挂载到这个 GameObject 上

### 2. 准备目标 GameObject

1. 在场景中找到或创建需要控制的 GameObject（例如角色的特效、动画对象等）
2. 确保这个对象初始状态可以是关闭的

### 3. 配置脚本参数

在 Inspector 中配置 `CharacterAnimation` 组件：

- **Target Object**: 拖入需要控制的 GameObject
- **Active Frames**: 设置激活持续的帧数（默认 30 帧，约 0.5 秒 @ 60fps）
- **Show Debug Log**: 勾选以查看调试信息

## 📋 使用场景示例

### 场景1：角色动作特效

```
Hierarchy:
├── CharacterAnimationController (挂载 CharacterAnimation.cs)
└── Character
    ├── Body (Sprite)
    └── ActionEffect (目标对象 - 初始关闭)
        └── ParticleSystem
```

当玩家输入时，ActionEffect 会激活 30 帧然后自动关闭。

### 场景2：UI 反馈动画

```
Hierarchy:
├── CharacterAnimationController (挂载 CharacterAnimation.cs)
└── Canvas
    └── FeedbackAnimation (目标对象 - 初始关闭)
        └── Image (带 Animator)
```

当玩家输入时，FeedbackAnimation 会激活并播放动画，30 帧后自动关闭。

### 场景3：角色表情变化

```
Hierarchy:
├── CharacterAnimationController (挂载 CharacterAnimation.cs)
└── Character
    ├── NormalFace (默认显示)
    └── ActionFace (目标对象 - 初始关闭)
```

当玩家输入时，ActionFace 会显示 30 帧，然后自动隐藏回到 NormalFace。

## 🎮 使用方法

### 自动触发（推荐）

脚本会自动订阅 `InputSystem.OnPlayerInput` 事件，当玩家输入时自动激活目标对象。

### 手动触发

```csharp
// 获取角色动画组件
CharacterAnimation characterAnim = GetComponent<CharacterAnimation>();

// 手动触发激活
characterAnim.TriggerActivation();

// 强制停止并关闭
characterAnim.ForceStop();

// 检查是否正在激活状态
bool isActive = characterAnim.IsActive();
```

## 🔧 参数调整

### 调整激活时长

修改 `Active Frames` 值：
- **30 帧** ≈ 0.5 秒 @ 60fps
- **60 帧** ≈ 1.0 秒 @ 60fps
- **15 帧** ≈ 0.25 秒 @ 60fps

**注意：** 实际时长取决于游戏的帧率。

### 使用时间而非帧数

如果需要使用秒数而非帧数，可以修改脚本：

```csharp
[Header("激活设置")]
public float activeDuration = 0.5f; // 秒

private IEnumerator ActivateCoroutine()
{
    isActive = true;
    targetObject.SetActive(true);
    
    yield return new WaitForSeconds(activeDuration);
    
    targetObject.SetActive(false);
    isActive = false;
    activeCoroutine = null;
}
```

## 🐛 故障排查

### 问题1：目标对象没有激活

**检查清单：**
1. ✅ 是否在 Inspector 中设置了 Target Object？
2. ✅ InputSystem 是否在场景中并正常工作？
3. ✅ 是否勾选了 Show Debug Log 查看日志？
4. ✅ 目标对象是否存在于场景中？

### 问题2：目标对象一直保持激活状态

**可能原因：**
- 协程被意外停止
- 脚本被禁用

**解决方法：**
- 调用 `ForceStop()` 方法重置状态
- 检查 Console 是否有错误信息

### 问题3：连续输入没有反应

**这是正常行为！**
- 激活期间会忽略所有新的输入
- 等待目标对象关闭后才能再次触发

### 问题4：激活时长不准确

**原因：**
- 帧数受游戏帧率影响
- 如果游戏卡顿，实际时长会变长

**解决方法：**
- 使用 `WaitForSeconds` 代替帧数等待（见上方"使用时间而非帧数"）

## 💡 高级用法

### 多个目标对象

如果需要同时控制多个对象，可以创建一个父对象：

```
Hierarchy:
├── CharacterAnimationController (挂载 CharacterAnimation.cs)
└── EffectGroup (目标对象)
    ├── Effect1
    ├── Effect2
    └── Effect3
```

将 `EffectGroup` 设置为 Target Object，所有子对象会一起激活/关闭。

### 配合 Animator 使用

目标对象可以包含 Animator 组件：

```
Hierarchy:
├── CharacterAnimationController (挂载 CharacterAnimation.cs)
└── AnimatedObject (目标对象，带 Animator)
```

当对象激活时，Animator 会自动开始播放动画。30 帧后对象关闭，动画也会停止。

### 配合粒子系统使用

```csharp
// 在目标对象上添加 ParticleSystem
// 设置 ParticleSystem 的 Stop Action 为 "Disable"
// 这样粒子播放完会自动禁用
```

## 📝 完整测试流程

1. **创建控制器**
   ```
   Hierarchy > 右键 > Create Empty
   命名为 "CharacterAnimationController"
   添加 CharacterAnimation.cs 脚本
   ```

2. **创建目标对象**
   ```
   创建一个 GameObject（如 Image、Sprite、粒子等）
   初始设置为关闭状态（可选）
   ```

3. **配置脚本**
   ```
   将目标对象拖入 Target Object 字段
   设置 Active Frames = 30
   勾选 Show Debug Log
   ```

4. **测试**
   ```
   运行游戏
   按下输入键（空格键或点击屏幕）
   观察目标对象是否激活 30 帧后关闭
   尝试连续输入，确认激活期间输入被忽略
   ```

## 🎨 推荐工作流程

1. 设计需要显示/隐藏的视觉效果
2. 创建对应的 GameObject（可以包含动画、粒子等）
3. 创建 CharacterAnimationController 并配置
4. 测试并调整 Active Frames 参数
5. 根据需要添加音效、其他视觉反馈

## ⚙️ 性能提示

- 使用对象池而非频繁激活/关闭可以提高性能
- 如果目标对象很复杂，考虑使用 Canvas Group 的 alpha 控制可见性
- 粒子系统建议使用 "Prewarm" 选项以避免激活时的延迟

## 🔄 与其他系统集成

### 与判定系统配合

```csharp
// 在判定脚本中
void OnPlayerInputReceived()
{
    bool isPerfect = CheckTiming();
    
    if (isPerfect)
    {
        // 触发完美特效
        characterAnimation.TriggerActivation();
    }
}
```

### 与音效系统配合

```csharp
private void OnPlayerInputReceived()
{
    if (isActive) return;
    
    // 播放音效
    AudioManager.Instance.PlaySound("action");
    
    // 激活视觉效果
    ActivateTarget();
}
```
