<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>分类管理</span>
        <el-button type="primary" @click="handleAdd">添加分类</el-button>
      </div>
    </template>

    <el-table :data="tableData" v-loading="loading" border>
      <el-table-column prop="Id" label="ID" width="80" />
      <el-table-column prop="Icon" label="图标" width="100" />
      <el-table-column prop="Name" label="分类名称" />
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
        <el-form-item label="分类名称" prop="name">
          <el-input v-model="form.name" />
        </el-form-item>
        <el-form-item label="图标">
          <el-input v-model="form.icon" placeholder="如：📱" />
        </el-form-item>
        <el-form-item label="图片">
          <el-input v-model="form.image" placeholder="图片URL" />
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
import { getCategoriesList, createCategory, updateCategory, deleteCategory } from '@/api/categories'

const loading = ref(false)
const tableData = ref([])
const dialogVisible = ref(false)
const dialogTitle = ref('添加分类')
const formRef = ref()
const editingId = ref(null)

const form = reactive({
  name: '',
  icon: '',
  image: '',
  sortOrder: 0,
  status: true
})

const rules = {
  name: [{ required: true, message: '请输入分类名称', trigger: 'blur' }]
}

const loadData = async () => {
  loading.value = true
  try {
    const res = await getCategoriesList()
    tableData.value = res.data
  } catch (error) {
    ElMessage.error('加载数据失败')
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  dialogTitle.value = '添加分类'
  editingId.value = null
  Object.assign(form, {
    name: '',
    icon: '',
    image: '',
    sortOrder: 0,
    status: true
  })
  dialogVisible.value = true
}

const handleEdit = (row) => {
  dialogTitle.value = '编辑分类'
  editingId.value = row.Id
  // 将列表行数据映射到表单字段（接口字段可能为大写）
  Object.assign(form, {
    name: row.Name ?? row.name ?? '',
    icon: row.Icon ?? row.icon ?? '',
    image: row.Image ?? row.image ?? '',
    sortOrder: row.SortOrder ?? row.sortOrder ?? 0,
    status: row.Status ?? row.status ?? true
  })
  dialogVisible.value = true
}

const handleSubmit = async () => {
  await formRef.value.validate()
  
  try {
    if (editingId.value) {
      await updateCategory(editingId.value, form)
      ElMessage.success('更新成功')
    } else {
      await createCategory(form)
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
    await ElMessageBox.confirm('确定要删除这个分类吗？', '提示', {
      type: 'warning'
    })
    await deleteCategory(row.Id)
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

