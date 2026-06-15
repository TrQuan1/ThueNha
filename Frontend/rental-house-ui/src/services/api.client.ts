import axios from 'axios'

// Cấu hình Axios với baseURL trỏ về Backend
const apiClient = axios.create({
  baseURL: 'https://localhost:7023/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

/**
 * Request Interceptor: Tự động đính kèm JWT Token vào Header của mọi request
 */
apiClient.interceptors.request.use(
  (config) => {
    // Lấy token từ localStorage
    const token = localStorage.getItem('token')

    // Nếu tồn tại token, gắn vào Header Authorization theo chuẩn Bearer
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    // Nếu có lỗi trong quá trình cấu hình request, reject promise
    return Promise.reject(error)
  },
)

export default apiClient
