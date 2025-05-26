<template>
  <div class="dm-area">
    <!-- DM Header -->
    <div class="dm-header">
      <div class="dm-info">
        <div class="dm-avatar">
          <img :src="currentUser?.avatar || '/placeholder.svg?height=24&width=24'" :alt="currentUser?.username" />
        </div>
        <h3 class="dm-username">{{ currentUser?.username || 'Direct Messages' }}</h3>
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
      </div>
    </div>

    <!-- Messages or Welcome Screen -->
    <div v-if="!userId" class="dm-welcome">
      <div class="welcome-content">
        <i class="bi bi-chat-dots welcome-icon"></i>
        <h2>Your place to talk</h2>
        <p>Select a friend to start messaging, or start a new conversation.</p>
      </div>
    </div>

    <div v-else class="messages-container">
      <!-- DM Messages would go here -->
      <div class="dm-message">
        <div class="message-avatar">
          <img :src="currentUser?.avatar" :alt="currentUser?.username" />
        </div>
        <div class="message-content">
          <div class="message-header">
            <span class="message-username">{{ currentUser?.username }}</span>
            <span class="message-timestamp">Today at 2:30 PM</span>
          </div>
          <div class="message-text">Hey! How are you doing?</div>
        </div>
      </div>
    </div>

    <!-- Message Input for DM -->
    <div v-if="userId" class="message-input-container">
      <div class="message-input-wrapper">
        <button class="input-action" title="Upload File">
          <i class="bi bi-plus-circle-fill"></i>
        </button>
        <div class="message-input">
          <textarea 
            v-model="newMessage"
            :placeholder="`Message @${currentUser?.username}`"
            @keydown.enter.exact.prevent="sendMessage"
            rows="1"
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
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const newMessage = ref('')

// Sample user data
const users = ref([
  { id: '1', username: 'Eris', avatar: '/placeholder.svg?height=32&width=32' },
  { id: '2', username: 'Sylphie', avatar: '/placeholder.svg?height=32&width=32' },
  { id: '3', username: 'Roxy', avatar: '/placeholder.svg?height=32&width=32' }
])

const props = defineProps({
  userId: String
})

const userIdRef = ref(props.userId);

const currentUser = computed(() => {
  return users.value.find(user => user.id === userIdRef.value)
})

const sendMessage = () => {
  if (newMessage.value.trim()) {
    console.log('Sending DM:', newMessage.value)
    newMessage.value = ''
  }
}
</script>

<style scoped>
.dm-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: #36393f;
}

.dm-header {
  height: 48px;
  background: #36393f;
  border-bottom: 1px solid #202225;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 16px;
}

.dm-info {
  display: flex;
  align-items: center;
  gap: 8px;
}

.dm-avatar img {
  width: 24px;
  height: 24px;
  border-radius: 50%;
}

.dm-username {
  color: #ffffff;
  font-size: 16px;
  font-weight: 600;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 16px;
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
}

.header-btn:hover {
  color: #dcddde;
}

.dm-welcome {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
}

.welcome-content {
  max-width: 400px;
  padding: 40px;
}

.welcome-icon {
  font-size: 80px;
  color: #4f545c;
  margin-bottom: 24px;
}

.welcome-content h2 {
  color: #ffffff;
  font-size: 24px;
  font-weight: 600;
  margin: 0 0 16px 0;
}

.welcome-content p {
  color: #b9bbbe;
  font-size: 16px;
  line-height: 1.5;
  margin: 0;
}

.messages-container {
  flex: 1;
  overflow-y: auto;
  padding: 16px;
}

.dm-message {
  display: flex;
  padding: 8px 0;
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
  color: #ffffff;
}

.message-timestamp {
  color: #a3a6aa;
  font-size: 12px;
}

.message-text {
  color: #dcddde;
  font-size: 16px;
  line-height: 1.375;
}

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
  resize: none;
  outline: none;
  font-family: inherit;
}

.message-input textarea::placeholder {
  color: #72767d;
}

.input-actions {
  display: flex;
  gap: 8px;
  padding-bottom: 11px;
}
</style>
