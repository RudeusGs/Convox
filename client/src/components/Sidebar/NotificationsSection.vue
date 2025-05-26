<template>
  <div class="section">
    <div class="section-header">
      <h3>Thông báo</h3>
      <button class="clear-btn" @click="$emit('clearAllNotifications')" title="Xóa tất cả">
        <i class="bi bi-check2-all"></i>
      </button>
    </div>
    
    <div class="notifications-container">
      <div v-for="notification in notifications" :key="notification.id" class="notification-item" :class="{ unread: notification.unread }">
        <div class="notification-icon">
          <i class="bi" :class="notification.icon"></i>
        </div>
        <div class="notification-content">
          <span class="notification-title">{{ notification.title }}</span>
          <span class="notification-message">{{ notification.message }}</span>
          <span class="notification-time">{{ notification.time }}</span>
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
  notifications: {
    type: Array,
    required: true
  }
})

defineEmits(['clearAllNotifications', 'dismissNotification'])
</script>

<style scoped>
.section {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.section-header {
  padding: 16px 16px 8px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-shrink: 0;
}

.section-header h3 {
  font-size: 12px;
  font-weight: 600;
  color: #b5bac1;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin: 0;
}

.clear-btn {
  background: none;
  border: none;
  color: #b5bac1;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 14px;
}

.clear-btn:hover {
  background: #35373c;
  color: #f2f3f5;
}

.notifications-container {
  flex: 1;
  overflow-y: auto;
  padding: 0 8px 16px;
}

.notification-item {
  display: flex;
  align-items: flex-start;
  padding: 8px;
  margin: 2px 0;
  border-radius: 6px;
  transition: all 0.15s ease;
  background: #2b2d31;
  border-left: 2px solid transparent;
  min-height: 60px;
}

.notification-item.unread {
  border-left-color: #5865f2;
  background: rgba(88, 101, 242, 0.1);
}

.notification-icon {
  margin-right: 8px;
  color: #b5bac1;
  font-size: 14px;
  margin-top: 2px;
  flex-shrink: 0;
}

.notification-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 0;
}

.notification-title {
  font-size: 12px;
  font-weight: 500;
  color: #f2f3f5;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.notification-message {
  font-size: 11px;
  color: #b5bac1;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.notification-time {
  font-size: 10px;
  color: #b5bac1;
}

.notification-close {
  background: none;
  border: none;
  color: #b5bac1;
  cursor: pointer;
  padding: 2px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 12px;
  flex-shrink: 0;
}

.notification-close:hover {
  background: #35373c;
  color: #f2f3f5;
}

.notifications-container::-webkit-scrollbar {
  width: 4px;
}

.notifications-container::-webkit-scrollbar-track {
  background: transparent;
}

.notifications-container::-webkit-scrollbar-thumb {
  background: #2b2d31;
  border-radius: 2px;
}

.notifications-container::-webkit-scrollbar-thumb:hover {
  background: #35373c;
}
</style>
