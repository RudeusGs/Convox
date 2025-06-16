<template>
  <div class="notification-dropdown" v-if="isOpen">
    <div class="notification-header">
      <h3>Thông báo</h3>
      <button class="clear-all-btn" @click="$emit('clearAllNotifications')" title="Đánh dấu tất cả đã đọc">
        <i class="bi bi-check2-all"></i>
      </button>
    </div>
    
    <div class="notifications-list">
      <div v-if="notifications.length === 0" class="no-notifications">
        <i class="bi bi-bell-slash"></i>
        <p>Không có thông báo mới</p>
      </div>
      
      <div v-for="notification in notifications" :key="notification.id" 
           class="notification-item" 
           :class="{ unread: notification.unread }">
        <div class="notification-icon">
          <i class="bi" :class="notification.icon"></i>
        </div>
        <div class="notification-content">
          <div class="notification-title">{{ notification.title }}</div>
          <div class="notification-message">{{ notification.message }}</div>
          <div class="notification-time">{{ notification.time }}</div>
        </div>
        <button class="notification-close" @click="$emit('dismissNotification', notification.id)">
          <i class="bi bi-x"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  isOpen: {
    type: Boolean,
    default: false
  },
  notifications: {
    type: Array,
    required: true
  }
})

defineEmits(['clearAllNotifications', 'dismissNotification'])
</script>

<style scoped>
.notification-dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  width: 360px;
  max-height: 480px;
  background: #2f3136;
  border: 1px solid #202225;
  border-radius: 8px;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.24);
  z-index: 1000;
  overflow: hidden;
}

.notification-header {
  padding: 16px;
  border-bottom: 1px solid #202225;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #36393f;
}

.notification-header h3 {
  font-size: 16px;
  font-weight: 600;
  color: #ffffff;
  margin: 0;
}

.clear-all-btn {
  background: none;
  border: none;
  color: #b9bbbe;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 14px;
}

.clear-all-btn:hover {
  background: #40444b;
  color: #dcddde;
}

.notifications-list {
  max-height: 400px;
  overflow-y: auto;
}

.no-notifications {
  padding: 40px 20px;
  text-align: center;
  color: #72767d;
}

.no-notifications i {
  font-size: 48px;
  margin-bottom: 16px;
  display: block;
}

.no-notifications p {
  margin: 0;
  font-size: 14px;
}

.notification-item {
  display: flex;
  align-items: flex-start;
  padding: 12px 16px;
  border-bottom: 1px solid #202225;
  transition: background 0.15s ease;
  position: relative;
}

.notification-item:hover {
  background: #34373c;
}

.notification-item.unread {
  background: rgba(88, 101, 242, 0.1);
  border-left: 3px solid #5865f2;
}

.notification-item.unread::before {
  content: '';
  position: absolute;
  left: 8px;
  top: 16px;
  width: 8px;
  height: 8px;
  background: #5865f2;
  border-radius: 50%;
}

.notification-icon {
  margin-right: 12px;
  color: #b9bbbe;
  font-size: 16px;
  margin-top: 2px;
  flex-shrink: 0;
}

.notification-content {
  flex: 1;
  min-width: 0;
}

.notification-title {
  font-size: 14px;
  font-weight: 500;
  color: #ffffff;
  margin-bottom: 4px;
  line-height: 1.2;
}

.notification-message {
  font-size: 13px;
  color: #b9bbbe;
  line-height: 1.3;
  margin-bottom: 4px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.notification-time {
  font-size: 11px;
  color: #72767d;
}

.notification-close {
  background: none;
  border: none;
  color: #72767d;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 14px;
  flex-shrink: 0;
  opacity: 0;
}

.notification-item:hover .notification-close {
  opacity: 1;
}

.notification-close:hover {
  background: #40444b;
  color: #dcddde;
}

.notifications-list::-webkit-scrollbar {
  width: 8px;
}

.notifications-list::-webkit-scrollbar-track {
  background: transparent;
}

.notifications-list::-webkit-scrollbar-thumb {
  background: #202225;
  border-radius: 4px;
}

.notifications-list::-webkit-scrollbar-thumb:hover {
  background: #1a1c1f;
}
</style>
