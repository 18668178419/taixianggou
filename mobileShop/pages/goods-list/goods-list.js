// pages/goods-list/goods-list.js
const api = require('../../utils/api.js')

Page({
  data: {
    keyword: '',
    categoryId: null,
    type: null,
    sortType: 'default',
    priceOrder: 'asc',
    goodsList: []
  },

  onLoad(options) {
    if (options.keyword) {
      
      this.setData({ keyword: options.keyword })
    }
    if (options.categoryId) {
      this.setData({ categoryId: parseInt(options.categoryId) })
    }
    if (options.type) {
      this.setData({ type: options.type })
    }
    this.loadGoods()
  },

  onSearchInput(e) {
    this.setData({
      keyword: e.detail.value
    })
  },

  onSearchConfirm() {
    this.loadGoods()
  },

  onSortTap(e) {
    const type = e.currentTarget.dataset.type
    if (type === 'price') {
      const priceOrder = this.data.priceOrder === 'asc' ? 'desc' : 'asc'
      this.setData({
        sortType: type,
        priceOrder
      })
    } else {
      this.setData({
        sortType: type
      })
    }
    this.loadGoods()
  },

  async loadGoods() {
    try {
      wx.showLoading({ title: '加载中...' })
      
      const params = {
        status: true,
        pageSize: 100
      }

      if (this.data.categoryId) {
        params.categoryId = this.data.categoryId
      }

      if (this.data.keyword) {
        params.keyword = this.data.keyword
      }

      if (this.data.type === 'recommend') {
        params.isRecommend = true
      } else if (this.data.type === 'hot') {
        params.isHot = true
      }

      const res = await api.getGoodsList(params)
      let goodsList = (res.list || []).map(item => ({
        ...item,
        originalPrice: item.originalPrice || null,
        tags: item.tags ? item.tags.split(',') : []
      }))

      // 客户端排序
      if (this.data.sortType === 'sales') {
        goodsList.sort((a, b) => b.sales - a.sales)
      } else if (this.data.sortType === 'price') {
        if (this.data.priceOrder === 'asc') {
          goodsList.sort((a, b) => a.price - b.price)
        } else {
          goodsList.sort((a, b) => b.price - a.price)
        }
      }

      this.setData({ goodsList })
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
