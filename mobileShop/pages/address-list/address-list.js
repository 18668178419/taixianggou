// pages/address-list/address-list.js
const api = require('../../utils/api.js')

Page({
  data: {
    list: [],
    loading: false,
    from: ''
  },

  onLoad(options) {
    this.setData({ from: options.from || '' })
  },

  onShow() {
    this.loadAddresses()
  },

  async loadAddresses() {
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
      const res = await api.getAddresses({ userId, pageSize: 50 })
      this.setData({ list: res.list || [] })
    } catch (error) {
      console.error('加载地址失败', error)
      wx.showToast({
        title: '加载失败',
        icon: 'none'
      })
    } finally {
      this.setData({ loading: false })
    }
  },

  onAddTap() {
    wx.navigateTo({
      url: '/pages/address-edit/address-edit'
    })
  },

  onEditTap(e) {
    const id = e.currentTarget.dataset.id
    wx.navigateTo({
      url: `/pages/address-edit/address-edit?id=${id}`
    })
  },

  async onSetDefaultTap(e) {
    const id = e.currentTarget.dataset.id
    try {
      await api.setDefaultAddress(id)
      wx.showToast({
        title: '设置成功',
        icon: 'success'
      })
      // 同步缓存给下单页使用
      this.syncDefaultToStorage()
      this.loadAddresses()
    } catch (error) {
      console.error('设置默认地址失败', error)
      wx.showToast({
        title: '设置失败',
        icon: 'none'
      })
    }
  },

  async syncDefaultToStorage() {
    const userId = wx.getStorageSync('userId') || 0
    if (!userId) return
    try {
      const address = await api.getDefaultAddress({ userId })
      if (address) {
        // order 页面只用到 name/phone/detail
        wx.setStorageSync('defaultAddress', {
          id: address.id,
          name: address.name,
          phone: address.phone,
          detail: `${address.province || ''}${address.city || ''}${address.district || ''}${address.detail || ''}`
        })
      }
    } catch (e) {
      console.error('同步默认地址失败', e)
    }
  },

  async onDeleteTap(e) {
    const id = e.currentTarget.dataset.id
    wx.showModal({
      title: '提示',
      content: '确定要删除这个地址吗？',
      success: async (res) => {
        if (res.confirm) {
          try {
            await api.deleteAddress(id)
            wx.showToast({
              title: '删除成功',
              icon: 'success'
            })
            this.loadAddresses()
          } catch (error) {
            console.error('删除地址失败', error)
            wx.showToast({
              title: '删除失败',
              icon: 'none'
            })
          }
        }
      }
    })
  },

  // 在下单页跳转过来时，点击整行即可选择地址并返回
  async onItemTap(e) {
    if (this.data.from !== 'order') return
    const item = e.currentTarget.dataset.item
    wx.setStorageSync('defaultAddress', {
      id: item.id,
      name: item.name,
      phone: item.phone,
      detail: `${item.province || ''}${item.city || ''}${item.district || ''}${item.detail || ''}`
    })
    wx.navigateBack()
  }
})


