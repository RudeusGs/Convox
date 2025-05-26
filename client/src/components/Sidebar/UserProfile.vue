<template>
  <div class="user-profile">
    <div class="profile-info" @click="$emit('toggleProfileMenu')">
      <div class="profile-avatar">
        <img :src="user.avatar" :alt="user.name" />
        <div class="status-indicator" :class="userStatus"></div>
      </div>
      <div class="profile-details">
        <span class="profile-name">{{ user.name }}</span>
        <span class="profile-status">{{ getStatusText(userStatus) }}</span>
      </div>
    </div>
    
    <div class="profile-controls">
      <button 
        class="control-btn"
        :class="{ active: isMuted }"
        @click="$emit('toggleMute')"
        :title="isMuted ? 'Bật mic' : 'Tắt mic'"
      >
        <i class="bi" :class="isMuted ? 'bi-mic-mute' : 'bi-mic'"></i>
      </button>
      <button 
        class="control-btn"
        :class="{ active: isDeafened }"
        @click="$emit('toggleDeafen')"
        :title="isDeafened ? 'Bật âm thanh' : 'Tắt âm thanh'"
      >
        <i class="bi" :class="isDeafened ? 'bi-volume-mute' : 'bi-volume-up'"></i>
      </button>
      <button class="control-btn" @click="$emit('openSettings')" title="Cài đặt">
        <i class="bi bi-gear"></i>
      </button>
    </div>
  </div>
</template>

<script setup>
defineProps({
  user: {
    type: Object,
    required: true
  },
  userStatus: {
    type: String,
    required: true
  },
  isMuted: {
    type: Boolean,
    default: false
  },
  isDeafened: {
    type: Boolean,
    default: false
  }
})

defineEmits(['toggleMute', 'toggleDeafen', 'openSettings', 'toggleProfileMenu'])

const getStatusText = (status) => {
  const statusMap = {
    online: 'Trực tuyến',
    away: 'Vắng mặt',
    busy: 'Bận',
    offline: 'Ngoại tuyến'
  }
  return statusMap[status] || 'Không xác định'
}
</script>

<style scoped>
.user-profile {
  padding: 12px;
  border-top: 1px solid #2b2d31;
  background: #1a1b1e;
  flex-shrink: 0;
}

.profile-info {
  display: flex;
  align-items: center;
  padding: 6px 8px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.15s ease;
  margin-bottom: 8px;
  min-height: 40px;
}

.profile-info:hover {
  background: #35373c;
}

.profile-avatar {
  position: relative;
  margin-right: 8px;
  flex-shrink: 0;
}

.profile-avatar img {
  width: 32px;
  height: 32px;
  border-radius: 50%;
}

.status-indicator {
  position: absolute;
  bottom: -1px;
  right: -1px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  border: 2px solid #1a1b1e;
}

.status-indicator.online { background: #23a55a; }
.status-indicator.away { background: #f0b232; }
.status-indicator.busy { background: #ed4245; }
.status-indicator.offline { background: #80848e; }

.profile-details {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.profile-name {
  font-size: 13px;
  font-weight: 600;
  color: #f2f3f5;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.2;
}

.profile-status {
  font-size: 11px;
  color: #b5bac1;
  line-height: 1.2;
}

.profile-controls {
  display: flex;
  gap: 4px;
}

.control-btn {
  background: none;
  border: none;
  color: #b5bac1;
  cursor: pointer;
  padding: 6px;
  border-radius: 4px;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  width: 32px;
  height: 32px;
}

.control-btn:hover {
  background: #35373c;
  color: #f2f3f5;
}

.control-btn.active {
  background: #ed4245;
  color: white;
}
</style>
