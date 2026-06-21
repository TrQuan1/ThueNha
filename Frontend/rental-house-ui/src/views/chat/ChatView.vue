<template>
  <div
    class="max-w-7xl mx-auto h-[85vh] bg-white rounded-2xl shadow-xl border border-gray-200 flex overflow-hidden my-6"
  >
    <div class="w-1/3 border-r border-gray-200 flex flex-col bg-gray-50">
      <div class="p-4 border-b border-gray-200 bg-white">
        <h2 class="text-xl font-extrabold text-gray-900">Tin nhắn</h2>
      </div>

      <div class="flex-1 overflow-y-auto">
        <div
          v-for="conv in chatStore.conversations"
          :key="conv.id"
          @click="selectConversation(conv)"
          :class="[
            'p-4 border-b border-gray-100 cursor-pointer transition hover:bg-gray-100 flex items-center gap-3',
            chatStore.activeConversationId === conv.id
              ? 'bg-blue-50 border-l-4 border-l-blue-600'
              : '',
          ]"
        >
          <div
            class="w-12 h-12 rounded-full bg-gray-800 text-white flex items-center justify-center font-bold shrink-0 relative"
          >
            {{ conv.partnerName[0] }}
            <span
              v-if="conv.unreadCount > 0"
              class="absolute -top-1 -right-1 bg-red-500 text-white text-[10px] w-5 h-5 flex items-center justify-center rounded-full border-2 border-white"
            >
              {{ conv.unreadCount }}
            </span>
          </div>

          <div class="flex-1 min-w-0">
            <div class="flex justify-between items-baseline mb-1">
              <h3 class="font-bold text-gray-900 truncate">{{ conv.partnerName }}</h3>
            </div>
            <p
              :class="[
                'text-sm truncate',
                conv.unreadCount > 0 ? 'font-bold text-gray-900' : 'text-gray-500',
              ]"
            >
              {{ conv.lastMessage }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <div class="w-2/3 flex flex-col bg-white">
      <div
        v-if="!chatStore.activeConversationId"
        class="flex-1 flex flex-col items-center justify-center text-gray-400"
      >
        <svg class="w-20 h-20 mb-4 text-gray-200" fill="currentColor" viewBox="0 0 20 20">
          <path
            fill-rule="evenodd"
            d="M18 10c0 3.866-3.582 7-8 7a8.841 8.841 0 01-4.083-.98L2 17l1.338-3.123C2.493 12.767 2 11.434 2 10c0-3.866 3.582-7 8-7s8 3.134 8 7zM7 9H5v2h2V9zm8 0h-2v2h2V9zM9 9h2v2H9V9z"
            clip-rule="evenodd"
          ></path>
        </svg>
        <p class="text-lg font-medium">Chọn một đoạn chat để bắt đầu nhắn tin</p>
      </div>

      <template v-else>
        <div
          class="px-6 py-4 border-b border-gray-200 shadow-sm z-10 bg-white flex items-center gap-3"
        >
          <div
            class="w-10 h-10 rounded-full bg-gray-800 text-white flex items-center justify-center font-bold"
          >
            {{ activePartner?.partnerName[0] }}
          </div>
          <div>
            <h3 class="font-bold text-gray-900">{{ activePartner?.partnerName }}</h3>
          </div>
        </div>

        <div
          class="flex-1 p-6 overflow-y-auto bg-gray-50 flex flex-col gap-4"
          ref="messagesContainer"
        >
          <div
            v-for="msg in chatStore.currentMessages"
            :key="msg.id"
            :class="['flex', msg.senderId === currentUserId ? 'justify-end' : 'justify-start']"
          >
            <div
              :class="[
                'max-w-[70%] px-4 py-2.5 rounded-2xl shadow-sm text-sm',
                msg.senderId === currentUserId
                  ? 'bg-blue-600 text-white rounded-br-none'
                  : 'bg-white border border-gray-200 text-gray-800 rounded-bl-none',
              ]"
            >
              {{ msg.content }}
            </div>
          </div>
        </div>

        <div class="p-4 bg-white border-t border-gray-200">
          <form @submit.prevent="handleSend" class="flex items-center gap-2">
            <input
              v-model="newMessage"
              type="text"
              placeholder="Nhập tin nhắn..."
              class="flex-1 bg-gray-100 border-transparent rounded-full px-4 py-3 focus:bg-white focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition"
            />
            <button
              type="submit"
              :disabled="!newMessage.trim()"
              class="w-12 h-12 rounded-full bg-blue-600 text-white flex items-center justify-center hover:bg-blue-700 transition disabled:bg-gray-300"
            >
              <svg class="w-5 h-5 transform rotate-90" fill="currentColor" viewBox="0 0 20 20">
                <path
                  d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.408l-7-14z"
                ></path>
              </svg>
            </button>
          </form>
        </div>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, nextTick, watch } from 'vue'
import { useChatStore } from '@/stores/chat.store'
import { useAuthStore } from '@/stores/auth.store'
import type { ConversationDto } from '@/stores/chat.store'

const chatStore = useChatStore()
const authStore = useAuthStore()
const currentUserId = computed(() => {
  const id = authStore.user?.userId
  return id ? Number(id) : 0
})

const messagesContainer = ref<HTMLElement | null>(null)
const newMessage = ref('')

const activePartner = computed(() => {
  return chatStore.conversations.find((c) => c.id === chatStore.activeConversationId)
})

// Hàm quan trọng nhất: Cuộn tự động
const scrollToBottom = () => {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTo({
        top: messagesContainer.value.scrollHeight,
        behavior: 'smooth', // Cuộn mượt mà
      })
    }
  })
}

// Theo dõi mảng messages, hễ có thay đổi (có tin mới) là gọi cuộn xuống đáy
watch(
  () => chatStore.currentMessages.length,
  () => {
    scrollToBottom()
  },
)

onMounted(async () => {
  // 1. Kết nối SignalR
  chatStore.initSignalR()
  // 2. Load danh sách chat
  await chatStore.loadConversations()
})

onUnmounted(() => {
  // Ngắt kết nối để tiết kiệm tài nguyên khi chuyển trang khác
  chatStore.disconnectSignalR()
})

const selectConversation = async (conv: ConversationDto) => {
  await chatStore.loadMessages(conv.id)
  // Đợi load xong thì cuộn xuống đáy lịch sử chat
  scrollToBottom()
}

const handleSend = async () => {
  if (!newMessage.value.trim() || !activePartner.value) return

  const content = newMessage.value.trim()
  newMessage.value = '' // Reset ô nhập liệu ngay lập tức

  await chatStore.sendMessage(
    chatStore.activeConversationId!,
    activePartner.value.partnerId,
    content,
  )
}
</script>

<style scoped>
/* Tuỳ biến scrollbar cho đẹp */
::-webkit-scrollbar {
  width: 6px;
}
::-webkit-scrollbar-track {
  background: transparent;
}
::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 10px;
}
::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}
</style>
