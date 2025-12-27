// app.js
const api = require('./utils/api.js')

App({
  async onLaunch() {
    // 展示本地存储能力
    const logs = wx.getStorageSync('logs') || []
    logs.unshift(Date.now())
    wx.setStorageSync('logs', logs)
   
    // 自动登录（静默，只拿openid和用户ID）
    try {
      const loginRes = await new Promise((resolve, reject) => {
        wx.login({
          success: resolve,
          fail: reject
        })
      })

      if (loginRes.code) {
        
        const res = await api.login(loginRes.code, null)
       
        // 后端返回：id, openid, nickName, avatarUrl...
        wx.setStorageSync('userId', res.id)
        wx.setStorageSync('openid', res.openid)
      }
    } catch (error) {
      console.error('自动登录失败', error)
    }
  },
  globalData: {
    userInfo: null,
    cartCount: 0
  }
})

