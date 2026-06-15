import axios from 'axios'

const apiClient = axios.create({
  baseURL: 'https://localhost:7023/api', // Giữ nguyên baseURL của bạn
  headers: {
    'Content-Type': 'application/json',
  },
})

// Bổ sung Request Interceptor
apiClient.interceptors.request.use(
  (config) => {
    // Lấy token từ localStorage (Đảm bảo lúc login bạn cũng lưu với key là 'token')
    const token = localStorage.getItem('token')

    // Nếu có token và config.headers tồn tại, gắn vào Header Authorization
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    // Xử lý khi có lỗi xảy ra trong quá trình setup request
    return Promise.reject(error)
  },
)

export default apiClient
