// pages/favorite-list/favorite-list.js
const api = require('../../utils/api.js')

Page({
  data: {
    list: [],
    loading: false
  },

  onShow() {
    this.loadFavorites()
  },

  async loadFavorites() {
    const userId = wx.getStorageSync('userId') || 0
    if (!userId) {
      wx.showToast({
        title: '请先登录',
        icon: 'none'
      })
      return
    }

    try {
      this.setData({ loading: true })
      const res = await api.getFavorites({ userId, pageSize: 100 })
      const list = (res.list || []).map(item => ({
        ...item,
        goodsPrice: Number(item.goodsPrice).toFixed(2)
      }))
      this.setData({ list })
    } catch (error) {
      console.error('加载收藏失败', error)
      wx.showToast({
        title: '加载失败',
        icon: 'none'
      })
    } finally {
      this.setData({ loading: false })
    }
  },

  onGoodsTap(e) {
    const goodsId = e.currentTarget.dataset.goodsId
    wx.navigateTo({
      url: `/pages/goods-detail/goods-detail?id=${goodsId}`
    })
  },

  async onUnfavoriteTap(e) {
    const id = e.currentTarget.dataset.id
    wx.showModal({
      title: '提示',
      content: '确定要取消收藏吗？',
      success: async (res) => {
        if (res.confirm) {
          try {
            await api.removeFavoriteById(id)
            wx.showToast({
              title: '已取消收藏',
              icon: 'success'
            })
            this.loadFavorites()
          } catch (error) {
            console.error('取消收藏失败', error)
            wx.showToast({
              title: '取消失败',
              icon: 'none'
            })
          }
        }
      }
    })
  }
})


