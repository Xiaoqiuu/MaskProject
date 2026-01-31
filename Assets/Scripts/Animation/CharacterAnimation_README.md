# 角色帧动画系统使用说明

## 📖 概述

`CharacterAnimation.cs` 是一个基于 4 张图片的帧动画控制器，模拟角色放手和抬手的动作。

## 🎬 动画逻辑

### 动作定义
- **放手动作**：1 → 1 → 2 → 3 → 4（手向下）
- **抬手动作**：4 → 3 → 2 → 1 → 1（手向上）

### 完整循环
```
输入 → 放手(1 1 2 3 4) → 抬手(4 3 2 1 1) → 待机(1)
```

### 打断逻辑

**关键规则**：只能在抬手期间打断，打断后切换为放手动作

#### 场景 1：正常完整循环
```
输入 → 放手: 1 1 2 3 4 → 抬手: 4 3 2 1 1 → 完成
```

#### 场景 2：在抬手第1帧时打断（帧4）
```
输入1 → 放手: 1 1 2 3 4 → 抬手: 4 (输入2) → 放手: 4 → 抬手: 4 3 2 1 1
完整序列：1 1 2 3 4 4 4 3 2 1 1
```

#### 场景 3：在抬手第2帧时打断（帧3）
```
输入1 → 放手: 1 1 2 3 4 → 抬手: 4 3 (输入2) → 放手: 3 4 → 抬手: 4 3 2 1 1
完整序列：1 1 2 3 4 4 3 3 4 4 3 2 1 1
```

#### 场景 4：在抬手第3帧时打断（帧2）
```
输入1 → 放手: 1 1 2 3 4 → 抬手: 4 3 2 (输入2) → 放手: 2 3 4 → 抬手: 4 3 2 1 1
完整序列：1 1 2 3 4 4 3 2 2 3 4 4 3 2 1 1
```

#### 场景 5：在抬手第4帧时打断（帧1）
```
输入1 → 放手: 1 1 2 3 4 → 抬手: 4 3 2 1 (输入2) → 放手: 1 2 3 4 → 抬手: 4 3 2 1 1
完整序列：1 1 2 3 4 4 3 2 1 1 2 3 4 4 3 2 1 1
```

#### 场景 6：在放手期间输入（忽略）
```
输入1 → 放手: 1 1 (输入2-忽略) 2 3 4 → 抬手: 4 3 2 1 1
完整序列：1 1 2 3 4 4 3 2 1 1
```

## 🛠️ 设置步骤

### 1. 准备图片资源

准备 4 张动画帧图片：
```
frame_1.png  (第1帧 - 默认状态/手最上)
frame_2.png  (第2帧)
frame_3.png  (第3帧)
frame_4.png  (第4帧 - 手最下)
```

### 2. 导入图片

1. 将图片拖入 Unity 项目
2. 选中所有图片
3. 在 Inspector 中设置：
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Single
   - 点击 **Apply**

### 3. 创建角色 UI

1. 在 Canvas 下创建 Image：
   ```
   Hierarchy > 右键 Canvas > UI > Image
   命名为 "Character"
   ```

2. 调整 Image 的位置和大小

3. 将 `CharacterAnimation.cs` 脚本挂载到这个 Image 上

### 4. 配置脚本

在 Inspector 中配置：

#### Animation Frames（动画帧）
- **Size**: 4
- **Element 0**: 拖入 frame_1.png（手最上）
- **Element 1**: 拖入 frame_2.png
- **Element 2**: 拖入 frame_3.png
- **Element 3**: 拖入 frame_4.png（手最下）

#### 播放设置
- **Frame Time**: 0.0167（约 1/60 秒，即 1 帧）

#### 调试信息
- **Show Debug Log**: 勾选以查看详细日志

## 🎮 使用方法

### 自动触发（推荐）

脚本会自动订阅 `InputSystem.OnPlayerInput` 事件：
- **第一次输入** → 开始放手动作
- **放手期间输入** → 忽略
- **抬手期间输入** → 打断，切换为放手，从当前帧继续
- **完成后输入** → 重新开始放手动作

### 手动触发

```csharp
// 获取角色动画组件
CharacterAnimation characterAnim = GetComponent<CharacterAnimation>();

// 手动触发动画
characterAnim.TriggerAnimation();

// 停止动画
characterAnim.StopAnimation();

// 检查是否正在播放
bool isPlaying = characterAnim.IsPlaying();

// 获取当前帧索引（0-3）
int frameIndex = characterAnim.GetCurrentFrameIndex();

// 检查当前方向（true=放手，false=抬手）
bool isGoingDown = characterAnim.IsGoingDown();
```

## 🔧 参数调整

### 调整动画速度

修改 `Frame Time` 值：
- **更快**：0.01（每帧 0.01 秒）
- **标准**：0.0167（每帧 1/60 秒）
- **更慢**：0.05（每帧 0.05 秒）

### 修改序列

如果需要不同的序列，修改脚本中的数组：

```csharp
// 放手序列：1 1 2 3 4
private readonly int[] downSequence = { 0, 0, 1, 2, 3 };

// 抬手序列：4 3 2 1 1
private readonly int[] upSequence = { 3, 2, 1, 0, 0 };

// 示例：更快的序列
private readonly int[] downSequence = { 0, 1, 2, 3 };
private readonly int[] upSequence = { 3, 2, 1, 0 };
```

## 🐛 故障排查

### 问题 1：动画不播放

**检查清单：**
1. ✅ 是否挂载到 Image 组件上？
2. ✅ 是否设置了所有 4 张图片？
3. ✅ InputSystem 是否正常工作？
4. ✅ 是否勾选了 Show Debug Log？

### 问题 2：打断不生效

**可能原因：**
- 在放手期间输入（这是正常的，会被忽略）
- 输入系统有问题

**解决方法：**
1. 勾选 Show Debug Log 查看日志
2. 确认当前是否在抬手阶段
3. 查看日志中的"方向"信息

### 问题 3：动画卡住

**可能原因：**
- Frame Time 设置为 0
- 协程被意外停止

**解决方法：**
- 检查 Frame Time 是否大于 0
- 调用 `StopAnimation()` 重置状态

## 💡 高级用法

### 根据游戏状态调整速度

```csharp
void Update()
{
    // 根据分数调整动画速度
    if (GameManager.Instance.Money > 100)
    {
        characterAnim.frameTime = 0.01f; // 更快
    }
    else
    {
        characterAnim.frameTime = 0.0167f; // 正常
    }
}
```

### 检测动画状态

```csharp
void Update()
{
    if (characterAnim.IsPlaying())
    {
        if (characterAnim.IsGoingDown())
        {
            Debug.Log("正在放手");
        }
        else
        {
            Debug.Log("正在抬手");
        }
    }
}
```

## � 调试日志示例

### 正常完整循环
```
[CharacterAnimation] 收到输入 - 当前帧:1, 方向:放手, 播放中:False
[CharacterAnimation] 开始放手动作
[CharacterAnimation] 放手 [0/4] 显示帧 1
[CharacterAnimation] 放手 [1/4] 显示帧 1
[CharacterAnimation] 放手 [2/4] 显示帧 2
[CharacterAnimation] 放手 [3/4] 显示帧 3
[CharacterAnimation] 放手 [4/4] 显示帧 4
[CharacterAnimation] 放手完成，开始抬手
[CharacterAnimation] 开始抬手动作
[CharacterAnimation] 抬手 [0/4] 显示帧 4
[CharacterAnimation] 抬手 [1/4] 显示帧 3
[CharacterAnimation] 抬手 [2/4] 显示帧 2
[CharacterAnimation] 抬手 [3/4] 显示帧 1
[CharacterAnimation] 抬手 [4/4] 显示帧 1
[CharacterAnimation] 抬手完成，回到待机
```

### 在抬手期间打断
```
[CharacterAnimation] 收到输入 - 当前帧:3, 方向:抬手, 播放中:True
[CharacterAnimation] 打断抬手动作，切换为放手，从帧 3 继续
[CharacterAnimation] 从帧 3 继续放手
[CharacterAnimation] 放手 [2/4] 显示帧 3
[CharacterAnimation] 放手 [3/4] 显示帧 4
[CharacterAnimation] 放手完成，开始抬手
```

### 在放手期间输入（忽略）
```
[CharacterAnimation] 收到输入 - 当前帧:2, 方向:放手, 播放中:True
[CharacterAnimation] 已经在放手，忽略输入
```

## 🎯 推荐配置

### 标准配置（60fps）
```
Frame Time: 0.0167
Animation Frames: 4张图片（按顺序）
Show Debug Log: ✓（测试时）
```

### 快速配置
```
Frame Time: 0.01
Animation Frames: 4张图片
Show Debug Log: ✗
```

## � 设计理念

这个动画系统模拟了一个"按键打击"的感觉：
- **放手** = 按下按键
- **抬手** = 松开按键
- **打断** = 快速连击

通过在抬手期间打断，可以实现连续快速的打击效果，非常适合音乐节奏游戏！

## 🎵 适用场景

- 音乐节奏游戏
- 打击类游戏
- 需要快速连击的游戏
- 任何需要"按下-松开"动作的场景
