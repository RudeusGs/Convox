<template>
  <div class="section">
    <div class="section-header">
      <h3>Thành viên</h3>
      <span class="member-count">{{ onlineMembers }}/{{ totalMembers }}</span>
    </div>
    
    <div class="members-container">
      <div v-for="member in members" :key="member.id" class="member-item">
        <div class="member-avatar">
          <img :src="member.avatar" :alt="member.name" />
          <div class="status-dot" :class="member.status"></div>
        </div>
        <div class="member-info">
          <span class="member-name">{{ member.name }}</span>
          <span class="member-role">{{ member.role }}</span>
        </div>
        <div class="member-actions">
          <button class="member-action" title="Nhắn tin">
            <i class="bi bi-chat"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  members: {
    type: Array,
    required: true
  },
  onlineMembers: {
    type: Number,
    required: true
  },
  totalMembers: {
    type: Number,
    required: true
  }
})
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

.member-count {
  font-size: 11px;
  color: #b5bac1;
}

.members-container {
  flex: 1;
  overflow-y: auto;
  padding: 0 8px 16px;
}

.member-item {
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

.member-item:hover {
  background: #35373c;
  color: #f2f3f5;
}

.member-item:hover .member-actions {
  opacity: 1;
}

.member-avatar {
  position: relative;
  margin-right: 8px;
  flex-shrink: 0;
}

.member-avatar img {
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

.member-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.member-name {
  font-size: 13px;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.2;
}

.member-role {
  font-size: 11px;
  color: #b5bac1;
  line-height: 1.2;
}

.member-actions {
  opacity: 0;
  transition: opacity 0.15s ease;
  flex-shrink: 0;
}

.member-action {
  background: none;
  border: none;
  color: #b5bac1;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 12px;
}

.member-action:hover {
  background: #2b2d31;
  color: #f2f3f5;
}

.members-container::-webkit-scrollbar {
  width: 4px;
}

.members-container::-webkit-scrollbar-track {
  background: transparent;
}

.members-container::-webkit-scrollbar-thumb {
  background: #2b2d31;
  border-radius: 2px;
}

.members-container::-webkit-scrollbar-thumb:hover {
  background: #35373c;
}
</style>
