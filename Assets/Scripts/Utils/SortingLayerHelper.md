# Unity 渲染层级调整指南

## 🎨 问题：传送带被 UI 背景遮挡

### 原因
Unity 的渲染顺序由以下因素决定：
1. **Camera Depth** - 相机深度
2. **Sorting Layer** - 排序层
3. **Order in Layer** - 层内顺序
4. **Z Position** - Z 轴位置（3D 模式）

## 🔧 解决方案

### 方案 1：使用 Sorting Layer（推荐）

#### 步骤 1：创建 Sorting Layers

1. 打开 **Tags & Layers** 设置：
   - 菜单：`Edit > Project Settings > Tags and Layers`
   - 或点击 Inspector 右上角的 `Layers` 下拉菜单 > `Edit Layers...`

2. 展开 **Sorting Layers** 部分

3. 点击 **+** 按钮添加新的层，按照从后到前的顺序：
   ```
   0. Default
   1. Background      (最后面 - UI 背景)
   2. Conveyor        (中间 - 传送带)
   3. GameObjects     (游戏对象 - 寿司等)
   4. Character       (角色)
   5. Effects         (特效)
   6. UI              (最前面 - UI 元素)
   ```

#### 步骤 2：设置 UI 背景

1. 选中 Canvas 下的背景 Image
2. 在 Inspector 中找到 **Canvas Renderer** 或 **Image** 组件
3. 如果背景是 Sprite：
   - 添加 **Sprite Renderer** 组件（如果没有）
   - 设置 **Sorting Layer** = `Background`
   - 设置 **Order in Layer** = `0`

#### 步骤 3：设置传送带

1. 选中传送带 Prefab 实例
2. 找到 **Sprite Renderer** 组件
3. 设置：
   - **Sorting Layer** = `Conveyor`
   - **Order in Layer** = `0`

#### 步骤 4：设置其他对象

- **寿司**：Sorting Layer = `GameObjects`, Order = `0`
- **角色**：Sorting Layer = `Character`, Order = `0`
- **特效**：Sorting Layer = `Effects`, Order = `0`

### 方案 2：调整 Canvas 设置

如果 UI 背景在 Canvas 中：

#### 选项 A：使用 Screen Space - Camera 模式

1. 选中 Canvas
2. 在 Inspector 中设置：
   - **Render Mode** = `Screen Space - Camera`
   - **Render Camera** = 拖入 Main Camera
   - **Plane Distance** = `10`（或更大的值）
   - **Sorting Layer** = `Background`

3. 这样 Canvas 就会在指定的 Sorting Layer 渲染

#### 选项 B：使用 World Space 模式

1. 选中 Canvas
2. 设置：
   - **Render Mode** = `World Space`
   - **Sorting Layer** = `Background`
   - **Order in Layer** = `0`
   - 调整 **Position Z** = `10`（远离相机）

### 方案 3：调整 Z 轴位置（简单但不推荐）

如果使用 2D 相机且 Sorting Layer 不起作用：

1. 选中传送带
2. 调整 **Transform** 的 **Position Z**：
   - 背景：Z = `10`
   - 传送带：Z = `0`
   - 游戏对象：Z = `-1`
   - 角色：Z = `-2`

**注意**：Z 值越小，越靠近相机（越在前面）

### 方案 4：使用多个 Canvas

将背景和游戏内容分离：

1. **背景 Canvas**：
   - Render Mode = `Screen Space - Overlay`
   - Sort Order = `0`

2. **游戏内容 Canvas**（如果需要）：
   - Render Mode = `Screen Space - Overlay`
   - Sort Order = `1`

3. **传送带**（Sprite）：
   - 不在 Canvas 中
   - Sorting Layer = `Conveyor`

## 📋 推荐的层级结构

```
Hierarchy:
├── Main Camera
├── Canvas (Background)
│   ├── Render Mode: Screen Space - Camera
│   ├── Sorting Layer: Background
│   └── Background Image
├── Conveyor (Prefab)
│   ├── Sprite Renderer
│   ├── Sorting Layer: Conveyor
│   └── Order in Layer: 0
├── GameObjects
│   ├── Sorting Layer: GameObjects
│   └── Sushi, etc.
└── Character
    ├── Sorting Layer: Character
    └── Order in Layer: 0
```

## 🐛 故障排查

### 问题 1：Sorting Layer 不起作用

**检查：**
- 是否所有对象都有 Sprite Renderer 或 Canvas Renderer？
- Sorting Layer 是否已正确创建？
- Camera 的 Projection 是否设置为 Orthographic（2D）？

### 问题 2：UI 和游戏对象混在一起

**解决：**
- UI 使用 Canvas（Screen Space）
- 游戏对象使用 Sprite Renderer + Sorting Layer
- 不要混用

### 问题 3：传送带时而在前时而在后

**原因：**
- Z 轴位置相同导致 Z-fighting
- 没有设置 Sorting Layer

**解决：**
- 使用 Sorting Layer 而不是 Z 轴
- 或确保 Z 轴位置有明显差异（至少 0.1）

## 💡 最佳实践

1. **统一使用 Sorting Layer**
   - 不要混用 Z 轴和 Sorting Layer
   - 为项目定义清晰的层级结构

2. **UI 和游戏内容分离**
   - UI 使用 Canvas
   - 游戏对象使用 Sprite Renderer

3. **命名规范**
   - Sorting Layer 名称要清晰易懂
   - 按照渲染顺序命名（Background, Midground, Foreground）

4. **文档化**
   - 在项目文档中记录 Sorting Layer 的用途
   - 团队成员都应该知道哪个层用于什么

## 🎯 快速修复步骤

### 如果你的背景是 UI Image：

1. 选中 Canvas
2. Render Mode = `Screen Space - Camera`
3. Render Camera = Main Camera
4. Sorting Layer = `Background`

### 如果你的背景是 Sprite：

1. 选中背景 Sprite
2. Sorting Layer = `Background`
3. Order in Layer = `0`

### 设置传送带：

1. 选中传送带
2. Sorting Layer = `Conveyor`（或 `Default`）
3. Order in Layer = `1`（比背景大）

## 📊 Sorting Layer 优先级示例

```
层级（从后到前）：
┌─────────────────────────────────┐
│ Background (Sorting Layer)      │ ← UI 背景
├─────────────────────────────────┤
│ Conveyor (Sorting Layer)        │ ← 传送带
├─────────────────────────────────┤
│ GameObjects (Sorting Layer)     │ ← 寿司等
├─────────────────────────────────┤
│ Character (Sorting Layer)       │ ← 角色
├─────────────────────────────────┤
│ Effects (Sorting Layer)         │ ← 特效
├─────────────────────────────────┤
│ UI (Sorting Layer)              │ ← UI 元素
└─────────────────────────────────┘
```

## 🔍 检查清单

- [ ] 已创建 Sorting Layers
- [ ] 背景设置为最后面的层
- [ ] 传送带设置为中间的层
- [ ] 所有对象都有正确的 Renderer 组件
- [ ] Camera 设置为 Orthographic（2D 项目）
- [ ] 测试运行，确认渲染顺序正确
