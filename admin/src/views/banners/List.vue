<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>轮播图管理</span>
        <el-button type="primary" @click="handleAdd">添加轮播图</el-button>
      </div>
    </template>

    <el-table :data="tableData" v-loading="loading" border>
      <el-table-column prop="Id" label="ID" width="80" />
      <el-table-column prop="Image" label="图片" width="150">
        <template #default="{ row }">
          <el-image :src="row.Image" style="width: 120px; height: 60px" fit="cover" />
        </template>
      </el-table-column>
      <el-table-column prop="Title" label="标题" />
      <el-table-column prop="SortOrder" label="排序" width="100" />
      <el-table-column prop="Status" label="状态" width="100">
        <template #default="{ row }">
          <el-tag :type="row.Status ? 'success' : 'danger'">
            {{ row.Status ? '启用' : '禁用' }}
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

    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="500px">
      <el-form :model="form" :rules="rules" ref="formRef" label-width="100px">
        <el-form-item label="标题">
          <el-input v-model="form.title" />
        </el-form-item>
        <el-form-item label="图片" prop="image">
          <el-input v-model="form.image" placeholder="图片URL" />
        </el-form-item>
        <el-form-item label="链接">
          <el-input v-model="form.link" placeholder="跳转链接" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="form.sortOrder" :min="0" />
        </el-form-item>
        <el-form-item label="状态">
          <el-switch v-model="form.status" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
      </template>
    </el-dialog>
  </el-card>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getBannersList, createBanner, updateBanner, deleteBanner } from '@/api/banners'

const loading = ref(false)
const tableData = ref([])
const dialogVisible = ref(false)
const dialogTitle = ref('添加轮播图')
const formRef = ref()
const editingId = ref(null)

const form = reactive({
  title: '',
  image: '',
  link: '',
  sortOrder: 0,
  status: true
})

const rules = {
  image: [{ required: true, message: '请输入图片URL', trigger: 'blur' }]
}

const loadData = async () => {
  loading.value = true
  try {
    const res = await getBannersList()
    tableData.value = res.data
  } catch (error) {
    ElMessage.error('加载数据失败')
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  dialogTitle.value = '添加轮播图'
  editingId.value = null
  Object.assign(form, {
    title: '',
    image: '',
    link: '',
    sortOrder: 0,
    status: true
  })
  dialogVisible.value = true
}

const handleEdit = (row) => {
  dialogTitle.value = '编辑轮播图'
  editingId.value = row.Id
  // 将列表行数据映射到表单字段（接口字段可能为大写）
  Object.assign(form, {
    title: row.Title ?? row.title ?? '',
    image: row.Image ?? row.image ?? '',
    link: row.Link ?? row.link ?? '',
    sortOrder: row.SortOrder ?? row.sortOrder ?? 0,
    status: row.Status ?? row.status ?? true
  })
  dialogVisible.value = true
}

const handleSubmit = async () => {
  await formRef.value.validate()

  try {
    if (editingId.value) {
      await updateBanner(editingId.value, form)
      ElMessage.success('更新成功')
    } else {
      await createBanner(form)
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    loadData()
  } catch (error) {
    ElMessage.error(editingId.value ? '更新失败' : '创建失败')
  }
}

const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('确定要删除这个轮播图吗？', '提示', {
      type: 'warning'
    })
    await deleteBanner(row.Id)
    ElMessage.success('删除成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

onMounted(() => {
  loadData()
})
</script>
