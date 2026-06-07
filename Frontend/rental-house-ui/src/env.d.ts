/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_URL: string // Bạn có thể khai báo thêm các biến môi trường khác ở đây sau này
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
