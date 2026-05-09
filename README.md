# 史上最好的猎手卡牌扩展

GitHub: https://github.com/Divank122/UltimateSilentCardExpansion-STS2

为《杀戮尖塔2》静默猎手添加30张新卡牌的模组。

## 简介

本MOD为静默猎手设计了30张全新卡牌，包括：
- **1张先古卡牌**：探索者
- **13张罕见卡牌**：弹性纤维、剥皮、蠕动、结块、疾风步、蛛网、混乱打击、针锋相对、扎透、护符、有备而来、快速整理、精打细算
- **16张稀有卡牌**：消解、风暴、起舞、鹰眼视觉、气流屏障、凝固、迷惑冲击、猛蚀、炼金术、刀山、钻心、厄咒、镇静剂、得心应手、化合、镜中倒影

## 文档

- [卡牌列表](CARDS.md) - 查看所有卡牌的详细信息
- [更新日志](CHANGELOG.md) - 查看版本更新历史

## 作者

- **卡牌设计 & 美术设计**：原始小金人
- **编码实现**：Divank

## AI使用声明

本项目在开发过程中使用了AI辅助工具进行代码编写、调试与美术绘制，所有AI生成内容均经过人工审核与调整。

## 依赖

- [BaseLib](https://github.com/Alchyr/BaseLib-StS2) 3.0.6+

## 安装

将以下文件放入游戏的 `mods/UltimateSilentCardExpansion` 文件夹中：
- `UltimateSilentCardExpansion.dll`
- `UltimateSilentCardExpansion.pck`
- `UltimateSilentCardExpansion.json`

## 开发

### 环境要求

- Godot 4.5.1 (Mono 版本)
- .NET 9.0 SDK

### 配置游戏路径

项目默认假设游戏安装在 Steam 默认路径：
```
D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2
```

如果你的游戏安装在其他位置，需要修改 `UltimateSilentCardExpansion.csproj` 文件中的 `Sts2Dir` 属性：
```xml
<Sts2Dir>你的游戏安装路径</Sts2Dir>
```

### 编译

```bash
dotnet build -c Release
```

编译后会自动将 DLL 和 JSON 文件复制到游戏 mods 目录。

### 导出 PCK

编译完成后，需要导出 PCK 文件（包含图片等资源）：

```bash
# Windows
"<Godot路径>\Godot_v4.5.1-stable_mono_win64.exe" --headless --export-release "Windows Desktop" "UltimateSilentCardExpansion.pck"
```

或者在 Godot 编辑器中：
1. 打开项目
2. 选择 **项目** → **导出**
3. 选择 "Windows Desktop" 预设
4. 点击 **导出**

## 版本历史

- **0.1.0** (2026-05-05) - 内测版本，完成所有29张卡牌
- **0.0.x** - 开发测试版本

## 许可证

CC BY-NC-SA 4.0
