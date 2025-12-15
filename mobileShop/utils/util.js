// 工具函数
const formatTime = date => {
  const year = date.getFullYear()
  const month = date.getMonth() + 1
  const day = date.getDate()
  const hour = date.getHours()
  const minute = date.getMinutes()
  const second = date.getSeconds()

  return `${[year, month, day].map(formatNumber).join('/')} ${[hour, minute, second].map(formatNumber).join(':')}`
}

const formatNumber = n => {
  n = n.toString()
  return n[1] ? n : `0${n}`
}

// 格式化价格
const formatPrice = (price) => {
  return price.toFixed(2)
}

// 格式化销量
const formatSales = (sales) => {
  if (sales >= 10000) {
    return (sales / 10000).toFixed(1) + '万+'
  } else if (sales >= 1000) {
    return (sales / 1000).toFixed(1) + 'k+'
  }
  return sales
}

module.exports = {
  formatTime,
  formatPrice,
  formatSales
}

