# Ultimate Silent Card Expansion

为《杀戮尖塔2》静默猎手添加新卡牌的模组。

## 依赖

- [BaseLib](https://github.com/Alchyr/BaseLib-StS2) 3.0.6+

## 安装

将以下文件放入游戏的 `mods/UltimateSilentCardExpansion` 文件夹中：
- `UltimateSilentCardExpansion.dll`
- `UltimateSilentCardExpansion.pck`
- `UltimateSilentCardExpansion.json`

## 卡牌列表

### 罕见卡牌

#### 针锋相对 (Counterstrike)
- **类型**: 攻击
- **耗能**: 3
- **效果**: 造成27点伤害。如果敌人的意图是攻击，额外造成一次伤害。
- **升级**: 伤害提升至32点。

### 稀有卡牌

#### 厄咒 (Bane)
- **类型**: 技能
- **耗能**: 1
- **效果**: 保留。获得1层无实体和9层中毒。在这个回合每受到一次攻击，减少2层中毒。消耗。
- **升级**: 耗能降为0，获得的中毒从9层减少到7层。

## 开发

### 环境要求

- Godot 4.5.1 (Mono 版本)
- .NET 9.0 SDK

### 编译

```bash
dotnet build -c Release
```

编译后会自动将 DLL 和 JSON 文件复制到游戏 mods 目录。

### 导出 PCK

编译完成后，需要导出 PCK 文件（包含图片等资源）：

```bash
# Windows
"<Godot路径>\Godot_v4.5.1-stable_mono_win64.exe" --headless --export-release "PCK" "<游戏路径>\mods\UltimateSilentCardExpansion\UltimateSilentCardExpansion.pck"
```

或者在 Godot 编辑器中：
1. 打开项目
2. 选择 **项目** → **导出**
3. 选择 "PCK" 预设
4. 点击 **导出**

## 更新日志

### 2026-04-23
- 新增卡牌：针锋相对 (Counterstrike)

### 2026-04-22
- 新增卡牌：厄咒 (Bane)
- 修复导出配置，移除冗余运行时文件
- 删除测试用占位卡牌

### 2026-04-19
- 初始项目结构

## 许可证

CC BY-NC-SA 4.0
