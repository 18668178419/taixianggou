<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>数据概览</span>
      </div>
    </template>
    <el-row :gutter="20">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-value">{{ stats.goodsCount }}</div>
          <div class="stat-label">商品总数</div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-value">{{ stats.orderCount }}</div>
          <div class="stat-label">订单总数</div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-value">{{ stats.unpaidCount }}</div>
          <div class="stat-label">待付款订单</div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-value">{{ stats.unshippedCount }}</div>
          <div class="stat-label">待发货订单</div>
        </el-card>
      </el-col>
    </el-row>
  </el-card>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getGoodsList } from '@/api/goods'
import { getOrdersList } from '@/api/orders'

const stats = ref({
  goodsCount: 0,
  orderCount: 0,
  unpaidCount: 0,
  unshippedCount: 0
})

const loadStats = async () => {
  try {
    const [goodsRes, ordersRes] = await Promise.all([
      getGoodsList({ pageSize: 1 }),
      getOrdersList({ pageSize: 1 })
    ])
    
    stats.value.goodsCount = goodsRes.data?.total || 0
    stats.value.orderCount = ordersRes.data?.total || 0
    
    const unpaidRes = await getOrdersList({ status: 'unpaid', pageSize: 1 })
    const unshippedRes = await getOrdersList({ status: 'unshipped', pageSize: 1 })
    
    stats.value.unpaidCount = unpaidRes.data?.total || 0
    stats.value.unshippedCount = unshippedRes.data?.total || 0
  } catch (error) {
    console.error('加载统计数据失败', error)
  }
}

onMounted(() => {
  loadStats()
})
</script>

<style scoped>
.stat-card {
  text-align: center;
}

.stat-value {
  font-size: 32px;
  font-weight: bold;
  color: #409EFF;
  margin-bottom: 10px;
}

.stat-label {
  font-size: 14px;
  color: #666;
}
</style>

