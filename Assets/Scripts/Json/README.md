# JSON 数据持久化系统

## 文件说明

- **GameData.cs**: 游戏数据类，定义需要保存的数据结构
- **JsonDataManager.cs**: JSON管理器，负责数据的读取和写入

## 使用方法

### 1. 游戏启动时自动加载数据
GameManager 的 `Awake()` 方法会自动调用 `LoadGameData()`

### 2. 游戏结束时保存数据
```csharp
// 在游戏结束时调用
GameManager.Instance.GameOver();
```

### 3. 手动保存数据
```csharp
GameManager.Instance.SaveGameData();
```

### 4. 重置存档（测试用）
```csharp
GameManager.Instance.ResetSaveData();
```

### 5. 访问历史最高数据
```csharp
int highestMoney = GameManager.Instance.maxMoney;
int highestSushi = GameManager.Instance.maxSushiCount;
```

## 数据存储位置

数据保存在: `Application.persistentDataPath + "/gamedata.json"`

不同平台路径：
- Windows: `C:/Users/用户名/AppData/LocalLow/公司名/游戏名/gamedata.json`
- Mac: `~/Library/Application Support/公司名/游戏名/gamedata.json`
- Android: `/storage/emulated/0/Android/data/包名/files/gamedata.json`

## 添加新的数据字段

在 `GameData.cs` 中添加新字段：
```csharp
[Serializable]
public class GameData
{
    public int maxMoney = 0;
    public int maxSushiCount = 0;
    public int newField = 0; // 新字段
}
```

然后在 `GameManager.SaveGameData()` 中更新该字段即可。


Written by Kiro❤️