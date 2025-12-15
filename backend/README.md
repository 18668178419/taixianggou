# 泰享购后端API

基于 .NET 9 + MySQL + SqlSugar 开发的后端API服务。

## 技术栈

- .NET 9
- MySQL
- SqlSugar ORM
- Swagger

## 数据库配置

数据库地址：121.199.70.187:3306
用户名：sa
密码：Admin@123
数据库名：taixianggou

## 快速开始

### 1. 创建数据库

执行 `database/schema.sql` 文件创建数据库和表结构。

### 2. 配置连接字符串

在 `appsettings.json` 中已配置好数据库连接字符串。

### 3. 运行项目

```bash
cd backend/TaiXiangGou.API
dotnet run
```

API服务将在 `http://localhost:5000` 启动。

### 4. 访问Swagger文档

打开浏览器访问：`http://localhost:5000/swagger`

## API接口

### 分类管理
- GET `/api/categories` - 获取分类列表
- GET `/api/categories/{id}` - 获取分类详情
- POST `/api/categories` - 创建分类
- PUT `/api/categories/{id}` - 更新分类
- DELETE `/api/categories/{id}` - 删除分类

### 轮播图管理
- GET `/api/banners` - 获取轮播图列表
- GET `/api/banners/{id}` - 获取轮播图详情
- POST `/api/banners` - 创建轮播图
- PUT `/api/banners/{id}` - 更新轮播图
- DELETE `/api/banners/{id}` - 删除轮播图

### 商品管理
- GET `/api/goods` - 获取商品列表（支持分页、筛选）
- GET `/api/goods/{id}` - 获取商品详情
- POST `/api/goods` - 创建商品
- PUT `/api/goods/{id}` - 更新商品
- DELETE `/api/goods/{id}` - 删除商品

### 订单管理
- GET `/api/orders` - 获取订单列表（支持分页、筛选）
- GET `/api/orders/{id}` - 获取订单详情
- POST `/api/orders` - 创建订单
- PUT `/api/orders/{id}` - 更新订单
- PUT `/api/orders/{id}/ship` - 发货
- DELETE `/api/orders/{id}` - 删除订单

## 响应格式

所有API统一返回格式：

```json
{
  "code": 200,
  "data": {},
  "message": "success"
}
```

## 注意事项

1. 生产环境需要修改 `appsettings.json` 中的数据库连接字符串
2. 需要配置CORS允许小程序域名访问
3. 建议添加身份验证和授权机制

