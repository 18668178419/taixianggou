# TabBar 图标说明

本目录用于存放 TabBar 的图标文件。

## 需要的图标文件

请在此目录下添加以下图标文件：

1. **home.png** / **home-active.png** - 首页图标（未选中/选中）
2. **category.png** / **category-active.png** - 分类图标（未选中/选中）
3. **cart.png** / **cart-active.png** - 购物车图标（未选中/选中）
4. **profile.png** / **profile-active.png** - 个人中心图标（未选中/选中）

## 图标规格

- 尺寸：81px × 81px
- 格式：PNG
- 颜色：未选中状态使用灰色，选中状态使用主题色（#ff6b6b）

## 临时方案

如果暂时没有图标，可以：

1. 使用在线图标库（如 iconfont）下载对应图标
2. 使用设计工具（如 Figma、Sketch）制作图标
3. 暂时移除 TabBar 配置中的 `iconPath` 和 `selectedIconPath` 字段，只显示文字

## 移除图标配置示例

如果暂时不想使用图标，可以在 `app.json` 中这样配置：

```json
{
  "pagePath": "pages/index/index",
  "text": "首页"
}
```

移除 `iconPath` 和 `selectedIconPath` 字段即可。

