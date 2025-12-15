# 小程序API对接说明

小程序已修改为从后端API获取数据，不再使用本地模拟数据。

## 修改内容

### 1. 新增API工具类

创建了 `utils/api.js` 文件，封装了所有API请求方法。

### 2. 修改的页面

- **首页** (`pages/index/index.js`) - 从API获取轮播图、分类、商品数据
- **分类页** (`pages/category/category.js`) - 从API获取分类和商品数据
- **商品列表页** (`pages/goods-list/goods-list.js`) - 从API获取商品列表
- **商品详情页** (`pages/goods-detail/goods-detail.js`) - 从API获取商品详情
- **订单提交页** (`pages/order/order.js`) - 通过API提交订单
- **订单详情页** (`pages/order-detail/order-detail.js`) - 从API获取订单详情
- **订单列表页** (`pages/order-list/order-list.js`) - 从API获取订单列表
- **个人中心页** (`pages/profile/profile.js`) - 从API获取订单统计

## API地址配置

在 `utils/api.js` 中配置了API基础地址：

```javascript
const API_BASE_URL = 'http://localhost:5000/api' // 开发环境
```

### 生产环境配置

生产环境需要修改为实际的后端API地址：

```javascript
const API_BASE_URL = 'https://your-api-domain.com/api' // 生产环境
```

## 数据格式转换

小程序端对API返回的数据进行了格式转换：

1. **商品图片** - JSON字符串转换为数组
2. **商品规格** - JSON字符串转换为对象
3. **商品标签** - 逗号分隔字符串转换为数组
4. **价格** - 确保为数字类型

## 注意事项

1. **网络请求域名配置**
   - 需要在微信小程序后台配置服务器域名
   - 开发环境可以在微信开发者工具中勾选"不校验合法域名"

2. **CORS配置**
   - 后端API需要配置CORS允许小程序域名访问

3. **错误处理**
   - 所有API请求都添加了错误处理和加载提示
   - 请求失败时会显示错误提示

4. **兼容性**
   - 保留了部分本地存储功能（如购物车）
   - 订单数据现在从API获取，不再使用本地存储

## 测试步骤

1. 确保后端API服务已启动（`http://localhost:5000`）
2. 确保数据库已创建并导入初始数据
3. 在微信开发者工具中打开小程序
4. 在项目设置中勾选"不校验合法域名"（开发环境）
5. 测试各个页面的数据加载

## 回退方案

如果需要临时回退到使用模拟数据，可以：

1. 将各页面的 `require('../../utils/api.js')` 改回 `require('../../data/mockData.js')`
2. 恢复原来的数据加载逻辑

