import api from './index'

export const getGoodsList = (params) => {
  return api.get('/goods', { params })
}

export const getGoodsById = (id) => {
  return api.get(`/goods/${id}`)
}

export const createGoods = (data) => {
  return api.post('/goods', data)
}

export const updateGoods = (id, data) => {
  return api.put(`/goods/${id}`, data)
}

export const deleteGoods = (id) => {
  return api.delete(`/goods/${id}`)
}

