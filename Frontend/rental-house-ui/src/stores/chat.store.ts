import { defineStore } from 'pinia'
import { signalRService } from '@/services/signalr.service'
import apiClient from '@/services/api.client'

export interface MessageDto {
  id: number
  conversationId: number
  senderId: number
  content: string
  createdAt: string
  isRead: boolean
}

export interface ConversationDto {
  id: number
  partnerId: number
  partnerName: string
  partnerAvatar: string
  lastMessage: string
  lastUpdatedAt: string
  unreadCount: number
}

export const useChatStore = defineStore('chat', {
  state: () => ({
    conversations: [] as ConversationDto[],
    currentMessages: [] as MessageDto[],
    activeConversationId: null as number | null,
    isSignalRConnected: false,
  }),

  actions: {
    initSignalR() {
      if (this.isSignalRConnected) return
      signalRService.startConnection()

      // Lắng nghe tin nhắn mới từ SignalR
      signalRService.onReceiveMessage((newMessage: MessageDto) => {
        this.handleIncomingMessage(newMessage)
      })
      this.isSignalRConnected = true
    },

    disconnectSignalR() {
      signalRService.stopConnection()
      this.isSignalRConnected = false
    },

    handleIncomingMessage(message: MessageDto) {
      // 1. Nếu đang mở đúng đoạn chat này -> Push vào list messages đang xem
      if (this.activeConversationId === message.conversationId) {
        this.currentMessages.push(message)
        // Có thể gọi API đánh dấu đã đọc ở đây
      }

      // 2. Cập nhật Sidebar Conversation
      const convIndex = this.conversations.findIndex((c) => c.id === message.conversationId)
      if (convIndex !== -1) {
        const conv = this.conversations[convIndex]
        conv.lastMessage = message.content
        conv.lastUpdatedAt = message.createdAt
        if (this.activeConversationId !== message.conversationId) {
          conv.unreadCount += 1
        }
        // Đẩy conversation này lên đầu mảng
        this.conversations.splice(convIndex, 1)
        this.conversations.unshift(conv)
      }
    },

    async loadConversations() {
      const res = await apiClient.get('/chat/conversations')
      this.conversations = res.data
    },

    async loadMessages(conversationId: number) {
      this.activeConversationId = conversationId
      const res = await apiClient.get(`/chat/conversations/${conversationId}/messages`)
      this.currentMessages = res.data

      // Reset unread count
      const conv = this.conversations.find((c) => c.id === conversationId)
      if (conv) conv.unreadCount = 0
    },

    async sendMessage(conversationId: number, receiverId: number, content: string) {
      // Cập nhật giao diện lập tức (Optimistic UI) - Optional
      // Gọi API thực tế
      const res = await apiClient.post('/chat/send', { conversationId, receiverId, content })

      // Tự thêm tin nhắn mình vừa gửi vào khung chat
      this.currentMessages.push(res.data)

      // Đẩy hội thoại lên đầu
      const convIndex = this.conversations.findIndex((c) => c.id === conversationId)
      if (convIndex !== -1) {
        const conv = this.conversations[convIndex]
        conv.lastMessage = content
        this.conversations.splice(convIndex, 1)
        this.conversations.unshift(conv)
      }
    },
  },
})
