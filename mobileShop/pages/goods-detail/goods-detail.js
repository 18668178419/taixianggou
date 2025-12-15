// pages/goods-detail/goods-detail.js
const api = require('../../utils/api.js')

Page({
  data: {
    goodsId: null,
    goods: {},
    selectedSpecs: {},
    selectedSpecsText: '',
    count: 1,
    showSpecModal: false,
    cartCount: 0
  },

  onLoad(options) {
    const id = parseInt(options.id)
    this.setData({ goodsId: id })
    this.loadGoods()
    this.updateCartCount()
  },

  onShow() {
    this.updateCartCount()
  },

  async loadGoods() {
    try {
      wx.showLoading({ title: '加载中...' })
      const goods = await api.getGoodsById(this.data.goodsId)
      if (goods) {
        // 处理JSON字段
        if (goods.images && typeof goods.images === 'string') {
          try {
            goods.images = JSON.parse(goods.images)
          } catch {
            goods.images = []
          }
        }
        if (goods.specs && typeof goods.specs === 'string') {
          try {
            goods.specs = JSON.parse(goods.specs)
          } catch {
            goods.specs = []
          }
        }
        if (goods.tags) {
          goods.tags = goods.tags.split(',')
        }
        goods.originalPrice = goods.originalPrice || null
        this.setData({ goods })
      }
    } catch (error) {
      console.error('加载商品失败', error)
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

  updateCartCount() {
    const cart = wx.getStorageSync('cart') || []
    const count = cart.reduce((sum, item) => sum + item.count, 0)
    this.setData({ cartCount: count })
    // 更新全局购物车数量
    getApp().globalData.cartCount = count
  },

  onSpecTap() {
    this.setData({ showSpecModal: true })
  },

  onSpecModalClose() {
    this.setData({ showSpecModal: false })
  },

  stopPropagation() {
    // 阻止事件冒泡
  },

  onSpecOptionTap(e) {
    const group = e.currentTarget.dataset.group
    const value = e.currentTarget.dataset.value
    const selectedSpecs = { ...this.data.selectedSpecs }
    selectedSpecs[group] = value
    
    // 生成规格文本
    const selectedSpecsText = Object.values(selectedSpecs).join(' / ')

    this.setData({
      selectedSpecs,
      selectedSpecsText
    })
  },

  onCountDecrease() {
    if (this.data.count > 1) {
      this.setData({
        count: this.data.count - 1
      })
    }
  },

  onCountIncrease() {
    if (this.data.count < this.data.goods.stock) {
      this.setData({
        count: this.data.count + 1
      })
    } else {
      wx.showToast({
        title: '库存不足',
        icon: 'none'
      })
    }
  },

  onCartTap() {
    wx.switchTab({
      url: '/pages/cart/cart'
    })
  },

  onAddCartTap() {
    // 检查是否选择了规格
    if (this.data.goods.specs && this.data.goods.specs.length > 0) {
      const allSpecsSelected = this.data.goods.specs.every(spec => 
        this.data.selectedSpecs[spec.name]
      )
      if (!allSpecsSelected) {
        this.setData({ showSpecModal: true })
        return
      }
    }
    this.addToCart()
  },

  onBuyNowTap() {
    // 检查是否选择了规格
    if (this.data.goods.specs && this.data.goods.specs.length > 0) {
      const allSpecsSelected = this.data.goods.specs.every(spec => 
        this.data.selectedSpecs[spec.name]
      )
      if (!allSpecsSelected) {
        this.setData({ showSpecModal: true })
        return
      }
    }
    this.buyNow()
  },

  onConfirmAddCart() {
    this.addToCart()
    this.setData({ showSpecModal: false })
  },

  onConfirmBuyNow() {
    this.buyNow()
    this.setData({ showSpecModal: false })
  },

  addToCart() {
    const cart = wx.getStorageSync('cart') || []
    const cartItem = {
      id: this.data.goods.id,
      goodsId: this.data.goods.id,
      name: this.data.goods.name,
      image: this.data.goods.image,
      price: this.data.goods.price,
      count: this.data.count,
      specs: this.data.selectedSpecsText || '默认',
      stock: this.data.goods.stock
    }

    // 检查购物车中是否已有相同商品
    const existingIndex = cart.findIndex(item => 
      item.goodsId === cartItem.goodsId && item.specs === cartItem.specs
    )

    if (existingIndex !== -1) {
      cart[existingIndex].count += cartItem.count
    } else {
      cart.push(cartItem)
    }

    wx.setStorageSync('cart', cart)
    this.updateCartCount()

    wx.showToast({
      title: '已加入购物车',
      icon: 'success'
    })
  },

  buyNow() {
    const orderItem = {
      id: this.data.goods.id,
      name: this.data.goods.name,
      image: this.data.goods.image,
      price: this.data.goods.price,
      count: this.data.count,
      specs: this.data.selectedSpecsText || '默认'
    }

    wx.setStorageSync('orderItems', [orderItem])
    wx.navigateTo({
      url: '/pages/order/order?type=buyNow'
    })
  }
})

