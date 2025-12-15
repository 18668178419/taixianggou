import api from './index'

export const getOrdersList = (params) => {
  return api.get('/orders', { params })
}

export const getOrderById = (id) => {
  return api.get(`/orders/${id}`)
}

export const shipOrder = (id) => {
  return api.put(`/orders/${id}/ship`)
}

export const deleteOrder = (id) => {
  return api.delete(`/orders/${id}`)
}

