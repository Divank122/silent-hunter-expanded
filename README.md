# Ultimate Silent Card Expansion

为《杀戮尖塔2》静默猎手添加新卡牌的模组。

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

## 文档

- [卡牌列表](CARDS.md)
- [更新日志](CHANGELOG.md)

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

## 许可证

CC BY-NC-SA 4.0
