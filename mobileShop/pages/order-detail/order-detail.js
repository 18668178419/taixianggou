// pages/order-detail/order-detail.js
const api = require('../../utils/api.js')

Page({
  data: {
    orderId: null,
    order: {},
    statusIcon: '',
    statusText: '',
    statusDesc: ''
  },

  onLoad(options) {
    const id = parseInt(options.id)
    this.setData({ orderId: id })
    this.loadOrder()
  },

  async loadOrder() {
    try {
      wx.showLoading({ title: '加载中...' })
      const order = await api.getOrderById(this.data.orderId)
    
      if (order) {
        const statusMap = {
          unpaid: { icon: '⏰', text: '待付款', desc: '请尽快完成支付' },
          unshipped: { icon: '📦', text: '待发货', desc: '商家正在准备商品' },
          shipped: { icon: '🚚', text: '待收货', desc: '商品正在路上' },
          completed: { icon: '✅', text: '已完成', desc: '订单已完成' }
        }
        
        const statusInfo = statusMap[order.status] || statusMap.unpaid
        
        this.setData({
          order:order,
          statusIcon: statusInfo.icon,
          statusText: statusInfo.text,
          statusDesc: statusInfo.desc
        })
      }
    } catch (error) {
      console.error('加载订单失败', error)
      wx.showToast({
        title: '加载失败',
        icon: 'none'
      })
      setTimeout(() => {
        wx.navigateBack()
      }, 1500)
    } finally {
      wx.hideLoading()
    }
  },

  async onPayOrder() {
    try {
      wx.showLoading({ title: '准备支付...' })
      
      // 获取用户openid
      const loginRes = await new Promise((resolve, reject) => {
        wx.login({
          success: resolve,
          fail: reject
        })
      })

      if (!loginRes.code) {
        wx.hideLoading()
        wx.showToast({
          title: '获取登录信息失败',
          icon: 'none'
        })
        return
      }

      // 从存储中获取openid，如果没有则通过code获取
      let openId = wx.getStorageSync('openid')
      if (!openId) {
        // 调用后端接口通过code换取openid
       
        const userInfo = await api.getOpenId(loginRes.code)
        openId = userInfo.openid
        if (!openId) {
          wx.hideLoading()
          wx.showToast({
            title: '获取用户信息失败',
            icon: 'none'
          })
          return
        }
        // 保存openid到本地存储
        wx.setStorageSync('openid', openId)
      }

      // 调用后端创建支付订单
      const paymentParams = await api.createPayment(this.data.orderId, openId)
      
      wx.hideLoading()

      // 调起微信支付
      const payRes = await new Promise((resolve, reject) => {
        wx.requestPayment({
          timeStamp: paymentParams.timeStamp,
          nonceStr: paymentParams.nonceStr,
          package: paymentParams.package,
          signType: paymentParams.signType,
          paySign: paymentParams.paySign,
          success: resolve,
          fail: reject
        })
      })

      if (payRes.errMsg === 'requestPayment:ok') {
        wx.showToast({
          title: '支付处理中',
          icon: 'success'
        })
        // 开始轮询支付状态
        this.pollPayStatus()
      }
    } catch (error) {
      wx.hideLoading()
      console.error('支付失败', error)
      
      if (error.errMsg && error.errMsg.includes('cancel')) {
        wx.showToast({
          title: '支付已取消',
          icon: 'none'
        })
      } else {
        wx.showToast({
          title: error.message || '支付失败',
          icon: 'none'
        })
      }
    }
  },

  async onCancelOrder() {
    wx.showModal({
      title: '提示',
      content: '确定要取消这个订单吗？',
      success: async (res) => {
        if (res.confirm) {
          try {
            wx.showLoading({ title: '取消中...' })
            await api.deleteOrder(this.data.orderId)
            
            wx.hideLoading()
            wx.showToast({
              title: '订单已取消',
              icon: 'success'
            })
            
            setTimeout(() => {
              wx.navigateBack()
            }, 1500)
          } catch (error) {
            wx.hideLoading()
            console.error('取消订单失败', error)
            wx.showToast({
              title: error.message || '取消订单失败',
              icon: 'none'
            })
          }
        }
      }
    })
  },

  onConfirmReceive() {
    wx.showModal({
      title: '提示',
      content: '确定已收到商品吗？',
      success: (res) => {
        if (res.confirm) {
          const orders = wx.getStorageSync('orders') || []
          const orderIndex = orders.findIndex(item => item.id === this.data.orderId)
          if (orderIndex !== -1) {
            orders[orderIndex].status = 'completed'
            wx.setStorageSync('orders', orders)
            this.loadOrder()
            
            wx.showToast({
              title: '确认收货成功',
              icon: 'success'
            })
          }
        }
      }
    })
  }
  ,

  // 轮询支付状态
  async pollPayStatus() {
    const maxRetry = 10
    const interval = 2000
    let count = 0

    const timer = setInterval(async () => {
      count += 1
      try {
        const res = await api.getPayStatus(this.data.orderId)
        if (res.orderStatus !== 'unpaid' || res.payStatus === 'success') {
          clearInterval(timer)
          wx.showToast({
            title: '支付成功',
            icon: 'success'
          })
          this.loadOrder()
        } else if (count >= maxRetry) {
          clearInterval(timer)
          wx.showToast({
            title: '支付结果未知，请刷新',
            icon: 'none'
          })
          this.loadOrder()
        }
      } catch (err) {
        if (count >= maxRetry) {
          clearInterval(timer)
          wx.showToast({
            title: '查询失败',
            icon: 'none'
          })
        }
      }
    }, interval)
  }
})

