import axios from 'axios'

// src/services/api.client.ts
const apiClient = axios.create({
  baseURL: 'https://localhost:7023/api', // Dùng HTTPS với port 7023
  headers: {
    'Content-Type': 'application/json',
  },
})

// Trạm gác (Interceptor) tự động nhét Token vào mỗi request (khi đã đăng nhập)
apiClient.interceptors.request.use(
  (config) => {
    // Nếu bạn lưu token bằng tên khác (ví dụ: 'jwt_token'), hãy sửa lại ở đây nhé
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

export default apiClient
