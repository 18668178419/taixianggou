<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>商品管理</span>
        <el-button type="primary" @click="$router.push('/goods/create')">添加商品</el-button>
      </div>
    </template>
    
    <el-form :inline="true" :model="searchForm" class="search-form">
      <el-form-item label="关键词">
        <el-input v-model="searchForm.keyword" placeholder="商品名称" clearable />
      </el-form-item>
      <el-form-item label="分类">
        <el-select v-model="searchForm.categoryId" placeholder="请选择" style="width: 150px;" clearable>
          <el-option
            v-for="item in categories"
            :key="item.id"
            :label="item.name"
            :value="item.id"
          />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="loadData">查询</el-button>
        <el-button @click="resetSearch">重置</el-button>
      </el-form-item>
    </el-form>

    <el-table :data="tableData" v-loading="loading" border>
      <el-table-column prop="Id" label="ID" align="center" width="80" />
      <el-table-column prop="Image" label="图片" align="center" width="100">
        <template #default="{ row }">
          <el-image :src="row.Image" style="width: 60px; height: 60px" fit="cover" />
        </template>
      </el-table-column>
      <el-table-column prop="Name" label="商品名称" min-width="200" />
      <el-table-column prop="CategoryId" label="分类ID" width="100" />
      <el-table-column prop="Price" label="价格" width="100">
        <template #default="{ row }">¥{{ row.Price }}</template>
      </el-table-column>
      <el-table-column prop="Stock" label="库存" width="100" />
      <el-table-column prop="Sales" label="销量" width="100" />
      <el-table-column prop="Status" label="状态" width="100">
        <template #default="{ row }">
          <el-tag :type="row.Status ? 'success' : 'danger'">
            {{ row.Status ? '上架' : '下架' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">编辑</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">删除</el-button>
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
import { ElMessage, ElMessageBox } from 'element-plus'
import { useRouter } from 'vue-router'
import { getGoodsList, deleteGoods } from '@/api/goods'
import { getCategoriesList } from '@/api/categories'

const router = useRouter()
const loading = ref(false)
const tableData = ref([])
const categories = ref([])

const searchForm = reactive({
  keyword: '',
  categoryId: null
})

const pagination = reactive({
  page: 1,
  pageSize: 20,
  total: 0
})

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      page: pagination.page,
      pageSize: pagination.pageSize,
      keyword: searchForm.keyword || undefined,
      categoryId: searchForm.categoryId || undefined
    }
    const res = await getGoodsList(params)
    tableData.value = res.data.list
    pagination.total = res.data.total
  } catch (error) {
    ElMessage.error('加载数据失败')
  } finally {
    loading.value = false
  }
}

const loadCategories = async () => {
  try {
    const res = await getCategoriesList()
    // 兼容接口字段大小写，统一成小写键给选择器使用
    categories.value = (res.data || []).map((item) => ({
      id: item.Id ?? item.id,
      name: item.Name ?? item.name,
      ...item
    }))
  } catch (error) {
    console.error('加载分类失败', error)
  }
}

const resetSearch = () => {
  searchForm.keyword = ''
  searchForm.categoryId = null
  pagination.page = 1
  loadData()
}

const handleEdit = (row) => {
  router.push(`/goods/edit/${row.Id}`)
}

const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('确定要删除这个商品吗？', '提示', {
      type: 'warning'
    })
    await deleteGoods(row.Id)
    ElMessage.success('删除成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

onMounted(() => {
  loadCategories()
  loadData()
})
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-form {
  margin-bottom: 20px;
}
</style>

