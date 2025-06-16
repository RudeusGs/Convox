<template>
  <div v-if="isOpen" class="modal-overlay" @click="closeModal">
    <div class="modal-container" @click.stop>
      <div class="modal-header">
        <h2>Mời mọi người tham gia {{ serverName }}</h2>
        <button class="close-btn" @click="closeModal">
          <i class="bi bi-x"></i>
        </button>
      </div>
      
      <div class="modal-content">
        <!-- Invite Link Section -->
        <div class="invite-section">
          <h3>Hoặc gửi link mời cho bạn bè</h3>
          <div class="invite-link-container">
            <div class="invite-link">
              <span>{{ inviteLink }}</span>
              <button class="copy-btn" @click="copyInviteLink" :class="{ copied: linkCopied }">
                <i class="bi" :class="linkCopied ? 'bi-check' : 'bi-clipboard'"></i>
                {{ linkCopied ? 'Đã sao chép!' : 'Sao chép' }}
              </button>
            </div>
          </div>
          
          <div class="invite-settings">
            <div class="setting-row">
              <label>Link hết hạn sau</label>
              <select v-model="inviteExpiry" class="setting-select">
                <option value="30m">30 phút</option>
                <option value="1h">1 giờ</option>
                <option value="6h">6 giờ</option>
                <option value="12h">12 giờ</option>
                <option value="1d">1 ngày</option>
                <option value="7d">7 ngày</option>
                <option value="never">Không bao giờ</option>
              </select>
            </div>
            
            <div class="setting-row">
              <label>Số lần sử dụng tối đa</label>
              <select v-model="maxUses" class="setting-select">
                <option value="1">1 lần</option>
                <option value="5">5 lần</option>
                <option value="10">10 lần</option>
                <option value="25">25 lần</option>
                <option value="50">50 lần</option>
                <option value="100">100 lần</option>
                <option value="unlimited">Không giới hạn</option>
              </select>
            </div>
          </div>
          
          <button class="generate-new-link" @click="generateNewLink">
            Tạo link mới
          </button>
        </div>
        
        <!-- Search Friends Section -->
        <div class="friends-section">
          <h3>Mời bạn bè</h3>
          <div class="search-container">
            <div class="search-input">
              <i class="bi bi-search"></i>
              <input 
                v-model="searchQuery" 
                type="text" 
                placeholder="Tìm kiếm bạn bè..." 
                @input="filterFriends"
              />
            </div>
          </div>
          
          <div class="friends-list">
            <div v-if="filteredFriends.length === 0" class="no-friends">
              <i class="bi bi-person-x"></i>
              <p>Không tìm thấy bạn bè nào</p>
            </div>
            
            <div v-for="friend in filteredFriends" :key="friend.id" class="friend-item">
              <div class="friend-avatar">
                <img :src="friend.avatar" :alt="friend.name" />
                <div class="status-dot" :class="friend.status"></div>
              </div>
              <div class="friend-info">
                <span class="friend-name">{{ friend.name }}</span>
                <span class="friend-status">{{ getStatusText(friend.status) }}</span>
              </div>
              <button 
                class="invite-btn"
                :class="{ invited: friend.invited }"
                @click="toggleInvite(friend)"
                :disabled="friend.invited"
              >
                <i class="bi" :class="friend.invited ? 'bi-check' : 'bi-plus'"></i>
                {{ friend.invited ? 'Đã mời' : 'Mời' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  isOpen: Boolean,
  serverName: String
})

const emit = defineEmits(['close'])

const searchQuery = ref('')
const linkCopied = ref(false)
const inviteExpiry = ref('7d')
const maxUses = ref('unlimited')
const inviteLink = ref('https://discord.gg/abc123xyz')

const friends = ref([
  { id: 1, name: 'Alice Johnson', avatar: '/placeholder.svg?height=32&width=32', status: 'online', invited: false },
  { id: 2, name: 'Bob Smith', avatar: '/placeholder.svg?height=32&width=32', status: 'away', invited: false },
  { id: 3, name: 'Carol Davis', avatar: '/placeholder.svg?height=32&width=32', status: 'offline', invited: false },
  { id: 4, name: 'David Wilson', avatar: '/placeholder.svg?height=32&width=32', status: 'busy', invited: true },
  { id: 5, name: 'Eve Brown', avatar: '/placeholder.svg?height=32&width=32', status: 'online', invited: false }
])

const filteredFriends = computed(() => {
  if (!searchQuery.value) return friends.value
  return friends.value.filter(friend =>
    friend.name.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

const copyInviteLink = async () => {
  try {
    await navigator.clipboard.writeText(inviteLink.value)
    linkCopied.value = true
    setTimeout(() => {
      linkCopied.value = false
    }, 2000)
  } catch (err) {
    console.error('Failed to copy link:', err)
  }
}

const generateNewLink = () => {
  const randomId = Math.random().toString(36).substring(2, 15)
  inviteLink.value = `https://discord.gg/${randomId}`
  console.log('Generated new invite link:', inviteLink.value)
}

const toggleInvite = (friend) => {
  if (!friend.invited) {
    friend.invited = true
    console.log(`Invited ${friend.name} to server`)
    // Simulate sending invite
    setTimeout(() => {
      console.log(`Invite sent to ${friend.name}`)
    }, 500)
  }
}

const getStatusText = (status) => {
  const statusMap = {
    online: 'Trực tuyến',
    away: 'Vắng mặt',
    busy: 'Bận',
    offline: 'Ngoại tuyến'
  }
  return statusMap[status] || 'Không xác định'
}

const filterFriends = () => {
  // Filtering is handled by computed property
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
  font-size: 18px;
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
  padding: 20px;
  overflow-y: auto;
}

.invite-section {
  margin-bottom: 24px;
}

.invite-section h3 {
  color: #ffffff;
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 16px 0;
}

.invite-link-container {
  margin-bottom: 16px;
}

.invite-link {
  display: flex;
  align-items: center;
  background: #2f3136;
  border: 1px solid #202225;
  border-radius: 4px;
  overflow: hidden;
}

.invite-link span {
  flex: 1;
  padding: 12px;
  color: #dcddde;
  font-family: 'Courier New', monospace;
  font-size: 14px;
  background: #40444b;
}

.copy-btn {
  background: #5865f2;
  border: none;
  color: white;
  padding: 12px 16px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  gap: 6px;
}

.copy-btn:hover {
  background: #4752c4;
}

.copy-btn.copied {
  background: #23a55a;
}

.invite-settings {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 16px;
}

.setting-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.setting-row label {
  color: #b9bbbe;
  font-size: 14px;
  font-weight: 500;
}

.setting-select {
  background: #40444b;
  border: 1px solid #202225;
  border-radius: 4px;
  padding: 6px 8px;
  color: #dcddde;
  font-size: 14px;
  outline: none;
  cursor: pointer;
}

.setting-select:focus {
  border-color: #5865f2;
}

.generate-new-link {
  background: none;
  border: 1px solid #4f545c;
  color: #dcddde;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.15s ease;
}

.generate-new-link:hover {
  border-color: #dcddde;
}

.friends-section h3 {
  color: #ffffff;
  font-size: 16px;
  font-weight: 600;
  margin: 0 0 16px 0;
}

.search-container {
  margin-bottom: 16px;
}

.search-input {
  position: relative;
  display: flex;
  align-items: center;
}

.search-input i {
  position: absolute;
  left: 12px;
  color: #72767d;
  font-size: 14px;
}

.search-input input {
  width: 100%;
  background: #40444b;
  border: 1px solid #202225;
  border-radius: 4px;
  padding: 8px 12px 8px 36px;
  color: #dcddde;
  font-size: 14px;
  outline: none;
}

.search-input input:focus {
  border-color: #5865f2;
}

.friends-list {
  max-height: 300px;
  overflow-y: auto;
}

.no-friends {
  text-align: center;
  padding: 40px 20px;
  color: #72767d;
}

.no-friends i {
  font-size: 48px;
  margin-bottom: 16px;
  display: block;
}

.no-friends p {
  margin: 0;
  font-size: 14px;
}

.friend-item {
  display: flex;
  align-items: center;
  padding: 8px 0;
  border-bottom: 1px solid #202225;
  gap: 12px;
}

.friend-item:last-child {
  border-bottom: none;
}

.friend-avatar {
  position: relative;
  flex-shrink: 0;
}

.friend-avatar img {
  width: 32px;
  height: 32px;
  border-radius: 50%;
}

.status-dot {
  position: absolute;
  bottom: -1px;
  right: -1px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  border: 2px solid #36393f;
}

.status-dot.online { background: #23a55a; }
.status-dot.away { background: #f0b232; }
.status-dot.busy { background: #ed4245; }
.status-dot.offline { background: #80848e; }

.friend-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.friend-name {
  color: #dcddde;
  font-weight: 500;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.friend-status {
  color: #b9bbbe;
  font-size: 12px;
}

.invite-btn {
  background: #5865f2;
  border: none;
  color: white;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  font-weight: 500;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
}

.invite-btn:hover:not(:disabled) {
  background: #4752c4;
}

.invite-btn.invited {
  background: #23a55a;
  cursor: not-allowed;
}

.invite-btn:disabled {
  opacity: 0.6;
}
</style>
