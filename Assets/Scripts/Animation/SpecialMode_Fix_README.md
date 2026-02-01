# 特殊模式切换协程问题修复

## 问题描述
退出奖励模式后，进入普通模式时，角色的动画失效，报错：
```
Coroutine couldn't be started because the the game object 'Image2' is inactive!
```

## 原因分析
- 进入奖励模式时，普通角色的GameObject被`SetActive(false)`关闭
- 退出奖励模式时，虽然根GameObject被重新激活，但子对象（如Image2）可能仍处于未激活状态
- `CharacterOutlineHover`组件尝试启动协程时，如果所在的GameObject未激活，会导致协程启动失败

## 解决方案

### 1. SpecialModeVisualController.cs 修改
在`TransitionToNormalMode()`方法中：
- 添加了`ActivateAllChildren()`方法，递归激活普通角色的所有子对象
- 确保所有包含协程的组件所在的GameObject都被正确激活

### 2. CharacterOutlineHover.cs 修改
- 添加了`OnEnable()`方法，在GameObject被重新激活时重置状态
- 添加了`OnDisable()`方法，在GameObject被停用时清理协程
- 在`OnInputInvoke()`方法中添加了GameObject激活状态检查，避免在未激活时启动协程

## 关键改进
1. **递归激活子对象**：确保所有子对象都被正确激活
2. **状态重置**：GameObject重新激活时，重置所有状态变量和协程
3. **安全检查**：启动协程前检查GameObject是否激活

## 角色动画逻辑
- 角色动画逻辑（CharacterAnimation.cs）保持不变
- 只修改了模式切换和协程管理的逻辑
- 角色整个GameObject的SetActive状态切换保持不变
