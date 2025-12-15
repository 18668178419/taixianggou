// pages/cart/cart.js
Page({
  data: {
    cartList: [],
    allChecked: false,
    totalPrice: 0,
    selectedCount: 0
  },

  onLoad() {
    this.loadCart()
  },

  onShow() {
    this.loadCart()
  },

  loadCart() {
    const cart = wx.getStorageSync('cart') || []
    // 为每个商品添加checked属性
    const cartList = cart.map(item => ({
      ...item,
      checked: item.checked !== undefined ? item.checked : true
    }))
    
    this.setData({ cartList })
    this.calculateTotal()
  },

  onItemCheck(e) {
    const index = e.currentTarget.dataset.index
    const cartList = this.data.cartList
    cartList[index].checked = !cartList[index].checked
    
    this.setData({ cartList })
    this.saveCart()
    this.calculateTotal()
  },

  onAllCheck() {
    const allChecked = !this.data.allChecked
    const cartList = this.data.cartList.map(item => ({
      ...item,
      checked: allChecked
    }))
    
    this.setData({
      cartList,
      allChecked
    })
    this.saveCart()
    this.calculateTotal()
  },

  onCountDecrease(e) {
    const index = e.currentTarget.dataset.index
    const cartList = this.data.cartList
    
    if (cartList[index].count > 1) {
      cartList[index].count--
      this.setData({ cartList })
      this.saveCart()
      this.calculateTotal()
    }
  },

  onCountIncrease(e) {
    const index = e.currentTarget.dataset.index
    const cartList = this.data.cartList
    
    if (cartList[index].count < cartList[index].stock) {
      cartList[index].count++
      this.setData({ cartList })
      this.saveCart()
      this.calculateTotal()
    } else {
      wx.showToast({
        title: '库存不足',
        icon: 'none'
      })
    }
  },

  onItemDelete(e) {
    const index = e.currentTarget.dataset.index
    const cartList = this.data.cartList
    
    wx.showModal({
      title: '提示',
      content: '确定要删除这个商品吗？',
      success: (res) => {
        if (res.confirm) {
          cartList.splice(index, 1)
          this.setData({ cartList })
          this.saveCart()
          this.calculateTotal()
          this.updateCartCount()
        }
      }
    })
  },

  calculateTotal() {
    const cartList = this.data.cartList
    const checkedItems = cartList.filter(item => item.checked)
    const totalPrice = checkedItems.reduce((sum, item) => sum + item.price * item.count, 0)
    const selectedCount = checkedItems.reduce((sum, item) => sum + item.count, 0)
    const allChecked = cartList.length > 0 && cartList.every(item => item.checked)
    
    this.setData({
      totalPrice: totalPrice.toFixed(2),
      selectedCount,
      allChecked
    })
  },

  saveCart() {
    const cart = this.data.cartList.map(item => {
      const { checked, ...rest } = item
      return rest
    })
    wx.setStorageSync('cart', cart)
    this.updateCartCount()
  },

  updateCartCount() {
    const cart = wx.getStorageSync('cart') || []
    const count = cart.reduce((sum, item) => sum + item.count, 0)
    getApp().globalData.cartCount = count
  },

  onCheckout() {
    const checkedItems = this.data.cartList.filter(item => item.checked)
    if (checkedItems.length === 0) {
      wx.showToast({
        title: '请选择要结算的商品',
        icon: 'none'
      })
      return
    }

    const orderItems = checkedItems.map(item => {
      const { checked, ...rest } = item
      return rest
    })
    
    wx.setStorageSync('orderItems', orderItems)
    wx.navigateTo({
      url: '/pages/order/order?type=cart'
    })
  },

  onGoShopping() {
    wx.switchTab({
      url: '/pages/index/index'
    })
  }
})

