<template>
  <div class="user-card-fixed">
    <div class="user-card">
      <!-- Avatar + trạng thái -->
      <div class="user-card-avatar">
        <img :src="avatarUrl" alt="avatar" />
        <span class="status-dot" :class="`status-${statusType}`"></span>
      </div>

      <!-- Tên + trạng thái chữ -->
      <div class="user-card-info">
        <div class="user-card-name">
          {{ displayName }}
        </div>
        <div class="user-card-status-text">
          {{ statusText }}
        </div>
      </div>

      <!-- Icon action -->
      <div class="user-card-actions">
        <button type="button" class="icon-btn" @click="$emit('toggle-mic')">
          <i class="bi bi-mic-fill"></i>
        </button>
        <button type="button" class="icon-btn" @click="$emit('toggle-audio')">
          <i class="bi bi-headphones"></i>
        </button>
        <!-- NEW: bật/tắt camera -->
        <button type="button" class="icon-btn" @click="$emit('toggle-camera')">
          <i class="bi bi-camera-video-fill"></i>
        </button>
        <button type="button" class="icon-btn" @click="$emit('open-settings')">
          <i class="bi bi-gear-fill"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'UserStatusCard',
  props: {
    displayName: {
      type: String,
      default: 'Rudeus Grey',
    },
    avatarUrl: {
      type: String,
      default:
        'https://dk2dv4ezy246u.cloudfront.net/widgets/sS0Tz7iyNkB_large.jpg',
    },
    statusText: {
      type: String,
      default: 'Trực tuyến',
    },
    statusType: {
      // online / idle / dnd / offline
      type: String,
      default: 'online',
    },
  },
}
</script>

<style scoped>
.user-card-fixed {
  position: fixed;
  left: 10px;
  bottom: 10px;
  z-index: 200;
}

.user-card {
  display: flex;
  align-items: center;
  gap: 10px;
  background-color: #111827;
  border-radius: 16px;
  padding: 8px 10px;
  min-width: 240px;
  max-width: 280px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.6);
}

.user-card-avatar {
  position: relative;
  width: 36px;
  height: 36px;
  border-radius: 999px;
  overflow: hidden;
  flex-shrink: 0;
}

.user-card-avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.status-dot {
  position: absolute;
  right: 0px;
  bottom: 0px;
  width: 10px;
  height: 10px;
  border-radius: 999px;
  border: 2px solid #111827;
  background-color: #22c55e;
}

.status-online {
  background-color: #22c55e;
}
.status-idle {
  background-color: #f59e0b;
}
.status-dnd {
  background-color: #ef4444;
}
.status-offline {
  background-color: #6b7280;
}

.user-card-info {
  flex: 1;
  min-width: 0;
}

.user-card-name {
  font-size: 14px;
  font-weight: 600;
  color: #f9fafb;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.user-card-status-text {
  font-size: 12px;
  color: #9ca3af;
}

.user-card-actions {
  display: flex;
  align-items: center;
  gap: 6px;
}

.icon-btn {
  width: 28px;
  height: 28px;
  border-radius: 999px;
  border: none;
  background-color: #1f2937;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  cursor: pointer;
  color: #e5e7eb;
  font-size: 14px;
}

.icon-btn:hover {
  background-color: #374151;
}

@media (max-width: 768px) {
  .user-card-fixed {
    left: 8px;
    right: 8px;
    bottom: 8px;
  }

  .user-card {
    width: 100%;
    max-width: none;
    padding-inline: 10px;
  }

  .user-card-status-text {
    font-size: 11px;
  }

  .icon-btn {
    width: 26px;
    height: 26px;
    font-size: 13px;
  }
}
</style>
