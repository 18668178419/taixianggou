// pages/order/order.js
const api = require('../../utils/api.js')

Page({
  data: {
    type: 'cart', // cart 或 buyNow
    address: null,
    orderItems: [],
    remark: '',
    shippingFee: 0.01,
    totalPrice: 0,
    finalPrice: 0
  },

  onLoad(options) {
    this.setData({ type: options.type || 'cart' })
    this.loadOrderItems()
    this.loadAddress()
  },

  loadOrderItems() {
    const orderItems = wx.getStorageSync('orderItems') || []
    const totalPrice = orderItems.reduce((sum, item) => sum + item.price * item.count, 0)
    const finalPrice = totalPrice + this.data.shippingFee

    this.setData({
      orderItems,
      totalPrice: totalPrice.toFixed(2),
      finalPrice: finalPrice.toFixed(2)
    })
  },

  loadAddress() {
    const address = wx.getStorageSync('defaultAddress') || null
    this.setData({ address })
  },

  onAddressTap() {
    wx.navigateTo({
      url: '/pages/address-list/address-list?from=order'
    })
  },

  onRemarkInput(e) {
    this.setData({ remark: e.detail.value })
  },

  async onSubmit() {
    if (!this.data.address) {
      wx.showToast({
        title: '请选择收货地址',
        icon: 'none'
      })
      return
    }

    if (this.data.orderItems.length === 0) {
      wx.showToast({
        title: '订单商品不能为空',
        icon: 'none'
      })
      return
    }

    try {
      wx.showLoading({ title: '提交中...' })
      
      // 构建订单数据
      const orderData = {
        // 使用后端用户表的id，而不是直接用openid
        userId: wx.getStorageSync('userId') || 0,
        userName: this.data.address.name,
        userPhone: this.data.address.phone,
        address: this.data.address.detail,
        totalPrice: parseFloat(this.data.totalPrice),
        shippingFee: parseFloat(this.data.shippingFee),
        finalPrice: parseFloat(this.data.finalPrice),
        remark: this.data.remark || '',
        status: 'unpaid',
        items: this.data.orderItems.map(item => ({
          goodsId: item.id,
          goodsName: item.name,
          goodsImage: item.image,
          goodsPrice: parseFloat(item.price),
          count: item.count,
          specs: item.specs || '默认'
        }))
      }

      const result = await api.createOrder(orderData)

      // 如果是购物车结算，清空购物车
      if (this.data.type === 'cart') {
        const cart = wx.getStorageSync('cart') || []
        const checkedItems = cart.filter(item => item.checked)
        const newCart = cart.filter(item => !item.checked || !checkedItems.find(ci => ci.id === item.id))
        wx.setStorageSync('cart', newCart)
      }

      // 清空临时订单数据
      wx.removeStorageSync('orderItems')

      wx.hideLoading()
      wx.showToast({
        title: '订单提交成功',
        icon: 'success',
        duration: 2000
      })

      setTimeout(() => {
        wx.redirectTo({
          url: `/pages/order-detail/order-detail?id=${result.id}`
        })
      }, 2000)
    } catch (error) {
      console.error('提交订单失败', error)
      wx.hideLoading()
      wx.showToast({
        title: '提交失败',
        icon: 'none'
      })
    }
  }
})
