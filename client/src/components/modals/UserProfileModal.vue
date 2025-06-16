<template>
  <div v-if="isOpen" class="modal-overlay" @click="closeModal">
    <div class="modal-container" @click.stop>
      <div class="modal-header">
        <h2>Hồ sơ người dùng</h2>
        <button class="close-btn" @click="closeModal">
          <i class="bi bi-x"></i>
        </button>
      </div>
      
      <div class="modal-content">
        <div class="profile-banner" :style="{ backgroundColor: user.bannerColor }">
          <div class="profile-avatar-large">
            <img :src="user.avatar" :alt="user.name" />
            <div class="status-indicator" :class="user.status"></div>
          </div>
        </div>
        
        <div class="profile-info">
          <div class="user-details">
            <h3 class="username">{{ user.name }}</h3>
            <p class="user-tag">{{ user.tag }}</p>
            <div class="status-section">
              <div class="status-text">
                <i class="bi" :class="getStatusIcon(user.status)"></i>
                {{ getStatusText(user.status) }}
              </div>
              <div v-if="user.customStatus" class="custom-status">
                <i class="bi bi-emoji-smile"></i>
                {{ user.customStatus }}
              </div>
            </div>
          </div>
          
          <div class="profile-actions">
            <button class="action-btn primary" @click="sendMessage">
              <i class="bi bi-chat-fill"></i>
              Nhắn tin
            </button>
            <button class="action-btn secondary" @click="addFriend">
              <i class="bi bi-person-plus-fill"></i>
              Kết bạn
            </button>
            <button class="action-btn secondary" @click="blockUser">
              <i class="bi bi-slash-circle-fill"></i>
              Chặn
            </button>
          </div>
        </div>
        
        <div class="profile-sections">
          <!-- About Section -->
          <div v-if="user.about" class="profile-section">
            <h4>Giới thiệu</h4>
            <p class="about-text">{{ user.about }}</p>
          </div>
          
          <!-- Member Since -->
          <div class="profile-section">
            <h4>Thành viên Discord từ</h4>
            <p class="member-since">{{ formatDate(user.joinedAt) }}</p>
          </div>
          
          <!-- Server Member Since -->
          <div v-if="user.serverJoinedAt" class="profile-section">
            <h4>Tham gia server từ</h4>
            <p class="member-since">{{ formatDate(user.serverJoinedAt) }}</p>
          </div>
          
          <!-- Roles -->
          <div v-if="user.roles && user.roles.length > 0" class="profile-section">
            <h4>Vai trò</h4>
            <div class="roles-list">
              <span 
                v-for="role in user.roles" 
                :key="role.id" 
                class="role-badge"
                :style="{ backgroundColor: role.color }"
              >
                {{ role.name }}
              </span>
            </div>
          </div>
          
          <!-- Activity -->
          <div v-if="user.activity" class="profile-section">
            <h4>Hoạt động</h4>
            <div class="activity-item">
              <div class="activity-icon">
                <i class="bi" :class="getActivityIcon(user.activity.type)"></i>
              </div>
              <div class="activity-details">
                <p class="activity-name">{{ user.activity.name }}</p>
                <p class="activity-details-text">{{ user.activity.details }}</p>
                <p class="activity-state">{{ user.activity.state }}</p>
              </div>
            </div>
          </div>
          
          <!-- Mutual Servers -->
          <div v-if="mutualServers.length > 0" class="profile-section">
            <h4>{{ mutualServers.length }} server chung</h4>
            <div class="mutual-servers">
              <div v-for="server in mutualServers" :key="server.id" class="mutual-server">
                <div class="server-icon" :style="{ backgroundColor: server.color }">
                  {{ server.name.charAt(0) }}
                </div>
                <span class="server-name">{{ server.name }}</span>
              </div>
            </div>
          </div>
          
          <!-- Mutual Friends -->
          <div v-if="mutualFriends.length > 0" class="profile-section">
            <h4>{{ mutualFriends.length }} bạn chung</h4>
            <div class="mutual-friends">
              <div v-for="friend in mutualFriends" :key="friend.id" class="mutual-friend">
                <img :src="friend.avatar" :alt="friend.name" class="friend-avatar" />
                <span class="friend-name">{{ friend.name }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const props = defineProps({
  isOpen: Boolean,
  user: Object
})

const emit = defineEmits(['close'])

const mutualServers = ref([
  { id: 1, name: 'Convox Team', color: '#5865f2' },
  { id: 2, name: 'Gaming Hub', color: '#57f287' }
])

const mutualFriends = ref([
  { id: 1, name: 'Alice', avatar: '/placeholder.svg?height=24&width=24' },
  { id: 2, name: 'Bob', avatar: '/placeholder.svg?height=24&width=24' }
])

const getStatusText = (status) => {
  const statusMap = {
    online: 'Trực tuyến',
    away: 'Vắng mặt',
    busy: 'Bận',
    offline: 'Ngoại tuyến'
  }
  return statusMap[status] || 'Không xác định'
}

const getStatusIcon = (status) => {
  const iconMap = {
    online: 'bi-circle-fill',
    away: 'bi-moon-fill',
    busy: 'bi-dash-circle-fill',
    offline: 'bi-circle'
  }
  return iconMap[status] || 'bi-circle'
}

const getActivityIcon = (type) => {
  const iconMap = {
    playing: 'bi-controller',
    listening: 'bi-music-note',
    watching: 'bi-eye-fill',
    streaming: 'bi-broadcast'
  }
  return iconMap[type] || 'bi-activity'
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const sendMessage = () => {
  console.log('Sending message to user:', props.user.name)
  emit('close')
}

const addFriend = () => {
  console.log('Adding friend:', props.user.name)
}

const blockUser = () => {
  if (confirm(`Bạn có chắc chắn muốn chặn ${props.user.name}?`)) {
    console.log('Blocking user:', props.user.name)
    emit('close')
  }
}

const closeModal = () => {
  emit('close')
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.85);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10000;
}

.modal-container {
  background: #36393f;
  border-radius: 8px;
  width: 90vw;
  max-width: 500px;
  max-height: 80vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.modal-header {
  padding: 20px;
  border-bottom: 1px solid #202225;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.modal-header h2 {
  color: #ffffff;
  font-size: 20px;
  font-weight: 600;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  color: #b9bbbe;
  cursor: pointer;
  padding: 8px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 20px;
}

.close-btn:hover {
  background: #40444b;
  color: #dcddde;
}

.modal-content {
  flex: 1;
  overflow-y: auto;
}

.profile-banner {
  height: 120px;
  position: relative;
  display: flex;
  align-items: flex-end;
  justify-content: center;
  padding-bottom: 20px;
}

.profile-avatar-large {
  position: relative;
  width: 80px;
  height: 80px;
  border: 6px solid #36393f;
  border-radius: 50%;
  overflow: hidden;
}

.profile-avatar-large img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.status-indicator {
  position: absolute;
  bottom: 4px;
  right: 4px;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: 3px solid #36393f;
}

.status-indicator.online { background: #23a55a; }
.status-indicator.away { background: #f0b232; }
.status-indicator.busy { background: #ed4245; }
.status-indicator.offline { background: #80848e; }

.profile-info {
  padding: 20px;
  text-align: center;
}

.username {
  color: #ffffff;
  font-size: 20px;
  font-weight: 600;
  margin: 0 0 4px 0;
}

.user-tag {
  color: #b9bbbe;
  font-size: 14px;
  margin: 0 0 16px 0;
}

.status-section {
  margin-bottom: 20px;
}

.status-text {
  color: #dcddde;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  margin-bottom: 4px;
}

.custom-status {
  color: #b9bbbe;
  font-size: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
}

.profile-actions {
  display: flex;
  gap: 8px;
  justify-content: center;
}

.action-btn {
  padding: 8px 16px;
  border-radius: 4px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.15s ease;
  border: none;
  display: flex;
  align-items: center;
  gap: 6px;
}

.action-btn.primary {
  background: #5865f2;
  color: white;
}

.action-btn.primary:hover {
  background: #4752c4;
}

.action-btn.secondary {
  background: #4f545c;
  color: #dcddde;
}

.action-btn.secondary:hover {
  background: #5d6269;
}

.profile-sections {
  padding: 0 20px 20px;
}

.profile-section {
  margin-bottom: 20px;
}

.profile-section h4 {
  color: #ffffff;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin: 0 0 8px 0;
}

.about-text, .member-since {
  color: #dcddde;
  font-size: 14px;
  line-height: 1.4;
  margin: 0;
}

.roles-list {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.role-badge {
  background: #4f545c;
  color: white;
  padding: 2px 6px;
  border-radius: 3px;
  font-size: 12px;
  font-weight: 500;
}

.activity-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
}

.activity-icon {
  width: 32px;
  height: 32px;
  background: #4f545c;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #dcddde;
  font-size: 16px;
  flex-shrink: 0;
}

.activity-details {
  flex: 1;
}

.activity-name {
  color: #ffffff;
  font-size: 14px;
  font-weight: 600;
  margin: 0 0 2px 0;
}

.activity-details-text, .activity-state {
  color: #b9bbbe;
  font-size: 12px;
  margin: 0;
  line-height: 1.3;
}

.mutual-servers, .mutual-friends {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.mutual-server, .mutual-friend {
  display: flex;
  align-items: center;
  gap: 8px;
}

.server-icon {
  width: 24px;
  height: 24px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 12px;
  font-weight: 600;
  flex-shrink: 0;
}

.friend-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  flex-shrink: 0;
}

.server-name, .friend-name {
  color: #dcddde;
  font-size: 14px;
}
</style>
