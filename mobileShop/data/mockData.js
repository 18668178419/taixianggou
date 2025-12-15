// 模拟数据
const mockData = {
  // 轮播图数据
  banners: [
    {
      id: 1,
      image: 'https://images.unsplash.com/photo-1441986300917-64674bd600d8?w=800',
      title: '春季新品上市',
      link: ''
    },
    {
      id: 2,
      image: 'https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=800',
      title: '限时特惠活动',
      link: ''
    },
    {
      id: 3,
      image: 'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800',
      title: '品牌大促销',
      link: ''
    }
  ],

  // 分类数据
  categories: [
    { id: 1, name: '手机数码', icon: '📱', image: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=200' },
    { id: 2, name: '电脑办公', icon: '💻', image: 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=200' },
    { id: 3, name: '家用电器', icon: '🏠', image: 'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=200' },
    { id: 4, name: '服装鞋帽', icon: '👔', image: 'https://images.unsplash.com/photo-1445205170230-053b83016050?w=200' },
    { id: 5, name: '美妆护肤', icon: '💄', image: 'https://images.unsplash.com/photo-1522338242472-255a1f213b8f?w=200' },
    { id: 6, name: '食品生鲜', icon: '🍎', image: 'https://images.unsplash.com/photo-1542838132-92c53300491e?w=200' },
    { id: 7, name: '运动户外', icon: '⚽', image: 'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=200' },
    { id: 8, name: '母婴玩具', icon: '🧸', image: 'https://images.unsplash.com/photo-1555252333-9f8e92e65df9?w=200' }
  ],

  // 商品数据
  goods: [
    {
      id: 1,
      name: 'iPhone 15 Pro Max 256GB 深空黑色',
      price: 8999,
      originalPrice: 9999,
      image: 'https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=400',
      images: [
        'https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=800',
        'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=800',
        'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=800'
      ],
      categoryId: 1,
      categoryName: '手机数码',
      sales: 1234,
      stock: 100,
      description: '全新iPhone 15 Pro Max，采用钛金属设计，配备A17 Pro芯片，支持ProRes视频录制。',
      tags: ['热销', '新品'],
      specs: [
        { name: '颜色', values: ['深空黑色', '原色钛金属', '白色钛金属', '蓝色钛金属'] },
        { name: '存储', values: ['256GB', '512GB', '1TB'] }
      ]
    },
    {
      id: 2,
      name: 'MacBook Pro 14英寸 M3芯片',
      price: 14999,
      originalPrice: 16999,
      image: 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400',
      images: [
        'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=800',
        'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=800'
      ],
      categoryId: 2,
      categoryName: '电脑办公',
      sales: 856,
      stock: 50,
      description: 'MacBook Pro 14英寸，搭载M3芯片，性能强劲，适合专业工作。',
      tags: ['热销'],
      specs: [
        { name: '芯片', values: ['M3', 'M3 Pro', 'M3 Max'] },
        { name: '内存', values: ['16GB', '32GB', '64GB'] }
      ]
    },
    {
      id: 3,
      name: 'AirPods Pro 第二代',
      price: 1899,
      originalPrice: 1999,
      image: 'https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400',
      images: [
        'https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=800'
      ],
      categoryId: 1,
      categoryName: '手机数码',
      sales: 2345,
      stock: 200,
      description: 'AirPods Pro 第二代，主动降噪，空间音频，MagSafe充电盒。',
      tags: ['热销', '新品'],
      specs: []
    },
    {
      id: 4,
      name: 'Nike Air Max 270 运动鞋',
      price: 899,
      originalPrice: 1299,
      image: 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400',
      images: [
        'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800'
      ],
      categoryId: 7,
      categoryName: '运动户外',
      sales: 567,
      stock: 150,
      description: 'Nike Air Max 270 经典运动鞋，舒适透气，时尚百搭。',
      tags: ['热销'],
      specs: [
        { name: '尺码', values: ['39', '40', '41', '42', '43', '44'] },
        { name: '颜色', values: ['黑色', '白色', '红色'] }
      ]
    },
    {
      id: 5,
      name: 'Dyson V15 无线吸尘器',
      price: 3999,
      originalPrice: 4999,
      image: 'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400',
      images: [
        'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800'
      ],
      categoryId: 3,
      categoryName: '家用电器',
      sales: 234,
      stock: 80,
      description: 'Dyson V15 无线吸尘器，强劲吸力，激光除螨，续航60分钟。',
      tags: ['热销'],
      specs: []
    },
    {
      id: 6,
      name: 'SK-II 神仙水 230ml',
      price: 1299,
      originalPrice: 1599,
      image: 'https://images.unsplash.com/photo-1522338242472-255a1f213b8f?w=400',
      images: [
        'https://images.unsplash.com/photo-1522338242472-255a1f213b8f?w=800'
      ],
      categoryId: 5,
      categoryName: '美妆护肤',
      sales: 1234,
      stock: 100,
      description: 'SK-II 神仙水，经典护肤精华，改善肌肤状态。',
      tags: ['热销', '新品'],
      specs: [
        { name: '规格', values: ['230ml', '330ml'] }
      ]
    },
    {
      id: 7,
      name: 'Fresh 有机苹果 5kg装',
      price: 49,
      originalPrice: 69,
      image: 'https://images.unsplash.com/photo-1542838132-92c53300491e?w=400',
      images: [
        'https://images.unsplash.com/photo-1542838132-92c53300491e?w=800'
      ],
      categoryId: 6,
      categoryName: '食品生鲜',
      sales: 3456,
      stock: 500,
      description: '新鲜有机苹果，脆甜多汁，营养丰富。',
      tags: ['热销'],
      specs: []
    },
    {
      id: 8,
      name: 'LEGO 乐高积木 城市系列',
      price: 299,
      originalPrice: 399,
      image: 'https://images.unsplash.com/photo-1555252333-9f8e92e65df9?w=400',
      images: [
        'https://images.unsplash.com/photo-1555252333-9f8e92e65df9?w=800'
      ],
      categoryId: 8,
      categoryName: '母婴玩具',
      sales: 789,
      stock: 120,
      description: 'LEGO 乐高积木城市系列，培养孩子创造力。',
      tags: ['热销'],
      specs: []
    }
  ],

  // 推荐商品
  recommendGoods: [1, 2, 3, 4],

  // 热门商品
  hotGoods: [5, 6, 7, 8]
}

module.exports = mockData

