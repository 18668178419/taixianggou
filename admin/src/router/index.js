import { createRouter, createWebHistory } from 'vue-router'
import Layout from '@/layout/index.vue'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue')
  },
  {
    path: '/',
    component: Layout,
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/Dashboard.vue'),
        meta: { title: '首页' }
      }
    ]
  },
  {
    path: '/goods',
    component: Layout,
    children: [
      {
        path: '',
        name: 'GoodsList',
        component: () => import('@/views/goods/List.vue'),
        meta: { title: '商品管理' }
      },
      {
        path: 'create',
        name: 'GoodsCreate',
        component: () => import('@/views/goods/Form.vue'),
        meta: { title: '添加商品' }
      },
      {
        path: 'edit/:id',
        name: 'GoodsEdit',
        component: () => import('@/views/goods/Form.vue'),
        meta: { title: '编辑商品' }
      }
    ]
  },
  {
    path: '/categories',
    component: Layout,
    children: [
      {
        path: '',
        name: 'CategoriesList',
        component: () => import('@/views/categories/List.vue'),
        meta: { title: '分类管理' }
      }
    ]
  },
  {
    path: '/banners',
    component: Layout,
    children: [
      {
        path: '',
        name: 'BannersList',
        component: () => import('@/views/banners/List.vue'),
        meta: { title: '轮播图管理' }
      }
    ]
  },
  {
    path: '/orders',
    component: Layout,
    children: [
      {
        path: '',
        name: 'OrdersList',
        component: () => import('@/views/orders/List.vue'),
        meta: { title: '订单管理' }
      },
      {
        path: 'detail/:id',
        name: 'OrderDetail',
        component: () => import('@/views/orders/Detail.vue'),
        meta: { title: '订单详情' }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 路由守卫
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.path === '/login') {
    next()
  } else if (!token && to.path !== '/login') {
    next('/login')
  } else {
    next()
  }
})

export default router

