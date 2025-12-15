// pages/category/category.js
const api = require('../../utils/api.js')

Page({
  data: {
    categories: [],
    currentCategoryId: null,
    currentCategory: null,
    goodsList: []
  },

  onLoad() {
    this.loadCategories()
  },

  async loadCategories() {
    try {
      wx.showLoading({ title: '加载中...' })
      const categories = await api.getCategories(true)
      this.setData({
        categories: categories || [],
        currentCategoryId: categories && categories.length > 0 ? categories[0].id : null
      })
      this.loadGoods()
    } catch (error) {
      console.error('加载分类失败', error)
      wx.showToast({
        title: '加载失败',
        icon: 'none'
      })
    } finally {
      wx.hideLoading()
    }
  },

  onCategorySelect(e) {
    const id = e.currentTarget.dataset.id
    this.setData({
      currentCategoryId: id
    })
    this.loadGoods()
  },

  async loadGoods() {
    if (!this.data.currentCategoryId) return
    
    try {
      wx.showLoading({ title: '加载中...' })
      const categoryId = this.data.currentCategoryId
      const currentCategory = this.data.categories.find(item => item.id === categoryId)
      const res = await api.getGoodsList({ categoryId, status: true, pageSize: 100 })
      const goodsList = (res.list || []).map(item => ({
        ...item,
        originalPrice: item.originalPrice || null,
        tags: item.tags ? item.tags.split(',') : []
      }))

      this.setData({
        currentCategory,
        goodsList
      })
    } catch (error) {
      console.error('加载商品失败', error)
      wx.showToast({
        title: '加载失败',
        icon: 'none'
      })
    } finally {
      wx.hideLoading()
    }
  },

  onGoodsTap(e) {
    const id = e.currentTarget.dataset.id
    wx.navigateTo({
      url: `/pages/goods-detail/goods-detail?id=${id}`
    })
  }
})

