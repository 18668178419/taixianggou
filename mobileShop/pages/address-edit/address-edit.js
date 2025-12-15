// pages/address-edit/address-edit.js
const api = require('../../utils/api.js')

Page({
  data: {
    id: null,
    name: '',
    phone: '',
    province: '',
    city: '',
    district: '',
    detail: '',
    isDefault: false,
    region: [],
    regionText: ''
  },

  onLoad(options) {
    if (options.id) {
      this.setData({ id: Number(options.id) })
      this.loadDetail()
      wx.setNavigationBarTitle({ title: '编辑地址' })
    } else {
      wx.setNavigationBarTitle({ title: '新增地址' })
    }
  },

  async loadDetail() {
    try {
      const address = await api.getAddressById(this.data.id)
      const region = [
        address.province || '',
        address.city || '',
        address.district || ''
      ]
      this.setData({
        name: address.name || '',
        phone: address.phone || '',
        province: address.province || '',
        city: address.city || '',
        district: address.district || '',
        detail: address.detail || '',
        isDefault: !!address.isDefault,
        region,
        regionText: region.filter(Boolean).join(' ')
      })
    } catch (error) {
      console.error('加载地址详情失败', error)
      wx.showToast({
        title: '加载失败',
        icon: 'none'
      })
    }
  },

  onInputChange(e) {
    const field = e.currentTarget.dataset.field
    this.setData({
      [field]: e.detail.value
    })
  },

  onRegionChange(e) {
    const region = e.detail.value || []
    const regionText = region.filter(Boolean).join(' ')
    this.setData({
      region,
      regionText,
      province: region[0] || '',
      city: region[1] || '',
      district: region[2] || ''
    })
  },

  onDefaultChange(e) {
    this.setData({
      isDefault: e.detail.value
    })
  },

  async onSaveTap() {
    const userId = wx.getStorageSync('userId') || 0
    if (!userId) {
      wx.showToast({
        title: '请先登录',
        icon: 'none'
      })
      return
    }

    if (!this.data.name || !this.data.phone) {
      wx.showToast({
        title: '请填写完整信息',
        icon: 'none'
      })
      return
    }

    if (!this.data.province || !this.data.city || !this.data.district) {
      wx.showToast({
        title: '请选择省市区',
        icon: 'none'
      })
      return
    }

    if (!this.data.detail) {
      wx.showToast({
        title: '请输入详细地址',
        icon: 'none'
      })
      return
    }

    const data = {
      userId,
      name: this.data.name,
      phone: this.data.phone,
      province: this.data.province,
      city: this.data.city,
      district: this.data.district,
      detail: this.data.detail,
      isDefault: this.data.isDefault
    }

    try {
      wx.showLoading({ title: '保存中...' })
      if (this.data.id) {
        await api.updateAddress(this.data.id, data)
      } else {
        await api.createAddress(data)
      }
      wx.hideLoading()
      wx.showToast({
        title: '保存成功',
        icon: 'success'
      })
      setTimeout(() => {
        wx.navigateBack()
      }, 800)
    } catch (error) {
      wx.hideLoading()
      console.error('保存地址失败', error)
      wx.showToast({
        title: '保存失败',
        icon: 'none'
      })
    }
  }
})


