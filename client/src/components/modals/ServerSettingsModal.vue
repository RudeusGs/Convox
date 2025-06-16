<template>
  <div v-if="isOpen" class="modal-overlay" @click="closeModal">
    <div class="modal-container" @click.stop>
      <div class="modal-header">
        <h2>Cài đặt Server</h2>
        <button class="close-btn" @click="closeModal">
          <i class="bi bi-x"></i>
        </button>
      </div>
      
      <div class="modal-content">
        <div class="settings-sidebar">
          <div class="settings-category">
            <h3>Server</h3>
            <button 
              v-for="item in serverSettings" 
              :key="item.id"
              class="settings-item"
              :class="{ active: activeSection === item.id }"
              @click="activeSection = item.id"
            >
              <i class="bi" :class="item.icon"></i>
              {{ item.label }}
            </button>
          </div>
          
          <div class="settings-category">
            <h3>Quản lý</h3>
            <button 
              v-for="item in managementSettings" 
              :key="item.id"
              class="settings-item"
              :class="{ active: activeSection === item.id }"
              @click="activeSection = item.id"
            >
              <i class="bi" :class="item.icon"></i>
              {{ item.label }}
            </button>
          </div>
        </div>
        
        <div class="settings-main">
          <!-- Overview Section -->
          <div v-if="activeSection === 'overview'" class="settings-section">
            <h3>Tổng quan Server</h3>
            <div class="form-group">
              <label>Tên Server</label>
              <input v-model="serverData.name" type="text" class="form-input" />
            </div>
            <div class="form-group">
              <label>Mô tả</label>
              <textarea v-model="serverData.description" class="form-textarea"></textarea>
            </div>
            <div class="form-group">
              <label>Icon Server</label>
              <div class="icon-upload">
                <div class="current-icon" :style="{ backgroundColor: serverData.color }">
                  {{ serverData.name.charAt(0) }}
                </div>
                <button class="upload-btn">Tải lên Icon</button>
              </div>
            </div>
          </div>
          
          <!-- Roles Section -->
          <div v-if="activeSection === 'roles'" class="settings-section">
            <h3>Vai trò</h3>
            <div class="roles-list">
              <div v-for="role in roles" :key="role.id" class="role-item">
                <div class="role-color" :style="{ backgroundColor: role.color }"></div>
                <span class="role-name">{{ role.name }}</span>
                <span class="role-members">{{ role.memberCount }} thành viên</span>
                <button class="role-edit-btn">Chỉnh sửa</button>
              </div>
            </div>
            <button class="add-role-btn">
              <i class="bi bi-plus"></i>
              Tạo vai trò
            </button>
          </div>
          
          <!-- Members Section -->
          <div v-if="activeSection === 'members'" class="settings-section">
            <h3>Thành viên</h3>
            <div class="members-search">
              <input type="text" placeholder="Tìm kiếm thành viên..." class="search-input" />
            </div>
            <div class="members-list">
              <div v-for="member in members" :key="member.id" class="member-item">
                <img :src="member.avatar" :alt="member.name" class="member-avatar" />
                <div class="member-info">
                  <span class="member-name">{{ member.name }}</span>
                  <span class="member-role">{{ member.role }}</span>
                </div>
                <div class="member-actions">
                  <button class="action-btn">Kick</button>
                  <button class="action-btn">Ban</button>
                </div>
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
  server: Object
})

const emit = defineEmits(['close'])

const activeSection = ref('overview')

const serverData = ref({
  name: props.server?.name || 'Server Name',
  description: 'Mô tả server...',
  color: props.server?.color || '#5865f2'
})

const serverSettings = ref([
  { id: 'overview', label: 'Tổng quan', icon: 'bi-house' },
  { id: 'roles', label: 'Vai trò', icon: 'bi-shield' },
  { id: 'emoji', label: 'Emoji', icon: 'bi-emoji-smile' },
  { id: 'stickers', label: 'Stickers', icon: 'bi-stickies' }
])

const managementSettings = ref([
  { id: 'members', label: 'Thành viên', icon: 'bi-people' },
  { id: 'invites', label: 'Lời mời', icon: 'bi-link-45deg' },
  { id: 'bans', label: 'Bans', icon: 'bi-hammer' },
  { id: 'audit-log', label: 'Audit Log', icon: 'bi-journal-text' }
])

const roles = ref([
  { id: 1, name: 'Admin', color: '#f04747', memberCount: 3 },
  { id: 2, name: 'Moderator', color: '#faa61a', memberCount: 5 },
  { id: 3, name: 'Member', color: '#99aab5', memberCount: 45 }
])

const members = ref([
  { id: 1, name: 'Alice Johnson', avatar: '/placeholder.svg?height=32&width=32', role: 'Admin' },
  { id: 2, name: 'Bob Smith', avatar: '/placeholder.svg?height=32&width=32', role: 'Moderator' },
  { id: 3, name: 'Carol Davis', avatar: '/placeholder.svg?height=32&width=32', role: 'Member' }
])

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
  max-width: 900px;
  height: 80vh;
  max-height: 600px;
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
  display: flex;
  overflow: hidden;
}

.settings-sidebar {
  width: 220px;
  background: #2f3136;
  padding: 20px 0;
  overflow-y: auto;
}

.settings-category {
  margin-bottom: 24px;
}

.settings-category h3 {
  color: #8e9297;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin: 0 0 8px 20px;
}

.settings-item {
  width: 100%;
  background: none;
  border: none;
  color: #b9bbbe;
  cursor: pointer;
  padding: 8px 20px;
  text-align: left;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 14px;
}

.settings-item:hover {
  background: #40444b;
  color: #dcddde;
}

.settings-item.active {
  background: #5865f2;
  color: #ffffff;
}

.settings-main {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
}

.settings-section h3 {
  color: #ffffff;
  font-size: 20px;
  font-weight: 600;
  margin: 0 0 20px 0;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  color: #b9bbbe;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 8px;
}

.form-input, .form-textarea {
  width: 100%;
  background: #40444b;
  border: 1px solid #202225;
  border-radius: 4px;
  padding: 10px;
  color: #dcddde;
  font-size: 16px;
  outline: none;
}

.form-input:focus, .form-textarea:focus {
  border-color: #5865f2;
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
}

.icon-upload {
  display: flex;
  align-items: center;
  gap: 16px;
}

.current-icon {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 24px;
  font-weight: 600;
}

.upload-btn {
  background: #5865f2;
  border: none;
  color: white;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.15s ease;
}

.upload-btn:hover {
  background: #4752c4;
}

.roles-list {
  margin-bottom: 16px;
}

.role-item {
  display: flex;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #202225;
  gap: 12px;
}

.role-color {
  width: 12px;
  height: 12px;
  border-radius: 50%;
}

.role-name {
  color: #dcddde;
  font-weight: 500;
  flex: 1;
}

.role-members {
  color: #b9bbbe;
  font-size: 12px;
}

.role-edit-btn {
  background: none;
  border: 1px solid #4f545c;
  color: #dcddde;
  padding: 4px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  transition: all 0.15s ease;
}

.role-edit-btn:hover {
  border-color: #dcddde;
}

.add-role-btn {
  background: #5865f2;
  border: none;
  color: white;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  gap: 8px;
}

.add-role-btn:hover {
  background: #4752c4;
}

.members-search {
  margin-bottom: 16px;
}

.search-input {
  width: 100%;
  background: #40444b;
  border: 1px solid #202225;
  border-radius: 4px;
  padding: 8px 12px;
  color: #dcddde;
  font-size: 14px;
  outline: none;
}

.search-input:focus {
  border-color: #5865f2;
}

.members-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.member-item {
  display: flex;
  align-items: center;
  padding: 12px;
  background: #2f3136;
  border-radius: 8px;
  gap: 12px;
}

.member-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
}

.member-info {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.member-name {
  color: #dcddde;
  font-weight: 500;
  font-size: 14px;
}

.member-role {
  color: #b9bbbe;
  font-size: 12px;
}

.member-actions {
  display: flex;
  gap: 8px;
}

.action-btn {
  background: none;
  border: 1px solid #4f545c;
  color: #dcddde;
  padding: 4px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  transition: all 0.15s ease;
}

.action-btn:hover {
  border-color: #ed4245;
  color: #ed4245;
}
</style>
