<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>订单管理</span>
      </div>
    </template>
    
    <el-form :inline="true" :model="searchForm" class="search-form">
      <el-form-item label="订单号">
        <el-input v-model="searchForm.orderNo" placeholder="订单号" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="searchForm.status" placeholder="请选择" clearable>
          <el-option label="待付款" value="unpaid" />
          <el-option label="待发货" value="unshipped" />
          <el-option label="待收货" value="shipped" />
          <el-option label="已完成" value="completed" />
          <el-option label="已取消" value="cancelled" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="loadData">查询</el-button>
        <el-button @click="resetSearch">重置</el-button>
      </el-form-item>
    </el-form>

    <el-table :data="tableData" v-loading="loading" border>
      <el-table-column prop="OrderNo" label="订单号" width="200" />
      <el-table-column prop="UserName" label="用户" width="120" />
      <el-table-column prop="FinalPrice" label="金额" width="120">
        <template #default="{ row }">¥{{ row.FinalPrice }}</template>
      </el-table-column>
      <el-table-column prop="Status" label="状态" width="120">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.Status)">
            {{ getStatusText(row.Status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="CreateTime" label="创建时间" width="180" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleDetail(row)">详情</el-button>
          <el-button 
            v-if="row.Status === 'unshipped'" 
            type="success" 
            size="small" 
            @click="handleShip(row)"
          >
            发货
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination
      v-model:current-page="pagination.page"
      v-model:page-size="pagination.pageSize"
      :total="pagination.total"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="loadData"
      @current-change="loadData"
      style="margin-top: 20px"
    />
  </el-card>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getOrdersList, shipOrder } from '@/api/orders'

const router = useRouter()
const loading = ref(false)
const tableData = ref([])

const searchForm = reactive({
  orderNo: '',
  status: ''
})

const pagination = reactive({
  page: 1,
  pageSize: 20,
  total: 0
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
    const params = {
      page: pagination.page,
      pageSize: pagination.pageSize,
      orderNo: searchForm.orderNo || undefined,
      status: searchForm.status || undefined
    }
    const res = await getOrdersList(params)
    tableData.value = res.data.list
    pagination.total = res.data.total
  } catch (error) {
    ElMessage.error('加载数据失败')
  } finally {
    loading.value = false
  }
}

const resetSearch = () => {
  searchForm.orderNo = ''
  searchForm.status = ''
  pagination.page = 1
  loadData()
}

const handleDetail = (row) => {
  router.push(`/orders/detail/${row.Id}`)
}

const handleShip = async (row) => {
  try {
    await ElMessageBox.confirm('确定要发货这个订单吗？', '提示', {
      type: 'warning'
    })
    await shipOrder(row.Id)
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

<style scoped>
.search-form {
  margin-bottom: 20px;
}
</style>

