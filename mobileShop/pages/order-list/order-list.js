// pages/order-list/order-list.js
const api = require('../../utils/api.js')

Page({
  data: {
    status: 'all',
    orders: [],
    statusText: {
      unpaid: '待付款',
      unshipped: '待发货',
      shipped: '待收货',
      completed: '已完成'
    }
  },

  onLoad(options) {
    const status = options.status || 'all'
    this.setData({ status })
    this.loadOrders()
  },

  onShow() {
    this.loadOrders()
  },

  async loadOrders() {
    try {
      wx.showLoading({ title: '加载中...' })
      const params = {
        page: 1,
        pageSize: 100
      }
      
      if (this.data.status !== 'all') {
        params.status = this.data.status
      }
      
      const res = await api.getOrdersList(params)
      this.setData({ orders: res.list || [] })
    } catch (error) {
      console.error('加载订单失败', error)
      wx.showToast({
        title: '加载失败',
        icon: 'none'
      })
    } finally {
      wx.hideLoading()
    }
  },

  onOrderTap(e) {
    const id = e.currentTarget.dataset.id
    wx.navigateTo({
      url: `/pages/order-detail/order-detail?id=${id}`
    })
  },

  stopPropagation() {
    // 阻止事件冒泡
  },

  onPayOrder(e) {
    const id = parseInt(e.currentTarget.dataset.id)
    wx.showModal({
      title: '模拟支付',
      content: '这是模拟支付，实际项目中需要接入支付接口',
      success: (res) => {
        if (res.confirm) {
          const orders = wx.getStorageSync('orders') || []
          const orderIndex = orders.findIndex(item => item.id === id)
          if (orderIndex !== -1) {
            orders[orderIndex].status = 'unshipped'
            wx.setStorageSync('orders', orders)
            this.loadOrders()
            
            wx.showToast({
              title: '支付成功',
              icon: 'success'
            })
          }
        }
      }
    })
  },

  async onCancelOrder(e) {
    const id = parseInt(e.currentTarget.dataset.id)
    wx.showModal({
      title: '提示',
      content: '确定要取消这个订单吗？',
      success: async (res) => {
        if (res.confirm) {
          try {
            wx.showLoading({ title: '取消中...' })
            await api.deleteOrder(id)
            
            wx.hideLoading()
            wx.showToast({
              title: '订单已取消',
              icon: 'success'
            })
            
            // 重新加载订单列表
            this.loadOrders()
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

  onConfirmReceive(e) {
    const id = parseInt(e.currentTarget.dataset.id)
    wx.showModal({
      title: '提示',
      content: '确定已收到商品吗？',
      success: (res) => {
        if (res.confirm) {
          const orders = wx.getStorageSync('orders') || []
          const orderIndex = orders.findIndex(item => item.id === id)
          if (orderIndex !== -1) {
            orders[orderIndex].status = 'completed'
            wx.setStorageSync('orders', orders)
            this.loadOrders()
            
            wx.showToast({
              title: '确认收货成功',
              icon: 'success'
            })
          }
        }
      }
    })
  },

  onGoShopping() {
    wx.switchTab({
      url: '/pages/index/index'
    })
  }
})

