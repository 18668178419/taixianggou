<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>{{ isEdit ? '编辑商品' : '添加商品' }}</span>
      </div>
    </template>
    
    <el-form :model="form" :rules="rules" ref="formRef" label-width="100px">
      <el-form-item label="商品名称" prop="name">
        <el-input v-model="form.name" placeholder="请输入商品名称" />
      </el-form-item>
      <el-form-item label="分类" prop="categoryId">
        <el-select v-model="form.categoryId" placeholder="请选择分类">
          <el-option
            v-for="item in categories"
            :key="item.id"
            :label="item.name"
            :value="item.id"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="价格" prop="price">
        <el-input-number v-model="form.price" :precision="2" :min="0" />
      </el-form-item>
      <el-form-item label="原价">
        <el-input-number v-model="form.originalPrice" :precision="2" :min="0" />
      </el-form-item>
      <el-form-item label="库存" prop="stock">
        <el-input-number v-model="form.stock" :min="0" />
      </el-form-item>
      <el-form-item label="主图" prop="image">
        <el-input v-model="form.image" placeholder="图片URL" />
      </el-form-item>
      <el-form-item label="商品图片">
        <el-input v-model="form.images" type="textarea" :rows="3" placeholder="JSON数组格式，如：" />
      </el-form-item>
      <el-form-item label="描述">
        <el-input v-model="form.description" type="textarea" :rows="4" />
      </el-form-item>
      <el-form-item label="标签">
        <el-input v-model="form.tags" placeholder="多个标签用逗号分隔" />
      </el-form-item>
      <el-form-item label="是否推荐">
        <el-switch v-model="form.isRecommend" />
      </el-form-item>
      <el-form-item label="是否热门">
        <el-switch v-model="form.isHot" />
      </el-form-item>
      <el-form-item label="状态">
        <el-switch v-model="form.status" active-text="上架" inactive-text="下架" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleSubmit">保存</el-button>
        <el-button @click="$router.back()">取消</el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { getGoodsById, createGoods, updateGoods } from '@/api/goods'
import { getCategoriesList } from '@/api/categories'

const route = useRoute()
const router = useRouter()
const formRef = ref()
const isEdit = ref(false)
const categories = ref([])

const form = reactive({
  id: null,
  name: '',
  categoryId: null,
  price: 0,
  originalPrice: null,
  stock: 0,
  image: '',
  images: '',
  description: '',
  tags: '',
  isRecommend: false,
  isHot: false,
  status: true
})

const rules = {
  name: [{ required: true, message: '请输入商品名称', trigger: 'blur' }],
  categoryId: [{ required: true, message: '请选择分类', trigger: 'change' }],
  price: [{ required: true, message: '请输入价格', trigger: 'blur' }],
  stock: [{ required: true, message: '请输入库存', trigger: 'blur' }]
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

const loadData = async () => {
 
  if (route.params.id) {
    isEdit.value = true
    try {
    
      const res = await getGoodsById(route.params.id)

      // 兼容接口字段大小写
      Object.assign(form, {
        ...form,
        ...res.data,
        id: res.data.Id ?? res.data.id ?? null,
        name: res.data.Name ?? res.data.name ?? '',
        categoryId: res.data.CategoryId ?? res.data.categoryId ?? null,
        price: res.data.Price ?? res.data.price ?? 0,
        originalPrice: res.data.OriginalPrice ?? res.data.originalPrice ?? null,
        stock: res.data.Stock ?? res.data.stock ?? 0,
        image: res.data.Image ?? res.data.image ?? '',
        images: res.data.Images ?? res.data.images ?? '',
        description: res.data.Description ?? res.data.description ?? '',
        tags: res.data.Tags ?? res.data.tags ?? '',
        isRecommend: res.data.IsRecommend ?? res.data.isRecommend ?? false,
        isHot: res.data.IsHot ?? res.data.isHot ?? false,
        status: res.data.Status ?? res.data.status ?? true
      })

      if (form.images) {
        try {
          form.images = JSON.stringify(JSON.parse(form.images), null, 2)
        } catch {
          // 保持原样
        }
      }
    } catch (error) {
      ElMessage.error('加载数据失败')
      router.back()
    }
  }
}

const handleSubmit = async () => {
  await formRef.value.validate()
  
  try {
    const submitData = { ...form }
    if (submitData.images) {
      try {
        submitData.images = JSON.stringify(JSON.parse(submitData.images))
      } catch {
        // 保持原样
      }
    }

    // 构建兼容后端的首字母大写字段
    const payload = {
      ...submitData,
      Id: submitData.id ?? route.params.id,
      Name: submitData.name,
      CategoryId: submitData.categoryId,
      Price: submitData.price,
      OriginalPrice: submitData.originalPrice,
      Stock: submitData.stock,
      Image: submitData.image,
      Images: submitData.images,
      Description: submitData.description,
      Tags: submitData.tags,
      IsRecommend: submitData.isRecommend,
      IsHot: submitData.isHot,
      Status: submitData.status
    }

    if (isEdit.value) {
      await updateGoods(route.params.id, payload)
      ElMessage.success('更新成功')
    } else {
      await createGoods(payload)
      ElMessage.success('创建成功')
    }
    router.back()
  } catch (error) {
    ElMessage.error(isEdit.value ? '更新失败' : '创建失败')
  }
}

onMounted(() => {
  loadCategories()
  loadData()
})
</script>

