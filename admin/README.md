# 泰享购管理后台

基于 Vue3 + Element Plus + Axios 开发的管理后台。

## 技术栈

- Vue 3
- Element Plus
- Vue Router
- Pinia
- Axios
- Vite

## 快速开始

### 1. 安装依赖

```bash
cd admin
npm install
```

### 2. 启动开发服务器

```bash
npm run dev
```

管理后台将在 `http://localhost:3000` 启动。

### 3. 构建生产版本

```bash
npm run build
```

## 功能模块

### 1. 登录
- 默认账号：admin / admin123

### 2. 首页
- 数据概览（商品总数、订单统计等）

### 3. 商品管理
- 商品列表（支持搜索、筛选）
- 添加商品
- 编辑商品
- 删除商品

### 4. 分类管理
- 分类列表
- 添加分类
- 编辑分类
- 删除分类

### 5. 轮播图管理
- 轮播图列表
- 添加轮播图
- 编辑轮播图
- 删除轮播图

### 6. 订单管理
- 订单列表（支持搜索、筛选）
- 订单详情
- 订单发货

## 配置说明

### API地址配置

在 `vite.config.js` 中配置了代理：

```javascript
proxy: {
  '/api': {
    target: 'http://localhost:5000',
    changeOrigin: true
  }
}
```

生产环境需要修改为实际的后端API地址。

## 注意事项

1. 确保后端API服务已启动
2. 生产环境需要配置正确的API地址
3. 建议添加权限管理功能

