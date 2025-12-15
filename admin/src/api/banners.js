import api from './index'

export const getBannersList = (params) => {
  return api.get('/banners', { params })
}

export const getBannerById = (id) => {
  return api.get(`/banners/${id}`)
}

export const createBanner = (data) => {
  return api.post('/banners', data)
}

export const updateBanner = (id, data) => {
  return api.put(`/banners/${id}`, data)
}

export const deleteBanner = (id) => {
  return api.delete(`/banners/${id}`)
}

