// pages/profile/profile.js
const api = require('../../utils/api.js')

Page({
  data: {
    userInfo: {},
    orderCount: {
      all: 0,
      unpaid: 0,
      unshipped: 0,
      shipped: 0,
      completed: 0
    }
  },

  onLoad() {
    this.loadUserInfo()
    this.loadOrderCount()
  },

  onShow() {
    this.loadOrderCount()
  },

  loadUserInfo() {
     const userInfo = wx.getStorageSync('userInfo') || {}
   
    
    this.setData({ userInfo })
  },

  async onLoginTap() {
    // 如果已经登录，不重复登录
    if (this.data.userInfo.nickName) {
      return
    }

    try {
      // 获取用户信息
      const profileRes = await new Promise((resolve, reject) => {
        wx.getUserProfile({
          desc: '用于完善用户资料',
          success: resolve,
          fail: reject
        })
      })

      const userInfo = profileRes.userInfo
      
      // 获取登录code
      const loginRes = await new Promise((resolve, reject) => {
        wx.login({
          success: resolve,
          fail: reject
        })
      })

      if (loginRes.code) {
        // 调用后端登录接口
        const res = await api.login(loginRes.code, userInfo)
        
        // 保存用户信息
        wx.setStorageSync('userInfo', userInfo)
        wx.setStorageSync('userId', res.id)
        wx.setStorageSync('openid', res.openid)
        
        this.setData({ userInfo })
        
        wx.showToast({
          title: '登录成功',
          icon: 'success'
        })
      } else {
        throw new Error('获取登录code失败')
      }
    } catch (error) {
      console.error('登录失败', error)
      if (error.errMsg && error.errMsg.includes('getUserProfile:fail')) {
        wx.showToast({
          title: '用户取消授权',
          icon: 'none'
        })
      } else {
        wx.showToast({
          title: '登录失败，请重试',
          icon: 'none'
        })
      }
    }
  },

  getUserProfile() {
    wx.getUserProfile({
      desc: '用于完善用户资料',
      success: (res) => {
        const userInfo = res.userInfo
        console.log(userInfo)
        wx.setStorageSync('userInfo', userInfo)
        this.setData({ userInfo })
      },
      fail: () => {
        console.log('用户拒绝授权')
      }
    })
  },

  async loadOrderCount() {
    try {
      const [allRes, unpaidRes, unshippedRes, shippedRes, completedRes] = await Promise.all([
        api.getOrdersList({ pageSize: 1 }),
        api.getOrdersList({ status: 'unpaid', pageSize: 1 }),
        api.getOrdersList({ status: 'unshipped', pageSize: 1 }),
        api.getOrdersList({ status: 'shipped', pageSize: 1 }),
        api.getOrdersList({ status: 'completed', pageSize: 1 })
      ])
      
      const orderCount = {
        all: allRes.total || 0,
        unpaid: unpaidRes.total || 0,
        unshipped: unshippedRes.total || 0,
        shipped: shippedRes.total || 0,
        completed: completedRes.total || 0
      }
      this.setData({ orderCount })
    } catch (error) {
      console.error('加载订单统计失败', error)
    }
  },

  onSettingTap() {
    wx.showToast({
      title: '设置功能开发中',
      icon: 'none'
    })
  },

  onOrderTap(e) {
    const status = e.currentTarget.dataset.status
    wx.navigateTo({
      url: `/pages/order-list/order-list?status=${status}`
    })
  },

  onAddressTap() {
    wx.navigateTo({
      url: '/pages/address-list/address-list'
    })
  },

  onCouponTap() {
    wx.showToast({
      title: '优惠券功能开发中',
      icon: 'none'
    })
  },

  onFavoriteTap() {
    wx.navigateTo({
      url: '/pages/favorite-list/favorite-list'
    })
  },

  onServiceTap() {
    wx.showToast({
      title: '客服功能开发中',
      icon: 'none'
    })
  },

  onAboutTap() {
    wx.showModal({
      title: '关于泰享购',
      content: '泰享购 - 您身边的购物好伙伴\n版本：1.0.0',
      showCancel: false
    })
  }
})
