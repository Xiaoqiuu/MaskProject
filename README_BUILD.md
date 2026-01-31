# Unity GitHub Actions 构建指南

## 概述
此项目使用 GitHub Actions 自动构建 Android APK 和 Windows 可执行文件。

## 构建配置

### 支持的平台
- **Android APK** (StandaloneWindows64)
- **Windows 64位** (Android)

### 工作流文件
`.github/workflows/unity-package-builder.yml`

## 必需的 GitHub Secrets

在使用此工作流之前，需要在 GitHub 仓库中配置以下 Secrets：

### 1. UNITY_LICENSE
Unity 许可证内容。获取方式：

```bash
# 在本地 Unity 编辑器中激活许可证后，运行：
# Windows:
type %USERPROFILE%\.local\share\unity3d\Unity\Unity_lic.ulf

# Linux/Mac:
cat ~/.local/share/unity3d/Unity/Unity_lic.ulf
```

将输出的完整内容复制到 GitHub Secrets 中。

### 2. UNITY_EMAIL
Unity 账户邮箱地址

### 3. UNITY_PASSWORD
Unity 账户密码

## 配置 Secrets 步骤

1. 进入 GitHub 仓库页面
2. 点击 `Settings` → `Secrets and variables` → `Actions`
3. 点击 `New repository secret`
4. 分别添加以上三个 secrets

## 触发构建

### 自动触发
- 推送到 `main` 分支
- 创建针对 `main` 分支的 Pull Request

### 手动触发
1. 进入 GitHub 仓库的 `Actions` 标签
2. 选择 `Unity Build (Windows & Android)` 工作流
3. 点击 `Run workflow` 按钮
4. 选择分支并点击 `Run workflow`

## 构建产物

构建完成后，可以在 Actions 运行页面下载：

- `Build-StandaloneWindows64` - Windows 可执行文件
- `Build-Android` - Android APK 文件

产物保留 7 天。

## 当前项目配置

- **Unity 版本**: 2022.3.48f1
- **产品名称**: MaskProject
- **包名**: com.DefaultCompany.2DProject
- **Android 最低 SDK**: API 22 (Android 5.1)
- **构建场景**: Assets/Scenes/SampleScene.unity

## 故障排除

### 构建失败
1. 检查 Unity License 是否正确配置
2. 确认 Unity 版本匹配 (2022.3.48f1)
3. 查看 Actions 日志获取详细错误信息

### Android 构建问题
- 确保 Android 平台已在 Unity 项目中正确配置
- 检查 AndroidMinSdkVersion 设置

### Windows 构建问题
- 确保场景已添加到 Build Settings
- 检查项目是否有编译错误

## 优化建议

### 减少构建时间
- 工作流已启用 Library 缓存
- 使用 `concurrency` 防止重复构建

### 磁盘空间
- Android 构建前会自动清理磁盘空间
- 使用 `jlumbroso/free-disk-space` action

## 注意事项

1. 首次构建可能需要较长时间（15-30分钟）
2. 后续构建会利用缓存加速（5-15分钟）
3. 免费 GitHub Actions 有使用时长限制
4. 构建产物会占用仓库存储空间配额
