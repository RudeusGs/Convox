<template>
  <div 
    class="channel-item"
    :class="{ 
      active: isActive,
      unread: channel.unread,
      muted: channel.muted,
      'voice-active': type === 'voice' && channel.activeUsers > 0
    }"
    @click="$emit('selectChannel', channel.id)"
  >
    <div class="channel-icon">
      <i class="bi" :class="getChannelIcon()"></i>
    </div>
    <span class="channel-name">{{ channel.name }}</span>
    <div class="channel-meta">
      <div v-if="type === 'voice' && channel.activeUsers > 0" class="voice-users">
        {{ channel.activeUsers }}
      </div>
      <div v-if="channel.unread && !channel.muted && type === 'text'" class="unread-count">
        {{ channel.unreadCount }}
      </div>
      <i v-if="channel.muted" class="bi bi-bell-slash mute-icon"></i>
      <i v-if="channel.locked" class="bi bi-lock lock-icon"></i>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  channel: {
    type: Object,
    required: true
  },
  isActive: {
    type: Boolean,
    default: false
  },
  type: {
    type: String,
    required: true,
    validator: (value) => ['text', 'voice'].includes(value)
  }
})

defineEmits(['selectChannel'])

const getChannelIcon = () => {
  return props.type === 'voice' ? 'bi-volume-up' : 'bi-hash'
}
</script>

<style scoped>
.channel-item {
  display: flex;
  align-items: center;
  padding: 6px 8px;
  margin: 1px 0;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.15s ease;
  color: #b5bac1;
  min-height: 32px;
}

.channel-item:hover {
  background: #35373c;
  color: #f2f3f5;
}

.channel-item.active {
  background: #5865f2;
  color: white;
}

.channel-item.unread {
  color: #f2f3f5;
  font-weight: 500;
}

.channel-item.muted {
  opacity: 0.6;
}

.channel-item.voice-active {
  color: #23a55a;
}

.channel-icon {
  margin-right: 6px;
  display: flex;
  align-items: center;
  font-size: 14px;
  flex-shrink: 0;
}

.channel-name {
  flex: 1;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.channel-meta {
  display: flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
}

.voice-users {
  background: #23a55a;
  color: white;
  font-size: 10px;
  padding: 2px 5px;
  border-radius: 8px;
  min-width: 16px;
  text-align: center;
  line-height: 1;
}

.unread-count {
  background: #ed4245;
  color: white;
  font-size: 10px;
  padding: 2px 5px;
  border-radius: 8px;
  min-width: 16px;
  text-align: center;
  line-height: 1;
}

.mute-icon, .lock-icon {
  font-size: 12px;
  color: #b5bac1;
}
</style>
