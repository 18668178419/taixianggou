<template>
  <el-card v-loading="loading">
    <template #header>
      <div class="card-header">
        <span>订单详情</span>
        <el-button @click="$router.back()">返回</el-button>
      </div>
    </template>

    <el-descriptions :column="2" border>
      <el-descriptions-item label="订单号">{{ order.OrderNo }}</el-descriptions-item>
      <el-descriptions-item label="状态">
        <el-tag :type="getStatusType(order.Status)">
          {{ getStatusText(order.Status) }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="用户姓名">{{ order.UserName }}</el-descriptions-item>
      <el-descriptions-item label="用户电话">{{ order.UserPhone }}</el-descriptions-item>
      <el-descriptions-item label="收货地址" :span="2">{{ order.Address }}</el-descriptions-item>
      <el-descriptions-item label="商品总价">¥{{ order.TotalPrice }}</el-descriptions-item>
      <el-descriptions-item label="运费">¥{{ order.ShippingFee }}</el-descriptions-item>
      <el-descriptions-item label="实付金额">¥{{ order.FinalPrice }}</el-descriptions-item>
      <el-descriptions-item label="备注" :span="2">{{ order.Remark || '-' }}</el-descriptions-item>
      <el-descriptions-item label="创建时间">{{ order.CreateTime }}</el-descriptions-item>
      <el-descriptions-item label="支付时间">{{ order.PayTime || '-' }}</el-descriptions-item>
      <el-descriptions-item label="发货时间">{{ order.ShipTime || '-' }}</el-descriptions-item>
      <el-descriptions-item label="收货时间">{{ order.ReceiveTime || '-' }}</el-descriptions-item>
    </el-descriptions>

    <el-divider>商品明细</el-divider>
    <el-table :data="order.Items" border>
      <el-table-column prop="GoodsName" label="商品名称" />
      <el-table-column prop="GoodsImage" label="图片" width="100">
        <template #default="{ row }">
          <el-image :src="row.GoodsImage" style="width: 60px; height: 60px" fit="cover" />
        </template>
      </el-table-column>
      <el-table-column prop="Specs" label="规格" />
      <el-table-column prop="GoodsPrice" label="单价">
        <template #default="{ row }">¥{{ row.GoodsPrice }}</template>
      </el-table-column>
      <el-table-column prop="Count" label="数量" />
      <el-table-column label="小计">
        <template #default="{ row }">¥{{ (row.GoodsPrice * row.Count).toFixed(2) }}</template>
      </el-table-column>
    </el-table>

    <div style="margin-top: 20px" v-if="order.Status === 'unshipped'">
      <el-button type="success" @click="handleShip">发货</el-button>
    </div>
  </el-card>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getOrderById, shipOrder } from '@/api/orders'

const route = useRoute()
const router = useRouter()
const loading = ref(false)
const order = reactive({
  OrderNo: '',
  Status: '',
  UserName: '',
  UserPhone: '',
  Address: '',
  TotalPrice: 0,
  ShippingFee: 0,
  FinalPrice: 0,
  Remark: '',
  CreateTime: '',
  PayTime: '',
  ShipTime: '',
  ReceiveTime: '',
  Items: []
})

const getStatusText = (status) => {
  const map = {
    unpaid: '待付款',
    unshipped: '待发货',
    shipped: '待收货',
    completed: '已完成',
    cancelled: '已取消'
  }
  return map[status] || status
}

const getStatusType = (status) => {
  const map = {
    unpaid: 'warning',
    unshipped: 'info',
    shipped: 'primary',
    completed: 'success',
    cancelled: 'danger'
  }
  return map[status] || ''
}

const loadData = async () => {
  loading.value = true
  try {
    const res = await getOrderById(route.params.id)
    Object.assign(order, res.data)
  } catch (error) {
    ElMessage.error('加载数据失败')
    router.back()
  } finally {
    loading.value = false
  }
}

const handleShip = async () => {
  try {
    await ElMessageBox.confirm('确定要发货这个订单吗？', '提示', {
      type: 'warning'
    })
    await shipOrder(order.Id)
    ElMessage.success('发货成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('发货失败')
    }
  }
}

onMounted(() => {
  loadData()
})
</script>

