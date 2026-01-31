# Sushi Management Game Project

这是一个基于 Unity 的寿司经营模拟游戏项目。玩家扮演寿司店员工，需要给流水线上的回转寿司按节奏盖上盖子。

## 🎮 游戏玩法 (Gameplay)

### 核心机制
*   **盖盖子**: 玩家需要跟随流水线的节奏（固定间距），给传送带上的寿司盖上盖子。
*   **寿司生成**: 流水线上会随机出现不同种类的寿司。
    *   **分值**: 不同寿司对应不同的“工资”（以日元显示）。
    *   **概率**: 高分值的寿司出现概率较低，低分值的寿司出现概率较高。

### 奖惩系统
*   **初始资金**: 游戏开始时的初始工资为 **1000日元**。
*   **投诉惩罚**: 如果玩家未能成功给寿司盖上盖子：
    *   收到“投诉”。
    *   **扣除工资**: 500日元（暂定）。
    *   **扣除SP**: 40点。

### Combo 加成系统
*   **Combo显示**: 位于人物左侧。
*   **加成规则**: 每累计 **100 Combo**，获得的工资额外增加 **10%**。
*   **上限**: 加成上限为 **50%**（即 500 Combo 后不再增加倍率）。

---

## ✅ 开发进度 (Development Status)

### 已实现功能 (Implemented)
*   **基础框架**:
    *   [GameManager.cs](Assets/Scripts/Logic/GameManager.cs): 单例管理，基础的金币（Money）和计数统计。
    *   [JsonDataManager.cs](Assets/Scripts/Json/JsonDataManager.cs): 数据的本地保存与读取（JSON格式）。
*   **寿司生成原型**:
    *   [SushiSpawner.cs](Assets/Scripts/Logic/SushiSpawner.cs): 实现了基础的物体生成和向右移动。
    *   [SuShi.cs](Assets/Scripts/GameObjects/Sushi/SuShi.cs): 寿司的基础生命周期（自动销毁）。

### 待实现功能 (To Do)
*   **节奏控制**: 将目前的随机时间生成 (`Random.Range`) 改为按固定节奏/网格间距生成。
*   **盖盖子交互**: 实现玩家输入检测与判定逻辑（在正确时机按下按键）。
*   **寿司差异化**:
    *   设计多种寿司预制体/数据结构。
    *   实现基于权重的随机生成算法（高分低概率）。
*   **经济与惩罚逻辑**:
    *   实现漏盖判定及扣钱/扣SP逻辑。
    *   实现初始工资设定。
*   **UI与反馈**:
    *   Combo 计数与显示系统。
    *   Combo 工资加成算法。
    *   SP 值系统实现。

---

## 📂 项目结构与脚本功能说明

### 1. 核心游戏逻辑 (`Assets/Scripts/Logic/`)

*   **`GameManager.cs`**
    *   **功能**: 游戏的总控制器，采用单例模式 (`Singleton`) 实现。
    *   **职责**:
        *   管理游戏内的实时数据：当前金币 (`Money`)、寿司数量 (`sushiCount`)。
        *   管理历史统计数据：历史最高金币 (`maxMoney`)、历史最高寿司数量 (`maxSushiCount`)。
        *   控制游戏的开始 (`gameStart`)。
        *   负责调用数据管理器进行数据的加载 (`LoadGameData`) 和保存 (`SaveGameData`)。
        *   提供接口获取当前寿司生成的进度时间。

*   **`SushiSpawner.cs`**
    *   **功能**: 负责寿司的自动生成逻辑。
    *   **职责**:
        *   根据设定的最小 (`minTime`) 和最大 (`maxTime`) 时间间隔，随机生成寿司。
        *   控制寿司生成的开关 (`isOn`)。
        *   实例化寿司预制体 (`SuShi`) 并给予初始速度 (`originSpeed`) 使其移动。

### 2. 游戏对象行为 (`Assets/Scripts/GameObjects/Sushi/`)

*   **`SuShi.cs`**
    *   **功能**: 挂载在寿司预制体上的脚本，定义单个寿司的行为。
    *   **职责**:
        *   包含寿司的组成部分引用（`rice` 米饭, `fish` 鱼）。
        *   管理寿司的生命周期，在存在一定时间 (`surviveTime`) 后自动销毁，防止场景中对象过多。

### 3. 数据持久化 (`Assets/Scripts/Json/`)

*   **`GameData.cs`**
    *   **功能**: 定义游戏存档的数据结构。
    *   **内容**:
        *   `maxMoney`: 历史最高金币数。
        *   `maxSushiCount`: 历史最高寿司生产数。
        *   `lastPlayTime`: 上次游玩时间。
        *   `totalGamesPlayed`: 总游玩次数。
    *   **特点**: 标记为 `[Serializable]` 以便转换为 JSON 格式。

*   **`JsonDataManager.cs`**
    *   **功能**: 处理数据的读取和写入操作。
    *   **职责**:
        *   利用 `JsonUtility` 将 `GameData` 对象序列化为 JSON 字符串并保存到本地文件。
        *   从本地文件读取 JSON 字符串并反序列化为 `GameData` 对象。
        *   存档路径默认为 `Application.persistentDataPath + "/gamedata.json"`。
        *   提供删除存档和检查存档是否存在的功能。

*   **`JsonTestExample.cs`**
    *   **功能**: 用于测试 JSON 数据存取功能的示例脚本（非核心逻辑）。


## 数据存档

游戏数据保存在设备的持久化数据路径下的 `gamedata.json` 文件中。
