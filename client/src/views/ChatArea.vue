<template>
  <div class="chat-area">
    <!-- Chat Header -->
    <div class="chat-header">
      <div class="channel-info">
        <i class="bi bi-hash channel-hash"></i>
        <h3 class="channel-name">{{ currentChannelName }}</h3>
        <div class="channel-topic">{{ currentChannelTopic }}</div>
      </div>
      <div class="header-actions">
        <button class="header-btn" title="Start Voice Call">
          <i class="bi bi-telephone-fill"></i>
        </button>
        <button class="header-btn" title="Start Video Call">
          <i class="bi bi-camera-video-fill"></i>
        </button>
        <button class="header-btn" title="Pin Messages">
          <i class="bi bi-pin-angle-fill"></i>
        </button>
        <button class="header-btn" title="Member List" @click="toggleMemberList">
          <i class="bi bi-people-fill"></i>
        </button>
        <button class="header-btn" title="Search">
          <i class="bi bi-search"></i>
        </button>
        <div class="notification-wrapper">
          <button 
            class="header-btn notification-btn" 
            :class="{ active: showNotifications }"
            title="Thông báo"
            @click="toggleNotifications"
          >
            <i class="bi bi-bell-fill"></i>
            <div v-if="unreadNotificationsCount > 0" class="notification-badge">
              {{ unreadNotificationsCount }}
            </div>
          </button>
          <NotificationDropdown
            :isOpen="showNotifications"
            :notifications="notifications"
            @clearAllNotifications="clearAllNotifications"
            @dismissNotification="dismissNotification"
          />
        </div>
      </div>
    </div>

    <!-- Messages Container -->
    <div class="messages-container" ref="messagesContainer">
      <div v-for="message in messages" :key="message.id" class="message-group">
        <div class="message-avatar">
          <img :src="message.avatar" :alt="message.username" />
        </div>
        <div class="message-content">
          <div class="message-header">
            <span class="message-username" :style="{ color: message.roleColor }">{{ message.username }}</span>
            <span class="message-timestamp">{{ formatTime(message.timestamp) }}</span>
          </div>
          <div class="message-text">{{ message.content }}</div>
        </div>
        <div class="message-actions">
          <button class="message-action" title="Add Reaction">
            <i class="bi bi-emoji-smile"></i>
          </button>
          <button class="message-action" title="Reply">
            <i class="bi bi-reply"></i>
          </button>
          <button class="message-action" title="More">
            <i class="bi bi-three-dots"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Message Input -->
    <div class="message-input-container">
      <div class="message-input-wrapper">
        <button class="input-action" title="Upload File">
          <i class="bi bi-plus-circle-fill"></i>
        </button>
        <div class="message-input">
          <textarea 
            v-model="newMessage"
            :placeholder="`Message #${currentChannelName}`"
            @keydown.enter.exact.prevent="sendMessage"
            @keydown.enter.shift.exact="newMessage += '\n'"
            rows="1"
            ref="messageInput"
          ></textarea>
        </div>
        <div class="input-actions">
          <button class="input-action" title="Emoji">
            <i class="bi bi-emoji-smile-fill"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, nextTick, watch, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import NotificationDropdown from '@/components/NotificationDropdown.vue'

// Props từ router
const props = defineProps({
  serverId: String,
  channelId: String
})

// Router
const route = useRoute()
const router = useRouter()

// State
const newMessage = ref('')
const messageInput = ref(null)
const messagesContainer = ref(null)
const showMemberList = ref(false)
const showNotifications = ref(false)

// Sample messages data
const messages = ref([
  {
    id: 1,
    username: 'Rudeus',
    avatar: '/placeholder.svg?height=40&width=40',
    roleColor: '#5865f2',
    timestamp: new Date(Date.now() - 3600000),
    content: 'Hey everyone! How\'s it going?'
  },
  {
    id: 2,
    username: 'Eris',
    avatar: '/placeholder.svg?height=40&width=40',
    roleColor: '#f04747',
    timestamp: new Date(Date.now() - 3000000),
    content: 'Pretty good! Just working on some new features for the app.'
  }
])

// Notifications data
const notifications = ref([
  {
    id: 'notif1',
    title: 'Tin nhắn mới',
    message: 'Alice đã gửi tin nhắn trong #general',
    time: '2 phút trước',
    icon: 'bi-chat-dots',
    unread: true
  },
  {
    id: 'notif2',
    title: 'Thành viên mới',
    message: 'Bob Smith đã tham gia workspace',
    time: '1 giờ trước',
    icon: 'bi-person-plus',
    unread: true
  },
  {
    id: 'notif3',
    title: 'Cuộc họp sắp tới',
    message: 'Cuộc họp nhóm bắt đầu trong 15 phút',
    time: '5 phút trước',
    icon: 'bi-calendar-event',
    unread: false
  },
  {
    id: 'notif4',
    title: 'Mention trong #frontend',
    message: 'Carol đã mention bạn trong cuộc thảo luận',
    time: '10 phút trước',
    icon: 'bi-at',
    unread: true
  },
  {
    id: 'notif5',
    title: 'Voice channel',
    message: 'David đã tham gia Họp nhóm',
    time: '15 phút trước',
    icon: 'bi-volume-up',
    unread: false
  }
])

// Computed
const currentChannelName = computed(() => {
  return props.channelId || route.params.channelId || 'general'
})

const currentChannelTopic = computed(() => {
  const topics = {
    general: 'General discussion about anything and everything',
    random: 'Random thoughts and conversations',
    memes: 'Share your favorite memes and funny content',
    announcements: 'Important announcements and updates'
  }
  return topics[currentChannelName.value] || 'Channel topic'
})

const unreadNotificationsCount = computed(() => {
  return notifications.value.filter(notif => notif.unread).length
})

// Methods
const sendMessage = () => {
  if (newMessage.value.trim()) {
    const message = {
      id: messages.value.length + 1,
      username: 'You',
      avatar: '/placeholder.svg?height=40&width=40',
      roleColor: '#ffffff',
      timestamp: new Date(),
      content: newMessage.value.trim()
    }
    messages.value.push(message)
    newMessage.value = ''
    
    nextTick(() => {
      scrollToBottom()
    })
  }
}

const formatTime = (timestamp) => {
  const now = new Date()
  const messageTime = new Date(timestamp)
  const diffInHours = (now - messageTime) / (1000 * 60 * 60)
  
  if (diffInHours < 24) {
    return messageTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  } else {
    return messageTime.toLocaleDateString()
  }
}

const scrollToBottom = () => {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

const toggleMemberList = () => {
  showMemberList.value = !showMemberList.value
}

const toggleNotifications = () => {
  showNotifications.value = !showNotifications.value
}

const clearAllNotifications = () => {
  notifications.value.forEach(notif => notif.unread = false)
}

const dismissNotification = (notifId) => {
  const index = notifications.value.findIndex(n => n.id === notifId)
  if (index > -1) {
    notifications.value.splice(index, 1)
  }
}

// Close notifications when clicking outside
const handleClickOutside = (event) => {
  const notificationWrapper = event.target.closest('.notification-wrapper')
  if (!notificationWrapper && showNotifications.value) {
    showNotifications.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

// Watch for route changes
watch(() => route.params, () => {
  console.log('Channel changed:', route.params)
}, { immediate: true })
</script>

<style scoped>
.chat-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: #36393f;
}

/* Chat Header */
.chat-header {
  height: 48px;
  background: #36393f;
  border-bottom: 1px solid #202225;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 16px;
  box-shadow: 0 1px 0 rgba(4, 4, 5, 0.2);
}

.channel-info {
  display: flex;
  align-items: center;
  gap: 8px;
}

.channel-hash {
  color: #8e9297;
  font-size: 20px;
}

.channel-name {
  color: #ffffff;
  font-size: 16px;
  font-weight: 600;
  margin: 0;
}

.channel-topic {
  color: #8e9297;
  font-size: 14px;
  margin-left: 8px;
  position: relative;
}

.channel-topic::before {
  content: '|';
  margin-right: 8px;
  color: #4f545c;
}

.header-actions {
  display: flex;
  gap: 16px;
  align-items: center;
}

.header-btn {
  background: none;
  border: none;
  color: #b9bbbe;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease-out;
  font-size: 16px;
  position: relative;
}

.header-btn:hover {
  color: #dcddde;
}

.notification-wrapper {
  position: relative;
}

.notification-btn.active {
  color: #5865f2;
}

.notification-badge {
  position: absolute;
  top: -4px;
  right: -4px;
  background: #ed4245;
  color: white;
  font-size: 10px;
  padding: 2px 5px;
  border-radius: 8px;
  min-width: 16px;
  text-align: center;
  line-height: 1;
  font-weight: 600;
}

/* Messages Container */
.messages-container {
  flex: 1;
  overflow-y: auto;
  padding: 16px 0;
}

.message-group {
  display: flex;
  padding: 8px 16px;
  position: relative;
  transition: background 0.15s ease-out;
}

.message-group:hover {
  background: rgba(4, 4, 5, 0.07);
}

.message-group:hover .message-actions {
  opacity: 1;
}

.message-avatar {
  width: 40px;
  height: 40px;
  margin-right: 16px;
  flex-shrink: 0;
}

.message-avatar img {
  width: 100%;
  height: 100%;
  border-radius: 50%;
}

.message-content {
  flex: 1;
  min-width: 0;
}

.message-header {
  display: flex;
  align-items: baseline;
  gap: 8px;
  margin-bottom: 4px;
}

.message-username {
  font-weight: 500;
  font-size: 16px;
  cursor: pointer;
}

.message-username:hover {
  text-decoration: underline;
}

.message-timestamp {
  color: #a3a6aa;
  font-size: 12px;
  font-weight: 500;
}

.message-text {
  color: #dcddde;
  font-size: 16px;
  line-height: 1.375;
  word-wrap: break-word;
  white-space: pre-wrap;
}

.message-actions {
  position: absolute;
  top: -16px;
  right: 16px;
  background: #2f3136;
  border: 1px solid #202225;
  border-radius: 8px;
  padding: 4px;
  display: flex;
  gap: 4px;
  opacity: 0;
  transition: opacity 0.15s ease-out;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}

.message-action {
  background: none;
  border: none;
  color: #b9bbbe;
  cursor: pointer;
  padding: 6px;
  border-radius: 4px;
  transition: all 0.15s ease-out;
  font-size: 14px;
}

.message-action:hover {
  background: #40444b;
  color: #dcddde;
}

/* Message Input */
.message-input-container {
  padding: 0 16px 24px;
}

.message-input-wrapper {
  background: #40444b;
  border-radius: 8px;
  padding: 0 16px;
  display: flex;
  align-items: flex-end;
  gap: 12px;
  min-height: 44px;
}

.input-action {
  background: none;
  border: none;
  color: #b9bbbe;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease-out;
  font-size: 20px;
  flex-shrink: 0;
}

.input-action:hover {
  color: #dcddde;
}

.message-input {
  flex: 1;
  padding: 11px 0;
}

.message-input textarea {
  width: 100%;
  background: none;
  border: none;
  color: #dcddde;
  font-size: 16px;
  line-height: 1.375;
  resize: none;
  outline: none;
  font-family: inherit;
  max-height: 200px;
  overflow-y: auto;
}

.message-input textarea::placeholder {
  color: #72767d;
}

.input-actions {
  display: flex;
  gap: 8px;
  padding-bottom: 11px;
}

/* Scrollbar */
.messages-container::-webkit-scrollbar {
  width: 14px;
}

.messages-container::-webkit-scrollbar-track {
  background: #2e3338;
}

.messages-container::-webkit-scrollbar-thumb {
  background: #202225;
  border: 3px solid #2e3338;
  border-radius: 7px;
}

.messages-container::-webkit-scrollbar-thumb:hover {
  background: #1a1c1f;
}
</style>
