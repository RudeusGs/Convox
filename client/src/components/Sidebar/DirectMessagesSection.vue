<template>
  <div class="section">
    <div class="section-header">
      <h3>Tin nhắn</h3>
      <button class="add-btn" @click="$emit('startNewDM')" title="Tin nhắn mới">
        <i class="bi bi-plus-lg"></i>
      </button>
    </div>
    
    <div class="dm-container">
      <div 
        v-for="dm in directMessages"
        :key="dm.id"
        class="dm-item"
        :class="{ active: activeDM === dm.id, unread: dm.unread }"
        @click="$emit('selectDM', dm.id)"
      >
        <div class="dm-avatar">
          <img :src="dm.avatar" :alt="dm.name" />
          <div class="status-dot" :class="dm.status"></div>
        </div>
        <div class="dm-info">
          <span class="dm-name">{{ dm.name }}</span>
          <span class="dm-last-message">{{ dm.lastMessage }}</span>
        </div>
        <div class="dm-meta">
          <span class="dm-time">{{ dm.time }}</span>
          <div v-if="dm.unread" class="dm-unread">{{ dm.unreadCount }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  directMessages: {
    type: Array,
    required: true
  },
  activeDM: {
    type: String,
    default: null
  }
})

defineEmits(['selectDM', 'startNewDM'])
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

.add-btn {
  background: none;
  border: none;
  color: #b5bac1;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 14px;
}

.add-btn:hover {
  background: #35373c;
  color: #f2f3f5;
}

.dm-container {
  flex: 1;
  overflow-y: auto;
  padding: 0 8px 16px;
}

.dm-item {
  display: flex;
  align-items: center;
  padding: 6px 8px;
  margin: 1px 0;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.15s ease;
  color: #b5bac1;
  min-height: 42px;
}

.dm-item:hover {
  background: #35373c;
  color: #f2f3f5;
}

.dm-item.active {
  background: #5865f2;
  color: white;
}

.dm-item.unread {
  color: #f2f3f5;
  font-weight: 500;
}

.dm-avatar {
  position: relative;
  margin-right: 8px;
  flex-shrink: 0;
}

.dm-avatar img {
  width: 28px;
  height: 28px;
  border-radius: 50%;
}

.status-dot {
  position: absolute;
  bottom: -1px;
  right: -1px;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  border: 2px solid #1e1f22;
}

.status-dot.online { background: #23a55a; }
.status-dot.away { background: #f0b232; }
.status-dot.busy { background: #ed4245; }
.status-dot.offline { background: #80848e; }

.dm-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.dm-name {
  font-size: 13px;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.2;
}

.dm-last-message {
  font-size: 11px;
  color: #b5bac1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.2;
}

.dm-meta {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 2px;
  flex-shrink: 0;
}

.dm-time {
  font-size: 10px;
  color: #b5bac1;
}

.dm-unread {
  background: #ed4245;
  color: white;
  font-size: 10px;
  padding: 2px 5px;
  border-radius: 8px;
  min-width: 16px;
  text-align: center;
  line-height: 1;
}

.dm-container::-webkit-scrollbar {
  width: 4px;
}

.dm-container::-webkit-scrollbar-track {
  background: transparent;
}

.dm-container::-webkit-scrollbar-thumb {
  background: #2b2d31;
  border-radius: 2px;
}

.dm-container::-webkit-scrollbar-thumb:hover {
  background: #35373c;
}
</style>
