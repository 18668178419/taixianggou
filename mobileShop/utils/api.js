// API工具类
const API_BASE_URL = 'http://localhost:5000/api' // 开发环境，生产环境需要修改
//const API_BASE_URL = 'https://txgapi.zzlol.cn/api' // 开发环境，生产环境需要修改
// 将字段名首字母大写改为小写
function toLowerCaseFirst(obj) {
  if (obj === null || obj === undefined) {
    return obj
  }
  
  if (Array.isArray(obj)) {
    return obj.map(item => toLowerCaseFirst(item))
  }
  
  if (typeof obj === 'object') {
    const result = {}
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        // 将首字母大写改为小写：Id -> id, Name -> name
        const newKey = key.charAt(0).toLowerCase() + key.slice(1)
        result[newKey] = toLowerCaseFirst(obj[key])
      }
    }
    return result
  }
  
  return obj
}

// 请求封装
const request = (url, method = 'GET', data = {}) => {
  return new Promise((resolve, reject) => {
    wx.request({
      url: API_BASE_URL + url,
      method: method,
      data: data,
      header: {
        'content-type': 'application/json'
      },
      success: (res) => {
        if (res.statusCode === 200) {
          if (res.data.code === 200) {
            // 转换字段名首字母为大写改为小写
            const data = toLowerCaseFirst(res.data.data)
            resolve(data)
          } else {
            reject(new Error(res.data.message || '请求失败'))
          }
        } else {
          reject(new Error('网络错误'))
        }
      },
      fail: (err) => {
        reject(err)
      }
    })
  })
}

// API方法
const api = {
  // 获取轮播图列表
  getBanners: (status = true) => {
    return request('/banners', 'GET', { status })
  },

  // 获取分类列表
  getCategories: (status = true) => {
    return request('/categories', 'GET', { status })
  },

  // 获取商品列表
  getGoodsList: (params = {}) => {
   
    return request('/goods', 'GET', params)
  },

  // 获取商品详情
  getGoodsById: (id) => {
    return request(`/goods/${id}`)
  },

  // 创建订单
  createOrder: (orderData) => {
    return request('/orders', 'POST', orderData)
  },

  // 获取订单列表
  getOrdersList: (params = {}) => {
    return request('/orders', 'GET', params)
  },

  // 获取订单详情
  getOrderById: (id) => {
    return request(`/orders/${id}`)
  },

  // 删除订单（取消订单）
  deleteOrder: (id) => {
    return request(`/orders/${id}`, 'DELETE')
  },

  // 创建支付订单
  createPayment: (orderId, openId) => {
    return request(`/orders/${orderId}/pay`, 'POST', { openId })
  },

  // 查询支付状态（轮询）
  getPayStatus: (orderId) => {
    return request(`/orders/${orderId}/pay-status`, 'GET')
  },

  // 通过code获取openid
  getOpenId: (code) => {
    return request('/auth/getOpenId', 'POST', { code })
  },

  // 小程序登录：code + 用户信息，返回用户id和openid
  login: (code, userInfo) => {
    return request('/auth/login', 'POST', {
      code,
      userInfo
    })
  },

  // 收藏相关
  getFavorites: (params = {}) => {
    return request('/favorites', 'GET', params)
  },
  checkFavorite: (params = {}) => {
    return request('/favorites/check', 'GET', params)
  },
  addFavorite: (data) => {
    return request('/favorites', 'POST', data)
  },
  removeFavoriteById: (id) => {
    return request(`/favorites/${id}`, 'DELETE')
  },
  removeFavoriteByGoods: (params = {}) => {
    return request('/favorites/by-goods', 'DELETE', params)
  },

  // 地址相关
  getAddresses: (params = {}) => {
    return request('/addresses', 'GET', params)
  },
  getAddressById: (id) => {
    return request(`/addresses/${id}`)
  },
  getDefaultAddress: (params = {}) => {
    return request('/addresses/default', 'GET', params)
  },
  createAddress: (data) => {
    return request('/addresses', 'POST', data)
  },
  updateAddress: (id, data) => {
    return request(`/addresses/${id}`, 'PUT', data)
  },
  deleteAddress: (id) => {
    return request(`/addresses/${id}`, 'DELETE')
  },
  setDefaultAddress: (id) => {
    return request(`/addresses/${id}/default`, 'PUT')
  }
}

module.exports = api

