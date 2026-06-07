// src/services/api.client.ts
import axios from 'axios'

// Khởi tạo instance với cấu hình mặc định
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7001',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
})

// Request Interceptor: Tự động đính kèm Token trước khi gửi request
apiClient.interceptors.request.use(
  (config) => {
    // Ưu tiên lấy từ localStorage.
    // Nếu dùng Pinia, bạn có thể import store ở đây (lưu ý import bên trong hàm để tránh lỗi vòng lặp khởi tạo)
    const token = localStorage.getItem('accessToken')

    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

// Response Interceptor: Trích xuất data và bắt lỗi Global
apiClient.interceptors.response.use(
  (response) => {
    // Trả về trực tiếp payload data từ backend để code ở Service gọn gàng hơn
    return response.data
  },
  (error) => {
    // Xử lý logic dùng chung (VD: 401 thì tự động logout)
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('accessToken')
      // window.location.href = '/login';
    }
    return Promise.reject(error)
  },
)

export default apiClient
