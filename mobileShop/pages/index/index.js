// pages/index/index.js
const api = require('../../utils/api.js')
const util = require('../../utils/util.js')

Page({
  data: {
    banners: [],
    categories: [],
    recommendGoods: [],
    goodsList: []
  },

  onLoad() {
    this.loadData()
   
  },

  onPullDownRefresh() {
    this.loadData(() => {
      wx.stopPullDownRefresh()
    })
  },

  async loadData(callback) {
    try {
      wx.showLoading({ title: '加载中...' })
      
      // 加载轮播图
      const banners = await api.getBanners(true)
      
      // 加载分类
      const categories = await api.getCategories(true)
     
      // 加载推荐商品
      const recommendRes = await api.getGoodsList({ isRecommend: true, pageSize: 4 })
      const recommendGoods = recommendRes.list || []
      // 加载商品列表
      const goodsRes = await api.getGoodsList({ status: true, pageSize: 20 })
      const goodsList = goodsRes.list || []
  
      this.setData({
        banners: banners || [],
        categories: categories || [],
        recommendGoods: recommendGoods.map(item => ({
          ...item,
          originalPrice: item.originalPrice || null,
          tags: item.tags ? item.tags.split(',') : [],
          images: item.images ? (typeof item.images === 'string' ? JSON.parse(item.images) : item.images) : []
        })),
        goodsList: goodsList.map(item => ({
          ...item,
          originalPrice: item.originalPrice || null,
          tags: item.tags ? item.tags.split(',') : [],
          images: item.images ? (typeof item.images === 'string' ? JSON.parse(item.images) : item.images) : []
        }))
      })

      if (callback) callback()
    } catch (error) {
      console.error('加载数据失败', error)
      wx.showToast({
        title: '加载失败',
        icon: 'none'
      })
      if (callback) callback()
    } finally {
      wx.hideLoading()
    }
  },

  onSearchTap() {
    wx.navigateTo({
      url: '/pages/goods-list/goods-list?keyword='
    })
  },

  onBannerTap(e) {
    const id = e.currentTarget.dataset.id
   
  },

  onCategoryTap(e) {
    const id = e.currentTarget.dataset.id
    wx.navigateTo({
      url: `/pages/goods-list/goods-list?categoryId=${id}`
    })
  },

  onMoreTap(e) {
    const type = e.currentTarget.dataset.type
    wx.navigateTo({
      url: `/pages/goods-list/goods-list?type=${type}`
    })
  },

  onGoodsTap(e) {
    const id = e.currentTarget.dataset.id
    wx.navigateTo({
      url: `/pages/goods-detail/goods-detail?id=${id}`
    })
  }
})
